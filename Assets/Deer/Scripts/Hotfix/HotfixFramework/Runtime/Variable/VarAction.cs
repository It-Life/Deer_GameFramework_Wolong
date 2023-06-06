// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-05-31 21-52-42
//修改作者:AlanDu
//修改时间:2023-05-31 21-52-42
//版 本:0.1 
// ===============================================

using System;
using GameFramework;

/// <summary>
/// Action
/// </summary>
public class VarAction : Variable<Action>
{
	/// <summary>
	/// 初始化 System.Action 变量类的新实例。
	/// </summary>
	public VarAction()
	{
	}

	/// <summary>
	/// 从 System.Action 到 System.Action 变量类的隐式转换。
	/// </summary>
	/// <param name="value">值。</param>
	public static implicit operator VarAction(Action value)
	{
		VarAction varValue = ReferencePool.Acquire<VarAction>();
		varValue.Value = value;
		return varValue;
	}

	/// <summary>
	/// 从 System.Action 变量类到 System.Action 的隐式转换。
	/// </summary>
	/// <param name="value">值。</param>
	public static implicit operator Action(VarAction value)
	{
		return value.Value;
	}
}