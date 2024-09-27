using System;

namespace Myna.Unity.Singletons
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SingletonAttribute : System.Attribute
    {
        public enum AssetSource
        {
            Resources,
            Addressables
        }

        public AssetSource Source { get; set; } = AssetSource.Resources;

        public string Address { get; set; } = string.Empty;

        public SingletonAttribute(string address)
        {
            const string resourcePrefix = "Resources/";

            if (address.StartsWith(resourcePrefix))
            {
                Address = address[resourcePrefix.Length..];
                Source = AssetSource.Resources;
            }
            else
            {
                Address = address;
                Source = AssetSource.Addressables;
            }

        }

        public SingletonAttribute(string address, AssetSource source)
        {
            Address = address;
            Source = source;
        }
    }
}
