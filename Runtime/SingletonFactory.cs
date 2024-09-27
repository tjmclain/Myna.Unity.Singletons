using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Myna.Unity.Singletons
{
    public static class SingletonFactory
    {
        private static bool _applicationStateIsChanging = false;

        public static T LoadAsset<T>() where T : Object
        {
            var attribute = GetSingletonAttribute<T>();
            if (attribute == null)
            {
                Debug.LogError($"LoadAsset: Type '{typeof(T)}' is missing a SingletonAttribute.");
                return default;
            }

            var handle = Addressables.LoadAssetAsync<T>(attribute.Address);
            var asset = handle.WaitForCompletion();
            Addressables.Release(handle);
            if (asset == null)
            {
                Debug.LogError(
                    $"LoadAsset: Failed to load asset of type '{typeof(T)}' at address '{attribute.Address}.'"
                );
            }
            return asset;
        }

        public static async Task<T> LoadAssetAsync<T>() where T : Object
        {
            var attribute = GetSingletonAttribute<T>();
            if (attribute == null)
            {
                Debug.LogError(
                    $"LoadAssetAsync: Type '{typeof(T)}' is missing a SingletonAttribute."
                );
                return default;
            }

            var handle = Addressables.LoadAssetAsync<T>(attribute.Address);
            await handle.Task;

            var asset = handle.Result;
            Addressables.Release(handle);
            if (asset == null)
            {
                Debug.LogError(
                    $"LoadAssetAsync: Failed to load asset of type '{typeof(T)}' at address '{attribute.Address}.'"
                );
            }
            return asset;
        }

        public static T CreateInstance<T>() where T : Object
        {
            if (_applicationStateIsChanging)
            {
                Debug.Log(
                    "CreateInstance: cannot create instance while the application is quitting or changing state."
                );
                return null;
            }

            var asset = LoadAsset<T>();
            return asset != null ? Object.Instantiate(asset) : null;
        }

        public static async Task<T> CreateInstanceAsync<T>() where T : Object
        {
            if (_applicationStateIsChanging)
            {
                Debug.Log(
                    "CreateInstanceAsync: cannot create instance while the application is quitting or changing state."
                );
                return null;
            }

            var asset = await LoadAssetAsync<T>();
            return asset != null ? Object.Instantiate(asset) : null;
        }

        private static SingletonAttribute GetSingletonAttribute<T>()
        {
            return typeof(T).GetCustomAttribute<SingletonAttribute>(false);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            _applicationStateIsChanging = false;

            Application.quitting -= OnApplicationQuit;
            Application.quitting += OnApplicationQuit;
        }

        private static void OnApplicationQuit()
        {
            _applicationStateIsChanging = true;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInitializeOnLoad()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
        }

        private static void OnPlaymodeStateChanged(UnityEditor.PlayModeStateChange playModeState)
        {
            _applicationStateIsChanging = playModeState switch
            {
                UnityEditor.PlayModeStateChange.ExitingEditMode => true,
                UnityEditor.PlayModeStateChange.ExitingPlayMode => true,
                _ => false
            };
        }
#endif
    }
}
