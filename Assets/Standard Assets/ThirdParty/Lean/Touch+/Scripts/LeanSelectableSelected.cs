using UnityEngine;
using UnityEngine.Events;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component fires events when the selectable has been selected for a certain amount of time.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanSelectableSelected")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selectable Selected")]
	public class LeanSelectableSelected : LeanSelectableBehaviour
	{
		public enum ResetType
		{
			None,
			OnSelect,
			OnDeselect
		}

		// Event signature
		[System.Serializable] public class SelectableEvent : UnityEvent<LeanSelectable> {}

		/// <summary>The finger must be held for this many seconds.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [FSA("Threshold")] [SerializeField] private float threshold = 1.0f;

		/// <summary>When should Seconds be reset to 0?</summary>
		public ResetType Reset { set { reset = value; } get { return reset; } } [FSA("Reset")] [SerializeField] private ResetType reset = ResetType.OnDeselect;

		/// <summary>Bypass LeanSelectable.HideWithFinger?</summary>
		public bool RawSelection { set { rawSelection = value; } get { return rawSelection; } } [FSA("RawSelection")] [SerializeField] private bool rawSelection;

		/// <summary>If the selecting finger went up, cancel timer?</summary>
		public bool RequireFinger { set { requireFinger = value; } get { return requireFinger; } } [FSA("RequireFinger")] [SerializeField] private bool requireFinger;

		/// <summary>Called on the first frame the conditions are met.</summary>
		public SelectableEvent OnSelectableDown { get { if (onSelectableDown == null) onSelectableDown = new SelectableEvent(); return onSelectableDown; } } [FSA("onDown")] [FSA("OnDown")] [SerializeField] private SelectableEvent onSelectableDown;

		/// <summary>Called on every frame the conditions are met.</summary>
		public SelectableEvent OnSelectableUpdate { get { if (onSelectableUpdate == null) onSelectableUpdate = new SelectableEvent(); return onSelectableUpdate; } } [FSA("onSelectableSet")] [FSA("onSet")] [FSA("OnSet")] [SerializeField] private SelectableEvent onSelectableUpdate;

		/// <summary>Called on the last frame the conditions are met.</summary>
		public SelectableEvent OnSelectableUp { get { if (onSelectableUp == null) onSelectableUp = new SelectableEvent(); return onSelectableUp; } } [FSA("onUp")] [FSA("OnUp")] [SerializeField] private SelectableEvent onSelectableUp;

		[SerializeField]
		private bool lastSet;

		[SerializeField]
		private float seconds;

		protected virtual void Update()
		{
			// See if the timer can be incremented
			var set = false;

			if (Selectable != null && Selectable.IsSelected == true)
			{
				var selectableByFinger = Selectable as LeanSelectableByFinger;

				if (requireFinger == false || (selectableByFinger != null && selectableByFinger.SelectingFinger != null))
				{
					seconds += Time.deltaTime;

					if (seconds >= threshold)
					{
						set = true;
					}
				}
			}

			// If this is the first frame of set, call down
			if (set == true && lastSet == false)
			{
				if (onSelectableDown != null)
				{
					onSelectableDown.Invoke(Selectable);
				}
			}

			// Call set every time if set
			if (set == true)
			{
				if (onSelectableUpdate != null)
				{
					onSelectableUpdate.Invoke(Selectable);
				}
			}

			// Store last value
			lastSet = set;
		}

		protected override void OnSelected()
		{
			if (reset == ResetType.OnSelect)
			{
				seconds = 0.0f;
			}

			// Reset value
			lastSet = false;
		}

		protected override void OnDeselected()
		{
			if (reset == ResetType.OnDeselect)
			{
				seconds = 0.0f;
			}

			if (lastSet == true)
			{
				if (onSelectableUp != null)
				{
					onSelectableUp.Invoke(Selectable);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanSelectableSelected;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSelectableSelected_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("threshold", "The finger must be held for this many seconds.");
			Draw("reset", "When should Seconds be reset to 0?");
			Draw("rawSelection", "Bypass LeanSelectable.HideWithFinger?");
			Draw("requireFinger", "If the selecting finger went up, cancel timer?");

			Separator();

			var usedA = Any(tgts, t => t.OnSelectableDown.GetPersistentEventCount() > 0);
			var usedB = Any(tgts, t => t.OnSelectableUpdate.GetPersistentEventCount() > 0);
			var usedC = Any(tgts, t => t.OnSelectableUp.GetPersistentEventCount() > 0);

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onSelectableDown");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("onSelectableUpdate");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("onSelectableUp");
			}
		}
	}
}
#endif