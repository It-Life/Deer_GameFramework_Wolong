// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-07-09 08-18-03  
//修改作者 : 杜鑫 
//修改时间 : 2021-07-09 08-18-03  
//版 本 : 0.1 
// ===============================================

namespace Deer
{
    /// <summary>
    /// 配置表信息
    /// </summary>
    public class ConfigInfo
    {
        /// <summary>
        /// 配置文件名
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string Size
        {
            get;
            set;
        }

        /// <summary>
        /// 文件MD5
        /// </summary>
        public string MD5
        {
            get;
            set;
        }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount
        {
            get;
            set;
        }
    }
}