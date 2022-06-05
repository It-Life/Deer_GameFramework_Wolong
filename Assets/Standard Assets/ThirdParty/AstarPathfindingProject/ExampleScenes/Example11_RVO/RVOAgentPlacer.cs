using UnityEngine;
using System.Collections;

namespace Pathfinding.Examples {
	/// <summary>
	/// Places ROV agents in circles.
	/// Used in a example scene
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_examples_1_1_r_v_o_agent_placer.php")]
	public class RVOAgentPlacer : MonoBehaviour {
		public int agents = 100;

		public float ringSize = 100;
		public LayerMask mask;

		public GameObject prefab;

		public Vector3 goalOffset;

		public float repathRate = 1;

		// Use this for initialization
		IEnumerator Start () {
			yield return null;

			for (int i = 0; i < agents; i++) {
				float angle = ((float)i / agents)*(float)System.Math.PI*2;

				Vector3 pos = new Vector3((float)System.Math.Cos(angle), 0, (float)System.Math.Sin(angle))*ringSize;
				Vector3 antipodal = -pos + goalOffset;

				GameObject go = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.Euler(0, angle+180, 0)) as GameObject;
				RVOExampleAgent ag = go.GetComponent<RVOExampleAgent>();

				if (ag == null) {
					Debug.LogError("Prefab does not have an RVOExampleAgent component attached");
					yield break;
				}

				//ag.radius = radius;
				go.transform.parent = transform;
				go.transform.position = pos;

				ag.repathRate = repathRate;
				ag.SetTarget(antipodal);

				ag.SetColor(GetColor(angle));
			}
		}

		const float rad2Deg = 360.0f/ ((float)System.Math.PI*2);

		public Color GetColor (float angle) {
			return AstarMath.HSVToRGB(angle * rad2Deg, 0.8f, 0.6f);
		}
	}
}
