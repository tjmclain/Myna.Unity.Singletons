using UnityEngine;

public class SingletonAsset<T> : ScriptableObject where T : ScriptableObject
{
	private static T _instance;

	public bool IsInitialized => GetInstance() != null;

	public T Instance
	{
		get => GetInstance();
		set => SetInstance(value);
	}

	public static T GetInstance()
	{
		if (_instance != null)
		{
			return _instance;
		}

		_instance = SingletonUtility.GetOrInitSingletonAsset<T>();
		return _instance;
	}

	public static void SetInstance(T instance)
	{
		SingletonUtility.SetSingleton(instance);
		_instance = instance;
	}
}