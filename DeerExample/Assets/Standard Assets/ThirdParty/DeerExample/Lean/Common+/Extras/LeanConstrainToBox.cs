using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component will constrain the current <b>transform.position</b> to the specified box shape.
	/// NOTE: Unlike <b>LeanConstrainToCollider</b>, this component doesn't use the physics system, so it can avoid certain issues if your constrain shape moves.</summary>
	[DefaultExecutionOrder(200)]
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanConstrainToBox")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Constrain To Box")]
	public class LeanConstrainToBox : MonoBehaviour
	{
		/// <summary>The transform the constraint will be applied relative to.
		/// None/null = World space.</summary>
		public Transform RelativeTo { set { relativeTo = value; } get { return relativeTo; } } [FSA("RelativeTo")] [SerializeField] private Transform relativeTo;

		/// <summary>The size of the box relative to the current space.</summary>
		public Vector3 Size { set { size = value; } get { return size; } } [FSA("Size")] [SerializeField] private Vector3 size = Vector3.one;

		/// <summary>The center of the box relative to the current space.</summary>
		public Vector3 Center { set { center = value; } get { return center; } } [FSA("Center")] [SerializeField] private Vector3 center;

		protected virtual void LateUpdate()
		{
			var matrix      = relativeTo != null ? relativeTo.localToWorldMatrix : Matrix4x4.identity;
			var oldPosition = transform.position;
			var local       = matrix.MultiplyPoint(oldPosition);
			var min         = center - size * 0.5f;
			var max         = center + size * 0.5f;
			var set         = false;

			if (local.x < min.x) { local.x = min.x; set = true; }
			if (local.y < min.y) { local.y = min.y; set = true; }
			if (local.z < min.z) { local.z = min.z; set = true; }
			if (local.x > max.x) { local.x = max.x; set = true; }
			if (local.y > max.y) { local.y = max.y; set = true; }
			if (local.z > max.z) { local.z = max.z; set = true; }

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

		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.matrix = relativeTo != null ? relativeTo.localToWorldMatrix : Matrix4x4.identity;

			Gizmos.DrawWireCube(center, size);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanConstrainToBox;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanConstrainToBox_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("relativeTo", "The transform the constraint will be applied relative to.\n\nNone/null = World space.");
			Draw("size", "The size of the box relative to the current space.");
			Draw("center", "The center of the box relative to the current space.");
		}
	}
}
#endif