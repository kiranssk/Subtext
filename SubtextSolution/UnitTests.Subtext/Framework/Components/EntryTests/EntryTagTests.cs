using System;
using System.Linq;
using System.Collections.Generic;
using MbUnit.Framework;
using Subtext.Framework;
using Subtext.Framework.Components;
using Subtext.Framework.Configuration;
using Subtext.Framework.Data;
using Subtext.Framework.Web.HttpModules;

namespace UnitTests.Subtext.Framework.Components.EntryTests
{
	[TestFixture]
	public class EntryTagTests
	{
		[Test]
		[RollBack]
		public void TagDoesNotRetrieveDraftEntry()
		{
			string hostname = UnitTestHelper.GenerateUniqueString();
			Config.CreateBlog("", "username", "password", hostname, string.Empty);
			UnitTestHelper.SetHttpContextWithBlogRequest(hostname, string.Empty);
            BlogRequest.Current.Blog = Config.GetBlog(hostname, string.Empty);

			Entry entry = UnitTestHelper.CreateEntryInstanceForSyndication("me", "title-zero", "body-zero");
			entry.IsActive = false;
			UnitTestHelper.Create(entry);
			List<string> tags = new List<string>(new string[] { "Tag1", "Tag2" });
			new DatabaseObjectProvider().SetEntryTagList(entry.Id, tags);
			ICollection<Entry> entries = Entries.GetEntriesByTag(1, "Tag1");
			Assert.AreEqual(0, entries.Count, "Should not retrieve draft entry.");
		}

		[Test]
		[RollBack]
		public void CanTagEntry()
		{
			string hostname = UnitTestHelper.GenerateUniqueString();
			Config.CreateBlog("", "username", "password", hostname, string.Empty);
			UnitTestHelper.SetHttpContextWithBlogRequest(hostname, string.Empty);
            BlogRequest.Current.Blog = Config.GetBlog(hostname, string.Empty);

			Entry entry = UnitTestHelper.CreateEntryInstanceForSyndication("me", "title-zero", "body-zero");
			UnitTestHelper.Create(entry);

			List<string> tags = new List<string>(new string[] {"Tag1", "Tag2"});
			new DatabaseObjectProvider().SetEntryTagList(entry.Id, tags);

			ICollection<Entry> entries = Entries.GetEntriesByTag(1, "Tag1");
			Assert.AreEqual(1, entries.Count);
			Assert.AreEqual(entry.Id, entries.First().Id);
		}
	}
}
