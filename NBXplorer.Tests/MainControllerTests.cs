using NBXplorer.Controllers;
using NBXplorer.Models;
using Xunit;

namespace NBXplorer.Tests
{
	public class MainControllerTests
	{
		[Fact]
		public void RejectsOversizedRpcBatches()
		{
			var exception = Assert.Throws<NBXplorerException>(() => MainController.ValidateRPCBatchSize(101));

			Assert.Equal(400, exception.Error.HttpCode);
			Assert.Equal("rpc-batch-too-large", exception.Error.Code);
			MainController.ValidateRPCBatchSize(100);
		}

		[Fact]
		public void RejectsOversizedRescans()
		{
			var exception = Assert.Throws<NBXplorerException>(() => MainController.ValidateRescanCount(1_001));

			Assert.Equal(400, exception.Error.HttpCode);
			Assert.Equal("rescan-too-large", exception.Error.Code);
			MainController.ValidateRescanCount(1_000);
		}
	}
}
