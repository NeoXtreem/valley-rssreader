using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Valley.RssReader.Common.Models;
using Valley.RssReader.Common.Services.Interfaces;
using Valley.RssReader.Core.Exceptions;
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
            if (!ModelState.IsValid)
            {
                TempData.Add("Failure", "Please enter a valid URL.");
                return CurrentUmbracoPage();
            }

            try
            {
                int homeId = Services.ContentService.GetById(new Guid("34e19223-ba7b-4fc3-beaa-8814f689babb")).Id;

                IContent[] rssItems = _rssItemMappingService.Map(_rssReaderService.Read(new Uri(rssFeedUrl.Url))).Select(m =>
                {
                    IContent rssItem = Services.ContentService.CreateContent($"RSS Item {m.Id}", homeId, "RssItem");
                    rssItem.SetValue("title", m.Title);
                    rssItem.SetValue("description", m.Description);
                    rssItem.SetValue("categories", m.Categories);
                    rssItem.SetValue("date", m.Date);
                    rssItem.SetValue("link", m.Link);
                    return rssItem;
                }).ToArray();

                // Delete all the old RSS items before publishing the newly imported ones.
                foreach (IContent rssItem in Services.ContentService.GetChildren(homeId))
                {
                    Services.ContentService.Delete(rssItem);
                }

                // Clear the cache so that clients will read the new items.
                ApplicationContext.ApplicationCache.RuntimeCache.ClearAllCache();

                // Save and publish all items, but delete any that fail to publish so that they're not available to the client.
                var errors = 0;
                foreach (IContent rssItem in rssItems)
                {
                    if (Services.ContentService.SaveAndPublishWithStatus(rssItem).Success) continue;
                    Services.ContentService.Delete(rssItem);
                    errors++;
                }

                if (errors > 0)
                {
                    TempData.Add("Failure", $"{(errors == rssItems.Length ? "All" : "Some")} items failed to publish.");
                }
                else
                {
                    TempData.Add("Success", $"URL {rssFeedUrl.Url} was successfully imported.");
                }

                return RedirectToCurrentUmbracoPage();
            }
            catch (RssReaderException e)
            {
                TempData.Add("Failure", $"{e.Message} URL: {e.Url}");
                return CurrentUmbracoPage();
            }
        }
    }
}
