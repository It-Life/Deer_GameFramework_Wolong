using UnityEngine;

public class GravitySphere : GravitySource {

	[SerializeField]
	float gravity = 9.81f;

	[SerializeField, Min(0f)]
	float innerFalloffRadius = 1f, innerRadius = 5f;

	[SerializeField, Min(0f)]
	float outerRadius = 10f, outerFalloffRadius = 15f;

	float innerFalloffFactor, outerFalloffFactor;

	public override Vector3 GetGravity (Vector3 position) {
		Vector3 vector = transform.position - position;
		float distance = vector.magnitude;
		if (distance > outerFalloffRadius || distance < innerFalloffRadius) {
			return Vector3.zero;
		}
		float g = gravity / distance;
		if (distance > outerRadius) {
			g *= 1f - (distance - outerRadius) * outerFalloffFactor;
		}
		else if (distance < innerRadius) {
			g *= 1f - (innerRadius - distance) * innerFalloffFactor;
		}
		return g * vector;
	}

	void Awake () {
		OnValidate();
	}

	void OnValidate () {
		innerFalloffRadius = Mathf.Max(innerFalloffRadius, 0f);
		innerRadius = Mathf.Max(innerRadius, innerFalloffRadius);
		outerRadius = Mathf.Max(outerRadius, innerRadius);
		outerFalloffRadius = Mathf.Max(outerFalloffRadius, outerRadius);

		innerFalloffFactor = 1f / (innerRadius - innerFalloffRadius);
		outerFalloffFactor = 1f / (outerFalloffRadius - outerRadius);
	}

	void OnDrawGizmos () {
		Vector3 p = transform.position;
		if (innerFalloffRadius > 0f && innerFalloffRadius < innerRadius) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(p, innerFalloffRadius);
		}
		Gizmos.color = Color.yellow;
		if (innerRadius > 0f && innerRadius < outerRadius) {
			Gizmos.DrawWireSphere(p, innerRadius);
		}
		Gizmos.DrawWireSphere(p, outerRadius);
		if (outerFalloffRadius > outerRadius) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(p, outerFalloffRadius);
		}
	}
}