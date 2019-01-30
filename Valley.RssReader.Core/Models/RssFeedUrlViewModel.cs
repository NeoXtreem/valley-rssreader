using System.ComponentModel.DataAnnotations;

namespace Valley.RssReader.Core.Models
{
    public class RssFeedUrlViewModel
    {
        [Url, Required]
        public string Url { get; set; }
    }
}
