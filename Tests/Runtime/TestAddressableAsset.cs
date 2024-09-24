using System.Collections;
using System.Collections.Generic;
using Myna.Unity.Singletons;
using UnityEngine;


[CreateAssetMenu(fileName = "TestAddressableAsset", menuName = "Testing/TestAddressableAsset")]
[Singleton(Path = "Assets/Packages/Myna.Unity.Singletons/Tests/Addressables/TestAddressableAsset.asset", Source = SingletonAttribute.AssetSource.Addressables)]
public class TestAddressableAsset : SingletonAsset<TestSingletonAsset>
{
    public bool TheBestFinalFantasyIsTen = true;
}
