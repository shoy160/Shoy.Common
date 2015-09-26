
using System;
using System.Collections.Generic;
using Shoy.AutoMapper.Attributes;
using Shoy.Core.Domain.Entities;

namespace Shoy.Wiki.Models.Dtos
{
    [Serializable]
    [AutoMap(typeof(Wiki))]
    public class WikiDto : DEntity<string>
    {
        public string Name { get; set; }

        [MapFrom("Description")]
        public string Desc { get; set; }

        public int Hots { get; set; }

        public DateTime CreateTime { get; set; }

        public GroupDto Group { get; set; }

        public List<DetailDto> Details { get; set; }
    }
}