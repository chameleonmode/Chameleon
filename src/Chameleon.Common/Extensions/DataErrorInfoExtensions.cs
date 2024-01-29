using System.ComponentModel;
using System.Reflection;

namespace Chameleon.Common.Extensions
{
    public static class DataErrorInfoExtensions
    {
        public static bool IsModelValid<T>(this T model) where T: IDataErrorInfo
        {
            bool isValid = true;
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            foreach (PropertyInfo p in properties)
            {
                if (!p.CanWrite || !p.CanRead)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(model[p.Name]))
                {
                    isValid = false;
                }
            }
            return isValid;
        }
    }
}
