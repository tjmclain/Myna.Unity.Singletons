#if UNITY_INCLUDE_TESTS
using System.Threading.Tasks;
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

		[Test]
		public async void InitAssetFromResourcesAsync()
		{
			var instance = await TestResourceAsset.GetOrCreateInstanceAsync();
			Assert.That(instance != null);
		}

		[Test]
		public async void InitBehaviourFromResourcesAsync()
		{
			var instance = await TestResourceBehaviour.GetOrCreateInstanceAsync();
			Assert.That(instance != null);
		}

		[Test]
		public async void InitAssetFromAddressablesAsync()
		{
			var instance = await TestAddressableAsset.GetOrCreateInstanceAsync();
			Assert.That(instance != null);
		}

		[Test]
		public async void InitBehaviourFromAddressablesAsync()
		{
			var instance = await TestAddressableBehaviour.GetOrCreateInstanceAsync();
			Assert.That(instance != null);
		}
	}
}
#endif