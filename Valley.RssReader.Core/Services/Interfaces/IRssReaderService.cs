using System;
using System.Collections.Generic;
using Valley.RssReader.Common.Entities;

namespace Valley.RssReader.Core.Services.Interfaces
{
    public interface IRssReaderService
    {
        IEnumerable<RssItemDto> Read(Uri uri);
    }
}
