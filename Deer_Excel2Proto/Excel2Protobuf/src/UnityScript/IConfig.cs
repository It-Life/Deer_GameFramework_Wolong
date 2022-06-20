// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-07-09 08-18-03  
//修改作者 : 杜鑫 
//修改时间 : 2021-07-09 08-18-03  
//版 本 : 0.1 
// ===============================================
using System.Collections;
namespace Deer
{
    public interface IConfig
    {
        string Name { get;}
        IEnumerator LoadConfig(string path);

        void Clear();
    }
}