using System.Collections.Generic;
using Valley.RssReader.Core.Entities;
using Valley.RssReader.Core.Models;

namespace Valley.RssReader.Core.Services.Interfaces
{
    public interface IRssItemMappingService
    {
        IEnumerable<RssItemViewModel> Map(IEnumerable<RssItemDto> input);

        IEnumerable<RssItemDto> Map(IEnumerable<RssItemViewModel> input);
    }
}
