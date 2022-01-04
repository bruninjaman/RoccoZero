using System.Text;

namespace Overwolf.Exstensions
{
    internal static class StringExstensions
    {
        internal static string Windows1251ToUtf8(this string windows1251String)
        {
            byte[] www = Encoding.GetEncoding("Windows-1251").GetBytes(windows1251String);
            //byte[] www8 = Encoding.Convert(Encoding.GetEncoding("Windows-1251"), Encoding.UTF8, www);
            return Encoding.UTF8.GetString(www);
        }
    }
}