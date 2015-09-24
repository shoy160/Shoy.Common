using System.Collections.Generic;
using System.Linq;
using Shoy.AutoMapper;
using Shoy.Core;
using Shoy.Core.Domain.Repositories;
using Shoy.Utility;
using Shoy.Utility.Helper;
using Shoy.Utility.Timing;
using Shoy.Wiki.Models;
using Shoy.Wiki.Models.Dtos;

namespace Shoy.Wiki.Contracts.Services
{
    public class WikiService : DService, IWikiContract
    {
        public WikiService(WikiDbContext unitOfWork)
            : base(unitOfWork)
        {
        }

        public IRepository<WikiDbContext, WikiGroup, string> Groups { private get; set; }
        public IRepository<WikiDbContext, Models.Wiki, string> Wikis { private get; set; }
        public IRepository<WikiDbContext, WikiDetail, string> Details { private get; set; }

        public IRepository<WikiGroup, string> GroupRepository
        {
            get { return Groups; }
        }

        public IRepository<Models.Wiki, string> WikiRepository
        {
            get { return Wikis; }
        }

        public DResult AddGroup(string name, string code, string creatorId, string logo = null)
        {
            if (Groups.Exists(g => g.Code == code || g.Group == name))
                return DResult.Error("词条分类已存在！");
            var item = new WikiGroup
            {
                Id = CombHelper.Guid16,
                Group = name,
                Code = code,
                Logo = logo,
                CreateTime = Clock.Now,
                WikiCount = 0,
                CreatorId = creatorId,
                Sort = Groups.Max(g => g.Sort) + 1
            };
            var result = Groups.Insert(item);
            return string.IsNullOrWhiteSpace(result)
                ? DResult.Error("添加词条分类失败！")
                : DResult.Success;
        }

        public DResult<Models.Wiki> AddWiki(string name, string groupId, string creatorId, string desc)
        {
            if (Wikis.Exists(w => w.Name == name))
                return DResult.Error<Models.Wiki>("词条已存在！");
            var group = Groups.SingleOrDefault(g => g.Id == groupId);
            if (group == null)
                return DResult.Error<Models.Wiki>("分组不存在！");
            var wiki = new Models.Wiki
            {
                Id = CombHelper.Guid16,
                Name = name,
                CreatorId = creatorId,
                GroupId = groupId,
                Hots = 0,
                Sort = 0,
                Description = desc,
                CreateTime = Clock.Now
            };
            wiki.Sort = Wikis.Max(w => w.Sort, w => w.GroupId == groupId) + 1;
            var result = UnitOfWork.Transaction(() =>
            {
                Wikis.Insert(wiki);
                group.WikiCount++;
                group.LastUpdateTime = Clock.Now;
                Groups.Update(group);
            });
            return result > 0 ? DResult.Succ(wiki) : DResult.Error<Models.Wiki>("添加词条失败！");
        }

        public DResult AddDetail(string wikiId, DetailDto detail)
        {
            if (!Wikis.Exists(w => w.Id == wikiId))
                return DResult.Error("词条不存在!");
            var item = detail.MapTo<WikiDetail>();
            item.Id = CombHelper.Guid16;
            item.WikiId = wikiId;
            item.Sort = Details.Max(d => d.Sort, d => d.WikiId == wikiId) + 1;
            var result = Details.Insert(item);
            return string.IsNullOrWhiteSpace(result)
                ? DResult.Error("添加词条详情失败！")
                : DResult.Success;
        }

        public DResult AddDetails(string wikiId, List<DetailDto> details)
        {
            if (!Wikis.Exists(w => w.Id == wikiId))
                return DResult.Error("词条不存在!");
            var list = details.MapTo<List<WikiDetail>>();
            var sort = Details.Max(d => d.Sort, d => d.WikiId == wikiId);
            foreach (var detail in list)
            {
                detail.Id = CombHelper.Guid16;
                detail.WikiId = wikiId;
                detail.Sort = ++sort;
            }
            var result = Details.Insert(list);
            return result <= 0
                ? DResult.Error("批量添加词条详情失败！")
                : DResult.Success;
        }

        public List<GroupDto> GroupList()
        {
            var groups = Groups.Table.OrderBy(g => g.Sort);
            var list = groups.MapTo<List<GroupDto>>();
            var wikis = Wikis.Where(t => t.Status == 0).GroupBy(t => t.GroupId)
                .ToDictionary(k => k.Key, v => v.OrderBy(w => w.Sort).Select(w => w.Name).ToList());
            foreach (var group in list)
            {
                if (wikis.ContainsKey(group.Id))
                    group.Wikis = wikis[group.Id];
            }
            return list;
        }

        public WikiDto Load(string wiki)
        {
            var item = Wikis.SingleOrDefault(t => t.Name == wiki && t.Status == 0);
            if (item == null)
                return null;
            var result = item.MapTo<WikiDto>();
            var group = Groups.SingleOrDefault(t => t.Id == item.GroupId);
            result.Group = group.MapTo<GroupDto>();
            var details = Details.Where(t => t.WikiId == item.Id).OrderBy(t => t.Sort);
            result.Details = details.MapTo<List<DetailDto>>();
            return result;
        }
    }
}