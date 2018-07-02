using AssetGenerator.Runtime.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AssetGenerator.Runtime.GLTFConverter
{
    internal partial class GLTFConverter
    {
        private sealed class ColorVertexAttribute : VertexAttribute
        {
            private readonly glTFLoader.Schema.Accessor.ComponentTypeEnum ComponentType;
            private readonly glTFLoader.Schema.Accessor.TypeEnum Type;
            private readonly bool normalized;
            private readonly IEnumerable<Vector4> Colors;
            private readonly GLTFConverter GLTFConverter;
            public ColorVertexAttribute(GLTFConverter glTFConverter, IEnumerable<Vector4> colors, MeshPrimitive.ColorComponentTypeEnum componentType, MeshPrimitive.ColorTypeEnum colorType)
            {
                GLTFConverter = glTFConverter;
                Colors = colors;
                switch (componentType)
                {
                    case MeshPrimitive.ColorComponentTypeEnum.FLOAT:
                        ComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                        normalized = false;
                        break;
                    case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE:
                        ComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                        normalized = true;
                        break;
                    case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT:
                        ComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                        normalized = true;
                        break;
                    default:
                        throw new NotSupportedException($"The color component type {componentType} is not supported!");
                }
                Type = colorType == MeshPrimitive.ColorTypeEnum.VEC3 ? glTFLoader.Schema.Accessor.TypeEnum.VEC3 : glTFLoader.Schema.Accessor.TypeEnum.VEC4;
            }
            private void WriteFloatColor(Vector4 color, Data geometryData)
            {
                geometryData.Writer.Write(color.X);
                geometryData.Writer.Write(color.Y);
                geometryData.Writer.Write(color.Z);

                if (Type == glTFLoader.Schema.Accessor.TypeEnum.VEC4)
                {
                    geometryData.Writer.Write(color.W);
                }
            }

            public override void Write(Data geometryData)
            {
                switch (ComponentType)
                {
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT:
                        Colors.ForEach(color =>
                        {
                            WriteFloatColor(color, geometryData);
                            GLTFConverter.Align(geometryData, (int)geometryData.Writer.BaseStream.Position, 4);
                        });
                        
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        Colors.ForEach(color =>
                        {
                            geometryData.Writer.Write(color.ConvertToNormalizedByteArray(Type));
                            GLTFConverter.Align(geometryData, (int)geometryData.Writer.BaseStream.Position, 4);
                        });
                        
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                        Colors.ForEach(color =>
                        {
                            geometryData.Writer.Write(color.ConvertToNormalizedUShortArray(Type));
                            GLTFConverter.Align(geometryData, (int)geometryData.Writer.BaseStream.Position, 4);
                        });
                        
                        break;
                    default:
                        throw new NotSupportedException($"The color component type {ComponentType} is not supported!");
                }
            }
            public override bool IsNormalized()
            {
                return normalized;
            }

            public override void Write(Data geometryData, IEnumerable<int> indices)
            {
                int index = indices.ElementAt(0);
                switch (ComponentType)
                {
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT:
                        WriteFloatColor(Colors.ElementAt(index), geometryData);
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        geometryData.Writer.Write(Colors.ElementAt(index).ConvertToNormalizedByteArray(Type));
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                        geometryData.Writer.Write(Colors.ElementAt(index).ConvertToNormalizedUShortArray(Type));
                        break;
                }
            }
        }
    }
}
