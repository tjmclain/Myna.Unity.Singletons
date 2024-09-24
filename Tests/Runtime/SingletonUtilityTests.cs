#if UNITY_INCLUDE_TESTS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Myna.Unity.Singletons.Tests
{
    public class SingletonUtilityTests
    {
        [Test]
        public void InitAssetFromResources()
        {
            var singleton = SingletonUtility.GetOrInitSingletonAsset<TestSingletonAsset>();
            Assert.That(singleton, Is.Not.Null);

            string message = singleton.Message;
            Assert.That(message, Is.EqualTo("Hello World"));

            // Dispose of the singleton and test that it was disposed correctly
            SingletonUtility.DisposeSingleton<TestSingletonAsset>();
            bool isInitialized = SingletonUtility.IsInitialized<TestSingletonAsset>();
            Assert.That(isInitialized, Is.False);
        }

        [Test]
        public void InitBehaviourFromAddressables()
        {
            var singleton = SingletonUtility.GetOrInitSingletonBehaviour<TestSingletonBehaviour>();
            Assert.That(singleton, Is.Not.Null);

            int value = singleton.Value;
            Assert.That(value, Is.EqualTo(415));

            // Dispose of the singleton and test that it was disposed correctly
            SingletonUtility.DisposeSingleton<TestSingletonBehaviour>();
            bool isInitialized = SingletonUtility.IsInitialized<TestSingletonBehaviour>();
            Assert.That(isInitialized, Is.False);
        }

        [Test]
        public void InitAssetFromAddressables()
        {
            var singleton = SingletonUtility.GetOrInitSingletonAsset<TestAddressableAsset>();
            Assert.That(singleton, Is.Not.Null);

            bool value = singleton.TheBestFinalFantasyIsTen;
            Assert.That(value, Is.EqualTo(true));

            // Dispose of the singleton and test that it was disposed correctly
            SingletonUtility.DisposeSingleton<TestAddressableAsset>();
            bool isInitialized = SingletonUtility.IsInitialized<TestAddressableAsset>();
            Assert.That(isInitialized, Is.False);
        }

        [UnityTest]
        public IEnumerator InitAssetFromResourcesAsync()
        {
            var routine = SingletonUtility.InitializeSingletonAsync<TestSingletonAsset>();

            yield return routine;

            // Test that the singleton was initialized correctly
            SingletonUtility.TryGetSingleton(out TestSingletonAsset singleton);
            Assert.That(singleton, Is.Not.Null);

            string message = singleton.Message;
            Assert.That(message, Is.EqualTo("Hello World"));

            // Dispose of the singleton and test that it was disposed correctly
            SingletonUtility.DisposeSingleton<TestSingletonAsset>();
            bool isInitialized = SingletonUtility.IsInitialized<TestSingletonAsset>();
            Assert.That(isInitialized, Is.False);
        }

        [UnityTest]
        public IEnumerator InitBehaviourFromAddressablesAsync()
        {
            var routine = SingletonUtility.InitializeSingletonAsync<TestSingletonBehaviour>();

            yield return routine;

            // Test that the singleton was initialized correctly
            SingletonUtility.TryGetSingleton(out TestSingletonBehaviour singleton);
            Assert.That(singleton, Is.Not.Null);

            int value = singleton.Value;
            Assert.That(value, Is.EqualTo(415));

            // Dispose of the singleton and test that it was disposed correctly
            SingletonUtility.DisposeSingleton<TestSingletonBehaviour>();
            bool isInitialized = SingletonUtility.IsInitialized<TestSingletonBehaviour>();
            Assert.That(isInitialized, Is.False);
        }

        [UnityTest]
        public IEnumerator InitAssetFromAddressablesAsync()
        {
            var routine = SingletonUtility.InitializeSingletonAsync<TestAddressableAsset>();

            yield return routine;

            // Test that the singleton was initialized correctly
            SingletonUtility.TryGetSingleton(out TestAddressableAsset singleton);
            Assert.That(singleton, Is.Not.Null);

            bool value = singleton.TheBestFinalFantasyIsTen;
            Assert.That(value, Is.EqualTo(true));

            // Dispose of the singleton and test that it was disposed correctly
            SingletonUtility.DisposeSingleton<TestAddressableAsset>();
            bool isInitialized = SingletonUtility.IsInitialized<TestAddressableAsset>();
            Assert.That(isInitialized, Is.False);
        }
    }
}
#endif
