using System.Collections.Generic;

//保存场景清单数据结构
public class SceneListDetailsItem
{
    /// <summary>
    /// 
    /// </summary>
    public long itemId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string offsetPos { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string rotation { get; set; }
}

public class SceneListDetailsRoot
{
    /// <summary>
    /// 
    /// </summary>
    public List <SceneListDetailsItem > sceneListDetails { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string sceneName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string sceneUrlBytes { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long storeId { get; set; }
}
