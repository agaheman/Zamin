namespace Zamin.Utilities.Eve.Extensions
{
    public static class StringExtensions
    {
        public static string ToPascalCase(this string s)
        {

            var words = s.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length > 1)
                return ConcatWordsToPascalCaseString(words);
            else
                return GetStringValuePascalCase(s);

        }

        private static string ConcatWordsToPascalCaseString(string[] words)
        {
            words.Select(word => word.Substring(0, 1).ToUpper() +
                                                     word.Substring(1).ToLower());

            var result = string.Concat(words);
            return result;
        }

        private static string GetStringValuePascalCase(string stringValue)
        {
            return stringValue.Substring(0, 1).ToUpper() + stringValue.Substring(1);
        }


        public static string UniformPersianCharacters(this string inputString)
        {
            return inputString.Replace((char)1610, (char)1740).Replace((char)1603, (char)1705);
        }

        public static void UniformPersianCharacters<T>(this T inputObject) where T : class
        {
            var objectProps = inputObject.GetType().GetProperties();
            foreach (var prop in objectProps)
            {
                var t = prop.PropertyType;
                if (prop.PropertyType == typeof(string) && prop.CanWrite)
                {
                    var propValue = prop.GetValue(inputObject);
                    if (propValue != null)
                    {
                        var uniformedValue = propValue.ToString().UniformPersianCharacters();
                        prop.SetValue(inputObject, uniformedValue);
                    }
                }
            }
        }
    }
}