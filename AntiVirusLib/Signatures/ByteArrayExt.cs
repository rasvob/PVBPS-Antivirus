using System.Text;

namespace AntiVirusLib.Signatures
{
    public static class ByteArrayExt
    {
        public static string ToHex(this byte[] bytes, bool upperCase = false)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            foreach (byte t in bytes)
                result.Append(t.ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }
    }
}