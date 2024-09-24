using System.Collections;
using System.Collections.Generic;
using Myna.Unity.Singletons;
using UnityEngine;

[Singleton(Path = "Assets/Packages/Myna.Unity.Singletons/Tests/Addressables/TestSingletonBehaviour.prefab", Source = SingletonAttribute.AssetSource.Addressables)]
public class TestSingletonBehaviour : SingletonBehaviour<TestSingletonBehaviour>
{
    public int Value = 415;
}
