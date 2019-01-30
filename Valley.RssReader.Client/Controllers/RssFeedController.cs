using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Valley.RssReader.Common.Entities;
using Valley.RssReader.Common.Models;
using Valley.RssReader.Common.Services.Interfaces;

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
            HttpResponseMessage response = await new HttpClient().GetAsync(_configuration.GetValue<string>("ServiceBaseAddress") + $"/Umbraco/Api/RssFeedApi/GetRssItems?pageIndex={pageIndex}&pageSize={pageSize}");

            if (response.IsSuccessStatusCode)
            {
                return Ok(_rssItemMappingService.Map(JsonConvert.DeserializeObject<IEnumerable<RssItemDto>>(await response.Content.ReadAsStringAsync())));
            }

            return BadRequest(response.RequestMessage);
        }
    }
}
