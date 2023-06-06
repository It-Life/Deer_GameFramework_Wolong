// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-15 20-56-53  
//修改作者 : 杜鑫 
//修改时间 : 2021-08-15 20-56-53  
//版 本 : 0.1 
// ===============================================
using System;
using UnityGameFramework.Runtime;

public static class EntityComponentExtension
{
    // 关于 EntityId 的约定：
    // 0 为无效
    // 正值用于和服务器通信的实体（如玩家角色、NPC、怪等，服务器只产生正值）
    // 负值用于本地生成的临时实体（如特效、FakeObject等）
    private static int m_SerialId = 0;

    public static EntityLogicBase GetGameEntity(this EntityComponent entityComponent, int entityId)
    {
        Entity entity = entityComponent.GetEntity(entityId);
        if (entity == null)
        {
            return null;
        }

        return (EntityLogicBase)entity.Logic;
    }

    public static void HideEntity(this EntityComponent entityComponent, EntityLogicBase entity)
    {
        entityComponent.HideEntity(entity.Entity);
    }

    public static void ShowEntity(this EntityComponent entityComponent, Type logicType, string entityGroup, int priority, EntityData data)
    {
        if (data == null)
        {
            Log.Warning("Data is invalid.");
            return;
        }
        if (!entityComponent.HasEntityGroup(entityGroup))
        {
            entityComponent.AddEntityGroup(entityGroup, 60, 60, 60,60);
        }
        entityComponent.ShowEntity(data.Id, logicType, AssetUtility.Entity.GetEntityAsset(data.GroupName,data.AssetName), entityGroup, priority, data);
    }

    public static int GenEntityId(this EntityComponent entityComponent)
    {
        m_SerialId++;
        return m_SerialId;
    }
}