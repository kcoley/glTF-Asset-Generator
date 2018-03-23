using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AssetGenerator
{
    internal class MGProperty
    {
        private object Value;
        public List<Vector3> GetVectors()
        {
            var v = Value as List<Vector3>;
            return v;
        }
    }
}
