using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Minesweeper.Engine.Utils
{
    public static class SerializationUtils
    {
        public static string ToBase64<T>(T[] array) where T : unmanaged => Convert.ToBase64String(MemoryMarshal.AsBytes(array.AsSpan()));
        public static T[] FromBase64<T>(string base64) where T : unmanaged
        {
            byte[] bytes = Convert.FromBase64String(base64);
            T[] array = new T[bytes.Length / Unsafe.SizeOf<T>()];
            bytes.AsSpan().CopyTo(MemoryMarshal.AsBytes(array.AsSpan()));
            return array;
        }
    }
}
