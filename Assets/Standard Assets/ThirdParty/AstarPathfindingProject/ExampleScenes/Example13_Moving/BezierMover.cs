using UnityEngine;

namespace Pathfinding.Examples {
	/// <summary>
	/// Moves an object along a spline.
	/// Helper script in the example scene called 'Moving'.
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_examples_1_1_bezier_mover.php")]
	public class BezierMover : MonoBehaviour {
		public Transform[] points;

		public float speed = 1;
		public float tiltAmount = 1f;

		float time = 0;

		Vector3 Position (float t) {
			int c = points.Length;
			int pt = Mathf.FloorToInt(t) % c;

			return AstarSplines.CatmullRom(points[(pt-1+c)%c].position, points[pt].position, points[(pt+1)%c].position, points[(pt+2)%c].position, t - Mathf.FloorToInt(t));
		}

		/// <summary>Update is called once per frame</summary>
		void Update () {
			float mn = time;
			float mx = time+1;

			while (mx - mn > 0.0001f) {
				float mid = (mn+mx)/2;

				Vector3 p = Position(mid);
				if ((p-transform.position).sqrMagnitude > (speed*Time.deltaTime)*(speed*Time.deltaTime)) {
					mx = mid;
				} else {
					mn = mid;
				}
			}

			time = (mn+mx)/2;

			const float dt = 0.001f;
			const float dt2 = 0.15f;
			Vector3 p1 = Position(time);
			Vector3 p2 = Position(time+dt);
			transform.position = p1;

			Vector3 p3 = Position(time+dt2);
			Vector3 p4 = Position(time+dt2 + dt);

			// Estimate the acceleration at the current point and use it to tilt the object inwards on the curve
			var acceleration = ((p4 - p3).normalized - (p2 - p1).normalized) / (p3 - p1).magnitude;
			var up = new Vector3(0, 1/(tiltAmount + 0.00001f), 0) + acceleration;
			transform.rotation = Quaternion.LookRotation(p2 - p1, up);
		}

		void OnDrawGizmos () {
			if (points.Length >= 3) {
				for (int i = 0; i < points.Length; i++) if (points[i] == null) return;

				Gizmos.color = Color.white;
				Vector3 pp = Position(0);
				for (int pt = 0; pt < points.Length; pt++) {
					for (int i = 1; i <= 100; i++) {
						var p = Position(pt + (i / 100f));
						Gizmos.DrawLine(pp, p);
						pp = p;
					}
				}
			}
		}
	}
}
