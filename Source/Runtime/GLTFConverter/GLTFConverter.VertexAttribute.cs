using System;
using System.Collections.Generic;
using System.Text;

namespace AssetGenerator.Runtime.GLTFConverter
{
    internal partial class GLTFConverter
    {
        private abstract class VertexAttribute
        {
            private readonly glTFLoader.Schema.Accessor.ComponentTypeEnum AccessorComponentType;
            private readonly glTFLoader.Schema.Accessor.TypeEnum AccessorType;

            public virtual glTFLoader.Schema.Accessor.ComponentTypeEnum GetAccessorComponentType()
            {
                return AccessorComponentType;
            }
            public virtual glTFLoader.Schema.Accessor.TypeEnum GetAccessorType()
            {
                return AccessorType;
            }
            public virtual bool IsNormalized()
            {
                return false;
            }
            public abstract void Write(Data geometryData);
            public abstract void Write(Data geometryData, IEnumerable<int> indices);
        }
    }
}
