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
        private sealed class TangentVertexAttribute : VertexAttribute
        { 
            private readonly IEnumerable<Vector4> Tangents;
            public TangentVertexAttribute(IEnumerable<Vector4> tangents)
            {
                Tangents = tangents;
            }

            public override void Write(Data geometryData)
            {
                geometryData.Writer.Write(Tangents);
            }

            public override void Write(Data geometryData, IEnumerable<int> indices)
            {
                int index = indices.ElementAt(0);
                geometryData.Writer.Write(Tangents.ElementAt(index));
            }

            public override Accessor.ComponentTypeEnum GetAccessorComponentType()
            {
                return glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
            }

            public override Accessor.TypeEnum GetAccessorType()
            {
                return glTFLoader.Schema.Accessor.TypeEnum.VEC4;
            }
        }
    }
}
