using glTFLoader.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AssetGenerator.Runtime.GLTFConverter
{
    internal partial class GLTFConverter
    {
        private sealed class PositionVertexAttribute : VertexAttribute
        {
            private readonly glTFLoader.Schema.Accessor.ComponentTypeEnum ComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
            private readonly glTFLoader.Schema.Accessor.TypeEnum Type = glTFLoader.Schema.Accessor.TypeEnum.VEC3;
            private IEnumerable<Vector3> Positions;
            public PositionVertexAttribute(IEnumerable<Vector3> positions)
            {
                Positions = positions;
            }

            public override void Write(Data geometryData)
            {
                geometryData.Writer.Write(Positions);
            }

            public override void Write(Data geometryData, int index)
            {
                geometryData.Writer.Write(Positions.ElementAt(index));
            }
            public override Accessor.ComponentTypeEnum GetAccessorComponentType()
            {
                return glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
            }

            public override Accessor.TypeEnum GetAccessorType()
            {
                return glTFLoader.Schema.Accessor.TypeEnum.VEC3;
            }
        }
    }
}
