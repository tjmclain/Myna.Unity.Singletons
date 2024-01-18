using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	public static bool IsInitialized => GetInstance() != null;

	public T Instance
	{
		get => GetInstance();
		set => SetInstance(value);
	}

	public static implicit operator T(SingletonBehaviour<T> singleton)
	{
		return singleton.Instance;
	}

	public void Initialize(T instance)
	{
		SetInstance(instance);
	}

	private static void SetInstance(T instance)
	{
		SingletonUtility.SetSingleton(instance);
		_instance = instance;

		if (instance != null && instance.transform.parent == null)
		{
			DontDestroyOnLoad(instance.gameObject);
		}
	}

	private static T GetInstance()
	{
		if (_instance != null)
		{
			return _instance;
		}

		_instance = SingletonUtility.GetOrInitSingletonBehaviour<T>();
		return _instance;
	}

	protected virtual void Awake()
	{
		SetInstance(this);
	}
}