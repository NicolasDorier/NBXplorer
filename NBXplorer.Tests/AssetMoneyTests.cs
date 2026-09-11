using NBitcoin;
using System;
using Xunit;

namespace NBXplorer.Tests
{
	public class AssetMoneyTests
	{
		[Fact]
		public void RejectsDivisibilityThatOverflowsScale()
		{
			var assetId = uint256.One;

			Assert.Throws<OverflowException>(() => new AssetMoney(assetId, 1.0m, 10));
			Assert.Throws<OverflowException>(() => new AssetMoney(assetId, 1L).ToDecimal(10));
		}

		[Fact]
		public void SupportsBitcoinStyleDivisibility()
		{
			var assetId = uint256.One;
			var amount = new AssetMoney(assetId, 1.23456789m, 8);

			Assert.Equal(123456789L, amount.Quantity);
			Assert.Equal(1.23456789m, amount.ToDecimal(8));
		}
	}
}
