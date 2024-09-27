using System.Collections;
using System.Collections.Generic;
using Myna.Unity.Singletons;
using UnityEngine;

[Singleton("Resources/TestResourceAsset")]
[CreateAssetMenu(fileName = "TestResourceAsset", menuName = "Myna/Singletons/Testing/TestResourceAsset")]
public class TestResourceAsset : SingletonAsset<TestResourceAsset>
{

}
