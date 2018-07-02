using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetGenerator.Runtime.GLTFConverter
{
    internal partial class GLTFConverter
    {
        private sealed class WeightsVertexAttribute : VertexAttribute
        {
            private readonly glTFLoader.Schema.Accessor.ComponentTypeEnum WeightsComponentType;
            private readonly bool WeightsNormalized;
            private readonly IEnumerable<IEnumerable<JointWeight>> VertexJointWeights;
            public WeightsVertexAttribute(IEnumerable<IEnumerable<JointWeight>> vertexJointWeights, MeshPrimitive.JointComponentTypeEnum jointsComponentType, MeshPrimitive.WeightComponentTypeEnum weightsComponentType)
            {
                VertexJointWeights = vertexJointWeights;

                switch (weightsComponentType)
                {
                    case MeshPrimitive.WeightComponentTypeEnum.FLOAT:
                        WeightsComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                        WeightsNormalized = false;
                        break;
                    case MeshPrimitive.WeightComponentTypeEnum.NORMALIZED_UNSIGNED_BYTE:
                        WeightsNormalized = true;
                        WeightsComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                        break;
                    case MeshPrimitive.WeightComponentTypeEnum.NORMALIZED_UNSIGNED_SHORT:
                        WeightsNormalized = true;
                        WeightsComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                        break;
                    default:
                        throw new NotSupportedException($"The weights component type {weightsComponentType} is not supported!");
                }
            }

            public override void Write(Data geometryData)
            {
                switch (WeightsComponentType)
                {
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT:
                        VertexJointWeights.ForEach(vertexJointWeight =>
                        {
                            void writeWeight(JointWeight jw, Data data) => data.Writer.Write(jw.Weight);
                            void writePadding(int num, Data data)
                            {
                                for (int i = 0; i < num; ++i)
                                {
                                    data.Writer.Write(0.0f);
                                }
                            }
                            WriteToBuffer(geometryData, writeWeight, writePadding);
                        });
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        VertexJointWeights.ForEach(vertexJointWeight =>
                        {
                            void writeWeight(JointWeight jw, Data data) => data.Writer.Write(jw.Weight * byte.MaxValue);
                            void writePadding(int num, Data data)
                            {
                                for (int i = 0; i < num; ++i)
                                {
                                    data.Writer.Write(Convert.ToByte(0));
                                }
                            }
                            WriteToBuffer(geometryData, writeWeight, writePadding);
                        });
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                        VertexJointWeights.ForEach(vertexJointWeight =>
                        {
                            void writeWeight(JointWeight jw, Data data) => data.Writer.Write(jw.Weight * ushort.MaxValue);
                            void writePadding(int num, Data data)
                            {
                                for (int i = 0; i < num; ++i)
                                {
                                    data.Writer.Write(Convert.ToUInt16(0));
                                }
                            }
                            WriteToBuffer(geometryData, writeWeight, writePadding);
                        });
                        break;
                    default:
                        throw new NotSupportedException($"The weights component type {WeightsComponentType} is not supported!");
                }
            }
            private void WriteToBuffer(Data geometryData, Action<JointWeight, Data> writeJointOrWeight, Action<int, Data> writePadding)
            {
                VertexJointWeights.ForEach(vertexJointWeight =>
                {
                    int count = vertexJointWeight.Count();
                    if (count > 4)
                    {
                        throw new Exception("vertices cannot have more than four weights!");

                    }
                    vertexJointWeight.ForEach(jointWeight =>
                    {
                        writeJointOrWeight(jointWeight, geometryData);
                    });
                    writePadding(4 - count, geometryData);
                });
            }
            private void WriteToBufferInterleaved(Data geometryData, int index, Action<IEnumerable<JointWeight>, Data> writeVertexWeights, Action<int, Data> writePadding)
            {
                var vertexWeights = VertexJointWeights.ElementAt(index);
                int count = vertexWeights.Count();
                if (count > 4)
                {
                    throw new Exception("vertex cannot have more than four weights!");
                }
                writeVertexWeights(vertexWeights, geometryData);
                writePadding(count, geometryData);
            }
            public override bool IsNormalized()
            {
                return WeightsNormalized;
            }

            public override void Write(Data geometryData, IEnumerable<int> indices)
            {
                int vertexIndex = indices.ElementAt(0);
                switch (WeightsComponentType)
                {
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT:
                        void writeFloatWeights(IEnumerable<JointWeight> vertexWeights, Data gData)
                        {
                            vertexWeights.ForEach(vertexWeight =>
                            {
                                gData.Writer.Write(vertexWeight.Weight);
                            });
                        };
                        void writeFloatPadding(int num, Data data)
                        {
                            for (int i = 0; i < num; ++i)
                            {
                                data.Writer.Write(0.0f);
                            }
                        }
                        WriteToBufferInterleaved(geometryData, vertexIndex, writeFloatWeights, writeFloatPadding);
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        void writeByteWeights(IEnumerable<JointWeight> vertexWeights, Data gData)
                        {
                            vertexWeights.ForEach(vertexWeight =>
                            {
                                gData.Writer.Write(vertexWeight.Weight * byte.MaxValue);
                            });
                        };
                        void writeBytePadding(int num, Data data)
                        {
                            for (int i = 0; i < num; ++i)
                            {
                                data.Writer.Write(Convert.ToByte(0));
                            }
                        }
                        WriteToBufferInterleaved(geometryData, vertexIndex, writeByteWeights, writeBytePadding);
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                        void writeShortWeights(IEnumerable<JointWeight> vertexWeights, Data gData)
                        {
                            vertexWeights.ForEach(vertexWeight =>
                            {
                                gData.Writer.Write(vertexWeight.Weight * ushort.MaxValue);
                            });
                        };
                        void writeShortPadding(int num, Data data)
                        {
                            for (int i = 0; i < num; ++i)
                            {
                                data.Writer.Write(Convert.ToUInt16(0));
                            }
                        }
                        WriteToBufferInterleaved(geometryData, vertexIndex, writeShortWeights, writeShortPadding);
                        break;
                    default:
                        throw new NotSupportedException($"The weights component type {WeightsComponentType} is not supported!");
                }
            }
        }
    }
}
