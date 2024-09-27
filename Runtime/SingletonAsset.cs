using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Myna.Unity.Singletons
{
    public class SingletonAsset<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        #region Properties
        public static T Instance
        {
            get => _instance;
            set => SetInstance(value);
        }

        public static bool IsInitialized => _instance != null;
        #endregion Properties

        #region Public Methods
        public static T GetOrCreateInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            var instance = SingletonFactory.CreateAsset<T>();
            SetInstance(instance);
            return instance;
        }

        public static async Task<T> GetOrCreateInstanceAsync()
        {
            if (_instance != null)
            {
                return _instance;
            }

            var instance = await SingletonFactory.CreateAssetAsync<T>();
            SetInstance(instance);
            return instance;
        }

        public static void SetInstance(T instance)
        {
            _instance = instance;
        }
        #endregion Public Methods
    }
}
