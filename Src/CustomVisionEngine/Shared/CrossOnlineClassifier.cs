using System;

namespace Plugin.CustomVisionEngine
{
    /// <summary>
    /// Cross CustomVisionEngine
    /// </summary>
    public static class CrossOnlineClassifier
    {
        private static Lazy<IOnlineClassifier> implementation = new Lazy<IOnlineClassifier>(() => CreateOnlineClassifier(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets if the plugin is supported on the current platform.
        /// </summary>
        public static bool IsSupported => implementation.Value == null ? false : true;

        /// <summary>
        /// Current plugin implementation to use
        /// </summary>
        public static IOnlineClassifier Current
        {
            get
            {
                var ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }

                return ret;
            }
        }

        static IOnlineClassifier CreateOnlineClassifier()
        {
#pragma warning disable IDE0022 // Use expression body for methods
            return new OnlineClassifierImplementation();
#pragma warning restore IDE0022 // Use expression body for methods
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
}
