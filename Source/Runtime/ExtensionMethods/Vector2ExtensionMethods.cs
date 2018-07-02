using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AssetGenerator.Runtime.ExtensionMethods
{
    internal static class Vector2ExtensionMethods
    {
        public static IEnumerable<byte> ConvertToNormalizedByteArray(this Vector2 source)
        {
            return new[]
            {
                Convert.ToByte(Math.Round(source.X * byte.MaxValue)),
                Convert.ToByte(Math.Round(source.Y * byte.MaxValue)),
            };
        }
        public static IEnumerable<ushort> ConvertToNormalizedUShortArray(this Vector2 source)
        {
            return new[]
            {
                Convert.ToUInt16(Math.Round(source.X * ushort.MaxValue)),
                Convert.ToUInt16(Math.Round(source.Y * ushort.MaxValue)),
            };
        }
    }
}
