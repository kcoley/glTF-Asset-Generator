using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AssetGenerator.Runtime.ExtensionMethods
{
    internal static class Vector4ExtensionMethods
    {
        public static IEnumerable<byte> ConvertToNormalizedByteArray(this Vector4 source, glTFLoader.Schema.Accessor.TypeEnum type)
        {
            var results = new List<byte> 
            {
                Convert.ToByte(Math.Round(source.X * byte.MaxValue)),
                Convert.ToByte(Math.Round(source.Y * byte.MaxValue)),
                Convert.ToByte(Math.Round(source.Z * byte.MaxValue)),        
            };
            if (type == glTFLoader.Schema.Accessor.TypeEnum.VEC4)
            {
                results.Add(Convert.ToByte(Math.Round(source.W * byte.MaxValue)));
            }

            return results;
        }
        public static IEnumerable<ushort> ConvertToNormalizedUShortArray(this Vector4 source, glTFLoader.Schema.Accessor.TypeEnum type)
        {
            var results = new List<ushort>
            {
                Convert.ToUInt16(Math.Round(source.X * ushort.MaxValue)),
                Convert.ToUInt16(Math.Round(source.Y * ushort.MaxValue)),
                Convert.ToUInt16(Math.Round(source.Z * ushort.MaxValue)),      
            };
            if (type == glTFLoader.Schema.Accessor.TypeEnum.VEC4)
            {
                results.Add(Convert.ToUInt16(Math.Round(source.W * ushort.MaxValue)));
            }

            return results;
        }
    }
}
