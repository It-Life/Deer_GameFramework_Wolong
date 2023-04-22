using UnityEngine;
using UnityEngine.Events;
using Lean.Common;

namespace Lean.Touch
{
	/// <summary>This component can calculate the average position of all selected objects in the scene.
	/// NOTE: You must manually call the <b>Calculate</b> method from somewhere (e.g. inspector event), then it will be output from the <b>OnPosition</b> event.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanSelectableCenter")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selectable Center")]
	public class LeanSelectableCenter : MonoBehaviour
	{
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}

		/// <summary>This allows you to output Seconds to UI text.</summary>
		public Vector3Event OnPosition { get { if (onPosition == null) onPosition = new Vector3Event(); return onPosition; } } [SerializeField] private Vector3Event onPosition;

		/// <summary>This method will calculate the position and output it using the <b>OnPosition</b> event.</summary>
		[ContextMenu("Calculate")]
		public void Calculate()
		{
			var total = default(Vector3);
			var count = 0;

			foreach (var selectable in LeanSelectable.Instances)
			{
				if (selectable.IsSelected == true)
				{
					total += selectable.transform.position;
					count  += 1;
				}
			}

			if (count > 0)
			{
				var center = total / count;

				if (onPosition != null)
				{
					onPosition.Invoke(center);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanSelectableCenter;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanSelectableCenter_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("onPosition");
		}
	}
}
#endif