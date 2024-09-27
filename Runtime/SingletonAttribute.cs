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
            Address = address;
        }
    }
}
