using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Shoy.Data.EntityFramework;
using Shoy.MvcDemo.AutoMapper;
using Shoy.MvcDemo.Models;

namespace Shoy.MvcDemo.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(int page = 0, int size = 15)
        {
            var context = new SimpleDbContextProvider<CodeFirstDbContext>(new UserDbContext());
            var respository = new EfRepositoryBase<CodeFirstDbContext, User, long>(context);
            var clsResp = new EfRepositoryBase<CodeFirstDbContext, MClass, string>(context);
            //  sql
            //            var users =
            //                respository.SqlQuery(
            //                    "select top 15 [UserID] as [Id],[Email],[TrueName],[AddedAt],[Role],[Status] from [TU_User] where [Role]=@role order by AddedAt desc",
            //                    new SqlParameter("@role", (byte)4));
            var managers = clsResp.Query(
                c => c.ClassManagerID.HasValue && c.ClassManagerID > 0 && c.Status == (byte) 0)
                .Include(c => c.Manager)
                .Include(c => c.Agency);
            Mapper.CreateMap<MClass, ClassDto>()
                .ForMember(s => s.Manager, d => d.MapFrom(c => c.Manager.TrueName))
                .ForMember(s => s.AgencyName, d => d.MapFrom(c => c.Agency.AgencyName));
            var cls = Mapper.Map<List<ClassDto>>(managers);
            //  inner join
            //            var users = respository
            //                .Query(u => u.Status == 0)
            //                .Join(managers, u => u.Id, m => m.ClassManagerID, (e, o) => e)                
            //                .Distinct()
            //                .OrderByDescending(u => u.AddedAt)
            //                .Skip(page * size)
            //                .Take(size)
            //                .ToList();
            //  exists
            //            var users = respository
            //                .Query(u => managers.Any(m => m.ClassManagerID == u.Id))
            //                .OrderByDescending(u => u.AddedAt)
            //                .Skip(page * size)
            //                .Take(size)
            //                .ToList();

            // LEFT OUTER JOIN
            //            var users = clsResp.Query(
            //                c => c.ClassManagerID.HasValue && c.ClassManagerID > 0 && c.Status == (byte)0)
            //                .Select(c => c.Manager).Distinct().ToList();
            return View(cls);
        }
    }
}