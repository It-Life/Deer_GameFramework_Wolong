using UnityEngine;
using System.Collections.Generic;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component allows you to define a shape using 2D points.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanShape")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Shape")]
	public class LeanShape : MonoBehaviour
	{
		/// <summary>Should the start and end points of this shape be connected, forming a loop?</summary>
		public bool ConnectEnds { set { connectEnds = value; } get { return connectEnds; } } [FSA("ConnectEnds")] [SerializeField] private bool connectEnds;

		/// <summary>If you want to visualize the shape, you can specify an output LineRenderer here.</summary>
		public LineRenderer Visual { set { visual = value; } get { return visual; } } [FSA("Visual")] [SerializeField] private LineRenderer visual;

		/// <summary>The points that define the shape.</summary>
		public List<Vector2> Points { get { if (points == null) points = new List<Vector2>(); return points; } } [FSA("Points")] [SerializeField] private List<Vector2> points;

		public static int Mod(int a, int b)
		{
			var m = a % b;
			
			return m < 0 ? m + b : m;
		}

		public Vector2 GetPoint(int index, bool reverse)
		{
			if (points != null && points.Count > 0)
			{
				if (reverse == true)
				{
					index = points.Count - index - 1;
				}

				if (connectEnds == true)
				{
					index = Mod(index, points.Count);
				}
				else
				{
					index = Mathf.Clamp(index, 0, points.Count - 1);
				}

				return points[index];
			}

			return default(Vector2);
		}

		public void UpdateVisual()
		{
			if (visual != null)
			{
				if (points != null)
				{
					visual.positionCount = points.Count;

					for (var i = points.Count - 1; i >= 0; i--)
					{
						visual.SetPosition(i, points[i]);
					}

					if (connectEnds == true)
					{
						visual.positionCount += 1;

						visual.SetPosition(visual.positionCount - 1, points[0]);
					}
				}
				else
				{
					visual.positionCount = 0;
				}
			}
		}

#if UNITY_EDITOR
		protected virtual void Start()
		{
			UpdateVisual();
		}
#endif

#if UNITY_EDITOR
		protected virtual void OnValidate()
		{
			UpdateVisual();
		}
#endif

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			if (points != null && points.Count > 1)
			{
				Gizmos.matrix = transform.localToWorldMatrix;

				if (connectEnds == true)
				{
					for (var i = 0; i < points.Count; i++)
					{
						Gizmos.DrawLine(points[i], points[(i + 1) % points.Count]);
					}
				}
				else
				{
					for (var i = 1; i < points.Count; i++)
					{
						Gizmos.DrawLine(points[i - 1], points[i]);
					}
				}
			}
		}
#endif
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using UnityEditor;
	using TARGET = LeanShape;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanShape_Editor : LeanEditor
	{
		private bool drawing;

		private int dragging = -1;

		private static float radius = 5.0f;

		private List<Vector2> points = new List<Vector2>();

		private List<Vector2> scaledPoints = new List<Vector2>();

		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("connectEnds", "Should the start and end points of this shape be connected, forming a loop?");
			Draw("visual", "If you want to visualize the shape, you can specify an output LineRenderer here.");

			Separator();

			if (GUILayout.Button(drawing == true ? "Cancel Drawing" : "Draw") == true)
			{
				drawing = !drawing;

				points.Clear();
			}

			if (drawing == true)
			{
				var rect = EditorGUILayout.BeginVertical();
				{
					EditorGUILayout.LabelField(string.Empty, GUILayout.Height(200.0f));
				}
				EditorGUILayout.EndVertical();

				GUI.Box(rect, "");

				var e = Event.current;

				if (rect.Contains(e.mousePosition) == true)
				{
					var point = e.mousePosition;

					if (e.type == EventType.MouseDown)
					{
						dragging = TryGet(point);

						if (dragging == -1)
						{
							dragging = points.Count;

							points.Add(point);
						}

						Repaint();
					}
					else if (e.type == EventType.MouseMove || e.type == EventType.MouseDrag)
					{
						if (dragging >= 0)
						{
							points[dragging] = point;

							Repaint();
						}
					}
					else if (e.type == EventType.MouseUp)
					{
						dragging = -1;
					}
				}

				for (var i = 0; i < points.Count - 1; i++)
				{
					Line(points[i], points[i + 1]);
				}

				for (var i = 0; i < points.Count; i++)
				{
					var point = points[i];
					GUI.DrawTexture(new Rect(point.x - 7.0f, point.y - 7.0f, 14.0f, 14.0f), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, true, 0.0f, Color.white, 0.0f, 0.0f);
					GUI.Label(new Rect(point.x - 10.0f, point.y - 10.0f, 20.0f, 20.0f), i.ToString(), EditorStyles.centeredGreyMiniLabel);
				}

				radius = EditorGUILayout.FloatField("Radius", radius);

				if (GUILayout.Button("Use These " + points.Count + " points!") == true)
				{
					Undo.RecordObject(tgt, "Shape Points Changed");

					tgt.Points.Clear();

					tgt.Points.AddRange(ScalePoints());

					tgt.UpdateVisual();

					EditorUtility.SetDirty(tgt);
				}
			}

			Separator();

			Draw("points", "The points that define the shape.");
		}

		private List<Vector2> ScalePoints()
		{
			var min = points[0];
			var max = points[0];

			foreach (var point in points)
			{
				min = Vector2.Min(min, point);
				max = Vector2.Max(max, point);
			}

			scaledPoints.Clear();

			var size = Mathf.Max(max.x - min.x, max.y - min.y) * 0.5f;

			if (size > 0.0f)
			{
				var center = new Vector2((min.x + max.x) * 0.5f, (min.y + max.y) * 0.5f);

				for (var i = 0; i < points.Count; i++)
				{
					var point = points[i] - center;

					point /= size;
					point.y = -point.y;

					scaledPoints.Add(point * radius);
				}
			}

			return scaledPoints;
		}

		private static void Line(Vector2 a, Vector2 b, float thickness = 4.0f)
		{
			var matrix = GUI.matrix;
			var vector = b - a;
			var angle  = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

			GUIUtility.ScaleAroundPivot(new Vector2((b - a).magnitude, thickness), new Vector2(a.x, a.y + 0.5f));
			GUIUtility.RotateAroundPivot(angle, a);

			GUI.DrawTexture(new Rect(a.x, a.y, 1, 1), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, true, 0.0f, Color.black, 0.0f, 0.0f);

			GUI.matrix = matrix;
		}

		private int TryGet(Vector2 point, float threshold = 10.0f)
		{
			for (var i = 0; i < points.Count; i++)
			{
				if (Vector2.Distance(points[i], point) <= threshold)
				{
					return i;
				}
			}

			return -1;
		}
	}
}
#endif