using UnityEngine;
using UnityEngine.Events;
using Lean.Common;

namespace Lean.Touch
{
	/// <summary>This component allows you to detect when a finger presses/taps on top of the current GameObject without using the LeanSelectable system.
	/// NOTE: This feature requires your scene to contain the <b>LeanPick</b> component.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanPickable")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Pickable")]
	public class LeanPickable : MonoBehaviour
	{
		[System.Serializable] public class LeanFingerEvent : UnityEvent<LeanFinger> {}
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}
		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}

		/// <summary>This event will be invoked when the specified finger touches this GameObject.</summary>
		public LeanFingerEvent OnFinger { get { if (onFinger == null) onFinger = new LeanFingerEvent(); return onFinger; } } [SerializeField] private LeanFingerEvent onFinger;

		/// <summary>The method used to find world coordinates from a finger. See LeanScreenDepth documentation for more information.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.DepthIntercept);

		/// <summary>This event will be invoked when this object is picked, and tell you the world space position it touched.</summary>
		public Vector3Event OnWorld { get { if (onWorld == null) onWorld = new Vector3Event(); return onWorld; } } [SerializeField] private Vector3Event onWorld;

		/// <summary>This event will be invoked when this object is picked, and tell you the screen space position it touched.</summary>
		public Vector2Event OnScreen { get { if (onScreen == null) onScreen = new Vector2Event(); return onScreen; } } [SerializeField] private Vector2Event onScreen;

		public void InvokePick(LeanFinger finger, Vector2 screenPosition)
		{
			if (onFinger != null)
			{
				onFinger.Invoke(finger);
			}

			var world = ScreenDepth.Convert(screenPosition, gameObject);

			if (onWorld != null)
			{
				onWorld.Invoke(world);
			}

			if (onScreen != null)
			{
				onScreen.Invoke(screenPosition);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanPickable;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSelectSelf_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			var usedA = Any(tgts, t => t.OnFinger.GetPersistentEventCount() > 0);
			var usedB = Any(tgts, t => t.OnWorld.GetPersistentEventCount() > 0);
			var usedC = Any(tgts, t => t.OnScreen.GetPersistentEventCount() > 0);

			if (showUnusedEvents == true || usedA == true)
			{
				Draw("onFinger");
			}

			if (showUnusedEvents == true || usedB == true)
			{
				Draw("ScreenDepth");
				Draw("onWorld");
			}

			if (showUnusedEvents == true || usedC == true)
			{
				Draw("onScreen");
			}
		}
	}
}
#endif