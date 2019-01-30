using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Valley.RssReader.Common.Entities;
using Valley.RssReader.Common.Models;
using Valley.RssReader.Common.Services.Interfaces;

namespace Valley.RssReader.Common.Services
{
    public class RssItemMappingService : IRssItemMappingService
    {
        private const string CategorySeparator = ", ";

        public RssItemMappingService()
        {
            Mapper.CreateMap<RssItemDto, RssItemViewModel>()
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date.ToString()))
                .ForMember(d => d.Categories, o => o.MapFrom(s => String.Join(CategorySeparator, s.Categories)))
                .ForMember(d => d.Link, o => o.MapFrom(s => s.Link.AbsoluteUri));

            Mapper.CreateMap<RssItemViewModel, RssItemDto>()
                .ForMember(d => d.Date, o => o.MapFrom(s => DateTimeOffset.Parse(s.Date)))
                .ForMember(d => d.Categories, o => o.MapFrom(s => s.Categories.Split(new[] { CategorySeparator }, StringSplitOptions.None)))
                .ForMember(d => d.Link, o => o.MapFrom(s => new Uri(s.Link)));
        }

        public IEnumerable<RssItemViewModel> Map(IEnumerable<RssItemDto> input)
        {
            return input.Select(Mapper.Map<RssItemDto, RssItemViewModel>);
        }

        public IEnumerable<RssItemDto> Map(IEnumerable<RssItemViewModel> input)
        {
            return input.Select(Mapper.Map<RssItemViewModel, RssItemDto>);
        }
    }
}
