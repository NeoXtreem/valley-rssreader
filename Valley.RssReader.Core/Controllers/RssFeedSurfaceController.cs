using System;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Valley.RssReader.Common.Models;
using Valley.RssReader.Common.Services.Interfaces;
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

        // GET
        public ActionResult Index()
        {
            Services.ContentService.DeleteContentOfType(1060);

            foreach (RssItemViewModel rssItem in _rssItemMappingService.Map(_rssReaderService.Read(new Uri("http://www.nu.nl/rss/Algemeen"))))
            {
                IContent rssItemContent = Services.ContentService.CreateContent($"RSS Item {rssItem.Id}", 1061, "RssItem");
                rssItemContent.SetValue("title", rssItem.Title);
                rssItemContent.SetValue("description", rssItem.Description);
                rssItemContent.SetValue("categories", rssItem.Categories);
                rssItemContent.SetValue("date", rssItem.Date);
                rssItemContent.SetValue("link", rssItem.Link);
                Services.ContentService.SaveAndPublishWithStatus(rssItemContent);
            }

            return PartialView("RssFeed");
        }
    }
}
