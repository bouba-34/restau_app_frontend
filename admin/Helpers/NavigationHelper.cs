using System.Web;

namespace admin.Helpers
{
    public static class NavigationHelper
    {
        public static Dictionary<string, object> BuildNavigationParameters(object parameters)
        {
            var dict = new Dictionary<string, object>();
            
            // Handle null
            if (parameters == null)
                return dict;

            // Get properties via reflection
            var properties = parameters.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(parameters);
                if (value != null)
                {
                    dict.Add(property.Name, value);
                }
            }

            return dict;
        }

        public static string CreateQueryString(object parameters)
        {
            if (parameters == null)
                return string.Empty;
                
            var properties = parameters.GetType().GetProperties();
            
            var queryParams = new List<string>();
            foreach (var property in properties)
            {
                var value = property.GetValue(parameters);
                if (value != null)
                {
                    queryParams.Add($"{property.Name}={HttpUtility.UrlEncode(value.ToString())}");
                }
            }

            if (queryParams.Count == 0)
                return string.Empty;
                
            return "?" + string.Join("&", queryParams);
        }
    }
}