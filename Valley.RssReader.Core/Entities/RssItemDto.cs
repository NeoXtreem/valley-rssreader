using System;
using System.Collections.Generic;

namespace Valley.RssReader.Core.Entities
{
    public class RssItemDto
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> Categories { get; set; }

        public DateTimeOffset Date { get; set; }

        public IEnumerable<Uri> Links { get; set; }
    }
}
