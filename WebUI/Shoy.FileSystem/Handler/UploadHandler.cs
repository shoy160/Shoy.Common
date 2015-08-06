using System.Linq;
using System.Web;
using Shoy.Utility.Extend;

namespace Shoy.FileSystem.Handler
{
    public class UploadHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            NomalUpload(context);
        }

        private void NomalUpload(HttpContext context)
        {
            if (context.Request.HttpMethod.ToLower() != "post")
            {
                //                var date = BsonValue.Create(DateTime.Parse("2015-07-29 09:49:29").ToUniversalTime());
                //                var list =
                //                    GridFsManager.Instance("db".Query("picture"))
                //                        .Find(Query.And(Query.GT("uploadDate", date), Query.EQ("", BsonValue.Create(""))));
                //                context.Response.Write(CommonExtension.ToJson(list.Select(t => new
                //                {
                //                    name = t.Name,
                //                    date = t.UploadDate.ToLocalTime().ToLongDateString()
                //                })));
                //                Util.InitConfig();
                return;
            }
            //查看原始数据
            //var stream = context.Request.InputStream;
            //using (var sr = new StreamReader(stream))
            //{
            //    System.IO.File.WriteAllText("d:\\form.txt", sr.ReadToEnd(), Encoding.UTF8);
            //}
            var type = "type".QueryOrForm(0);
            var helper = new UploadHelper(context, (FileType)type);
            var result = helper.Save();
            if (!result.Status)
            {
                Util.ResponseJson(CommonExtension.ToJson(new
                {
                    state = 0,
                    msg = result.Message
                }));
                return;
            }
            Util.ResponseJson(CommonExtension.ToJson(new
            {
                state = 1,
                urls = result.Data.Values.ToArray(),
                keys = context.Request.Files.AllKeys
            }));
        }
    }
}