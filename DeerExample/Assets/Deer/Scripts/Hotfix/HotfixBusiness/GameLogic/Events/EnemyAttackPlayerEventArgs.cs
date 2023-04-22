using GameFramework;
using GameFramework.Event;

/// <summary>
/// 敌人攻击到玩家 的事件
/// </summary>
public sealed class EnemyAttackPlayerEventArgs : GameEventArgs
{
	public static readonly int EventId = typeof(EnemyAttackPlayerEventArgs).GetHashCode();

	public EnemyAttackPlayerEventArgs()
	{
		UserData = null;
	}


	public override int Id
	{
		get
		{
			return EventId;
		}
	}

	/// <summary>
	/// 获取用户自定义数据。
	/// </summary>
	public object UserData
	{
		get;
		private set;
	}

	public float Damage
	{
		get;
		private set;
	}

	public static EnemyAttackPlayerEventArgs Create(float damage, object userData)
	{
		EnemyAttackPlayerEventArgs enemyAttackPlayerEventArgs = ReferencePool.Acquire<EnemyAttackPlayerEventArgs>();
		enemyAttackPlayerEventArgs.Damage = damage;
		enemyAttackPlayerEventArgs.UserData = userData;
		return enemyAttackPlayerEventArgs;
	}

	public override void Clear()
	{
		UserData = null;
	}
}
