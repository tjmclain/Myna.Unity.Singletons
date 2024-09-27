using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Myna.Unity.Singletons
{
    public static class SingletonFactory
    {
        private static bool _applicationStateIsChanging = false;

        #region Public Methods
        public static T CreateAsset<T>() where T : ScriptableObject
        {
            if (_applicationStateIsChanging)
            {
                Debug.LogWarning("CreateAsset: _applicationStateIsChanging");
                return null;
            }

            var asset = Load<T>();
            return asset != null ? asset : ScriptableObject.CreateInstance<T>();
        }

        public static async Task<T> CreateAssetAsync<T>() where T : ScriptableObject
        {
            if (_applicationStateIsChanging)
            {
                Debug.LogWarning("CreateAsset: _applicationStateIsChanging");
                return null;
            }

            var asset = await LoadAsync<T>();
            return asset != null ? asset : ScriptableObject.CreateInstance<T>();
        }

        public static T CreateBehaviour<T>() where T : Component
        {
            if (_applicationStateIsChanging)
            {
                Debug.LogWarning("CreateBehaviour: _applicationStateIsChanging");
                return null;
            }

            var asset = Load<T>();
            return asset != null
                ? Object.Instantiate(asset)
                : new GameObject(typeof(T).Name).AddComponent<T>();

        }

        public static async Task<T> CreateBehaviourAsync<T>() where T : Component
        {
            if (_applicationStateIsChanging)
            {
                Debug.LogWarning("CreateBehaviour: _applicationStateIsChanging");
                return null;
            }

            var asset = await LoadAsync<T>();
            return asset != null
                ? Object.Instantiate(asset)
                : new GameObject(typeof(T).Name).AddComponent<T>();

        }
        #endregion Public Methods

        #region Private Methods
        private static T Load<T>() where T : Object
        {
            var attribute = GetSingletonAttribute<T>();
            if (attribute == null)
            {
                Debug.LogWarning($"Load: attribute == null; type = {typeof(T)}");
                return null;
            }

            string address = attribute.Address;

            var asset = attribute.Source switch
            {
                SingletonAttribute.AssetSource.Resources => Resources.Load<T>(address),
                SingletonAttribute.AssetSource.Addressables => LoadAssetFromAddressables<T>(address),
                _ => null
            };

            if (asset == null)
            {
                Debug.LogWarning($"Load: asset == null; type = {typeof(T)}, address = {address}");
            }
            return asset;
        }

        private static async Task<T> LoadAsync<T>() where T : Object
        {
            var attribute = GetSingletonAttribute<T>();
            if (attribute == null)
            {
                Debug.LogWarning($"LoadAsync: attribute == null; type = {typeof(T)}");
                return null;
            }

            string address = attribute.Address;
            var asset = attribute.Source switch
            {
                SingletonAttribute.AssetSource.Resources => await LoadAssetFromResourcesAsync<T>(attribute.Address),
                SingletonAttribute.AssetSource.Addressables => await LoadAssetFromAddressablesAsync<T>(attribute.Address),
                _ => await Task.FromResult<T>(null)
            };

            if (asset == null)
            {
                Debug.LogWarning($"LoadAsync: asset == null; type = {typeof(T)}, address = {address}");
            }
            return asset;
        }

        private static T LoadAssetFromAddressables<T>(string address) where T : Object
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(address);
                var go = handle.WaitForCompletion();
                Addressables.Release(handle);
                return go != null && go.TryGetComponent(typeof(T), out Component component)
                    ? component as T : null;
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<T>(address);
                var asset = handle.WaitForCompletion();
                Addressables.Release(handle);
                return asset;
            }
        }

        private static async Task<T> LoadAssetFromResourcesAsync<T>(string path) where T : Object
        {
            var request = Resources.LoadAsync<T>(path);
            while (!request.isDone)
            {
                await Task.Yield();
            }
            return request.asset as T;
        }

        private static async Task<T> LoadAssetFromAddressablesAsync<T>(string address) where T : Object
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(address);
                await handle.Task;
                var go = handle.Result;
                Addressables.Release(handle);
                return go != null && go.TryGetComponent(typeof(T), out Component component)
                    ? component as T : null;
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<T>(address);
                var asset = handle.WaitForCompletion();
                Addressables.Release(handle);
                return asset;
            }
        }

        private static SingletonAttribute GetSingletonAttribute<T>()
        {
            return typeof(T).GetCustomAttribute<SingletonAttribute>(false);
        }

        #region Initialization
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
        #endregion Initialization
        #endregion Private Methods
    }
}
