using System.Collections;
using System.Collections.Generic;
using Myna.Unity.Singletons;
using UnityEngine;

[Singleton("TestSingletonAsset")]
[CreateAssetMenu(fileName = "TestSingletonAsset", menuName = "Testing/TestSingletonAsset")]
public class TestSingletonAsset : SingletonAsset<TestSingletonAsset>
{
    public string Message = "Hello World";
}
