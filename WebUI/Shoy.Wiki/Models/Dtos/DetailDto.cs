using System;
using Shoy.AutoMapper.Attributes;
using Shoy.Core.Domain.Entities;

namespace Shoy.Wiki.Models.Dtos
{
    [Serializable]
    [AutoMap(typeof(WikiDetail))]
    public class DetailDto : DEntity<string>
    {
        public string Title { get; set; }
        public string Detail { get; set; }
        public int Sort { get; set; }
        public string Version { get; set; }
    }
}