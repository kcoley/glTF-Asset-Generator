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
            private readonly glTFLoader.Schema.Accessor.ComponentTypeEnum ComponentType;
            private readonly bool normalized;
            private readonly IEnumerable<IEnumerable<Vector2>> TextureCoordSets;
            public TextureCoordsVertexAttribute(IEnumerable<IEnumerable<Vector2>> textureCoordSets, MeshPrimitive.TextureCoordsComponentTypeEnum componentType)
            {
                TextureCoordSets = textureCoordSets;
                switch (componentType)
                {
                    case MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT:
                        ComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                        normalized = false;
                        break;
                    case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE:
                        ComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                        normalized = true;
                        break;
                    case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT:
                        ComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
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
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT:
                        TextureCoordSets.ForEach(textureCoord =>
                        {
                            geometryData.Writer.Write(textureCoord);
                        });
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        TextureCoordSets.ForEach(textureCoordSet =>
                        {
                            textureCoordSet.ForEach(textureCoord =>
                            {
                                geometryData.Writer.Write(textureCoord.ConvertToNormalizedByteArray());
                            });

                        });
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                        TextureCoordSets.ForEach(textureCoordSet =>
                        {
                            textureCoordSet.ForEach(textureCoord =>
                            {
                                geometryData.Writer.Write(textureCoord.ConvertToNormalizedUShortArray());
                            });

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

            public override void Write(Data geometryData, IEnumerable<int> indices)
            {
                int textureCoordSetIndex = indices.ElementAt(0);
                int textureCoordIndex = indices.ElementAt(1);
                switch (ComponentType)
                {
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT:
                        geometryData.Writer.Write(TextureCoordSets.ElementAt(textureCoordSetIndex).ElementAt(textureCoordIndex));
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        geometryData.Writer.Write(TextureCoordSets.ElementAt(textureCoordSetIndex).ElementAt(textureCoordIndex).ConvertToNormalizedByteArray());
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                        geometryData.Writer.Write(TextureCoordSets.ElementAt(textureCoordSetIndex).ElementAt(textureCoordIndex).ConvertToNormalizedUShortArray());
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
