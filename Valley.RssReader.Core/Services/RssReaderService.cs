using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using Valley.RssReader.Common.Entities;
using Valley.RssReader.Core.Services.Interfaces;

namespace Valley.RssReader.Core.Services
{
    public class RssReaderService : IRssReaderService
    {
        public IEnumerable<RssItemDto> Read(Uri uri)
        {
            using (XmlReader reader = XmlReader.Create(uri.AbsoluteUri))
            {
                return SyndicationFeed.Load(reader).Items.Select(i => new RssItemDto
                {
                    Id = i.Id,
                    Title = i.Title.Text,
                    Description = i.Summary.Text,
                    Categories = i.Categories.Select(c => c.Name),
                    Date = i.PublishDate,
                    Link = i.Links.First().GetAbsoluteUri()
                });
            }
        }
    }
}
