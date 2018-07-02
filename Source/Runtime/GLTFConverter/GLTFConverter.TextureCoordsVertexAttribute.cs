using AssetGenerator.Runtime.ExtensionMethods;
using glTFLoader.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.Runtime.GLTFConverter
{
    internal partial class GLTFConverter
    {
        private sealed class TextureCoordsVertexAttribute : VertexAttribute
        {
            private readonly Accessor.ComponentTypeEnum ComponentType;
            private readonly bool normalized;
            private readonly IEnumerable<Vector2> TextureCoordSet;
            private readonly GLTFConverter GLTFConverter;
            public TextureCoordsVertexAttribute(GLTFConverter glTFConverter, IEnumerable<Vector2> textureCoordSets, MeshPrimitive.TextureCoordsComponentTypeEnum componentType)
            {
                GLTFConverter = glTFConverter;
                TextureCoordSet = textureCoordSets;
                switch (componentType)
                {
                    case MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT:
                        ComponentType = Accessor.ComponentTypeEnum.FLOAT;
                        normalized = false;
                        break;
                    case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE:
                        ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                        normalized = true;
                        break;
                    case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT:
                        ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                        normalized = true;
                        break;
                    default:
                        throw new NotSupportedException($"The texture coords component type {componentType} is not supported!");
                }
            }

            public override void Write(Data geometryData)
            {
                switch (ComponentType)
                {
                    case Accessor.ComponentTypeEnum.FLOAT:
                        TextureCoordSet.ForEach(textureCoord =>
                        {
                            geometryData.Writer.Write(textureCoord);
                        });
                        break;
                    case Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        TextureCoordSet.ForEach(textureCoord =>
                        {
                            geometryData.Writer.Write(textureCoord.ConvertToNormalizedByteArray());
                            GLTFConverter.Align(geometryData, (int)geometryData.Writer.BaseStream.Position, 4);
                        });  
                        break;
                    case Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                        TextureCoordSet.ForEach(textureCoord =>
                        {
                                geometryData.Writer.Write(textureCoord.ConvertToNormalizedUShortArray());
                        });
                        break;
                    default:
                        throw new NotSupportedException($"The texture coordinate component type {ComponentType} is not supported!");
                }
            }
            public override bool IsNormalized()
            {
                return normalized;
            }

            public override void Write(Data geometryData, int index)
            {
                switch (ComponentType)
                {
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT:
                        geometryData.Writer.Write(TextureCoordSet.ElementAt(index));
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        geometryData.Writer.Write(TextureCoordSet.ElementAt(index).ConvertToNormalizedByteArray());
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                        geometryData.Writer.Write(TextureCoordSet.ElementAt(index).ConvertToNormalizedUShortArray());
                        break;
                    default:
                        throw new NotSupportedException($"The texture coordinate componet type {ComponentType} is not supported!");
                }
            }

            public override Accessor.ComponentTypeEnum GetAccessorComponentType()
            {
                return ComponentType;
            }

            public override Accessor.TypeEnum GetAccessorType()
            {
                return glTFLoader.Schema.Accessor.TypeEnum.VEC2;
            }
        }
    }
}
