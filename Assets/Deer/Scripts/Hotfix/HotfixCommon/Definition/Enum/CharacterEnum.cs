// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2022-03-17 18-19-43  
//修改作者 : 杜鑫 
//修改时间 : 2022-03-17 18-19-43  
//版 本 : 0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterEnum
{
}

public enum CharacterStateEnum 
{
    
}
public enum MoveMode
{
    Null = 0,
    /// <summary>
    /// 往前走
    /// </summary>
    Forward = 1,
    /// <summary>
    /// 往前跑
    /// </summary>
    ForwardRun = 2,
    /// <summary>
    /// 往后退
    /// </summary>
    Back = 3,
    /// <summary>
    /// 往后跑
    /// </summary>
    BackRun = 4,
    /// <summary>
    /// 走路进入跳跃
    /// </summary>
    Jump = 5,
    /// <summary>
    /// 跑步进入跳跃
    /// </summary>
    JumpRun = 6,
}

public enum MoveType 
{
    Null = 0,
    /// <summary>
    /// 向点运动
    /// </summary>
    MoveToPos = 1,
    /// <summary>
    /// 向角色运动
    /// </summary>
    MoveToTarget = 2,
    /// <summary>
    /// 向方向运动
    /// </summary>
    MoveToDir = 3,
    /// <summary>
    /// 向点运动(不带寻路，用于网络同步情况)
    /// </summary>
    TransToPos = 4,
}

public enum MoveDirection
{
    Right = 0,
    Left = 1,
    Up = 2,
    Down = 3,
    Back= 4,
    Forward= 5,
}

public enum SceneCharacterFace 
{
    Free = 0,
    LeftAndRight = 1,
    Left = 2,
    Right = 3,
}