using MongoDB.Bson;

namespace Tweetus.Web.Utilities.Extensions
{
    public static class StringExtensions
    {
        public static ObjectId ToObjectId(this string str)
        {
            return new ObjectId(str);
        }

        public static string ToAsciiString(this string text)
        {
            string result = string.Empty;

            foreach (char c in text)
            {
                int intValue = (int)c;
                result += intValue.ToString();
            }

            return result;
        }
    }
}
