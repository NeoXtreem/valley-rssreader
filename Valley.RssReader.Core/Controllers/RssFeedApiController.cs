using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Web.WebApi;
using Valley.RssReader.Common.Entities;
using Valley.RssReader.Common.Models;
using Valley.RssReader.Common.Services.Interfaces;

namespace Valley.RssReader.Core.Controllers
{
    public class RssFeedApiController : UmbracoApiController
    {
        private readonly IRssItemMappingService _rssItemMappingService;

        public RssFeedApiController(IRssItemMappingService rssItemMappingService) => _rssItemMappingService = rssItemMappingService;

        [System.Web.Http.AcceptVerbs("GET")]
        public IEnumerable<RssItemDto> GetRssItems(int pageIndex, int pageSize)
        {
            int start = pageIndex * pageSize;
            int end = Math.Min(start + pageSize, Services.ContentService.CountChildren(1061));

            // Check if all the items requested are cached first. If any are missing, then stop using the cache and get the whole page from Umbraco.
            var cachedItems = new List<RssItemDto>();
            for (int i = start; i < end; i++)
            {
                var cachedItem = (RssItemDto)ApplicationContext.ApplicationCache.RuntimeCache.GetCacheItem(i.ToString());
                if (cachedItem is null) break;

                cachedItems.Add(cachedItem);
            }

            // Only return the cached items if the whole page was cached.
            if (cachedItems.Count == end - start) return cachedItems;

            RssItemDto[] rssItems = _rssItemMappingService.Map(Services.ContentService.GetPagedChildren(1061, pageIndex, pageSize, out long totalRecords).Select(c => new RssItemViewModel
            {
                Title = c.GetValue<string>("title"),
                Description = c.GetValue<string>("description"),
                Categories = c.GetValue<string>("categories"),
                Date = c.GetValue<string>("date"),
                Link = c.GetValue<string>("link")
            })).ToArray();

            // Cache the retrieved items.
            for (int i = start; i < Math.Min(start + pageSize, totalRecords); i++)
            {
                int index = i;
                ApplicationContext.ApplicationCache.RuntimeCache.InsertCacheItem(index.ToString(), () => rssItems[index - start]);
            }

            return rssItems;
        }
    }
}
