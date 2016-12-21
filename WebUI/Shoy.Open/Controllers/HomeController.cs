using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Shoy.Open.Controllers
{
    public class HomeController : ApiController
    {
        private static readonly List<User> Users = new List<User>
        {
            //new User {Id = 1, Name = "shoy", Address = "chengdu"}
        };

        static int _idSeed;

        [HttpGet]
        [Route("code")]
        public string Code(string code = null)
        {
            var list = HttpContext.Current.Request.Url.AbsoluteUri.Split('#');
            if (list.Length == 2)
                return list[1];
            return code;
        }


        /// <summary> 添加用户 </summary>
        /// <param name="user">用户数据</param>
        /// <returns></returns>
        public User Post(User user)
        {
            user.Id = ++_idSeed;
            Users.Add(user);

            return user;
        }

        /// <summary> 修改用户 </summary>
        /// <param name="id">用户编号</param>
        /// <param name="user">新用户信息</param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, User user)
        {
            var current = Get(id);
            if (current == null)
                return NotFound();

            user.Id = current.Id;
            Users.Remove(current);
            Users.Add(user);

            return Ok();
        }

        /// <summary> 删除用户 </summary>
        /// <param name="id">用户编号</param>
        public void Delete(int id)
        {
            var current = Get(id);
            Users.Remove(current);
        }

        /// <summary> 获取用户列表 </summary>
        /// <returns>FFF</returns>
        [Authorize]
        [Route("users")]
        public IEnumerable<User> GetAll()
        {
            var users = Users;
            var identity = User.Identity as ClaimsIdentity;
            if (identity == null) return new List<User>();
            var name = identity.FindFirst(ClaimTypes.Name);
            if (name == null) return new List<User>();
            var id = users.Count + 1;
            users.Add(new User { Id = id, Name = name.Value });
            return users;
        }

        /// <summary> 获取指定用户 </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public User Get(int id)
        {
            return Users.Find(i => i.Id == id);
        }
    }

    public class User
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
    }
}
