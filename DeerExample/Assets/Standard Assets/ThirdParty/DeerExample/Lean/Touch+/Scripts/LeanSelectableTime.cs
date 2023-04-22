using UnityEngine;
using UnityEngine.Events;
using Lean.Common;

namespace Lean.Touch
{
	/// <summary>This component counts how many seconds this <b>LeanSelectable</b> has been selected and sends it out via event.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanSelectableTime")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selectable Time")]
	public class LeanSelectableTime : LeanSelectableBehaviour
	{
		[System.Serializable] public class FloatEvent : UnityEvent<float> {}

		public enum SendType
		{
			WhileSelected,
			WhileSelectedAndWhenDeselected,
			Always
		}

		/// <summary>This allows you to control when the events will be invoked.
		/// WhileSelected = Every frame while this object is selected.
		/// WhileSelectedAndWhenDeselected = Every frame while this object is selected, and the first frame it gets deselected.
		/// Always = Every frame, regardless of the selection.</summary>
		public SendType Send { set { send = value; } get { return send; } } [SerializeField] private SendType send;

		/// <summary>Based on the <b>Send</b> setting, this event will be invoked.
		/// Float = Seconds selected.</summary>
		public FloatEvent OnSeconds { get { if (onSeconds == null) onSeconds = new FloatEvent(); return onSeconds;  } } [SerializeField] private FloatEvent onSeconds;

		[SerializeField]
		private float seconds;

		protected virtual void Update()
		{
			if (Selectable != null && Selectable.IsSelected == true)
			{
				seconds += Time.deltaTime;
			}
			else if (seconds > 0.0f)
			{
				seconds = 0.0f;

				if (send == SendType.WhileSelected)
				{
					return;
				}
			}
			else if (send != SendType.Always)
			{
				return;
			}

			if (onSeconds != null)
			{
				onSeconds.Invoke(seconds);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanSelectableTime;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanPlane_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("send", "This allows you to control when the events will be invoked.\n\nWhileSelected = Every frame while this object is selected.\n\nWhileSelectedAndWhenDeselected = Every frame while this object is selected, and the first frame it gets deselected.\n\nAlways = Every frame, regardless of the selection.");
			
			Separator();
			
			Draw("onSeconds");
		}
	}
}
#endif