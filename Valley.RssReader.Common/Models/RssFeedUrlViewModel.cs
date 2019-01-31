using System.ComponentModel.DataAnnotations;

namespace Valley.RssReader.Common.Models
{
    public class RssFeedUrlViewModel
    {
        [Url, Required]
        public string Url { get; set; }
    }
}
