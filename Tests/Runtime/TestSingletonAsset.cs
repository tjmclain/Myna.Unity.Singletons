using System.Collections;
using System.Collections.Generic;
using Myna.Unity.Singletons;
using UnityEngine;

[CreateAssetMenu(fileName = "TestSingletonAsset", menuName = "Testing/TestSingletonAsset")]
[Singleton(Path = "TestSingletonAsset", Source = SingletonAttribute.AssetSource.Resources)]
public class TestSingletonAsset : SingletonAsset<TestSingletonAsset>
{
    public string Message = "Hello World";
}
