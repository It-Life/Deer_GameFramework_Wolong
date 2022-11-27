using UnityEngine;
using UnityEngine.Events;
using Lean.Common;

namespace Lean.Touch
{
	/// <summary>This component can be used to pick objects in your scene that have the <b>LeanPickable</b> component attached.
	/// NOTE: This component requires you to call the <b>SelectScreenPosition</b> method externally (e.g. using the LeanFingerTap component).</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanPick")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Pick")]
	public class LeanPick : MonoBehaviour
	{
		[System.Serializable] public class LeanPickableEvent : UnityEvent<LeanPickable> {}

		public LeanScreenQuery ScreenQuery = new LeanScreenQuery(LeanScreenQuery.MethodType.Raycast);

		/// <summary>The tag required for an object to be selected.</summary>
		public string RequiredTag { set { requiredTag = value; } get { return requiredTag; } } [SerializeField] private string requiredTag;

		/// <summary>This event will be invoked when an object is picked.</summary>
		public LeanPickableEvent OnPickable { get { if (onPickable == null) onPickable = new LeanPickableEvent(); return onPickable; } } [SerializeField] private LeanPickableEvent onPickable;

		/// <summary>This method allows you to pick at the finger's <b>StartScreenPosition</b>.
		/// NOTE: This method be called from somewhere for this component to work (e.g. LeanFingerTap).</summary>
		public void PickStartScreenPosition(LeanFinger finger)
		{
			SelectScreenPosition(finger, finger.StartScreenPosition);
		}

		/// <summary>This method allows you to pick at the finger's current <b>ScreenPosition</b>.
		/// NOTE: This method be called from somewhere for this component to work (e.g. LeanFingerTap).</summary>
		public void PickScreenPosition(LeanFinger finger)
		{
			SelectScreenPosition(finger, finger.ScreenPosition);
		}

		/// <summary>This method allows you to initiate selection of a finger at a custom screen position.
		/// NOTE: This method be called from a custom script for this component to work.</summary>
		public void SelectScreenPosition(LeanFinger finger, Vector2 screenPosition)
		{
			var result = ScreenQuery.Query<LeanPickable>(gameObject, screenPosition);

			// Discard if tag doesn't match
			if (result != null && string.IsNullOrEmpty(RequiredTag) == false && result.tag != RequiredTag)
			{
				result = null;
			}

			if (result != null)
			{
				if (onPickable != null)
				{
					onPickable.Invoke(result);
				}

				result.InvokePick(finger, screenPosition);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanPick;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanPick_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			base.OnInspector();

			Draw("ScreenQuery");
			Draw("requiredTag", "The tag required for an object to be selected.");

			Separator();

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			if (showUnusedEvents == true || Any(tgts, t => t.OnPickable.GetPersistentEventCount() > 0))
			{
				Draw("onPickable");
			}
		}
	}
}
#endif