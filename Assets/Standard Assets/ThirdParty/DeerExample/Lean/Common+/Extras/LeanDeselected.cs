using UnityEngine;
using UnityEngine.Events;

namespace Lean.Common
{
	/// <summary>This component allows you to detect when any selectable object in the scene has been deselected.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanDeselected")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Deselected")]
	public class LeanDeselected : MonoBehaviour
	{
		[System.Serializable] public class LeanSelectableEvent : UnityEvent<LeanSelectable> {}

		public LeanSelectableEvent OnSelectable { get { if (onSelectable == null) onSelectable = new LeanSelectableEvent(); return onSelectable; } } [SerializeField] private LeanSelectableEvent onSelectable;

		protected virtual void OnEnable()
		{
			LeanSelect.OnAnyDeselected += HandleAnyDeselected;
		}
		protected virtual void OnDisable()
		{
			LeanSelect.OnAnyDeselected -= HandleAnyDeselected;
		}

		private void HandleAnyDeselected(LeanSelect select, LeanSelectable selectable)
		{
			if (onSelectable != null)
			{
				onSelectable.Invoke(selectable);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanDeselected;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanDeselected_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("onSelectable");
		}
	}
}
#endif