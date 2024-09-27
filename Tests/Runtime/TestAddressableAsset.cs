using System.Collections;
using System.Collections.Generic;
using Myna.Unity.Singletons;
using UnityEngine;

[Singleton("Tests/Addressables/TestAddressableAsset.asset")]
[CreateAssetMenu(fileName = "TestAddressableAsset", menuName = "Myna/Singletons/Testing/TestAddressableAsset")]
public class TestAddressableAsset : SingletonAsset<TestAddressableAsset>
{

}
