using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component will constrain the current <b>transform.position</b> to the specified axis shape.
	/// NOTE: Unlike <b>LeanConstrainToCollider</b>, this component doesn't use the physics system, so it can avoid certain issues if your constrain shape moves.</summary>
	[DefaultExecutionOrder(200)]
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanConstrainToAxis")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Constrain To Axis")]
	public class LeanConstrainToAxis : MonoBehaviour
	{
		public enum AxisType
		{
			X,
			Y,
			Z,
			MinX,
			MaxX,
			MinY,
			MaxY,
			MinZ,
			MaxZ,
		}

		/// <summary>The transform the constraint will be applied relative to.
		/// None/null = World space.</summary>
		public Transform RelativeTo { set { relativeTo = value; } get { return relativeTo; } } [FSA("RelativeTo")] [SerializeField] private Transform relativeTo;

		/// <summary>The axis that will be constrained.</summary>
		public AxisType Axis { set { axis = value; } get { return axis; } } [FSA("Axis")] [SerializeField] private AxisType axis;

		/// <summary>The minimum position value relative to the current space.</summary>
		public float Minimum { set { minimum = value; } get { return minimum; } } [FSA("Minimum")] [SerializeField] private float minimum = -1.0f;

		/// <summary>The maximum position value relative to the current space.</summary>
		public float Maximum { set { maximum = value; } get { return maximum; } } [FSA("Maximum")] [SerializeField] private float maximum = 1.0f;

		protected virtual void LateUpdate()
		{
			var matrix      = relativeTo != null ? relativeTo.localToWorldMatrix : Matrix4x4.identity;
			var oldPosition = transform.position;
			var local       = matrix.MultiplyPoint(oldPosition);
			var set         = false;

			if ((axis == AxisType.X || axis == AxisType.MinX) && local.x < minimum) { local.x = minimum; set = true; }
			if ((axis == AxisType.X || axis == AxisType.MaxX) && local.x > maximum) { local.x = maximum; set = true; }
			if ((axis == AxisType.Y || axis == AxisType.MinY) && local.y < minimum) { local.y = minimum; set = true; }
			if ((axis == AxisType.Y || axis == AxisType.MaxY) && local.y > maximum) { local.y = maximum; set = true; }
			if ((axis == AxisType.Z || axis == AxisType.MinZ) && local.z < minimum) { local.z = minimum; set = true; }
			if ((axis == AxisType.Z || axis == AxisType.MaxZ) && local.z > maximum) { local.z = maximum; set = true; }

			if (set == true)
			{
				var newPosition = matrix.inverse.MultiplyPoint(local);

				if (Mathf.Approximately(oldPosition.x, newPosition.x) == false ||
					Mathf.Approximately(oldPosition.y, newPosition.y) == false ||
					Mathf.Approximately(oldPosition.z, newPosition.z) == false)
				{
					transform.position = newPosition;
				}
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.matrix = relativeTo != null ? relativeTo.localToWorldMatrix : Matrix4x4.identity;

			if (axis == AxisType.X || axis == AxisType.MinX) DrawLine(new Vector3(minimum, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f));
			if (axis == AxisType.X || axis == AxisType.MaxX) DrawLine(new Vector3(maximum, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f));
			if (axis == AxisType.Y || axis == AxisType.MinY) DrawLine(new Vector3(0.0f, minimum, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
			if (axis == AxisType.Y || axis == AxisType.MaxY) DrawLine(new Vector3(0.0f, maximum, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
			if (axis == AxisType.Z || axis == AxisType.MinZ) DrawLine(new Vector3(0.0f, 0.0f, minimum), new Vector3(0.0f, 0.0f, 1.0f));
			if (axis == AxisType.Z || axis == AxisType.MaxZ) DrawLine(new Vector3(0.0f, 0.0f, maximum), new Vector3(0.0f, 0.0f, 1.0f));
		}

		private void DrawLine(Vector3 center, Vector3 axis)
		{
			var x = new Vector3(1.0f - axis.x, 0.0f, 0.0f) * 1000.0f;
			var y = new Vector3(0.0f, 1.0f - axis.y, 0.0f) * 1000.0f;
			var z = new Vector3(0.0f, 0.0f, 1.0f - axis.z) * 1000.0f;

			Gizmos.DrawLine(center - x, center + x);
			Gizmos.DrawLine(center - y, center + y);
			Gizmos.DrawLine(center - z, center + z);
		}
#endif
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanConstrainToAxis;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanConstrainToAxis_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("relativeTo", "The transform the constraint will be applied relative to.\n\nNone/null = World space.");
			Draw("axis", "The axis that will be constrained.");

			if (Any(tgts, t =>
				t.Axis == LeanConstrainToAxis.AxisType.X ||
				t.Axis == LeanConstrainToAxis.AxisType.Y ||
				t.Axis == LeanConstrainToAxis.AxisType.Z ||
				t.Axis == LeanConstrainToAxis.AxisType.MinX ||
				t.Axis == LeanConstrainToAxis.AxisType.MinY ||
				t.Axis == LeanConstrainToAxis.AxisType.MinZ))
			{
				BeginIndent();
					Draw("minimum", "The minimum position value relative to the current space.");
				EndIndent();
			}

			if (Any(tgts, t =>
				t.Axis == LeanConstrainToAxis.AxisType.X ||
				t.Axis == LeanConstrainToAxis.AxisType.Y ||
				t.Axis == LeanConstrainToAxis.AxisType.Z ||
				t.Axis == LeanConstrainToAxis.AxisType.MaxX ||
				t.Axis == LeanConstrainToAxis.AxisType.MaxY ||
				t.Axis == LeanConstrainToAxis.AxisType.MaxZ))
			{
				BeginIndent();
					Draw("maximum", "The maximum position value relative to the current space.");
				EndIndent();
			}
		}
	}
}
#endif