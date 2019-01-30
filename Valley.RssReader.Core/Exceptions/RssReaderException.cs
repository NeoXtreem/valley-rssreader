using System;

namespace Valley.RssReader.Core.Exceptions
{
    public class RssReaderException : Exception
    {
        public RssReaderException()
        {
        }

        public RssReaderException(string message)
            : base(message)
        {
        }

        public RssReaderException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public string Url { get; set; }
    }
}
