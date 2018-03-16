
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;
using NSubstitute;

namespace MockingFrameworks.Tests.NSubstitute
{
	[TestClass]
	public class NSubstituteTests
	{
		[TestMethod]
		public void StartupTest()
		{
			true.ShouldBeTrue();
		}
	}
}
