using System;
using System.Collections.Generic;
using Shoy.AutoMapper.Attributes;
using Shoy.Core.Domain.Entities;

namespace Shoy.Wiki.Models.Dtos
{
    [Serializable]
    [AutoMap(typeof(WikiGroup))]
    public class GroupDto : DEntity<string>
    {
        [MapFrom("Group")]
        public string Name { get; set; }

        public string Code { get; set; }

        public string Logo { get; set; }

        public int Sort { get; set; }

        public List<string> Wikis { get; set; }

        public GroupDto()
        {
            Wikis = new List<string>();
        }
    }
}