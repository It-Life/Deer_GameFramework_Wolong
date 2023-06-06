using UnityEngine;
using System.Collections.Generic;
using Pathfinding.RVO;
using Pathfinding.RVO.Sampled;

namespace Pathfinding.Examples {
	/// <summary>
	/// RVO Example Scene Unit Controller.
	/// Controls AIs and camera in the RVO example scene.
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_group_controller.php")]
	public class GroupController : MonoBehaviour {
		public GUIStyle selectionBox;
		public bool adjustCamera = true;

		Vector2 start, end;
		bool wasDown = false;


		List<RVOExampleAgent> selection = new List<RVOExampleAgent>();

		Simulator sim;

		Camera cam;

		public void Start () {
			cam = Camera.main;
			var simu = RVOSimulator.active;
			if (simu == null) {
				this.enabled = false;
				throw new System.Exception("No RVOSimulator in the scene. Please add one");
			}

			sim = simu.GetSimulator();
		}

		public void Update () {
			if (adjustCamera) {
				//Adjust camera
				List<Agent> agents = sim.GetAgents();

				float max = 0;
				for (int i = 0; i < agents.Count; i++) {
					float d = Mathf.Max(Mathf.Abs(agents[i].Position.x), Mathf.Abs(agents[i].Position.y));
					if (d > max) {
						max = d;
					}
				}

				float hh = max / Mathf.Tan((cam.fieldOfView*Mathf.Deg2Rad/2.0f));
				float hv = max / Mathf.Tan(Mathf.Atan(Mathf.Tan(cam.fieldOfView*Mathf.Deg2Rad/2.0f)*cam.aspect));

				var yCoord = Mathf.Max(hh, hv)*1.1f;
				yCoord = Mathf.Max(yCoord, 20);
				yCoord = Mathf.Min(yCoord, cam.farClipPlane - 1f);
				cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(0, yCoord, 0), Time.smoothDeltaTime*2);
			}

			if (Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.Mouse0)) {
				Order();
			}
		}

		// Update is called once per frame
		void OnGUI () {
			if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && !Input.GetKey(KeyCode.A)) {
				Select(start, end);
				wasDown = false;
			}

			if (Event.current.type == EventType.MouseDrag && Event.current.button == 0) {
				end = Event.current.mousePosition;

				if (!wasDown) { start = end; wasDown = true; }
			}

			if (Input.GetKey(KeyCode.A)) wasDown = false;
			if (wasDown) {
				Rect r = Rect.MinMaxRect(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y), Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y));
				if (r.width > 4 && r.height > 4)
					GUI.Box(r, "", selectionBox);
			}
		}

		public void Order () {
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit)) {
				float radsum = 0;
				for (int i = 0; i < selection.Count; i++) radsum += selection[i].GetComponent<RVOController>().radius;

				float radius = radsum / (Mathf.PI);
				radius *= 2f;

				for (int i = 0; i < selection.Count; i++) {
					float deg = 2*Mathf.PI*i/selection.Count;
					Vector3 p = hit.point + new Vector3(Mathf.Cos(deg), 0, Mathf.Sin(deg))*radius;
					//Debug.DrawRay (p,Vector3.up*4,Color.cyan);
					//Debug.Break();
					selection[i].SetTarget(p);
					selection[i].SetColor(GetColor(deg));
					selection[i].RecalculatePath();
				}
			}
		}

		public void Select (Vector2 _start, Vector2 _end) {
			_start.y = Screen.height - _start.y;
			_end.y = Screen.height - _end.y;

			Vector2 start = Vector2.Min(_start, _end);
			Vector2 end = Vector2.Max(_start, _end);

			if ((end-start).sqrMagnitude < 4*4) return;

			selection.Clear();

			RVOExampleAgent[] rvo = FindObjectsOfType(typeof(RVOExampleAgent)) as RVOExampleAgent[];
			for (int i = 0; i < rvo.Length; i++) {
				Vector2 sp = cam.WorldToScreenPoint(rvo[i].transform.position);
				if (sp.x > start.x && sp.y > start.y && sp.x < end.x && sp.y < end.y) {
					selection.Add(rvo[i]);
				}
			}
		}

		/// <summary>Radians to degrees constant</summary>
		const float rad2Deg = 360.0f/ ((float)System.Math.PI*2);

		/// <summary>Color from an angle</summary>
		public Color GetColor (float angle) {
			return AstarMath.HSVToRGB(angle * rad2Deg, 0.8f, 0.6f);
		}
	}
}
