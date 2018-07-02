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

            public override void Write(Data geometryData, int index)
            {
                geometryData.Writer.Write(Normals.ElementAt(index));
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
