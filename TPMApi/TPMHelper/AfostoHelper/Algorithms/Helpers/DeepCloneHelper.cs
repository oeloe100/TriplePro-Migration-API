using Newtonsoft.Json;

namespace TPMHelper.AfostoHelper.Algorithms.Helpers
{
    public static class DeepCloneHelper
    {
        internal static T DeepClone<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (source == null)
            {
                return default;
            }

            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }
    }
}
