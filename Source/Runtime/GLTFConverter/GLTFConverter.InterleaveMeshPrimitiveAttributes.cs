using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AssetGenerator.Runtime.GLTFConverter
{
    internal partial class GLTFConverter
    {
        /// <summary>
        /// Interleaves the primitive attributes to a single bufferview
        /// </summary>
        private Dictionary<string, int> InterleaveMeshPrimitiveAttributes(MeshPrimitive meshPrimitive, Data geometryData)
        {
            var attributes = new Dictionary<string, int>();
            var availableAttributes = new Dictionary<attributeEnum, VertexAttribute>();
            int vertexCount = 0;

            // create bufferview
            var bufferView = CreateBufferView(bufferIndex, "Interleaved attributes", 1, 0, null);
            bufferViews.Add(bufferView);
            int bufferviewIndex = bufferViews.Count() - 1;

            int byteOffset = 0;

            if (meshPrimitive.Positions != null && meshPrimitive.Positions.Any())
            {
                vertexCount = meshPrimitive.Positions.Count();
                //get the max and min values
                Vector3[] minMaxPositions = GetMinMaxPositions(meshPrimitive);
                var min = new[] { minMaxPositions[0].X, minMaxPositions[0].Y, minMaxPositions[0].Z };
                var max = new[] { minMaxPositions[1].X, minMaxPositions[1].Y, minMaxPositions[1].Z };
                var positionAccessor = CreateAccessor(bufferviewIndex, byteOffset, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, meshPrimitive.Positions.Count(), "Position Accessor", max, min, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                accessors.Add(positionAccessor);
                attributes.Add("POSITION", accessors.Count() - 1);
                availableAttributes.Add(attributeEnum.POSITION, new PositionVertexAttribute(meshPrimitive.Positions));
                byteOffset += sizeof(float) * 3;
            }
            if (meshPrimitive.Normals != null && meshPrimitive.Normals.Any())
            {
                var normalAccessor = CreateAccessor(bufferviewIndex, byteOffset, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, meshPrimitive.Normals.Count(), "Normal Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                accessors.Add(normalAccessor);
                attributes.Add("NORMAL", accessors.Count() - 1);
                availableAttributes.Add(attributeEnum.NORMAL, new NormalVertexAttribute(meshPrimitive.Normals));
                byteOffset += sizeof(float) * 3;
            }
            if (meshPrimitive.Tangents != null && meshPrimitive.Tangents.Any())
            {
                var tangentAccessor = CreateAccessor(bufferviewIndex, byteOffset, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, meshPrimitive.Tangents.Count(), "Tangent Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC4, null);
                accessors.Add(tangentAccessor);
                attributes.Add("TANGENT", accessors.Count() - 1);
                availableAttributes.Add(attributeEnum.TANGENT, new TangentVertexAttribute(meshPrimitive.Tangents));
                byteOffset += sizeof(float) * 4;
            }
            if (meshPrimitive.TextureCoordSets != null && meshPrimitive.TextureCoordSets.Any())
            {
                int textureCoordSetIndex = 0;
                foreach (var textureCoordSet in meshPrimitive.TextureCoordSets)
                {
                    int offset = 0;
                    switch (meshPrimitive.TextureCoordsComponentType)
                    {
                        case MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT:
                            offset = sizeof(float) * 2;
                            break;
                        case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE:
                            offset = sizeof(byte) * 2;
                            break;
                        case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT:
                            offset = sizeof(ushort) * 2;
                            break;
                        default:
                            throw new NotImplementedException("Accessor component type " + meshPrimitive.TextureCoordsComponentType + " not supported!");
                    }
                    var textureCoordsVertexAttribute = new TextureCoordsVertexAttribute(this, meshPrimitive.TextureCoordSets.ElementAt(textureCoordSetIndex), meshPrimitive.TextureCoordsComponentType);
                    availableAttributes.Add(textureCoordSetIndex == 0 ? attributeEnum.TEXCOORDS_0 : attributeEnum.TEXCOORDS_1, textureCoordsVertexAttribute);
                    var textureCoordAccessor = CreateAccessor(bufferviewIndex, byteOffset, textureCoordsVertexAttribute.GetAccessorComponentType(), textureCoordSet.Count(), "Texture Coord " + textureCoordSetIndex, null, null, textureCoordsVertexAttribute.GetAccessorType(), textureCoordsVertexAttribute.IsNormalized());
                    accessors.Add(textureCoordAccessor);
                    attributes.Add("TEXCOORD_" + textureCoordSetIndex, accessors.Count() - 1);

                    offset = GetPaddedSize(offset, 4);
                    byteOffset += offset;
                    ++textureCoordSetIndex;
                }
            }
            if (meshPrimitive.Colors != null && meshPrimitive.Colors.Any())
            {
                bool normalized = false;
                glTFLoader.Schema.Accessor.TypeEnum vectorType;
                int offset;
                if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3)
                {
                    offset = 3;
                    vectorType = glTFLoader.Schema.Accessor.TypeEnum.VEC3;
                }
                else if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC4)
                {
                    offset = 4;
                    vectorType = glTFLoader.Schema.Accessor.TypeEnum.VEC4;
                }
                else
                {
                    throw new NotImplementedException("Color of type " + meshPrimitive.ColorType + " not supported!");
                }
                glTFLoader.Schema.Accessor.ComponentTypeEnum colorAccessorComponentType;
                switch (meshPrimitive.ColorComponentType)
                {
                    case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE:
                        colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                        offset *= sizeof(byte);
                        normalized = true;
                        break;
                    case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT:
                        colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                        offset *= sizeof(ushort);
                        normalized = true;
                        break;
                    case MeshPrimitive.ColorComponentTypeEnum.FLOAT:
                        colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                        offset *= sizeof(float);
                        break;
                    default:
                        throw new NotImplementedException("Color component type " + meshPrimitive.ColorComponentType + " not supported!");

                }
                int totalByteLength = (int)geometryData.Writer.BaseStream.Position;
                offset = GetPaddedSize(offset, 4);
                var colorAccessor = CreateAccessor(bufferviewIndex, byteOffset, colorAccessorComponentType, meshPrimitive.Colors.Count(), "Color Accessor", null, null, vectorType, normalized);
                accessors.Add(colorAccessor);
                attributes.Add("COLOR_0", accessors.Count() - 1);
                availableAttributes.Add(attributeEnum.COLOR, new ColorVertexAttribute(this, meshPrimitive.Colors, meshPrimitive.ColorComponentType, meshPrimitive.ColorType));
                byteOffset += offset;
            }
            bufferView.ByteStride = byteOffset;

            for (int i = 0; i < vertexCount; ++i)
            {
                foreach (var availableAttribute in availableAttributes)
                {
                    availableAttribute.Value.Write(geometryData, i);

                    int totalByteLength = (int)geometryData.Writer.BaseStream.Position;
                    Align(geometryData, totalByteLength, 4);
                }
            }
            bufferView.ByteLength = (int)geometryData.Writer.BaseStream.Position;

            return attributes;
        }
    }
}
