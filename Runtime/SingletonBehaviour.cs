using System.Threading.Tasks;
using UnityEngine;

namespace Myna.Unity.Singletons
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public T Instance
        {
            get => _instance;
            set => SetInstance(value);
        }

        public static bool IsInitialized => _instance != null;

        public static void SetInstance(T instance)
        {
            if (instance != null)
            {
                DontDestroyOnLoad(instance.gameObject);
            }
            _instance = instance;
        }

        public static T GetOrCreateInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = SingletonFactory.CreateInstance<T>();
            return _instance;
        }

        public static async Task<T> InitializeAsync()
        {
            var instance = await SingletonFactory.CreateInstanceAsync<T>();
            SetInstance(instance);
            return instance;
        }

        protected virtual void Awake()
        {
            SetInstance(this as T);
        }
    }
}
