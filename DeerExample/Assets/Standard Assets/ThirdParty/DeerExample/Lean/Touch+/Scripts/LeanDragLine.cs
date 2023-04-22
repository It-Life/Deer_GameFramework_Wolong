using UnityEngine;
using UnityEngine.Events;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component modifies LeanDragTrail to draw straight lines.
	/// NOTE: This requires you to enable LeanTouch.RecordFingers.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanDragLine")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Line")]
	public class LeanDragLine : LeanDragTrail
	{
		[System.Serializable] public class Vector3Vector3Event : UnityEvent<Vector3, Vector3> {}
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}

		/// <summary>The StartWidth and EndWidth values will be increased by this value multiplied by the line length.
		/// 0 = No change.</summary>
		public float WidthScale { set { widthScale = value; } get { return widthScale; } } [FSA("WidthScale")] [SerializeField] private float widthScale = 0.0f;

		/// <summary>The minimum length of the straight line in world space.
		/// -1 = Unrestricted.</summary>
		public float LengthMin { set { lengthMin = value; } get { return lengthMin; } } [FSA("LengthMin")] [SerializeField] private float lengthMin = -1.0f;

		/// <summary>The maximum length of the straight line in world space.
		/// -1 = Unrestricted.</summary>
		public float LengthMax { set { lengthMax = value; } get { return lengthMax; } } [FSA("LengthMax")] [SerializeField] private float lengthMax = -1.0f;

		/// <summary>Enable this if the line should begin from this Transform's position.</summary>
		public bool StartAtOrigin { set { startAtOrigin = value; } get { return startAtOrigin; } } [FSA("StartAtOrigin")] [SerializeField] private bool startAtOrigin;

		/// <summary>Drag the line backwards?</summary>
		public bool Invert { set { invert = value; } get { return invert; } } [FSA("Invert")] [SerializeField] private bool invert;

		/// <summary>This event gets called when a trail drawing finger goes up.
		/// Vector3 = Start world point.</summary>
		public Vector3Event OnReleasedFrom { get { if (onReleasedFrom == null) onReleasedFrom = new Vector3Event(); return onReleasedFrom; } } [SerializeField] private Vector3Event onReleasedFrom;

		/// <summary>This event gets called when a trail drawing finger goes up. The parameter contains the end point in world space.
		/// Vector3 = End world point.</summary>
		public Vector3Event OnReleasedTo { get { if (onReleasedTo == null) onReleasedTo = new Vector3Event(); return onReleasedTo; } } [SerializeField] private Vector3Event onReleasedTo;

		/// <summary>This event gets called when a trail drawing finger goes up. The parameter contains the end point in world space.
		/// Vector3 = Vector between start and end world points.</summary>
		public Vector3Event OnReleasedDelta { get { if (onReleasedDelta == null) onReleasedDelta = new Vector3Event(); return onReleasedDelta; } } [SerializeField] private Vector3Event onReleasedDelta;

		/// <summary>This event gets called when a trail drawing finger goes up.
		/// Vector3 = Start point in world space.
		/// Vector3 = End point in world space.</summary>
		public Vector3Vector3Event OnReleasedFromTo { get { if (onReleasedFromTo == null) onReleasedFromTo = new Vector3Vector3Event(); return onReleasedFromTo; } } [SerializeField] private Vector3Vector3Event onReleasedFromTo;

		protected override void UpdateLine(FingerData fingerData, LeanFinger finger, LineRenderer line)
		{
			var color0 = startColor;
			var color1 = endColor;
			var width  = fingerData.Width;

			if (finger != null)
			{
				// Reserve points
				line.positionCount = 2;

				// Calculate preliminary points
				var point0 = ScreenDepth.Convert(finger.StartScreenPosition, gameObject);
				var point1 = ScreenDepth.Convert(finger.ScreenPosition, gameObject);

				if (startAtOrigin == true)
				{
					point0 = transform.position;
				}

				// Get length, and clamp?
				var length = Vector3.Distance(point0, point1);

				if (lengthMin >= 0.0f && length < lengthMin)
				{
					length = lengthMin;
				}

				if (lengthMax >= 0.0f && length > lengthMax)
				{
					length = lengthMax;
				}

				if (invert == true)
				{
					point1 = point0 - (point1 - point0);
				}

				// Write straight line
				line.SetPosition(0, point0);
				line.SetPosition(1, point0 + Vector3.Normalize(point1 - point0) * length);
			}
			else
			{
				fingerData.Age += Time.deltaTime;

				var alpha = Mathf.InverseLerp(fadeTime, 0.0f, fingerData.Age);

				color0.a *= alpha;
				color1.a *= alpha;
			}

			if (widthScale != 0.0f && line.positionCount == 2)
			{
				var point0 = line.GetPosition(0);
				var point1 = line.GetPosition(1);
				var length = Vector3.Distance(point0, point1);

				width += length * widthScale;
			}

			line.startColor      = color0;
			line.endColor        = color1;
			line.widthMultiplier = width;
		}

		protected override void HandleFingerUp(LeanFinger finger)
		{
			var fingerData = LeanFingerData.Find(fingerDatas, finger);

			if (fingerData != null)
			{
				var line = fingerData.Line;

				if (line != null && line.positionCount == 2)
				{
					var worldFrom = line.GetPosition(0);
					var worldTo   = line.GetPosition(1);

					if (onReleasedFrom != null)
					{
						onReleasedFrom.Invoke(worldFrom);
					}

					if (onReleasedTo != null)
					{
						onReleasedTo.Invoke(worldTo);
					}

					if (onReleasedDelta != null)
					{
						onReleasedDelta.Invoke(worldTo - worldFrom);
					}

					if (onReleasedFromTo != null)
					{
						onReleasedFromTo.Invoke(worldFrom, worldTo);
					}
				}

				fingerData.Finger = null; // The line will gradually fade out in Update
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanDragLine;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanDragLine_Editor : LeanDragTrail_Editor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			base.OnInspector();

			Separator();

			Draw("widthScale", "The LineRenderer.widthMultiplier values will be increased by this value multiplied by the line length.\n\n0 = No change.");
			Draw("lengthMin", "The minimum length of the straight line in world space.\n\n-1 = Unrestricted.");
			Draw("lengthMax", "The maximum length of the straight line in world space.\n\n-1 = Unrestricted.");
			Draw("startAtOrigin", "Enable this if the line should begin from this Transform's position.");
			Draw("invert", "Drag the line backwards?");

			Separator();

			var usedA = Any(tgts, t => t.OnReleasedFrom.GetPersistentEventCount() > 0);
			var usedB = Any(tgts, t => t.OnReleasedTo.GetPersistentEventCount() > 0);
			var usedC = Any(tgts, t => t.OnReleasedDelta.GetPersistentEventCount() > 0);
			var usedD = Any(tgts, t => t.OnReleasedFromTo.GetPersistentEventCount() > 0);

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onReleasedFrom");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("onReleasedTo");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("onReleasedDelta");
			}

			if (usedD == true || showUnusedEvents == true)
			{
				Draw("onReleasedFromTo");
			}
		}
	}
}
#endif