using UnityEngine;
using UnityEngine.Events;
using Lean.Common;

namespace Lean.Touch
{
	/// <summary>This component implements the 'IDroppable' interface, and will count the amount of times you drop a GameObject with the LeanSelectableDrop component on it.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanDropCount")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drop Count")]
	public class LeanDropCount : MonoBehaviour, IDropHandler
	{
		[System.Serializable] public class IntEvent : UnityEvent<int> {}

		/// <summary>The amount of times you've dropped an object on this.</summary>
		public int Count { set { if (count != value) { count = value; UpdateState(true); } } get { return count; } } [SerializeField] private int count;

		/// <summary>When the amount of selected objects changes, this event is invoked with the current count.</summary>
		public IntEvent OnCount { get { if (onCount == null) onCount = new IntEvent(); return onCount; } } [SerializeField] private IntEvent onCount;

		/// <summary>The minimum amount of objects that must be selected for a match.
		/// -1 = Max.</summary>
		public int MatchMin { set { matchMin = value; } get { return matchMin; } } [SerializeField] private int matchMin = -1;

		/// <summary>The maximum amount of objects that can be selected for a match.
		/// -1 = Max.</summary>
		public int MatchMax { set { matchMax = value; } get { return matchMax; } } [SerializeField] private int matchMax = -1;

		/// <summary>When the amount of selected objects matches the <b>RequiredCount</b>, this event will be invoked.</summary>
		public UnityEvent OnMatch { get { if (onMatch == null) onMatch = new UnityEvent(); return onMatch; } } [SerializeField] private UnityEvent onMatch;

		/// <summary>When the amount of selected objects no longer matches the <b>RequiredCount</b>, this event will be invoked.</summary>
		public UnityEvent OnUnmatch { get { if (onUnmatch == null) onUnmatch = new UnityEvent(); return onUnmatch; } } [SerializeField] private UnityEvent onUnmatch;

		[SerializeField]
		private bool inside;

		// Implemented from the IDropHandler interface
		public void HandleDrop(GameObject droppedGameObject, LeanFinger finger)
		{
			Count += 1;
		}

		private void UpdateState(bool changed)
		{
			if (changed == true)
			{
				if (onCount != null)
				{
					onCount.Invoke(count);
				}
			}

			var min       = matchMin >= 0 ? matchMin : count;
			var max       = matchMax >= 0 ? matchMax : count;
			var raw       = count;
			var newInside = raw >= min && raw <= max;

			if (newInside != inside)
			{
				inside = newInside;

				if (inside == true)
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
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanDropCount;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanDropCount_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			if (Draw("count", "The amount of times you've dropped an object on this.") == true)
			{
				Each(tgts, t => t.Count = serializedObject.FindProperty("count").intValue);
			}

			Separator();

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