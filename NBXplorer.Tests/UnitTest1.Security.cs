using NBXplorer.Models;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace NBXplorer.Tests
{
	public partial class UnitTest1
	{
		[FactWithTimeout]
		public async Task DerivationEndpointsRequireAuthentication()
		{
			using var tester = CreateTesterNoAutoStart();
			tester.Start();
			tester.WaitSynchronized();

			using var response = await tester.HttpClient.PostAsync("v1/cryptos/BTC/derivations", null);

			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[FactWithTimeout]
		public async Task DerivationEndpointsAllowNoAuthentication()
		{
			using var tester = CreateTesterNoAutoStart();
			tester.AdditionalFlags.Add("--noauth");
			tester.Start();
			tester.WaitSynchronized();

			using var response = await tester.HttpClient.PostAsync("v1/cryptos/BTC/derivations", null);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		[FactWithTimeout]
		public async Task PruneRejectsInvalidDaysToKeep()
		{
			using var tester = CreateTester();
			tester.WaitSynchronized();
			var wallet = await tester.Client.GenerateWalletAsync();
			var invalidValues = new[]
			{
				-1.0,
				double.NaN,
				double.PositiveInfinity,
				TimeSpan.MaxValue.TotalDays + 1.0
			};

			foreach (var daysToKeep in invalidValues)
			{
				await AssertBadRequest(
					"invalid-days-to-keep",
					() => tester.Client.PruneAsync(wallet.DerivationScheme, new PruneRequest { DaysToKeep = daysToKeep }));
			}

			await tester.Client.PruneAsync(wallet.DerivationScheme, new PruneRequest { DaysToKeep = 0.0 });
		}

		[FactWithTimeout]
		public async Task ScanUTXOSetRejectsInvalidRanges()
		{
			using var tester = CreateTester();
			tester.WaitSynchronized();
			var invalidValues = new (int? BatchSize, int? GapLimit, int? From)[]
			{
				(0, null, null),
				(null, 0, null),
				(null, null, -1)
			};

			foreach (var value in invalidValues)
			{
				var wallet = await tester.Client.GenerateWalletAsync();
				await AssertBadRequest(
					"invalid-scan-parameters",
					() => tester.Client.ScanUTXOSetAsync(wallet.DerivationScheme, value.BatchSize, value.GapLimit, value.From));
			}
		}

		[FactWithTimeout]
		public async Task GroupChildrenRejectsNullBodies()
		{
			using var tester = CreateTester();
			tester.WaitSynchronized();
			var group = await tester.Client.CreateGroupAsync();

			await AssertBadRequest(
				"invalid-group-children",
				() => tester.Client.AddGroupChildrenAsync(group.GroupId, null));
			await AssertBadRequest(
				"invalid-group-children",
				() => tester.Client.RemoveGroupChildrenAsync(group.GroupId, null));

			var unchanged = await tester.Client.AddGroupChildrenAsync(group.GroupId, Array.Empty<GroupChild>());
			Assert.Empty(unchanged.Children);
		}

		private static async Task AssertBadRequest(string expectedCode, Func<Task> action)
		{
			var exception = await Assert.ThrowsAsync<NBXplorerException>(action);
			Assert.Equal(400, exception.Error.HttpCode);
			Assert.Equal(expectedCode, exception.Error.Code);
		}
	}
}
