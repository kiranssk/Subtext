using System;
using MbUnit.Framework;
using Subtext.Framework;
using Subtext.Framework.Web;

namespace UnitTests.Subtext.Framework.Web
{
	/// <summary>
	/// Contains tests of our handling of Http.
	/// </summary>
	[TestFixture]
	public class HttpHelperTests
	{
		/// <summary>
		/// Tests that we correctly parse if-modified-since from the request.
		/// Unfortunately, this unit test is time-zone sensitive.
		/// </summary>
		[RowTest]
		[Row("4/12/2006", "04-12-2006")]
		[Row("12 Apr 2006 06:59:33 GMT", "4/11/2006 11:59:33 PM")]
		[Row("Wed, 12 Apr 2006 06:59:33 GMT", "04-11-2006 23:59:33")]
		public void TestIfModifiedSinceExtraction(string received, string expected)
		{
			SimulatedHttpRequest workerRequest = UnitTestHelper.SetHttpContextWithBlogRequest("localhost", "");
            workerRequest.Headers.Add("If-Modified-Since", received);

			DateTime expectedDate = DateTimeHelper.ParseUnknownFormatUTC(expected);
			Console.WriteLine("{0}\t{1}\t{2}", received, expected, expectedDate.ToUniversalTime());

			Assert.AreEqual(expectedDate, HttpHelper.GetIfModifiedSinceDateUTC());
		}
	}
}