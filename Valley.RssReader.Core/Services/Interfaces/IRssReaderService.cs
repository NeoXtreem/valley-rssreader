using System;
using System.Collections.Generic;
using Valley.RssReader.Core.Models;

namespace Valley.RssReader.Core.Services.Interfaces
{
    public interface IRssReaderService
    {
        IEnumerable<RssItemViewModel> Read(Uri uri);
    }
}
