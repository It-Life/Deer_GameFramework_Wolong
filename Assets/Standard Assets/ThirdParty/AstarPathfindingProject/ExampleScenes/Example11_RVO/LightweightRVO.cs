using UnityEngine;
using System.Collections.Generic;
using Pathfinding.RVO;

namespace Pathfinding.Examples {
	[RequireComponent(typeof(MeshFilter))]
	/// <summary>
	/// Lightweight RVO Circle Example.
	/// Lightweight script for simulating agents in a circle trying to reach their antipodal positions.
	/// This script, compared to using lots of RVOAgents shows the real power of the RVO simulator when
	/// little other overhead (e.g GameObjects) is present.
	///
	/// For example with this script, I can simulate 5000 agents at around 50 fps on my laptop (with desired simulation fps = 10 and interpolation, 2 threads)
	/// however when using prefabs, only instantiating the 5000 agents takes 10 seconds and it runs at around 5 fps.
	///
	/// This script will render the agents by generating a square for each agent combined into a single mesh with appropriate UV.
	///
	/// A few GUI buttons will be drawn by this script with which the user can change the number of agents.
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_lightweight_r_v_o.php")]
	public class LightweightRVO : MonoBehaviour {
		/// <summary>Number of agents created at start</summary>
		public int agentCount = 100;

		/// <summary>
		/// How large is the area where agents are placed.
		/// For e.g the circle example, it corresponds
		/// </summary>
		public float exampleScale = 100;


		public enum RVOExampleType {
			Circle,
			Line,
			Point,
			RandomStreams,
			Crossing
		}

		public RVOExampleType type = RVOExampleType.Circle;

		/// <summary>Agent radius</summary>
		public float radius = 3;

		/// <summary>Max speed for an agent</summary>
		public float maxSpeed = 2;

		/// <summary>How far in the future too look for agents</summary>
		public float agentTimeHorizon = 10;

		[HideInInspector]
		/// <summary>How far in the future too look for obstacles</summary>
		public float obstacleTimeHorizon = 10;

		/// <summary>Max number of neighbour agents to take into account</summary>
		public int maxNeighbours = 10;

		/// <summary>
		/// Offset from the agent position the actual drawn postition.
		/// Used to get rid of z-buffer issues
		/// </summary>
		public Vector3 renderingOffset = Vector3.up*0.1f;

		/// <summary>Enable the debug flag for all agents</summary>
		public bool debug = false;

		/// <summary>Mesh for rendering</summary>
		Mesh mesh;

		/// <summary>Reference to the simulator in the scene</summary>
		Pathfinding.RVO.Simulator sim;

		/// <summary>All agents handled by this script</summary>
		List<IAgent> agents;

		/// <summary>Goals for each agent</summary>
		List<Vector3> goals;

		/// <summary>Color for each agent</summary>
		List<Color> colors;

		Vector3[] verts;
		Vector2[] uv;
		int[] tris;
		Color[] meshColors;
		Vector2[] interpolatedVelocities;
		Vector2[] interpolatedRotations;

		public void Start () {
			mesh = new Mesh();
			RVOSimulator rvoSim = FindObjectOfType(typeof(RVOSimulator)) as RVOSimulator;
			if (rvoSim == null) {
				Debug.LogError("No RVOSimulator could be found in the scene. Please add a RVOSimulator component to any GameObject");
				return;
			}
			sim = rvoSim.GetSimulator();
			GetComponent<MeshFilter>().mesh = mesh;

			CreateAgents(agentCount);
		}

		public void OnGUI () {
			if (GUILayout.Button("2")) CreateAgents(2);
			if (GUILayout.Button("10")) CreateAgents(10);
			if (GUILayout.Button("100")) CreateAgents(100);
			if (GUILayout.Button("500")) CreateAgents(500);
			if (GUILayout.Button("1000")) CreateAgents(1000);
			if (GUILayout.Button("5000")) CreateAgents(5000);

			GUILayout.Space(5);

			if (GUILayout.Button("Random Streams")) {
				type = RVOExampleType.RandomStreams;
				CreateAgents(agents != null ? agents.Count : 100);
			}

			if (GUILayout.Button("Line")) {
				type = RVOExampleType.Line;
				CreateAgents(agents != null ? Mathf.Min(agents.Count, 100) : 10);
			}

			if (GUILayout.Button("Circle")) {
				type = RVOExampleType.Circle;
				CreateAgents(agents != null ? agents.Count : 100);
			}

			if (GUILayout.Button("Point")) {
				type = RVOExampleType.Point;
				CreateAgents(agents != null ? agents.Count : 100);
			}

			if (GUILayout.Button("Crossing")) {
				type = RVOExampleType.Crossing;
				CreateAgents(agents != null ? agents.Count : 100);
			}
		}

		private float uniformDistance (float radius) {
			float v = Random.value + Random.value;

			if (v > 1) return radius * (2-v);
			else return radius * v;
		}

		/// <summary>Create a number of agents in circle and restart simulation</summary>
		public void CreateAgents (int num) {
			this.agentCount = num;

			agents = new List<IAgent>(agentCount);
			goals = new List<Vector3>(agentCount);
			colors = new List<Color>(agentCount);

			sim.ClearAgents();

			if (type == RVOExampleType.Circle) {
				float circleRad = Mathf.Sqrt(agentCount * radius * radius * 4 / Mathf.PI) * exampleScale * 0.05f;

				for (int i = 0; i < agentCount; i++) {
					Vector3 pos = new Vector3(Mathf.Cos(i * Mathf.PI * 2.0f / agentCount), 0, Mathf.Sin(i * Mathf.PI * 2.0f / agentCount)) * circleRad * (1 + Random.value * 0.01f);
					IAgent agent = sim.AddAgent(new Vector2(pos.x, pos.z), pos.y);
					agents.Add(agent);
					goals.Add(-pos);
					colors.Add(AstarMath.HSVToRGB(i * 360.0f / agentCount, 0.8f, 0.6f));
				}
			} else if (type == RVOExampleType.Line) {
				for (int i = 0; i < agentCount; i++) {
					Vector3 pos = new Vector3((i % 2 == 0 ? 1 : -1) * exampleScale, 0, (i / 2) * radius * 2.5f);
					IAgent agent = sim.AddAgent(new Vector2(pos.x, pos.z), pos.y);
					agents.Add(agent);
					goals.Add(new Vector3(-pos.x, pos.y, pos.z));
					colors.Add(i % 2 == 0 ? Color.red : Color.blue);
				}
			} else if (type == RVOExampleType.Point) {
				for (int i = 0; i < agentCount; i++) {
					Vector3 pos = new Vector3(Mathf.Cos(i * Mathf.PI * 2.0f / agentCount), 0, Mathf.Sin(i * Mathf.PI * 2.0f / agentCount)) * exampleScale;
					IAgent agent = sim.AddAgent(new Vector2(pos.x, pos.z), pos.y);
					agents.Add(agent);
					goals.Add(new Vector3(0, pos.y, 0));
					colors.Add(AstarMath.HSVToRGB(i * 360.0f / agentCount, 0.8f, 0.6f));
				}
			} else if (type == RVOExampleType.RandomStreams) {
				float circleRad = Mathf.Sqrt(agentCount * radius * radius * 4 / Mathf.PI) * exampleScale * 0.05f;

				for (int i = 0; i < agentCount; i++) {
					float angle = Random.value * Mathf.PI * 2.0f;
					float targetAngle = Random.value * Mathf.PI * 2.0f;
					Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * uniformDistance(circleRad);
					IAgent agent = sim.AddAgent(new Vector2(pos.x, pos.z), pos.y);
					agents.Add(agent);
					goals.Add(new Vector3(Mathf.Cos(targetAngle), 0, Mathf.Sin(targetAngle)) * uniformDistance(circleRad));
					colors.Add(AstarMath.HSVToRGB(targetAngle * Mathf.Rad2Deg, 0.8f, 0.6f));
				}
			} else if (type == RVOExampleType.Crossing) {
				float distanceBetweenGroups = exampleScale * radius * 0.5f;
				int directions = (int)Mathf.Sqrt(agentCount / 25f);
				directions = Mathf.Max(directions, 2);

				const int AgentsPerDistance = 10;
				for (int i = 0; i < agentCount; i++) {
					float angle = ((i % directions)/(float)directions) * Mathf.PI * 2.0f;
					var dist = distanceBetweenGroups * ((i/(directions*AgentsPerDistance) + 1) + 0.3f*Random.value);
					Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * dist;
					IAgent agent = sim.AddAgent(new Vector2(pos.x, pos.z), pos.y);
					agent.Priority = (i % directions) == 0 ? 1 : 0.01f;
					agents.Add(agent);
					goals.Add(-pos.normalized * distanceBetweenGroups * 3);
					colors.Add(AstarMath.HSVToRGB(angle * Mathf.Rad2Deg, 0.8f, 0.6f));
				}
			}

			SetAgentSettings();

			verts = new Vector3[4*agents.Count];
			uv = new Vector2[verts.Length];
			tris = new int[agents.Count*2*3];
			meshColors = new Color[verts.Length];
		}

		void SetAgentSettings () {
			for (int i = 0; i < agents.Count; i++) {
				IAgent agent = agents[i];
				agent.Radius = radius;
				agent.AgentTimeHorizon = agentTimeHorizon;
				agent.ObstacleTimeHorizon = obstacleTimeHorizon;
				agent.MaxNeighbours = maxNeighbours;
				agent.DebugDraw = i == 0 && debug;
			}
		}

		public void Update () {
			if (agents == null || mesh == null) return;

			if (agents.Count != goals.Count) {
				Debug.LogError("Agent count does not match goal count");
				return;
			}

			SetAgentSettings();

			// Make sure the array is large enough
			if (interpolatedVelocities == null || interpolatedVelocities.Length < agents.Count) {
				var velocities = new Vector2[agents.Count];
				var directions = new Vector2[agents.Count];
				// Copy over the old velocities
				if (interpolatedVelocities != null) for (int i = 0; i < interpolatedVelocities.Length; i++) velocities[i] = interpolatedVelocities[i];
				if (interpolatedRotations != null) for (int i = 0; i < interpolatedRotations.Length; i++) directions[i] = interpolatedRotations[i];
				interpolatedVelocities = velocities;
				interpolatedRotations = directions;
			}

			for (int i = 0; i < agents.Count; i++) {
				IAgent agent = agents[i];

				// Move agent
				// This is the responsibility of this script, not the RVO system
				Vector2 pos = agent.Position;
				var deltaPosition = Vector2.ClampMagnitude(agent.CalculatedTargetPoint - pos, agent.CalculatedSpeed * Time.deltaTime);
				pos += deltaPosition;
				agent.Position = pos;

				// All agents are on the same plane
				agent.ElevationCoordinate = 0;

				// Set the desired velocity for all agents
				var target = new Vector2(goals[i].x, goals[i].z);
				var dist = (target - pos).magnitude;
				agent.SetTarget(target, Mathf.Min(dist, maxSpeed), maxSpeed*1.1f);

				interpolatedVelocities[i] += deltaPosition;
				if (interpolatedVelocities[i].magnitude > maxSpeed*0.1f) {
					interpolatedVelocities[i] = Vector2.ClampMagnitude(interpolatedVelocities[i], maxSpeed*0.1f);
					interpolatedRotations[i] = Vector2.Lerp(interpolatedRotations[i], interpolatedVelocities[i], agent.CalculatedSpeed * Time.deltaTime*4f);
				}

				//Debug.DrawRay(new Vector3(pos.x, 0, pos.y), new Vector3(interpolatedVelocities[i].x, 0, interpolatedVelocities[i].y) * 10);
				// Create a square with the "forward" direction along the agent's velocity
				Vector3 forward = new Vector3(interpolatedRotations[i].x, 0, interpolatedRotations[i].y).normalized * agent.Radius;
				if (forward == Vector3.zero) forward = new Vector3(0, 0, agent.Radius);
				Vector3 right = Vector3.Cross(Vector3.up, forward);
				Vector3 orig = new Vector3(agent.Position.x, agent.ElevationCoordinate, agent.Position.y) + renderingOffset;


				int vc = 4*i;
				int tc = 2*3*i;
				verts[vc+0] = (orig + forward - right);
				verts[vc+1] = (orig + forward + right);
				verts[vc+2] = (orig - forward + right);
				verts[vc+3] = (orig - forward - right);

				uv[vc+0] = (new Vector2(0, 1));
				uv[vc+1] = (new Vector2(1, 1));
				uv[vc+2] = (new Vector2(1, 0));
				uv[vc+3] = (new Vector2(0, 0));

				meshColors[vc+0] = colors[i];
				meshColors[vc+1] = colors[i];
				meshColors[vc+2] = colors[i];
				meshColors[vc+3] = colors[i];

				tris[tc+0] = (vc + 0);
				tris[tc+1] = (vc + 1);
				tris[tc+2] = (vc + 2);

				tris[tc+3] = (vc + 0);
				tris[tc+4] = (vc + 2);
				tris[tc+5] = (vc + 3);
			}

			//Update the mesh
			mesh.Clear();
			mesh.vertices = verts;
			mesh.uv = uv;
			mesh.colors = meshColors;
			mesh.triangles = tris;
			mesh.RecalculateNormals();
		}
	}
}
