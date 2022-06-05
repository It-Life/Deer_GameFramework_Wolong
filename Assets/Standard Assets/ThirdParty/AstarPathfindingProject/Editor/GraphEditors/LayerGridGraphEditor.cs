using UnityEngine;
using UnityEditor;

namespace Pathfinding {
	[CustomGraphEditor(typeof(LayerGridGraph), "Layered Grid Graph")]
	public class LayerGridGraphEditor : GridGraphEditor {
		public override void OnInspectorGUI (NavGraph target) {
			var graph = target as LayerGridGraph;

			base.OnInspectorGUI(target);

			if (graph.neighbours != NumNeighbours.Four) {
				Debug.Log("Note: Only 4 neighbours per grid node is allowed in this graph type");
			}
		}

		protected override void DrawJPS (GridGraph graph) {
			// No JPS for layered grid graph
		}

		protected override void DrawMiddleSection (GridGraph graph) {
			var layerGridGraph = graph as LayerGridGraph;

			DrawNeighbours(graph);

			layerGridGraph.characterHeight = EditorGUILayout.FloatField("Character Height", layerGridGraph.characterHeight);
			DrawMaxClimb(graph);

			DrawMaxSlope(graph);
			DrawErosion(graph);

			layerGridGraph.mergeSpanRange = EditorGUILayout.FloatField("Merge Span Range", layerGridGraph.mergeSpanRange);
		}

		protected override void DrawMaxClimb (GridGraph graph) {
			var layerGridGraph = graph as LayerGridGraph;

			base.DrawMaxClimb(graph);
			layerGridGraph.maxClimb = Mathf.Clamp(layerGridGraph.maxClimb, 0, layerGridGraph.characterHeight);

			if (layerGridGraph.maxClimb == layerGridGraph.characterHeight) {
				EditorGUILayout.HelpBox("Max climb needs to be smaller or equal to character height", MessageType.Info);
			}
		}

		protected override void DrawTextureData (GridGraph.TextureData data, GridGraph graph) {
			// No texture data for layered grid graphs
		}

		protected override void DrawNeighbours (GridGraph graph) {
			graph.neighbours = NumNeighbours.Four;
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.EnumPopup(new GUIContent("Connections", "Only 4 connections per node is possible on layered grid graphs"), graph.neighbours);
			EditorGUI.EndDisabledGroup();
		}

		protected override void DrawCutCorners (GridGraph graph) {
			// No corner cutting since only 4 neighbours are possible
		}

		protected override void DrawCollisionEditor (GraphCollision collision) {
			base.DrawCollisionEditor(collision);

			if (collision.thickRaycast) {
				EditorGUILayout.HelpBox("Note: Thick raycast cannot be used with this graph type", MessageType.Error);
			}
		}

		protected override void DrawUse2DPhysics (GraphCollision collision) {
			// 2D physics does not make sense for a layered grid graph
			collision.use2D = false;
		}
	}
}
