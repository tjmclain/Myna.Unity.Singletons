#if UNITY_INCLUDE_TESTS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace Myna.Unity.Singletons.Tests
{
	public class SingletonTests
	{
		[Test]
		public void InitAssetFromResources()
		{
			var instance = TestResourceAsset.GetOrCreateInstance();
			Assert.That(instance != null);
		}

		[Test]
		public void InitBehaviourFromResources()
		{
			var instance = TestResourceBehaviour.GetOrCreateInstance();
			Assert.That(instance != null);
		}

		[Test]
		public void InitAssetFromAddressables()
		{
			var instance = TestAddressableAsset.GetOrCreateInstance();
			Assert.That(instance != null);
		}

		[Test]
		public void InitBehaviourFromAddressables()
		{
			var instance = TestAddressableBehaviour.GetOrCreateInstance();
			Assert.That(instance != null);
		}
	}
}
#endif