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
                IEnumerable<IContent> rssItems = _rssItemMappingService.Map(_rssReaderService.Read(new Uri(rssFeedUrl.Url))).Select(m =>
                {
                    IContent rssItem = Services.ContentService.CreateContent($"RSS Item {m.Id}", 1061, "RssItem");
                    rssItem.SetValue("title", m.Title);
                    rssItem.SetValue("description", m.Description);
                    rssItem.SetValue("categories", m.Categories);
                    rssItem.SetValue("date", m.Date);
                    rssItem.SetValue("link", m.Link);
                    return rssItem;
                });

                Services.ContentService.DeleteContentOfType(1060);

                foreach (IContent rssItem in rssItems)
                {
                    Services.ContentService.SaveAndPublishWithStatus(rssItem);
                }

                TempData.Add("Success", $"URL {rssFeedUrl.Url} was successfully imported.");
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
