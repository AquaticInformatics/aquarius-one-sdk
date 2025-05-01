namespace ONE.ClientSDK.Utilities
{
    public static class HelperExtensions
    {
        public static bool IsSuccessStatusCode(this int statusCode) => statusCode >= 200 && statusCode <= 299;
    }
}