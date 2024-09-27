using System.Collections;
using System.Collections.Generic;
using Myna.Unity.Singletons;
using UnityEngine;

[CreateAssetMenu(fileName = "TestAddressableAsset", menuName = "Testing/TestAddressableAsset")]
[Singleton("Assets/Packages/Myna.Unity.Singletons/Tests/Addressables/TestAddressableAsset.asset")]
public class TestAddressableAsset : SingletonAsset<TestSingletonAsset>
{
    public bool TheBestFinalFantasyIsTen = true;
}
