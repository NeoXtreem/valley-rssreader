using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Schema;
using Valley.RssReader.Common.Entities;
using Valley.RssReader.Core.Exceptions;
using Valley.RssReader.Core.Services.Interfaces;

namespace Valley.RssReader.Core.Services
{
    public class RssReaderService : IRssReaderService
    {
        public IEnumerable<RssItemDto> Read(Uri uri)
        {
            using (XmlReader reader = XmlReader.Create(uri.AbsoluteUri, new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse, ValidationType = ValidationType.DTD }))
            {
                try
                {
                    return SyndicationFeed.Load(reader).Items.Select(i => new RssItemDto
                    {
                        Id = i.Id,
                        Title = i.Title.Text,
                        Description = i.Summary.Text,
                        Categories = i.Categories.Select(c => c.Name),
                        Date = i.PublishDate,
                        Links = i.Links.Select(l => l.GetAbsoluteUri())
                    });
                }
                catch (XmlException e)
                {
                    throw new RssReaderException($"Unable to read RSS feed from URL provided. {e.Message}", e) { Url = uri.AbsoluteUri };
                }
                catch (XmlSchemaException e)
                {
                    throw new RssReaderException($"XML from RSS feed in incorrect format. {e.Message}", e) { Url = uri.AbsoluteUri };
                }
            }
        }
    }
}
