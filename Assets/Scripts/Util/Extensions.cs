
namespace Assets.Scripts.Util
{
    public static class Extensions
    {
        public static string Repeat(this string str, int times)
        {
            string result = "";
            for (int i = 0; i < times; i++)
            {
                result += str;
            }
            return result;
        }
    }
}