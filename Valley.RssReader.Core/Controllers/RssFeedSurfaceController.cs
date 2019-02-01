using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Autofac.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Valley.RssReader.Common.Services.Interfaces;
using Valley.RssReader.Core.Exceptions;
using Valley.RssReader.Core.Models;
using Valley.RssReader.Core.Services.Interfaces;

namespace Valley.RssReader.Core.Controllers
{
    public class RssFeedSurfaceController : SurfaceController
    {
        private readonly IRssReaderService _rssReaderService;
        private readonly IRssItemMappingService _rssItemMappingService;

        public RssFeedSurfaceController(IRssReaderService rssReaderService, IRssItemMappingService rssItemMappingService)
        {
            _rssReaderService = rssReaderService;
            _rssItemMappingService = rssItemMappingService;
        }

        public ActionResult RssUrlForm()
        {
            return PartialView("RssUrlForm", new RssFeedUrlViewModel());
        }

        public ActionResult RssFeed()
        {
            return PartialView("RssFeed");
        }

        [HttpPost]
        public ActionResult ImportUrl(RssFeedUrlViewModel rssFeedUrl)
        {
            const string failure = "Failure";
            const string success = "Success";

            if (!ModelState.IsValid)
            {
                TempData.Add(failure, "Please enter a valid URL.");
                return CurrentUmbracoPage();
            }

            try
            {
                int homeId = Services.ContentService.GetRootContent().Single(c => c.Name == "Home").Id;

                IContent[] newRssItems = _rssItemMappingService.Map(_rssReaderService.Read(new Uri(rssFeedUrl.Url))).Select(m =>
                {
                    IContent rssItem = Services.ContentService.CreateContent($"RSS Item {m.Id}", homeId, "RssItem");
                    rssItem.SetValue("rssId", m.Id);
                    rssItem.SetValue("title", m.Title);
                    rssItem.SetValue("description", m.Description);
                    rssItem.SetValue("categories", m.Categories);
                    rssItem.SetValue("date", m.Date);
                    rssItem.SetValue("link", m.Link);
                    return rssItem;
                }).ToArray();

                var rssItemContentComparer = new RssItemContentComparer();
                IContent[] oldRssItems = Services.ContentService.GetChildren(homeId).ToArray();
                IContent[] rssItemsToPublish = newRssItems.Except(oldRssItems, rssItemContentComparer).ToArray();

                // Save and publish all new items, but delete any that fail to publish so that they're not available to the client.
                var errors = 0;
                foreach (IContent rssItem in rssItemsToPublish)
                {
                    if (Services.ContentService.SaveAndPublishWithStatus(rssItem).Success) continue;
                    LogHelper.Warn(GetType(), $"{rssItem.Name} failed to publish.");
                    Services.ContentService.Delete(rssItem);
                    errors++;
                }

                bool updated = errors == 0 || errors < rssItemsToPublish.Length;
                if (updated)
                {
                    // Delete any old RSS items from the content tree that are no longer part of the feed.
                    oldRssItems.Except(newRssItems, rssItemContentComparer).AsParallel().ForAll(c => Services.ContentService.Delete(c));

                    // Clear the cache so that clients will read the new items.
                    ApplicationContext.ApplicationCache.RuntimeCache.ClearAllCache();
                }

                if (errors > 0)
                {
                    TempData.Add(failure, $"{(updated ? "Some" : "All")} items failed to publish.");
                }
                else
                {
                    string message = $"URL {rssFeedUrl.Url} was successfully imported.";
                    LogHelper.Info(GetType(), message);
                    TempData.Add(success, message);
                }

                return RedirectToCurrentUmbracoPage();
            }
            catch (RssReaderException e)
            {
                string message = $"{e.Message} URL: {e.Url}";
                LogHelper.WarnWithException(GetType(), message, e);
                TempData.Add(failure, message);
                return CurrentUmbracoPage();
            }
        }

        private class RssItemContentComparer : IEqualityComparer<IContent>
        {
            // Products are equal if their names and product numbers are equal.
            public bool Equals(IContent x, IContent y)
            {
                if (Object.ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;

                // Check whether the content nodes' IDs are equal.
                return x.GetValue<string>("rssId") == y.GetValue<string>("rssId");
            }

            public int GetHashCode(IContent product) => product.GetValue<string>("rssId").GetHashCode();
        }
    }
}
