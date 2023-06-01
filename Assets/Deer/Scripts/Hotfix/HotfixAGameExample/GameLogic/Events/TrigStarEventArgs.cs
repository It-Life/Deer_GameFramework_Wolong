using GameFramework;
using GameFramework.Event;

/// <summary>
/// 玩家碰撞到星星 的事件
/// </summary>
public class TrigStarEventArgs : GameEventArgs
{
	public static readonly int EventId = typeof(TrigStarEventArgs).GetHashCode();

	public TrigStarEventArgs()
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

	//测试数据
	public float StarNum
	{
		get;
		private set;
	}

	/// <summary>
	/// 获取用户自定义数据。
	/// </summary>
	public object UserData
	{
		get;
		private set;
	}


	public static TrigStarEventArgs Create(float starNum, object userData)
	{
		TrigStarEventArgs eventArgs = ReferencePool.Acquire<TrigStarEventArgs>();
		eventArgs.UserData = userData;
		eventArgs.StarNum = starNum;
		return eventArgs;
	}

	public override void Clear()
	{
		UserData = null;
	}
}