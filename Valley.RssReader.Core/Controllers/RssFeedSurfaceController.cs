using System;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Valley.RssReader.Core.Models;
using Valley.RssReader.Core.Services.Interfaces;

namespace Valley.RssReader.Core.Controllers
{
    public class RssFeedSurfaceController : SurfaceController
    {
        private readonly IRssReaderService _rssReaderService;

        public RssFeedSurfaceController(IRssReaderService rssReaderService) => _rssReaderService = rssReaderService;

        // GET
        public ActionResult Index()
        {
            Services.ContentService.DeleteContentOfType(1060);

            foreach (RssItemViewModel rssItem in _rssReaderService.Read(new Uri("http://www.nu.nl/rss/Algemeen")))
            {
                IContent rssItemContent = Services.ContentService.CreateContent($"RSS Item {rssItem.Id}", 1061, "RssItem");
                rssItemContent.SetValue("title", rssItem.Title);
                rssItemContent.SetValue("description", rssItem.Description);
                rssItemContent.SetValue("categories", String.Join(", ", rssItem.Categories));
                rssItemContent.SetValue("date", rssItem.Date.ToString());
                rssItemContent.SetValue("link", rssItem.Link.AbsoluteUri);
                Services.ContentService.SaveAndPublishWithStatus(rssItemContent);
            }

            return PartialView("RssFeed");
        }
    }
}
