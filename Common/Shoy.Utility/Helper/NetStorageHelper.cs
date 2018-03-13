using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Shoy.Utility.Logging;

namespace Shoy.Utility.Helper
{
    /// <summary> 网盘映射辅助类 </summary>
    public class NetStorageHelper
    {
        private static readonly ILogger Logger = LogManager.Logger<NetStorageHelper>();

        #region 实体定义
        public enum ERROR_ID
        {
            ERROR_SUCCESS = 0,  // Success 
            ERROR_ACCESS_DENIED = 5,
            ERROR_NOT_ENOUGH_MEMORY = 8,
            ERROR_READ_FAULT = 30,
            Windows_cannot_find_the_network_path = 51,
            ERROR_BAD_NETPATH = 53,
            ERROR_NETWORK_BUSY = 54,
            ERROR_NETWORK_ACCESS_DENIED = 65,
            ERROR_BAD_DEV_TYPE = 66,
            ERROR_BAD_NET_NAME = 67,
            ERROR_REQ_NOT_ACCEP = 71,
            ERROR_ALREADY_ASSIGNED = 85,
            ERROR_INVALID_PASSWORD = 86,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_OPEN_FAILED = 110,
            ERROR_INVALID_LEVEL = 124,
            ERROR_BUSY = 170,
            ERROR_MORE_DATA = 234,
            ERROR_NO_BROWSER_SERVERS_FOUND = 6118,
            ERROR_NO_NETWORK = 1222,
            ERROR_INVALID_HANDLE_STATE = 1609,
            ERROR_EXTENDED_ERROR = 1208,
            ERROR_DEVICE_ALREADY_REMEMBERED = 1202,
            ERROR_NO_NET_OR_BAD_PATH = 1203,
            the_user_has_not_been_granted_the_requested_logon_type_at_this_computer = 1385,
            unknown_user_name_or_bad_password = 1326,
            ERROR_ACCOUNT_RESTRICTION = 1327,
            ERROR_INVALID_WORKSTATION = 1329,
            logon_request_contained_an_invalid_logon_type_value = 1367,
        }

        public enum RESOURCE_SCOPE
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET = 2,
            RESOURCE_REMEMBERED = 3,
            RESOURCE_RECENT = 4,
            RESOURCE_CONTEXT = 5
        }

        public enum RESOURCE_TYPE
        {
            RESOURCETYPE_ANY = 0,
            RESOURCETYPE_DISK = 1,
            RESOURCETYPE_PRINT = 2,
            RESOURCETYPE_RESERVED = 8,
        }

        public enum RESOURCE_USAGE
        {
            RESOURCEUSAGE_CONNECTABLE = 1,
            RESOURCEUSAGE_CONTAINER = 2,
            RESOURCEUSAGE_NOLOCALDEVICE = 4,
            RESOURCEUSAGE_SIBLING = 8,
            RESOURCEUSAGE_ATTACHED = 16,
            RESOURCEUSAGE_ALL = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
        }

        public enum RESOURCE_DISPLAYTYPE
        {
            RESOURCEDISPLAYTYPE_GENERIC = 0,
            RESOURCEDISPLAYTYPE_DOMAIN = 1,
            RESOURCEDISPLAYTYPE_SERVER = 2,
            RESOURCEDISPLAYTYPE_SHARE = 3,
            RESOURCEDISPLAYTYPE_FILE = 4,
            RESOURCEDISPLAYTYPE_GROUP = 5,
            RESOURCEDISPLAYTYPE_NETWORK = 6,
            RESOURCEDISPLAYTYPE_ROOT = 7,
            RESOURCEDISPLAYTYPE_SHAREADMIN = 8,
            RESOURCEDISPLAYTYPE_DIRECTORY = 9,
            RESOURCEDISPLAYTYPE_TREE = 10,
            RESOURCEDISPLAYTYPE_NDSCONTAINER = 11
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NETRESOURCE
        {
            public RESOURCE_SCOPE dwScope;
            public RESOURCE_TYPE dwType;
            public RESOURCE_DISPLAYTYPE dwDisplayType;
            public RESOURCE_USAGE dwUsage;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpLocalName;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpRemoteName;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpComment;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpProvider;
        }
        #endregion

        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int WNetGetConnection(
            [MarshalAs(UnmanagedType.LPTStr)] string localName,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
            ref int length);

        [DllImport("mpr.dll", CharSet = CharSet.Ansi)]
        private static extern int WNetAddConnection2(NETRESOURCE[] lpNetResource, string password, string username,
            int flag);

        [DllImport("mpr.dll", CharSet = CharSet.Ansi)]
        private static extern int WNetCancelConnection2(string lpname, int flag, int force);

        /// <summary>  
        /// 映射网络驱动器  
        /// </summary>  
        /// <param name="localName">本地盘符 如U:</param>  
        /// <param name="remotePath">远程路经 如\\\\172.18.118.106\\f</param>  
        /// <param name="userName">远程服务器用户名</param>  
        /// <param name="password">远程服务器密码</param>  
        /// <returns>true映射成功，false映射失败</returns>  
        public static bool Connect(string localName, string remotePath, string userName, string password)
        {
            var shareDriver = new NETRESOURCE[1];
            shareDriver[0].dwScope = RESOURCE_SCOPE.RESOURCE_GLOBALNET;
            shareDriver[0].dwType = RESOURCE_TYPE.RESOURCETYPE_DISK;
            shareDriver[0].dwDisplayType = RESOURCE_DISPLAYTYPE.RESOURCEDISPLAYTYPE_SHARE;
            shareDriver[0].dwUsage = RESOURCE_USAGE.RESOURCEUSAGE_CONNECTABLE;
            shareDriver[0].lpLocalName = localName;
            shareDriver[0].lpRemoteName = remotePath;

            Disconnect(localName, 1);
            var ret = WNetAddConnection2(shareDriver, password, userName, 1);
            Logger.Debug($"Connect code:{ret}");
            if (ret != 0)
            {
                Logger.Warn($"网盘连接异常,code:{ret}");
            }

            return ret == 0;
        }

        /// <summary> 断开网路驱动器 </summary>  
        /// <param name="lpName">映射的盘符</param>  
        /// <param name="force">true时如果打开映射盘文件夹，也会断开,返回成功 false时打开映射盘文件夹，返回失败</param>  
        /// <returns></returns>  
        public static bool Disconnect(string lpName, int force)
        {
            var ret = WNetCancelConnection2(lpName, 1, force);
            Logger.Debug($"Disconnect code:{ret}");
            //2250连接不存在，0断开成功
            return ret == 0 || ret == 2250;
        }

        /// <summary>  
        /// 给定一个路径，返回的网络路径或原始路径。  
        /// 例如：给定路径 P:\2008年2月29日(P:为映射的网络驱动器名)，可能会返回：“//networkserver/照片/2008年2月9日”  
        /// </summary>   
        /// <param name="originalPath">指定的路径</param>  
        /// <returns>如果是本地路径，返回值与传入参数值一样；如果是本地映射的网络驱动器</returns>   
        public static string GetRealPath(string originalPath)
        {
            var sb = new StringBuilder(512);
            var size = sb.Capacity;
            if (originalPath.Length <= 2 || originalPath[1] != ':')
                return originalPath;
            var c = originalPath[0];
            if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z'))
                return originalPath;
            var error = WNetGetConnection(originalPath.Substring(0, 2),
                sb, ref size);

            if (error != 0) return originalPath;
            //var dir = new DirectoryInfo(originalPath);
            var path = Path.GetFullPath(originalPath)
                .Substring(Path.GetPathRoot(originalPath).Length);
            return Path.Combine(sb.ToString().TrimEnd(), path);

        }
    }
}