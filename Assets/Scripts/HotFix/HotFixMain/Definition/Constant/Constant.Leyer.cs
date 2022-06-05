using UnityEngine;

/// <summary>
/// 游戏常量类
/// </summary>
public static partial class Constant
{
    /// <summary>
    /// 层
    /// </summary>
    public static class Leyer
    {
        public const string DefaultLayerName = "Default";
        /// <summary>
        /// Default层
        /// </summary>
        public static readonly int DefaultLayerId = LayerMask.NameToLayer(DefaultLayerName);



        public const string UILayerName = "UI";
        /// <summary>
        /// UI层
        /// </summary>
        public static readonly int UILayerId = LayerMask.NameToLayer(UILayerName);


        public const string IgnoreAirWallLayerName = "IgnoreAirWall";
        /// <summary>
        /// Pet层
        /// </summary>
        public static readonly int IgnoreAirWallLayerId = LayerMask.NameToLayer(IgnoreAirWallLayerName);



        /// <summary>
        /// Player层
        /// </summary>
        public const string PlayerLayerName = "Player";
        public static readonly int PlayerLayerId = LayerMask.NameToLayer(PlayerLayerName);


        /// <summary>
        /// Monster层
        /// </summary>
        public const string MonsterLayerName = "Monster";
        public static readonly int MonsterLayerId = LayerMask.NameToLayer(MonsterLayerName);

        /// <summary>
        /// Monster2层
        /// </summary>
        public const string Monster2LayerName = "Monster2";
        public static readonly int Monster2LayerId = LayerMask.NameToLayer(Monster2LayerName);

        /// <summary>
        /// NoShadowMonster层
        /// </summary>
        public const string NoShadowMonsterName = "NoShadowMonster";
        public static readonly int NoShadowMonsterLayerId = LayerMask.NameToLayer(NoShadowMonsterName);

        /// <summary>
        /// Npc层
        /// </summary>
        public const string NpcLayerName = "Npc";
        public static readonly int NpcLayerId = LayerMask.NameToLayer(NpcLayerName);

        /// <summary>
        /// 关卡锁层(锁所有人)
        /// </summary>
        public const string NodeLockAllLayerName = "NodeLockAll";
        public static readonly int NodeLockAllLayerId = LayerMask.NameToLayer(NodeLockAllLayerName);



        /// <summary>
        /// 关卡锁层(锁玩家)
        /// </summary>
        public const string NodeLockPlayerLayerName = "NodeLockPlayer";
        public static readonly int NodeLockPlayerLayerId = LayerMask.NameToLayer(NodeLockPlayerLayerName);

        /// <summary>
        /// 关卡锁层(锁玩家)，没有击退/浮空反弹效果
        /// </summary>
        public const string NodeLockPlayerLayerName2 = "NodeLockPlayer2";
        public static readonly int NodeLockPlayerLayerId2 = LayerMask.NameToLayer(NodeLockPlayerLayerName2);

        /// <summary>
        /// 关卡锁层(锁怪物)
        /// </summary>
        public const string NodeLockMonsterLayerName = "NodeLockMonster";
        public static readonly int NodeLockMonsterLayerId = LayerMask.NameToLayer(NodeLockMonsterLayerName);


        /// <summary>
        /// SceneUnit层
        /// </summary>
        public const string SceneUnit = "SceneUnit";
        public static readonly int SceneUnitId = LayerMask.NameToLayer(SceneUnit);

        /// <summary>
        /// Trigger层
        /// </summary>
        public const string Trigger = "Trigger";
        public static readonly int TriggerId = LayerMask.NameToLayer(Trigger);


        /// <summary>
        /// Bullet层
        /// </summary>
        public const string Bullet = "Bullet";
        public static readonly int BulletId = LayerMask.NameToLayer(Bullet);




        /// <summary>
        /// Wall层
        /// </summary>
        public const string Wall = "Wall";
        public static readonly int WallId = LayerMask.NameToLayer(Wall);




        /// <summary>
        /// Ground层
        /// </summary>
        public const string Ground = "Ground";
        public static readonly int GroundId = LayerMask.NameToLayer(Ground);





        /// <summary>
        /// CameraEffect层
        /// </summary>
        public const string CameraEffect = "CameraEffect";
        public static readonly int CameraEffectId = LayerMask.NameToLayer(CameraEffect);

        public static int GetLayerBits(params int[] layer)
        {
            int bits = 0;
            for (int i = 0; i < layer.Length; i++)
            {
                bits = bits | 1 << layer[i];
            }
            return bits;
        }

    }

}

