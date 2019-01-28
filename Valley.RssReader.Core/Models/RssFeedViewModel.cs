using System.Collections.Generic;

namespace Valley.RssReader.Core.Models
{
    public class RssFeedViewModel
    {
        public IEnumerable<RssItemViewModel> RssItems { get; set; }
    }
}
