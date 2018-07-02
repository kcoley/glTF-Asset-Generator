using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AssetGenerator.Runtime.GLTFConverter
{
    internal partial class GLTFConverter
    {
        private sealed class NormalVertexAttribute : VertexAttribute
        {
            private readonly glTFLoader.Schema.Accessor.ComponentTypeEnum ComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
            private readonly glTFLoader.Schema.Accessor.TypeEnum Type = glTFLoader.Schema.Accessor.TypeEnum.VEC3;
            private IEnumerable<Vector3> Normals;
            public NormalVertexAttribute(IEnumerable<Vector3> normals)
            {
                Normals = normals;
            }

            public override void Write(Data geometryData)
            {
                geometryData.Writer.Write(Normals);
            }

            public override void Write(Data geometryData, IEnumerable<int> indices)
            {
                int index = indices.ElementAt(0);
                geometryData.Writer.Write(Normals.ElementAt(index));
            }
        }
    }
}
