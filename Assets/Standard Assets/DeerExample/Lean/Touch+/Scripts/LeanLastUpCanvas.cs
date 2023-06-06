using UnityEngine;
using Lean.Common;

namespace Lean.Touch
{
	/// <summary>This component allows you to detect when the last finger stops touching the current UI element.
	/// NOTE: This requires you to enable the RaycastTarget setting on your UI graphic.
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanLastUpCanvas")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Last Up Canvas")]
	public class LeanLastUpCanvas : LeanLastUp
	{
		public bool ElementOverlapped(LeanFinger finger)
		{
			var results = LeanTouch.RaycastGui(finger.ScreenPosition, -1);

			if (results != null && results.Count > 0)
			{
				if (results[0].gameObject == gameObject)
				{
					return true;
				}
			}

			return false;
		}

		protected override void HandleFingerDown(LeanFinger finger)
		{
			if (ElementOverlapped(finger) == true)
			{
				base.HandleFingerDown(finger);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanLastUpCanvas;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanLastUpCanvas_Editor : LeanLastUp_Editor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			base.OnInspector();
		}
	}
}
#endif