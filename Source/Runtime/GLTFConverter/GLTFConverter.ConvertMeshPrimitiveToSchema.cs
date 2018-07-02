using AssetGenerator.Runtime.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AssetGenerator.Runtime.GLTFConverter
{
    internal partial class GLTFConverter
    {
        /// <summary>
        /// Converts runtime mesh primitive to schema.
        /// </summary>
        private glTFLoader.Schema.MeshPrimitive ConvertMeshPrimitiveToSchema(Node runtimeNode, MeshPrimitive runtimeMeshPrimitive, GLTF gltf, glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
        {
            var mPrimitive = CreateInstance<glTFLoader.Schema.MeshPrimitive>();
            var attributes = new Dictionary<string, int>();
            if (runtimeMeshPrimitive.Interleave != null && runtimeMeshPrimitive.Interleave == true)
            {
                attributes = InterleaveMeshPrimitiveAttributes(runtimeMeshPrimitive, geometryData, bufferIndex);
            }
            else
            {
                if (runtimeMeshPrimitive.Positions != null && runtimeMeshPrimitive.Positions.Any())
                {
                    var positionsVertexAttribute = new PositionVertexAttribute(runtimeMeshPrimitive.Positions);
                    ;
                    //Create BufferView for the position
                    int byteLength = sizeof(float) * 3 * runtimeMeshPrimitive.Positions.Count();
                    float[] min = new float[] { };
                    float[] max = new float[] { };

                    //get the max and min values
                    Vector3[] minMaxPositions = GetMinMaxPositions(runtimeMeshPrimitive);
                    min = new[] { minMaxPositions[0].X, minMaxPositions[0].Y, minMaxPositions[0].Z };
                    max = new[] { minMaxPositions[1].X, minMaxPositions[1].Y, minMaxPositions[1].Z };
                    int byteOffset = (int)geometryData.Writer.BaseStream.Position;

                    var bufferView = CreateBufferView(bufferIndex, "Positions", byteLength, byteOffset, null);
                    bufferViews.Add(bufferView);
                    int bufferviewIndex = bufferViews.Count() - 1;

                    // Create an accessor for the bufferView
                    var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, runtimeMeshPrimitive.Positions.Count(), "Positions Accessor", max, min, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                    accessors.Add(accessor);
                    positionsVertexAttribute.Write(geometryData);
                    attributes.Add("POSITION", accessors.Count() - 1);
                }
                if (runtimeMeshPrimitive.Normals != null && runtimeMeshPrimitive.Normals.Any())
                {
                    var normalsVertexAttribute = new NormalVertexAttribute(runtimeMeshPrimitive.Normals);
                    // Create BufferView
                    int byteLength = sizeof(float) * 3 * runtimeMeshPrimitive.Normals.Count();
                    // Create a bufferView
                    int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                    var bufferView = CreateBufferView(bufferIndex, "Normals", byteLength, byteOffset, null);

                    bufferViews.Add(bufferView);
                    int bufferviewIndex = bufferViews.Count() - 1;

                    // Create an accessor for the bufferView
                    var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, runtimeMeshPrimitive.Normals.Count(), "Normals Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                    accessors.Add(accessor);
                    normalsVertexAttribute.Write(geometryData);
                    attributes.Add("NORMAL", accessors.Count() - 1);
                }
                if (runtimeMeshPrimitive.Tangents != null && runtimeMeshPrimitive.Tangents.Any())
                {
                    var tangentVertexAttribute = new TangentVertexAttribute(runtimeMeshPrimitive.Tangents);
                    // Create BufferView
                    int byteLength = sizeof(float) * 4 * runtimeMeshPrimitive.Tangents.Count();
                    // Create a bufferView
                    int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                    var bufferView = CreateBufferView(bufferIndex, "Tangents", byteLength, byteOffset, null);


                    bufferViews.Add(bufferView);
                    int bufferviewIndex = bufferViews.Count() - 1;

                    // Create an accessor for the bufferView
                    var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, runtimeMeshPrimitive.Tangents.Count(), "Tangents Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC4, null);
                    accessors.Add(accessor);
                    tangentVertexAttribute.Write(geometryData);
                    attributes.Add("TANGENT", accessors.Count() - 1);
                }
                if (runtimeMeshPrimitive.Colors != null && runtimeMeshPrimitive.Colors.Any())
                {
                    var colorVertexAttribute = new ColorVertexAttribute(this, runtimeMeshPrimitive.Colors, runtimeMeshPrimitive.ColorComponentType, runtimeMeshPrimitive.ColorType);
                    int vectorSize = runtimeMeshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3 ? 3 : 4;

                    // Create BufferView
                    int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                    colorVertexAttribute.Write(geometryData);
                    int byteLength = (int)geometryData.Writer.BaseStream.Position - byteOffset;
 
                    int? byteStride = null;
                    switch (runtimeMeshPrimitive.ColorComponentType)
                    {
                        case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE:
                            if (vectorSize == 3)
                            {
                                byteStride = 4;
                            }
                            break;
                        case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT:
                            if (vectorSize == 3)
                            {
                                byteStride = 8;
                            }
                            break;
                        default: //Default to ColorComponentTypeEnum.FLOAT:
                            break;
                    }

                    var bufferView = CreateBufferView(bufferIndex, "Colors", byteLength, byteOffset, byteStride);
                    bufferViews.Add(bufferView);
                    int bufferviewIndex = bufferViews.Count() - 1;

                    // Create an accessor for the bufferView
                    var accessor = CreateAccessor(bufferviewIndex, 0, colorVertexAttribute.GetAccessorComponentType(), runtimeMeshPrimitive.Colors.Count(), "Colors Accessor", null, null, colorVertexAttribute.GetAccessorType(), colorVertexAttribute.IsNormalized());
                    accessors.Add(accessor);
                    attributes.Add("COLOR_0", accessors.Count() - 1);
                }
                if (runtimeMeshPrimitive.TextureCoordSets != null)
                {
                    int i = 0;
                    foreach (var textureCoordSet in runtimeMeshPrimitive.TextureCoordSets)
                    {
                        var textureCoordVertexAttribute = new TextureCoordsVertexAttribute(this, textureCoordSet, runtimeMeshPrimitive.TextureCoordsComponentType);
                        int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        textureCoordVertexAttribute.Write(geometryData);
                        int byteLength = (int)geometryData.Writer.BaseStream.Position - byteOffset;

                        glTFLoader.Schema.Accessor accessor;
                        glTFLoader.Schema.Accessor.ComponentTypeEnum accessorComponentType;
                        // we normalize only if the texture coord accessor type is not float
                        //bool normalized = runtimeMeshPrimitive.TextureCoordsComponentType != MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT;
                        int? byteStride = null;
                        switch (runtimeMeshPrimitive.TextureCoordsComponentType)
                        {
                            case MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT:
                                break;
                            case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE:
                                byteStride = 4;
                                break;
                            case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT:
                                break;
                            default: // Default to Float
                                break;
                        }

                        var bufferView = CreateBufferView(bufferIndex, "Texture Coords " + i, byteLength, byteOffset, byteStride);
                        bufferViews.Add(bufferView);
                        int bufferviewIndex = bufferViews.Count() - 1;
                        // Create Accessor
                        accessor = CreateAccessor(bufferviewIndex, 0, textureCoordVertexAttribute.GetAccessorComponentType(), textureCoordSet.Count(), "UV Accessor " + i, null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC2, textureCoordVertexAttribute.IsNormalized());
                        accessors.Add(accessor);

                        attributes.Add("TEXCOORD_" + i, accessors.Count() - 1);
                        ++i;
                    }
                }

            }
            if (runtimeMeshPrimitive.Indices != null && runtimeMeshPrimitive.Indices.Any())
            {
                int byteLength;
                int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                glTFLoader.Schema.Accessor.ComponentTypeEnum indexComponentType;

                switch (runtimeMeshPrimitive.IndexComponentType)
                {
                    case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_BYTE:
                        indexComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                        byteLength = sizeof(byte) * runtimeMeshPrimitive.Indices.Count();
                        break;
                    case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_SHORT:
                        byteLength = sizeof(ushort) * runtimeMeshPrimitive.Indices.Count();
                        indexComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                        break;
                    case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_INT:
                        byteLength = sizeof(uint) * runtimeMeshPrimitive.Indices.Count();
                        indexComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_INT;
                        break;
                    default:
                        throw new InvalidEnumArgumentException("Unrecognized Index Component Type Enum " + runtimeMeshPrimitive.IndexComponentType);
                }
                glTFLoader.Schema.BufferView bufferView = CreateBufferView(bufferIndex, "Indices", byteLength, byteOffset, null);
                bufferViews.Add(bufferView);
                int bufferviewIndex = bufferViews.Count() - 1;

                var accessor = CreateAccessor(bufferviewIndex, 0, indexComponentType, runtimeMeshPrimitive.Indices.Count(), "Indices Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.SCALAR, null);
                accessors.Add(accessor);
                switch (indexComponentType)
                {
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_INT:
                        foreach (var index in runtimeMeshPrimitive.Indices)
                        {
                            geometryData.Writer.Write(Convert.ToUInt32(index));
                        }
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        foreach (var index in runtimeMeshPrimitive.Indices)
                        {
                            geometryData.Writer.Write(Convert.ToByte(index));
                        }
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                        foreach (var index in runtimeMeshPrimitive.Indices)
                        {
                            geometryData.Writer.Write(Convert.ToUInt16(index));
                        }
                        break;
                    default:
                        throw new InvalidEnumArgumentException("Unsupported Index Component Type");
                }

                mPrimitive.Indices = accessors.Count() - 1;
            }
            if (runtimeMeshPrimitive.VertexJointWeights != null && runtimeMeshPrimitive.VertexJointWeights.Any())
            {
                var weightsVertexAttribute = new WeightsVertexAttribute(runtimeMeshPrimitive.VertexJointWeights, runtimeMeshPrimitive.WeightComponentType);
                int weightByteOffset = (int)geometryData.Writer.BaseStream.Position;
                // get weights
                weightsVertexAttribute.Write(geometryData);
                var weightByteLength = (int)geometryData.Writer.BaseStream.Position - weightByteOffset;

                var weights = runtimeMeshPrimitive.VertexJointWeights.Select(jointWeight => jointWeight.Select(jWeight => jWeight.Weight));

                var bufferView = CreateBufferView(bufferIndex, "weights buffer view", weightByteLength, weightByteOffset, null);
                bufferViews.Add(bufferView);

                var weightAccessor = CreateAccessor(bufferViews.Count() - 1, 0, weightsVertexAttribute.GetAccessorComponentType(), runtimeMeshPrimitive.VertexJointWeights.Count(), "weights accessor", null, null, weightsVertexAttribute.GetAccessorType(), null);
                accessors.Add(weightAccessor);
                attributes.Add("WEIGHTS_0", accessors.Count() - 1);

                var jointsVertexAttribute = new JointsVertexAttribute(this, runtimeMeshPrimitive.VertexJointWeights, runtimeMeshPrimitive.JointComponentType);

                int jointByteOffset = (int)geometryData.Writer.BaseStream.Position;
                jointsVertexAttribute.Write(geometryData);
                int jointByteLength = (int)geometryData.Writer.BaseStream.Position - jointByteOffset;
               
                var jointsBufferView = CreateBufferView(bufferIndex, "joint indices buffer view", jointByteLength, jointByteOffset, null);
                bufferViews.Add(jointsBufferView);

                var jointsAccessor = CreateAccessor(bufferViews.Count() - 1, 0, jointsVertexAttribute.GetAccessorComponentType(), runtimeMeshPrimitive.VertexJointWeights.Count(), "joint indices accessor", null, null, jointsVertexAttribute.GetAccessorType(), jointsVertexAttribute.IsNormalized());
                accessors.Add(jointsAccessor);
                attributes.Add("JOINTS_0", accessors.Count() - 1);
            }

            mPrimitive.Attributes = attributes;
            if (runtimeMeshPrimitive.Material != null)
            {
                var nMaterial = ConvertMaterialToSchema(runtimeMeshPrimitive.Material, gltf);
                materials.Add(nMaterial);
                mPrimitive.Material = materials.Count() - 1;
            }

            switch (runtimeMeshPrimitive.Mode)
            {
                case MeshPrimitive.ModeEnum.TRIANGLES:
                    //glTF defaults to triangles
                    break;
                case MeshPrimitive.ModeEnum.POINTS:
                    mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.POINTS;
                    break;
                case MeshPrimitive.ModeEnum.LINES:
                    mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.LINES;
                    break;
                case MeshPrimitive.ModeEnum.LINE_LOOP:
                    mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.LINE_LOOP;
                    break;
                case MeshPrimitive.ModeEnum.LINE_STRIP:
                    mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.LINE_STRIP;
                    break;
                case MeshPrimitive.ModeEnum.TRIANGLE_FAN:
                    mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.TRIANGLE_FAN;
                    break;
                case MeshPrimitive.ModeEnum.TRIANGLE_STRIP:
                    mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.TRIANGLE_STRIP;
                    break;
            }

            return mPrimitive;
        }
    }
}
