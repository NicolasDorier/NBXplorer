using NBitcoin;
using NBitcoin.RPC;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NBXplorer.Tests
{
	public class RPCClientExtensionsTests
	{
		[Fact]
		public async Task MissingRpcReturnsNoTransactionsFromBlocks()
		{
			RPCClient rpc = null;
			var requested = new HashSet<(uint256 BlockId, uint256 TransactionId)>
			{
				(uint256.One, new uint256(2))
			};

			var result = await rpc.GetTransactionFromBlocks(requested);

			Assert.Empty(result);
		}

		[Theory]
		[InlineData(100, 200)]
		[InlineData(5_000, 10_000)]
		[InlineData(10_000, 10_000)]
		public void ScanRetryDelayDoublesUpToTenSeconds(int current, int expected)
		{
			Assert.Equal(expected, RPCClientExtensions.NextScanRetryDelay(current));
		}
	}
}
