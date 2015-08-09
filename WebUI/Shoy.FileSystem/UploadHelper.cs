using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Shoy.Utility;
using Shoy.Utility.Extend;

namespace Shoy.FileSystem
{
    /// <summary> 上传文件辅助 </summary>
    public class UploadHelper
    {
        private readonly HttpContext _context;
        private readonly FileType _fileType;

        //        private string _errorMsg;

        public UploadHelper(HttpContext context = null, FileType fileType = FileType.Image)
        {
            _context = context ?? HttpContext.Current;
            _fileType = fileType;
        }

        /// <summary>
        /// 检查文件存在以及文件格式
        /// </summary>
        /// <returns></returns>
        private DResult CheckFile()
        {
            if (_context == null || _context.Request.Files.Count == 0)
            {
                return DResult.Error("请上传文件！");
            }
            foreach (string name in _context.Request.Files.Keys)
            {
                var file = _context.Request.Files[name];
                if (file == null)
                {
                    return DResult.Error("文件不存在！");
                }
                var result = Util.CheckFile(file.FileName, file.ContentLength, _fileType);
                if (!result.Status)
                    return result;
            }
            return DResult.Success;
        }

        public DResult<Dictionary<string, string>> Save()
        {
            var result = CheckFile();
            if (!result.Status)
                return DResult.Error<Dictionary<string, string>>(result.Message);
            var mongo = "save".QueryOrForm(0);
            if (mongo >= 0 || _fileType == FileType.Audio)
            {
                return DResult.Succ(GridFsSave());
            }
            var list = new Dictionary<string, string>();
            foreach (string name in _context.Request.Files.Keys)
            {
                var file = _context.Request.Files[name];
                if (file == null) continue;
                string fileName;
                if (_fileType == FileType.ZipFile)
                {
                    fileName = Path.Combine(Contains.BaseDirectory, "upload/paper/marking",
                        DateTime.Now.ToString("yyyyMM"),
                        Guid.NewGuid().ToString("N"));
                    Util.UnZipFile(file.InputStream, fileName);
                }
                else
                {
                    var path = DirectoryHelper.Instance.GetCurrentDirectory(_fileType, true);
                    fileName = Util.GenerateFileName(Path.GetExtension(file.FileName));
                    fileName = Path.Combine(path, fileName);
                    file.SaveAs(fileName);
                }
                list.Add(name, fileName.Replace(Contains.BaseDirectory, Contains.BaseUrl).Replace("\\", "/"));
            }
            return DResult.Succ(list);
        }

        private Dictionary<string, string> GridFsSave()
        {
            var database = (_fileType == FileType.Image ? "picture" : "attach");
            var manager = GridFsManager.Instance(database);
            var list = new Dictionary<string, string>();
            foreach (string name in _context.Request.Files.Keys)
            {
                var file = _context.Request.Files[name];
                if (file == null) continue;
                var fileName = manager.Save(file.InputStream);
                list.Add(name, string.Format("{0}mongo/{1}/{2}{3}", Contains.BaseUrl, database, fileName,
                    Path.GetExtension(file.FileName)));
            }
            return list;
        }
    }

    public enum FileType : byte
    {
        /// <summary> 所有类型 </summary>
        All = 0,

        /// <summary> 图片类型 </summary>
        Image = 1,

        /// <summary> 视频类型 </summary>
        Video = 2,

        /// <summary> 附件类型 </summary>
        Attach = 3,

        /// <summary> 认证文件，私密文件 </summary>
        Authentication = 4,

        /// <summary> 压缩文件，需解压 </summary>
        ZipFile = 5,

        /// <summary> 音频类型 </summary>
        Audio = 6
    }
}