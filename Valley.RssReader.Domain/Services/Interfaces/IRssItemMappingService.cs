using System.Collections.Generic;
using Valley.RssReader.Common.Entities;
using Valley.RssReader.Common.Models;

namespace Valley.RssReader.Common.Services.Interfaces
{
    public interface IRssItemMappingService
    {
        IEnumerable<RssItemViewModel> Map(IEnumerable<RssItemDto> input);

        IEnumerable<RssItemDto> Map(IEnumerable<RssItemViewModel> input);
    }
}
