using UnityEngine;
using UnityEngine.Events;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component allows you to detect when a specific amount of selectable objects in the scene have been selected.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanSelectedCount")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Selected Count")]
	public class LeanSelectedCount : MonoBehaviour
	{
		[System.Serializable] public class IntEvent : UnityEvent<int> {}

		/// <summary>When the amount of selected objects changes, this event is invoked with the current count.</summary>
		public IntEvent OnCount { get { if (onCount == null) onCount = new IntEvent(); return onCount; } } [SerializeField] private IntEvent onCount;

		/// <summary>The minimum amount of objects that must be selected for a match.
		/// -1 = Max.</summary>
		public int MatchMin { set { matchMin = value; } get { return matchMin; } } [FSA("MatchMin")] [SerializeField] private int matchMin = -1;

		/// <summary>The maximum amount of objects that can be selected for a match.
		/// -1 = Max.</summary>
		public int MatchMax { set { matchMax = value; } get { return matchMax; } } [FSA("MatchMax")] [SerializeField] private int matchMax = -1;

		/// <summary>When the amount of selected objects matches the <b>RequiredCount</b>, this event will be invoked.</summary>
		public UnityEvent OnMatch { get { if (onMatch == null) onMatch = new UnityEvent(); return onMatch; } } [SerializeField] private UnityEvent onMatch;

		/// <summary>When the amount of selected objects no longer matches the <b>RequiredCount</b>, this event will be invoked.</summary>
		public UnityEvent OnUnmatch { get { if (onUnmatch == null) onUnmatch = new UnityEvent(); return onUnmatch; } } [SerializeField] private UnityEvent onUnmatch;

		public bool Matched { set { SetMatched(value); } get { return matched; } } [FSA("inside")] [SerializeField] private bool matched;

		protected virtual void OnEnable()
		{
			LeanSelectable.OnAnySelected   += HandleAnyA;
			LeanSelectable.OnAnyDeselected += HandleAnyA;
			LeanSelectable.OnAnyEnabled    += HandleAnyB;
			LeanSelectable.OnAnyDisabled   += HandleAnyB;

			UpdateState();
		}
		protected virtual void OnDisable()
		{
			LeanSelectable.OnAnySelected   -= HandleAnyA;
			LeanSelectable.OnAnySelected   -= HandleAnyA;
			LeanSelectable.OnAnyEnabled    -= HandleAnyB;
			LeanSelectable.OnAnyDisabled   -= HandleAnyB;
		}

		private void HandleAnyA(LeanSelect select, LeanSelectable selectable)
		{
			UpdateState();
		}

		private void HandleAnyB(LeanSelectable selectable)
		{
			UpdateState();
		}

		private void SetMatched(bool value)
		{
			if (matched != value)
			{
				matched = value;

				if (matched == true)
				{
					if (onMatch != null)
					{
						onMatch.Invoke();
					}
				}
				else
				{
					if (onUnmatch != null)
					{
						onUnmatch.Invoke();
					}
				}
			}
		}

		private void UpdateState()
		{
			var min = matchMin >= 0 ? matchMin : LeanSelectable.Instances.Count;
			var max = matchMax >= 0 ? matchMax : LeanSelectable.Instances.Count;
			var raw = LeanSelectable.IsSelectedCount;

			SetMatched(raw >= min && raw <= max);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanSelectedCount;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSelectedCount_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			var usedA = Any(tgts, t => t.OnCount.GetPersistentEventCount() > 0);
			var usedB = Any(tgts, t => t.OnMatch.GetPersistentEventCount() > 0);
			var usedC = Any(tgts, t => t.OnUnmatch.GetPersistentEventCount() > 0);

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onCount");
			}

			if (usedB == true || usedC == true || showUnusedEvents == true)
			{
				if (Draw("matched") == true)
				{
					Each(tgts, t => t.Matched = serializedObject.FindProperty("matched").boolValue, true);
				}
				Draw("matchMin", "The minimum amount of objects that must be selected for a match.\n\n-1 = Max.");
				Draw("matchMax", "The maximum amount of objects that can be selected for a match.\n\n-1 = Max.");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("onMatch");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("onUnmatch");
			}
		}
	}
}
#endif