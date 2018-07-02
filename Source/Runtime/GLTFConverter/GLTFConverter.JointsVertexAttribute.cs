using glTFLoader.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetGenerator.Runtime.GLTFConverter
{
    internal partial class GLTFConverter
    {
        private sealed class JointsVertexAttribute : VertexAttribute
        {
            private readonly Accessor.ComponentTypeEnum JointsComponentType;
            private readonly IEnumerable<IEnumerable<JointWeight>> VertexJointWeights;
            private readonly GLTFConverter glTFConverter;
            public JointsVertexAttribute(GLTFConverter glTFConverter, IEnumerable<IEnumerable<JointWeight>> vertexJointWeights, MeshPrimitive.JointComponentTypeEnum jointsComponentType)
            {
                this.glTFConverter = glTFConverter;
                VertexJointWeights = vertexJointWeights;
                switch (jointsComponentType)
                {
                    case MeshPrimitive.JointComponentTypeEnum.UNSIGNED_BYTE:
                        JointsComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                        break;
                    case MeshPrimitive.JointComponentTypeEnum.UNSIGNED_SHORT:
                        JointsComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                        break;
                    default:
                        throw new NotSupportedException($"The joints component type {jointsComponentType} is not supported!");
                }
            }

            public override void Write(Data geometryData)
            {
                switch (JointsComponentType)
                {
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        VertexJointWeights.ForEach(vertexJointWeight =>
                        {
                            void writeWeight(JointWeight jw, Data data) => data.Writer.Write(this.glTFConverter.ConvertNodeToSchema(jw.Joint.Node) * byte.MaxValue);
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
                            void writeWeight(JointWeight jw, Data data) => data.Writer.Write(this.glTFConverter.ConvertNodeToSchema(jw.Joint.Node) * ushort.MaxValue);
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
                        throw new NotSupportedException($"The weights component type {JointsComponentType} is not supported!");
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
                    throw new Exception("vertex cannot have more than four joints!");
                }
                writeVertexWeights(vertexWeights, geometryData);
                writePadding(count, geometryData);
            }
            public override bool IsNormalized()
            {
                return false;
            }

            public override void Write(Data geometryData, int index)
            {
                switch (JointsComponentType)
                {
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        void writeFloatWeights(IEnumerable<JointWeight> vertexWeights, Data gData)
                        {
                            vertexWeights.ForEach(vertexWeight =>
                            {
                                gData.Writer.Write(this.glTFConverter.ConvertNodeToSchema(vertexWeight.Joint.Node) * byte.MaxValue);
                            });
                        };
                        void writeFloatPadding(int num, Data data)
                        {
                            for (int i = 0; i < num; ++i)
                            {
                                data.Writer.Write(Convert.ToByte(0));
                            }
                        }
                        WriteToBufferInterleaved(geometryData, index, writeFloatWeights, writeFloatPadding);
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                        void writeByteWeights(IEnumerable<JointWeight> vertexWeights, Data gData)
                        {
                            vertexWeights.ForEach(vertexWeight =>
                            {
                                gData.Writer.Write(this.glTFConverter.ConvertNodeToSchema(vertexWeight.Joint.Node) * ushort.MaxValue);
                            });
                        };
                        void writeBytePadding(int num, Data data)
                        {
                            for (int i = 0; i < num; ++i)
                            {
                                data.Writer.Write(Convert.ToUInt16(0));
                            }
                        }
                        WriteToBufferInterleaved(geometryData, index, writeByteWeights, writeBytePadding);
                        break;

                    default:
                        throw new NotSupportedException($"The joint component type {JointsComponentType} is not supported!");
                }
            }
            public override Accessor.ComponentTypeEnum GetAccessorComponentType()
            {
                return JointsComponentType;
            }

            public override Accessor.TypeEnum GetAccessorType()
            {
                return Accessor.TypeEnum.VEC4;
            }
        }
    }
}
