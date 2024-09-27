using System.Threading.Tasks;
using UnityEngine;

namespace Myna.Unity.Singletons
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        #region Properties
        public T Instance
        {
            get => _instance;
            set => SetInstance(value);
        }

        public static bool IsInitialized => _instance != null;
        #endregion

        #region Public Methods
        public static T GetOrCreateInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            var instance = SingletonFactory.CreateBehaviour<T>();
            SetInstance(instance);
            return instance;
        }

        public static async Task<T> GetOrCreateInstanceAsync()
        {
            if (_instance != null)
            {
                return _instance;
            }

            var instance = await SingletonFactory.CreateBehaviourAsync<T>();
            SetInstance(instance);
            return instance;
        }

        public static void SetInstance(T instance)
        {
            if (instance != null)
            {
                DontDestroyOnLoad(instance.gameObject);
            }
            _instance = instance;
        }
        #endregion Public Methods

        #region MonoBehaviour Implementation
        protected virtual void Awake()
        {
            SetInstance(this as T);
        }
        #endregion MonoBehaviour Implementation
    }
}
