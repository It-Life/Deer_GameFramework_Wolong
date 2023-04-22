using UnityEngine;

public class Star : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
	{
		if (other != null && other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			//当玩家碰到星星时, 发送TrigStarEventArgs消息
			GameEntry.Event.Fire(this, TrigStarEventArgs.Create(1, null));

			gameObject.SetActive(false);
		}
	}
}
