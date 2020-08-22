using Microsoft.Toolkit.Uwp.Helpers;
using Windows.ApplicationModel.Resources;

namespace Project_Alphonse_Elric.Helpers
{
    public static class ResourceExtensions
    {
        private static ResourceLoader _resLoader = new ResourceLoader();

        private static readonly RoamingObjectStorageHelper _roamingObjectStorage = new RoamingObjectStorageHelper();

        public static string GetLocalized(this string resourceKey)
        {
            return _resLoader.GetString(resourceKey);
        }
    }
}
