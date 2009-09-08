#region Disclaimer/Info
///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at Google Code at http://code.google.com/p/subtext/
// The development mailing list is at subtext-devs@lists.sourceforge.net 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using log4net;
using Subtext.Configuration;
using Subtext.Extensibility;
using Subtext.Extensibility.Interfaces;
using Subtext.Framework.Components;
using Subtext.Framework.Configuration;
using Subtext.Framework.Logging;
using Subtext.Framework.Providers;
using Subtext.Framework.Text;
using Subtext.Framework.Services;
using Subtext.Framework.Emoticons;

namespace Subtext.Framework
{
    /// <summary>
    /// Static class used to get entries (blog posts, comments, etc...) 
    /// from the data store.
    /// </summary>
    public static class Entries
    {
        private readonly static ILog log = new Log();

        /// <summary>
        /// Returns a collection of Posts for a give page and index size.
        /// </summary>
        /// <param name="postType"></param>
        /// <param name="categoryID">-1 means not to filter by a categoryID</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IPagedCollection<EntryStatsView> GetPagedEntries(PostType postType, int? categoryID, int pageIndex, int pageSize)
        {
            return ObjectProvider.Instance().GetPagedEntries(postType, categoryID, pageIndex, pageSize);
        }

        public static EntryDay GetSingleDay(DateTime dt)
        {
            return ObjectProvider.Instance().GetEntryDay(dt);
        }

        /// <summary>
        /// Gets the entries to display on the home page.
        /// </summary>
        /// <param name="itemCount">Item count.</param>
        /// <returns></returns>
        public static ICollection<EntryDay> GetHomePageEntries(int itemCount)
        {
            return GetBlogPosts(itemCount, PostConfig.DisplayOnHomepage | PostConfig.IsActive);
        }

        /// <summary>
        /// Gets the specified number of entries using the <see cref="PostConfig"/> flags 
        /// specified.
        /// </summary>
        /// <remarks>
        /// This is used to get the posts displayed on the home page.
        /// </remarks>
        /// <param name="itemCount">Item count.</param>
        /// <param name="pc">Pc.</param>
        /// <returns></returns>
        public static ICollection<EntryDay> GetBlogPosts(int itemCount, PostConfig pc)
        {
            return ObjectProvider.Instance().GetBlogPosts(itemCount, pc);
        }

        public static ICollection<EntryDay> GetPostsByCategoryID(int itemCount, int catID)
        {
            return ObjectProvider.Instance().GetPostsByCategoryID(itemCount, catID);
        }
        #region EntryCollections

        /// <summary>
        /// Gets the main syndicated entries.
        /// </summary>
        /// <param name="itemCount">Item count.</param>
        /// <returns></returns>
        public static ICollection<Entry> GetMainSyndicationEntries(int itemCount)
        {
            return GetRecentPosts(itemCount, PostType.BlogPost, PostConfig.IncludeInMainSyndication | PostConfig.IsActive, true /* includeCategories */);
        }

        /// <summary>
        /// Gets the comments (including trackback, etc...) for the specified entry.
        /// </summary>
        /// <param name="parentEntry">Parent entry.</param>
        /// <returns></returns>
        public static ICollection<FeedbackItem> GetFeedBack(Entry parentEntry)
        {
            return ObjectProvider.Instance().GetFeedbackForEntry(parentEntry);
        }

        /// <summary>
        /// Returns the itemCount most recent posts.  
        /// This is used to support MetaBlogAPI...
        /// </summary>
        /// <param name="itemCount"></param>
        /// <param name="postType"></param>
        /// <param name="postConfig"></param>
        /// <param name="includeCategories"></param>
        /// <returns></returns>
        public static ICollection<Entry> GetRecentPosts(int itemCount, PostType postType, PostConfig postConfig, bool includeCategories)
        {
            return ObjectProvider.Instance().GetEntries(itemCount, postType, postConfig, includeCategories);
        }

        /// <summary>
        /// Returns the posts for the specified month for the Month Archive section.
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static ICollection<Entry> GetPostsByMonth(int month, int year)
        {
            return ObjectProvider.Instance().GetPostsByMonth(month, year);
        }

        public static ICollection<Entry> GetPostsByDayRange(DateTime start, DateTime stop, PostType postType, bool activeOnly)
        {
            return ObjectProvider.Instance().GetPostsByDayRange(start, stop, postType, activeOnly);
        }

        public static ICollection<Entry> GetEntriesByCategory(int itemCount, int catID, bool activeOnly)
        {
            return ObjectProvider.Instance().GetEntriesByCategory(itemCount, catID, activeOnly);
        }

        public static ICollection<Entry> GetEntriesByTag(int itemCount, string tagName)
        {
            return ObjectProvider.Instance().GetEntriesByTag(itemCount, tagName);
        }
        #endregion

        #region Single Entry

        /// <summary>
        /// Searches the data store for the first comment with a 
        /// matching checksum hash.
        /// </summary>
        /// <param name="checksumHash">Checksum hash.</param>
        /// <returns></returns>
        public static FeedbackItem GetFeedbackByChecksumHash(string checksumHash)
        {
            return ObjectProvider.Instance().GetFeedbackByChecksumHash(checksumHash);
        }

        /// <summary>
        /// Gets the entry from the data store by id. Only returns an entry if it is 
        /// within the current blog (Config.CurrentBlog).
        /// </summary>
        /// <param name="entryId">The ID of the entry.</param>
        /// <param name="postConfig">The entry option used to constrain the search.</param>
        /// <param name="includeCategories">Whether the returned entry should have its categories collection populated.</param>
        /// <returns></returns>
        public static Entry GetEntry(int entryId, PostConfig postConfig, bool includeCategories)
        {
            bool isActive = ((postConfig & PostConfig.IsActive) == PostConfig.IsActive);
            return ObjectProvider.Instance().GetEntry(entryId, isActive, includeCategories);
        }
        #endregion

        #region Create
        public static IEnumerable<int> GetCategoryIdsFromCategoryTitles(Entry entry)
        {
            int[] categoryIds;
            Collection<int> catIds = new Collection<int>();
            //Ok, we have categories specified in the entry, but not the IDs.
            //We need to do something.
            foreach (string category in entry.Categories)
            {
                LinkCategory cat = ObjectProvider.Instance().GetLinkCategory(category, true);
                if (cat != null)
                {
                    catIds.Add(cat.Id);
                }
            }
            categoryIds = new int[catIds.Count];
            catIds.CopyTo(categoryIds, 0);
            return categoryIds;
        }

        #endregion

        /// <summary>
        /// Converts a title of a blog post into a friendly, but URL safe string.
        /// </summary>
        /// <param name="title">The original title of the blog post.</param>
        /// <param name="entryId">The id of the current entry.</param>
        /// <returns></returns>
        public static string AutoGenerateFriendlyUrl(string title, int entryId)
        {
            if (title == null)
                throw new ArgumentNullException("title");

            FriendlyUrlSettings friendlyUrlSettings = FriendlyUrlSettings.Settings;
            if (friendlyUrlSettings == null)
            {
                //Default to old behavior.
                return AutoGenerateFriendlyUrl(title, char.MinValue, entryId, TextTransform.None);
            }

            TextTransform textTransform = friendlyUrlSettings.TextTransformation;
            string wordSeparator = friendlyUrlSettings.SeparatingCharacter;
            int wordCount = friendlyUrlSettings.WordCountLimit;

            // break down to number of words. If 0 (or less) don't mess with the title
            if (wordCount > 0)
            {
                //only do this is there are more words than allowed.
                string[] words;
                words = title.Split(" ".ToCharArray());

                if (words.Length > wordCount)
                {
                    //now strip the title down to the number of allowed words
                    int wordCharCounter = 0;
                    for (int i = 0; i < wordCount; i++)
                    {
                        wordCharCounter = wordCharCounter + words[i].Length + 1;
                    }

                    title = title.Substring(0, wordCharCounter - 1);
                }
            }

            // separating characters are limited due to the problems certain chars
            // can cause. Only - _ and . are allowed
            if ((wordSeparator == "_") || (wordSeparator == ".") || (wordSeparator == "-"))
            {
                return AutoGenerateFriendlyUrl(title, wordSeparator[0], entryId, textTransform);
            }
            else
            {
                //invalid separator or none defined.
                return AutoGenerateFriendlyUrl(title, char.MinValue, entryId, textTransform);
            }
        }

        private static string ReplaceUnicodeCharacters(string text)
        {
            string normalized = text.Normalize(NormalizationForm.FormKD);
            Encoding removal = Encoding.GetEncoding(Encoding.ASCII.CodePage, new EncoderReplacementFallback(string.Empty), new DecoderReplacementFallback(string.Empty));
            byte[] bytes = removal.GetBytes(normalized);
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// Converts a title of a blog post into a friendly, but URL safe string.
        /// </summary>
        /// <param name="title">The original title of the blog post.</param>
        /// <param name="wordSeparator">The string used to separate words in the title.</param>
        /// <param name="entryId">The id of the current entry.</param>
        /// <param name="textTransform">Used to specify a change to the casing of the string.</param>
        /// <returns></returns>
        public static string AutoGenerateFriendlyUrl(string title, char wordSeparator, int entryId, TextTransform textTransform)
        {
            if (title == null)
            {
                throw new ArgumentNullException("title");
            }

            string entryName = RemoveNonWordCharacters(title);
            entryName = ReplaceSpacesWithSeparator(entryName, wordSeparator);
            entryName = ReplaceUnicodeCharacters(entryName);
            entryName = HttpUtility.UrlEncode(entryName);
            entryName = RemoveTrailingPeriods(entryName);
            entryName = entryName.Trim(new char[] { wordSeparator });
            entryName = entryName.RemoveDoubleCharacter('.');
            if (wordSeparator != char.MinValue && wordSeparator != '.')
                entryName = entryName.RemoveDoubleCharacter(wordSeparator);

            if (entryName.IsNumeric())
            {
                entryName = "n" + wordSeparator + entryName;
            }

            string newEntryName = FriendlyUrlSettings.TransformString(entryName, textTransform);

            int tryCount = 0;
            Entry currentEntry = ObjectProvider.Instance().GetEntry(newEntryName, false, false);

            while (currentEntry != null)
            {
                if (currentEntry.Id == entryId) //This means that we are updating the same entry, so should allow same entryname
                    break;
                switch (tryCount)
                {
                    case 0:
                        newEntryName = entryName + wordSeparator + "Again";
                        break;
                    case 1:
                        newEntryName = entryName + wordSeparator + "Yet" + wordSeparator + "Again";
                        break;
                    case 2:
                        newEntryName = entryName + wordSeparator + "And" + wordSeparator + "Again";
                        break;
                    case 3:
                        newEntryName = entryName + wordSeparator + "Once" + wordSeparator + "More";
                        break;
                    case 4:
                        newEntryName = entryName + wordSeparator + "To" + wordSeparator + "Beat" + wordSeparator + "A" + wordSeparator +
                                           "Dead" + wordSeparator + "Horse";
                        break;
                }
                if (tryCount++ > 5)
                    break; //Allow an exception to get thrown later.

                newEntryName = FriendlyUrlSettings.TransformString(newEntryName, textTransform);
                currentEntry = ObjectProvider.Instance().GetEntry(newEntryName, false, false);
            }

            return newEntryName;
        }

        private static string ReplaceSpacesWithSeparator(string text, char wordSeparator)
        {
            if (wordSeparator == char.MinValue)
            {
                //Special case if we are just removing spaces.
                return text.ToPascalCase();
            }
            else
            {
                return text.Replace(' ', wordSeparator);
            }
        }

        private static string RemoveNonWordCharacters(string text)
        {
            Regex regex = new Regex(@"[\w\d\.\- ]+", RegexOptions.Compiled);
            MatchCollection matches = regex.Matches(text);
            StringBuilder cleansedText = new StringBuilder();

            foreach (Match match in matches)
            {
                if (match.Value.Length > 0)
                {
                    cleansedText.Append(match.Value);
                }
            }
            return cleansedText.ToString();
        }

        private static string RemoveTrailingPeriods(string text)
        {
            Regex regex = new Regex(@"\.+$", RegexOptions.Compiled);
            return regex.Replace(text, string.Empty);
        }

        #region Update

        /// <summary>
        /// Updates the specified entry in the data provider.
        /// </summary>
        /// <param name="entry">Entry.</param>
        /// <returns></returns>
        public static void Update(Entry entry, ISubtextContext context)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            if (NullValue.IsNull(entry.DateSyndicated) && entry.IsActive && entry.IncludeInMainSyndication)
                entry.DateSyndicated = Config.CurrentBlog.TimeZone.Now;

            Update(entry, context, null /* categoryIds */);
        }

        /// <summary>
        /// Updates the specified entry in the data provider 
        /// and attaches the specified categories.
        /// </summary>
        /// <param name="entry">Entry.</param>
        /// <param name="categoryIDs">Category Ids this entry belongs to.</param>
        /// <returns></returns>
        public static void Update(Entry entry, ISubtextContext context, IEnumerable<int> categoryIds)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }
            var repository = ObjectProvider.Instance();
            var transform = new CompositeTextTransformation();
            transform.Add(new XhtmlConverter());
            transform.Add(new EmoticonsTransformation(context));
            //TODO: Maybe use a INinjectParameter to control this.
            transform.Add(new KeywordExpander(repository));
            EntryPublisher publisher = new EntryPublisher(context, null, new SlugGenerator(FriendlyUrlSettings.Settings));
            publisher.Publish(entry);

            //ObjectProvider.Instance().Update(entry, categoryIds);
        }

        #endregion

        #region Entry Category List

        /// <summary>
        /// Sets the categories for this entry.
        /// </summary>
        /// <param name="entryId">The entry id.</param>
        /// <param name="categories">The categories.</param>
        public static void SetEntryCategoryList(int entryId, IEnumerable<int> categories)
        {
            ObjectProvider.Instance().SetEntryCategoryList(entryId, categories);
        }

        #endregion

        #region Tag Utility Functions

        public static bool RebuildAllTags()
        {
            foreach (EntryDay day in GetBlogPosts(0, PostConfig.None))
            {
                foreach (Entry e in day)
                {
                    ObjectProvider.Instance().SetEntryTagList(e.Id, HtmlHelper.ParseTags(e.Body));
                }
            }
            return true;
        }

        #endregion
    }
}




