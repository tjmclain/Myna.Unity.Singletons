using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

using UnityObject = UnityEngine.Object;

namespace Myna.Unity.Singletons
{
    public static class SingletonUtility
    {
        private static readonly Dictionary<Type, UnityObject> _singletons = new();
        private static bool _applicationStateIsChanging = false;

        public static bool IsInitialized<T>() where T : UnityObject
        {
            return TryGetSingleton<T>(out _) && !_applicationStateIsChanging;
        }

        public static T GetOrInitSingletonAsset<T>() where T : ScriptableObject
        {
            var type = typeof(T);
            if (TryGetSingleton(out T instance))
            {
                return instance;
            }

            if (_applicationStateIsChanging)
            {
                Debug.LogWarning(
                    $"{nameof(GetOrInitSingletonAsset)}: Could not get instance because the application state is changing"
                );
                return null;
            }

            if (TryLoadSingleton(out instance))
            {
                _singletons[type] = instance;
                Debug.Log(
                    $"{nameof(GetOrInitSingletonAsset)}: load from resources, type = {type.Name}",
                    instance
                );
                return instance;
            }

            instance = ScriptableObject.CreateInstance<T>();
            Debug.Log(
                $"{nameof(GetOrInitSingletonAsset)}: creating new instance, type = {type.Name}",
                instance
            );
            return instance;
        }

        public static T GetOrInitSingletonBehaviour<T>() where T : MonoBehaviour
        {
            if (TryGetSingleton(out T instance))
            {
                return instance;
            }

            if (_applicationStateIsChanging)
            {
                Debug.LogWarning(
                    $"{nameof(GetOrInitSingletonBehaviour)}: Could not get instance because the application state is changing"
                );
                return null;
            }

            instance = UnityObject.FindObjectOfType<T>();
            if (instance != null)
            {
                SetSingleton(instance);
                Debug.Log(
                    $"{nameof(GetOrInitSingletonBehaviour)}: found in scene, type = {typeof(T).Name}",
                    instance
                );
                return instance;
            }

            if (TryLoadSingleton<T>(out var prefab))
            {
                instance = UnityObject.Instantiate(prefab);
                SetSingleton(instance);
                Debug.Log(
                    $"{nameof(GetOrInitSingletonBehaviour)}: load from resources, type = {typeof(T).Name}",
                    instance
                );
                return instance;
            }

            GameObject go = new() { name = typeof(T).Name };
            instance = go.AddComponent<T>();
            SetSingleton(instance);

            Debug.Log(
                $"{nameof(GetOrInitSingletonBehaviour)}: creating new instance, type = {typeof(T).Name}",
                instance
            );
            return instance;
        }

        public static bool TryGetSingleton<T>(out T singleton) where T : UnityObject
        {
            if (!_singletons.TryGetValue(typeof(T), out var obj))
            {
                singleton = default;
                return false;
            }

            singleton = obj as T;
            return singleton != null;
        }

        public static IEnumerator InitializeSingletonAsync<T>() where T : UnityObject
        {
            if (IsInitialized<T>())
            {
                Debug.Log(
                    $"{nameof(InitializeSingletonAsync)}: singleton of type '{typeof(T).Name}' is already initialized."
                );
                yield break;
            }

            if (_applicationStateIsChanging)
            {
                Debug.LogWarning(
                    $"{nameof(InitializeSingletonAsync)}: Failed to initialize because the application state is changing"
                );
                yield break;
            }

            bool typeIsMonoBehaviour = TypeIsMonoBehaviour<T>();
            if (typeIsMonoBehaviour)
            {
                var sceneInstance = UnityObject.FindObjectOfType<T>();
                if (sceneInstance != null)
                {
                    SetSingleton(sceneInstance);
                    Debug.Log(
                        $"{nameof(InitializeSingletonAsync)}: found instance in scene of type '{typeof(T).Name}'",
                        sceneInstance
                    );
                    yield break;
                }
            }

            var attribute = GetSingletonAttribute<T>();
            if (attribute == null)
            {
                Debug.LogError(
                    $"{nameof(LoadFromResourcesAsync)}: Could not get {nameof(SingletonAttribute)} for type '{typeof(T).Name}'"
                );
                yield break;
            }

            var loadRoutine = attribute.Source switch
            {
                SingletonAttribute.AssetSource.Addressables
                    => LoadFromAddressablesAsync<T>(attribute.Path),
                _ => LoadFromResourcesAsync<T>(attribute.Path)
            };

            T singleton = null;
            while (loadRoutine.MoveNext())
            {
                var current = loadRoutine.Current;
                singleton = current as T;
                if (singleton != null)
                {
                    break;
                }
                yield return current;
            }

            if (singleton == null)
            {
                Debug.LogError(
                    $"{nameof(LoadFromResourcesAsync)}: Failed to load singleton of type '{typeof(T).Name}'"
                );
                yield break;
            }

            if (typeIsMonoBehaviour)
            {
                singleton = UnityObject.Instantiate(singleton);
                Debug.Log(
                    $"{nameof(LoadFromResourcesAsync)}: Instantiated {typeof(MonoBehaviour).Name} singleton of type '{typeof(T).Name}'"
                );
            }

            SetSingleton(singleton);
        }

        public static void SetSingleton<T>(T instance) where T : UnityObject
        {
            _singletons[typeof(T)] = instance;
        }

        public static void DisposeSingleton<T>() where T : UnityObject
        {
            if (!TryGetSingleton(out T singleton))
            {
                Debug.LogWarning(
                    $"{nameof(DisposeSingleton)}: No registered singleton of type '{typeof(T).Name}'"
                );
                return;
            }

            _singletons.Remove(typeof(T));

            if (singleton is MonoBehaviour monoBehaviour)
            {
                if (Application.isPlaying)
                {
                    UnityObject.Destroy(monoBehaviour.gameObject);
                }
                else
                {
                    UnityObject.DestroyImmediate(monoBehaviour.gameObject);
                }

                Debug.Log(
                    $"{nameof(DisposeSingleton)}: Destroyed singleton game object for type '{typeof(T).Name}'"
                );
            }
        }

        public static SingletonAttribute GetSingletonAttribute<T>()
        {
            return typeof(T).GetCustomAttribute<SingletonAttribute>();
        }

        public static bool TypeIsMonoBehaviour<T>()
        {
            return typeof(MonoBehaviour).IsAssignableFrom(typeof(T));
        }

        public static bool TryGetSingletonPath<T>(out string path)
        {
            var attribute = GetSingletonAttribute<T>();
            if (attribute == null)
            {
                Debug.LogError(
                    $"{nameof(TryGetSingletonPath)}: Could not get {nameof(SingletonAttribute)} for type '{typeof(T).Name}'"
                );
                path = string.Empty;
                return false;
            }

            if (string.IsNullOrEmpty(attribute.Path))
            {
                Debug.LogError(
                    $"{nameof(TryGetSingletonPath)}: Path for singleton type '{typeof(T).Name}' is empty"
                );
                path = string.Empty;
                return false;
            }

            path = attribute.Path;
            return true;
        }

        public static bool TryLoadSingleton<T>(out T asset) where T : UnityObject
        {
            var attribute = GetSingletonAttribute<T>();
            if (attribute == null)
            {
                Debug.LogError(
                    $"{nameof(TryLoadSingleton)}: Could not get {nameof(SingletonAttribute)} for type '{typeof(T).Name}'"
                );
                asset = default;
                return false;
            }

            return attribute.Source switch
            {
                SingletonAttribute.AssetSource.Addressables
                    => TryLoadFromAddressables(attribute.Path, out asset),
                _ => TryLoadFromResources(attribute.Path, out asset)
            };
        }

        public static bool TryLoadFromResources<T>(out T asset) where T : UnityObject
        {
            if (!TryGetSingletonPath<T>(out string path))
            {
                asset = default;
                return false;
            }

            return TryLoadFromResources(path, out asset);
        }

        public static bool TryLoadFromResources<T>(string path, out T asset) where T : UnityObject
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError(
                    $"{nameof(TryLoadFromResources)}: empty singleton path for type '{typeof(T).Name}'"
                );
                asset = default;
                return false;
            }

            asset = Resources.Load<T>(path);
            return asset != null;
        }

        /// <remarks>
        /// This could result in a performance hit
        /// Prefer using <see cref="InitializeSingletonAsync"/> or <see cref="LoadFromAddressablesAsync"/>
        /// </remarks>
        public static bool TryLoadFromAddressables<T>(out T asset) where T : UnityObject
        {
            if (!TryGetSingletonPath<T>(out string path))
            {
                asset = default;
                return false;
            }

            return TryLoadFromAddressables(path, out asset);
        }

        /// <remarks>
        /// This could result in a performance hit
        /// Prefer using <see cref="InitializeSingletonAsync"/> or <see cref="LoadFromAddressablesAsync"/>
        /// </remarks>
        public static bool TryLoadFromAddressables<T>(string path, out T asset)
            where T : UnityObject
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError(
                    $"{nameof(TryLoadFromAddressables)}: empty singleton path for type '{typeof(T).Name}'"
                );
                asset = default;
                return false;
            }

            bool typeIsMonoBehaviour = TypeIsMonoBehaviour<T>();
            if (typeIsMonoBehaviour)
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(path);
                handle.WaitForCompletion();

                if (handle.Result is not GameObject gameObject)
                {
                    Debug.LogError(
                        $"{nameof(TryLoadFromAddressables)}: handle.Result is not a GameObject; T = {typeof(T).Name}"
                    );
                    asset = default;
                    return false;
                }

                asset = gameObject.GetComponent(typeof(T)) as T;
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<T>(path);
                handle.WaitForCompletion();

                asset = handle.Result;
            }

            if (asset == null)
            {
                Debug.LogError(
                    $"{nameof(TryLoadFromAddressables)}: asset is not type '{typeof(T).Name}'; path = {path}"
                );
                return false;
            }

            return true;
        }

        public static IEnumerator LoadFromResourcesAsync<T>(string path = "") where T : UnityObject
        {
            // Validate our 'path' parameter
            if (string.IsNullOrEmpty(path) && !TryGetSingletonPath<T>(out path))
            {
                Debug.LogError(
                    $"{nameof(LoadFromResourcesAsync)}: Could not find {nameof(path)} for type '{typeof(T).Name}'"
                );
                yield break;
            }

            var request = Resources.LoadAsync<T>(path);
            yield return request;
            yield return request.asset;
        }

        public static IEnumerator LoadFromAddressablesAsync<T>(string path = "")
            where T : UnityObject
        {
            // Validate our 'path' parameter
            if (string.IsNullOrEmpty(path) && !TryGetSingletonPath<T>(out path))
            {
                Debug.LogError(
                    $"{nameof(LoadFromAddressablesAsync)}: Could not find {nameof(path)} for type '{typeof(T).Name}'"
                );
                yield break;
            }

            bool typeIsMonoBehaviour = TypeIsMonoBehaviour<T>();
            if (typeIsMonoBehaviour)
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(path);
                if (!handle.IsDone)
                {
                    yield return handle;
                }

                if (handle.Result is not GameObject gameObject)
                {
                    Debug.LogError(
                        $"{nameof(LoadFromAddressablesAsync)}: handle.Result is not a GameObject; T = {typeof(T).Name}"
                    );
                    yield break;
                }

                T monoBehaviour = gameObject.GetComponent(typeof(T)) as T;
                yield return monoBehaviour;
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<T>(path);
                if (!handle.IsDone)
                {
                    yield return handle;
                }

                yield return handle.Result;
            }
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
