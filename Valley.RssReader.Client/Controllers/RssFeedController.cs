using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Valley.RssReader.Core.Entities;
using Valley.RssReader.Core.Models;
using Valley.RssReader.Core.Services.Interfaces;

namespace Valley.RssReader.Client.Controllers
{
    [Route("api/[controller]")]
    public class RssFeedController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IRssItemMappingService _rssItemMappingService;

        public RssFeedController(IConfiguration configuration, IRssItemMappingService rssItemMappingService)
        {
            _configuration = configuration;
            _rssItemMappingService = rssItemMappingService;
        }

        [HttpGet("[action]/{pageIndex}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<RssItemViewModel>>> GetRssItems(int pageIndex, int pageSize)
        {
            #if DEBUG
            const string addressSetting = "ServiceBaseAddressLocal";
            #else
            const string addressSetting = "ServiceBaseAddress";
            #endif

            HttpResponseMessage response = await new HttpClient().GetAsync(QueryHelpers.AddQueryString(
                new Uri(new Uri(_configuration.GetValue<string>(addressSetting)), "Umbraco/Api/RssFeedApi/GetRssItems").AbsoluteUri,
                new (string key, int value)[] { ("pageIndex", pageIndex), ("pageSize", pageSize) }.ToDictionary(p => p.key, p => p.value.ToString())));

            if (response.IsSuccessStatusCode)
            {
                return Ok(_rssItemMappingService.Map(JsonConvert.DeserializeObject<IEnumerable<RssItemDto>>(await response.Content.ReadAsStringAsync())));
            }

            return BadRequest(response.RequestMessage);
        }
    }
}
