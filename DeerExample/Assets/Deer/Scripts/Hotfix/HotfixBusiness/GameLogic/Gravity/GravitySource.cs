using UnityEngine;

public class GravitySource : MonoBehaviour
{
	public virtual Vector3 GetGravity(Vector3 position)
	{
		return Physics.gravity;
	}

	void OnEnable()
	{
		CustomGravity.Register(this);
	}

	void OnDisable()
	{
		CustomGravity.Unregister(this);
	}
}
