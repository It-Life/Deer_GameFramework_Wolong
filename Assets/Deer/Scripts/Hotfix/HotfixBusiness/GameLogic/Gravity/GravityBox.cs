using UnityEngine;

public class GravityBox : GravitySource {

	[SerializeField]
	float gravity = 9.81f;

	[SerializeField]
	Vector3 boundaryDistance = Vector3.one;

	[SerializeField, Min(0f)]
	float innerDistance = 0f, innerFalloffDistance = 0f;

	[SerializeField, Min(0f)]
	float outerDistance = 0f, outerFalloffDistance = 0f;

	float innerFalloffFactor, outerFalloffFactor;

	public override Vector3 GetGravity (Vector3 position) {
		position =
			transform.InverseTransformDirection(position - transform.position);

		Vector3 vector = Vector3.zero;
		int outside = 0;
		if (position.x > boundaryDistance.x) {
			vector.x = boundaryDistance.x - position.x;
			outside = 1;
		}
		else if (position.x < -boundaryDistance.x) {
			vector.x = -boundaryDistance.x - position.x;
			outside = 1;
		}

		if (position.y > boundaryDistance.y) {
			vector.y = boundaryDistance.y - position.y;
			outside += 1;
		}
		else if (position.y < -boundaryDistance.y) {
			vector.y = -boundaryDistance.y - position.y;
			outside += 1;
		}

		if (position.z > boundaryDistance.z) {
			vector.z = boundaryDistance.z - position.z;
			outside += 1;
		}
		else if (position.z < -boundaryDistance.z) {
			vector.z = -boundaryDistance.z - position.z;
			outside += 1;
		}

		if (outside > 0) {
			float distance = outside == 1 ?
				Mathf.Abs(vector.x + vector.y + vector.z) : vector.magnitude;
			if (distance > outerFalloffDistance) {
				return Vector3.zero;
			}
			float g = gravity / distance;
			if (distance > outerDistance) {
				g *= 1f - (distance - outerDistance) * outerFalloffFactor;
			}
			return transform.TransformDirection(g * vector);
		}

		Vector3 distances;
		distances.x = boundaryDistance.x - Mathf.Abs(position.x);
		distances.y = boundaryDistance.y - Mathf.Abs(position.y);
		distances.z = boundaryDistance.z - Mathf.Abs(position.z);
		if (distances.x < distances.y) {
			if (distances.x < distances.z) {
				vector.x = GetGravityComponent(position.x, distances.x);
			}
			else {
				vector.z = GetGravityComponent(position.z, distances.z);
			}
		}
		else if (distances.y < distances.z) {
			vector.y = GetGravityComponent(position.y, distances.y);
		}
		else {
			vector.z = GetGravityComponent(position.z, distances.z);
		}
		return transform.TransformDirection(vector);
	}

	float GetGravityComponent (float coordinate, float distance) {
		if (distance > innerFalloffDistance) {
			return 0f;
		}
		float g = gravity;
		if (distance > innerDistance) {
			g *= 1f - (distance - innerDistance) * innerFalloffFactor;
		}
		return coordinate > 0f ? -g : g;
	}

	void Awake () {
		OnValidate();
	}

	void OnValidate () {
		boundaryDistance = Vector3.Max(boundaryDistance, Vector3.zero);
		float maxInner = Mathf.Min(
			Mathf.Min(boundaryDistance.x, boundaryDistance.y), boundaryDistance.z
		);
		innerDistance = Mathf.Min(innerDistance, maxInner);
		innerFalloffDistance =
			Mathf.Max(Mathf.Min(innerFalloffDistance, maxInner), innerDistance);
		outerFalloffDistance = Mathf.Max(outerFalloffDistance, outerDistance);

		innerFalloffFactor = 1f / (innerFalloffDistance - innerDistance);
		outerFalloffFactor = 1f / (outerFalloffDistance - outerDistance);
	}

	void OnDrawGizmos () {
		Gizmos.matrix =
			Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		Vector3 size;
		if (innerFalloffDistance > innerDistance) {
			Gizmos.color = Color.cyan;
			size.x = 2f * (boundaryDistance.x - innerFalloffDistance);
			size.y = 2f * (boundaryDistance.y - innerFalloffDistance);
			size.z = 2f * (boundaryDistance.z - innerFalloffDistance);
			Gizmos.DrawWireCube(Vector3.zero, size);
		}
		if (innerDistance > 0f) {
			Gizmos.color = Color.yellow;
			size.x = 2f * (boundaryDistance.x - innerDistance);
			size.y = 2f * (boundaryDistance.y - innerDistance);
			size.z = 2f * (boundaryDistance.z - innerDistance);
			Gizmos.DrawWireCube(Vector3.zero, size);
		}
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(Vector3.zero, 2f * boundaryDistance);
		if (outerDistance > 0f) {
			Gizmos.color = Color.yellow;
			DrawGizmosOuterCube(outerDistance);
		}
		if (outerFalloffDistance > outerDistance) {
			Gizmos.color = Color.cyan;
			DrawGizmosOuterCube(outerFalloffDistance);
		}
	}

	void DrawGizmosOuterCube (float distance) {
		Vector3 a, b, c, d;
		a.y = b.y = boundaryDistance.y;
		d.y = c.y = -boundaryDistance.y;
		b.z = c.z = boundaryDistance.z;
		d.z = a.z = -boundaryDistance.z;
		a.x = b.x = c.x = d.x = boundaryDistance.x + distance;
		DrawGizmosRect(a, b, c, d);
		a.x = b.x = c.x = d.x = -a.x;
		DrawGizmosRect(a, b, c, d);

		a.x = d.x = boundaryDistance.x;
		b.x = c.x = -boundaryDistance.x;
		a.z = b.z = boundaryDistance.z;
		c.z = d.z = -boundaryDistance.z;
		a.y = b.y = c.y = d.y = boundaryDistance.y + distance;
		DrawGizmosRect(a, b, c, d);
		a.y = b.y = c.y = d.y = -a.y;
		DrawGizmosRect(a, b, c, d);

		a.x = d.x = boundaryDistance.x;
		b.x = c.x = -boundaryDistance.x;
		a.y = b.y = boundaryDistance.y;
		c.y = d.y = -boundaryDistance.y;
		a.z = b.z = c.z = d.z = boundaryDistance.z + distance;
		DrawGizmosRect(a, b, c, d);
		a.z = b.z = c.z = d.z = -a.z;
		DrawGizmosRect(a, b, c, d);
		
		distance *= 0.5773502692f;
		Vector3 size = boundaryDistance;
		size.x = 2f * (size.x + distance);
		size.y = 2f * (size.y + distance);
		size.z = 2f * (size.z + distance);
		Gizmos.DrawWireCube(Vector3.zero, size);
	}

	void DrawGizmosRect (Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
		Gizmos.DrawLine(a, b);
		Gizmos.DrawLine(b, c);
		Gizmos.DrawLine(c, d);
		Gizmos.DrawLine(d, a);
	}
}