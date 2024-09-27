using System.Threading.Tasks;
using UnityEngine;

namespace Myna.Unity.Singletons
{
    public class SingletonAsset<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        public static T Instance
        {
            get => _instance;
            set => SetInstance(value);
        }

        public static bool IsInitialized => _instance != null;

        public static T GetOrCreateInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = SingletonFactory.LoadAsset<T>();
            return _instance;
        }

        public static void SetInstance(T instance)
        {
            if (instance != null)
            {
                DontDestroyOnLoad(instance);
            }

            _instance = instance;
        }

        public static async Task<T> InitializeAsync()
        {
            var instance = await SingletonFactory.LoadAssetAsync<T>();
            SetInstance(instance);
            return instance;
        }
    }
}
