using UnityEngine;
using UnityEngine.Events;

namespace Lean.Common
{
	/// <summary>This component allows you to display text showing the currently selected object count.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanSelectedText")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Selected Text")]
	public class LeanSelectedString : MonoBehaviour
	{
		[System.Serializable] public class StringEvent : UnityEvent<string> {}

		/// <summary>The format of the string.
		/// {0} = The amount of objects that can be selected.
		/// {1} = The amount of selected objects.
		/// {2} = The remaining objects to be selected.
		/// {3} = The percentage of selected objects.
		/// {4} = The percentage of objects remaining to be selected.</summary>
		public string Format { set { format = value; } get { return format; } } [SerializeField] private string format = "You have selected {1} out of {0} objects!";

		/// <summary>The formatted string will be output using this event.</summary>
		public StringEvent OnString { get { if (onString == null) onString = new StringEvent(); return onString; } } [SerializeField] private StringEvent onString;

		/// <summary>This method will immediately update the string and output it.</summary>
		[ContextMenu("Update Now")]
		public void UpdateNow()
		{
			if (onString != null)
			{
				var dataA = LeanSelectable.Instances.Count;
				var dataB = LeanSelectable.IsSelectedCount;
				var dataC = dataA - dataB;
				var dataD = (dataB / (float)dataA) * 100;
				var dataE = (dataC / (float)dataA) * 100;

				onString.Invoke(string.Format(format, dataA, dataB, dataC, dataD, dataE));
			}
		}

		protected virtual void OnEnable()
		{
			LeanSelectable.OnAnyEnabled     += HandleA;
			LeanSelectable.OnAnyDisabled    += HandleA;
			LeanSelectable.OnAnySelected    += HandleB;
			LeanSelectable.OnAnyDeselected  += HandleB;

			UpdateNow();
		}

		protected virtual void OnDisable()
		{
			LeanSelectable.OnAnyEnabled     -= HandleA;
			LeanSelectable.OnAnyDisabled    -= HandleA;
			LeanSelectable.OnAnySelected    -= HandleB;
			LeanSelectable.OnAnyDeselected  -= HandleB;
		}

		private void HandleA(LeanSelectable selectable)
		{
			UpdateNow();
		}

		private void HandleB(LeanSelect select, LeanSelectable selectable)
		{
			UpdateNow();
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanSelectedString;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSelectedString_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("format", "The format of the string.\n\n{0} = The amount of objects that can be selected.\n\n{1} = The amount of selected objects.\n\n{2} = The remaining objects to be selected.\n\n{3} = The percentage of selected objects.\n\n{4} = The percentage of objects remaining to be selected.");

			Separator();

			Draw("onString");
		}
	}
}
#endif