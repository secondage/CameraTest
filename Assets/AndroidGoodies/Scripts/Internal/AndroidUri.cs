#if UNITY_ANDROID
using UnityEngine;

namespace DeadMosquito.AndroidGoodies
{
    static class AndroidUri
    {
        private const string UriClass = "android.net.Uri";

        public static AndroidJavaObject Parse(string uriString)
        {
            using (var uriClass = new AndroidJavaClass(UriClass))
            {
                return uriClass.CallStatic<AndroidJavaObject>("parse", uriString);
            }
        }

        public static AndroidJavaObject FromFile(string filePath)
        {
            using (var uriClass = new AndroidJavaClass(UriClass))
            {
                return uriClass.CallStatic<AndroidJavaObject>("fromFile", new AndroidJavaObject("java.io.File", filePath));
            }
        }
    }
}
#endif