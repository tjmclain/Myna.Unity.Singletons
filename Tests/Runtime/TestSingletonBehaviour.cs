using Myna.Unity.Singletons;

[Singleton(
    "Assets/Packages/Myna.Unity.Singletons/Tests/Addressables/TestSingletonBehaviour.prefab"
)]
public class TestSingletonBehaviour : SingletonBehaviour<TestSingletonBehaviour>
{
    public int Value = 415;
}
