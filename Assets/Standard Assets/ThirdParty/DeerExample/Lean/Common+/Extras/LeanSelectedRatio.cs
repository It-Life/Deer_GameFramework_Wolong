using UnityEngine;
using UnityEngine.Events;

namespace Lean.Common
{
	/// <summary>This component allows you to display a number showing the current ratio of selected objects, where a value of 0 means nothing has been selected, and a value of 1 means everything has been selected.
	/// This can be used with the UI Image Fill, as well as other components.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanSelectedRatio")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Selected Ratio")]
	public class LeanSelectedRatio : MonoBehaviour
	{
		[System.Serializable] public class FloatEvent : UnityEvent<float> {}

		/// <summary>Inverse the ratio value?</summary>
		public bool Inverse { set { inverse = value; } get { return inverse; } } [SerializeField] private bool inverse;

		/// <summary>The formatted string will be output using this event.</summary>
		public FloatEvent OnRatio { get { if (onRatio == null) onRatio = new FloatEvent(); return onRatio; } } [SerializeField] private FloatEvent onRatio;

		/// <summary>This method will immediately update the ratio.</summary>
		[ContextMenu("Update Now")]
		public void UpdateNow()
		{
			if (onRatio != null)
			{
				var ratio = LeanSelectable.IsSelectedCount / (float)LeanSelectable.Instances.Count;

				if (inverse == true)
				{
					ratio = 1.0f - ratio;
				}

				onRatio.Invoke(ratio);
			}
		}

		protected virtual void Update()
		{
			UpdateNow();
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanSelectedRatio;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSelectedRatio_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("inverse", "Inverse the ratio value?");

			Separator();

			Draw("onRatio");
		}
	}
}
#endif