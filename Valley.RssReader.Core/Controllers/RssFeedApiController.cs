using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using Umbraco.Web.WebApi;
using Valley.RssReader.Common.Entities;
using Valley.RssReader.Common.Models;
using Valley.RssReader.Common.Services.Interfaces;
using Valley.RssReader.Core.Services.Interfaces;

namespace Valley.RssReader.Core.Controllers
{
    public class RssFeedApiController : UmbracoApiController
    {
        private readonly IRssItemMappingService _rssItemMappingService;
        private readonly ICachingService<RssItemDto> _cachingService;

        public RssFeedApiController(IRssItemMappingService rssItemMappingService, ICachingService<RssItemDto> cachingService)
        {
            _rssItemMappingService = rssItemMappingService;
            _cachingService = cachingService;
        }

        [AcceptVerbs("GET")]
        public IEnumerable<RssItemDto> GetRssItems(int pageIndex, int pageSize)
        {
            int homeId = Services.ContentService.GetRootContent().Single(c => c.Name == "Home").Id;
            int start = pageIndex * pageSize;
            int end = Math.Min(start + pageSize, Services.ContentService.CountChildren(homeId));
            if (end < start) return Enumerable.Empty<RssItemDto>();

            _cachingService.CacheProvider = ApplicationContext.ApplicationCache.RuntimeCache;

            // Check if all the items requested are cached first. If any are missing, then stop using the cache and get the whole page from Umbraco.
            if (_cachingService.Get(Enumerable.Range(start, end - start).Select(i => i.ToString()), out IEnumerable<RssItemDto> cachedItems))
            {
                return cachedItems;
            }

            RssItemDto[] rssItems = _rssItemMappingService.Map(Services.ContentService.GetPagedChildren(homeId, pageIndex, pageSize, out long totalRecords).Select(c => new RssItemViewModel
            {
                Title = c.GetValue<string>("title"),
                Description = c.GetValue<string>("description"),
                Categories = c.GetValue<string>("categories"),
                Date = c.GetValue<string>("date"),
                Link = c.GetValue<string>("link")
            })).ToArray();

            // Cache the retrieved items.
            _cachingService.Insert(Enumerable.Range(start, Math.Min(pageSize, Convert.ToInt32(totalRecords) - start)).ToDictionary(i => i.ToString(), i => rssItems[i - start]));

            return rssItems;
        }
    }
}
