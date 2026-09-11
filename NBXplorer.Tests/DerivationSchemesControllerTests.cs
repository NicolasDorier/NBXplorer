using NBXplorer.Controllers;
using NBXplorer.Models;
using Xunit;

namespace NBXplorer.Tests
{
	public class DerivationSchemesControllerTests
	{
		[Theory]
		[InlineData(-1, null)]
		[InlineData(null, -1)]
		[InlineData(2, 1)]
		[InlineData(10_001, null)]
		[InlineData(null, 10_001)]
		public void RejectsInvalidAddressGenerationRanges(int? minAddresses, int? maxAddresses)
		{
			var request = new TrackWalletRequest
			{
				DerivationOptions = new[]
				{
					new TrackDerivationOption
					{
						MinAddresses = minAddresses,
						MaxAddresses = maxAddresses
					}
				}
			};

			var exception = Assert.Throws<NBXplorerException>(() => DerivationSchemesController.ValidateDerivationOptions(request));

			Assert.Equal(400, exception.Error.HttpCode);
			Assert.Equal("invalid-address-generation-range", exception.Error.Code);
		}

		[Fact]
		public void AcceptsBoundedAddressGenerationRanges()
		{
			var request = new TrackWalletRequest
			{
				DerivationOptions = new[]
				{
					new TrackDerivationOption { MinAddresses = 0, MaxAddresses = 0 },
					new TrackDerivationOption { MinAddresses = 500, MaxAddresses = 10_000 },
					new TrackDerivationOption()
				}
			};

			DerivationSchemesController.ValidateDerivationOptions(request);
		}
	}
}
