namespace Zamin.Utilities.Eve.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Get default value of special type
        /// </summary>
        /// <param name="type">Type to get default value</param>
        /// <returns></returns>
        public static object GetDefault(Type type)
        {
            if (!type.IsByRef)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        /// <summary>
        /// Get type of special property in a class
        /// </summary>
        /// <typeparam name="T">Class Type</typeparam>
        /// <param name="propertyName">name of filed in class</param>
        /// <returns></returns>
        public static Type GetPropertyType<T>(string propertyName)
        {
            Type myType = typeof(T);
            var pType = myType.GetProperties().FirstOrDefault(x => x.Name.ToLower() == propertyName.ToLower()).PropertyType;
            return pType;
        }

        public static object ChangeToNotNullType(object value, Type conversion)
        {
            var t = conversion;
            Type u = Nullable.GetUnderlyingType(t);

            if (u != null)
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }
    }
}
