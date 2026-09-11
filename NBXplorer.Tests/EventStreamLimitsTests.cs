using NBXplorer.Controllers;
using NBXplorer.Models;
using Xunit;

namespace NBXplorer.Tests
{
	public class EventStreamLimitsTests
	{
		[Fact]
		public void EventQueryLimitMustBeWithinBounds()
		{
			Assert.Equal("invalid-limit", Assert.Throws<NBXplorerException>(() => MainController.ValidateEventLimit(0)).Error.Code);
			Assert.Equal("invalid-limit", Assert.Throws<NBXplorerException>(() => MainController.ValidateEventLimit(10_001)).Error.Code);

			Assert.Equal(10_000, MainController.NormalizeEventLimit(null));
			MainController.ValidateEventLimit(1);
			MainController.ValidateEventLimit(10_000);
		}

		[Theory]
		[InlineData(999, 1, true)]
		[InlineData(1_000, 1, false)]
		[InlineData(999, 2, false)]
		[InlineData(0, 1_000, true)]
		[InlineData(0, 1_001, false)]
		[InlineData(999, int.MaxValue, false)]
		public void WebSocketRulesHaveAPerConnectionCeiling(int existing, int added, bool expected)
		{
			Assert.Equal(expected, MainController.CanAppendEventRules(existing, added));
		}
	}
}
