using NBitcoin;
using NBXplorer.Backend;
using System;
using System.Linq;
using Xunit;

namespace NBXplorer.Tests
{
	public class IndexerTests
	{
		[Fact]
		public void UnorderedBlocksAreMatchedToHeadersByHash()
		{
			var first = Network.RegTest.Consensus.ConsensusFactory.CreateBlock();
			var second = Network.RegTest.Consensus.ConsensusFactory.CreateBlock();
			second.Header.HashPrevBlock = first.GetHash();
			var headers = new BlockHeaders(new[]
			{
				new RPCBlockHeader(first.GetHash(), first.Header.HashPrevBlock, 10, DateTimeOffset.UnixEpoch, first.Header.HashMerkleRoot),
				new RPCBlockHeader(second.GetHash(), second.Header.HashPrevBlock, 11, DateTimeOffset.UnixEpoch, second.Header.HashMerkleRoot)
			});

			var matched = Indexer.MatchBlocksToHeaders(new[] { second, first }, headers).ToArray();

			Assert.Collection(
				matched,
				pair => Assert.Equal(first.GetHash(), pair.Header.Hash),
				pair => Assert.Equal(second.GetHash(), pair.Header.Hash));
			Assert.All(matched, pair => Assert.Equal(pair.Block.GetHash(), pair.Header.Hash));
		}
	}
}
