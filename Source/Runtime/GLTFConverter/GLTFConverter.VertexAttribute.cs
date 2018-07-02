using System;
using System.Collections.Generic;
using System.Text;

namespace AssetGenerator.Runtime.GLTFConverter
{
    internal partial class GLTFConverter
    {
        private abstract class VertexAttribute
        {
            public abstract glTFLoader.Schema.Accessor.ComponentTypeEnum GetAccessorComponentType();

            public abstract glTFLoader.Schema.Accessor.TypeEnum GetAccessorType();

            public virtual bool IsNormalized()
            {
                return false;
            }
            public abstract void Write(Data geometryData);
            public abstract void Write(Data geometryData, IEnumerable<int> indices);
        }
    }
}
