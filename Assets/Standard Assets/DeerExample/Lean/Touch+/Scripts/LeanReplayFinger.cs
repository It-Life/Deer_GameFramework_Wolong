using UnityEngine;
using System.Collections.Generic;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This shows you how to record a finger's movement data that can later be replayed.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanReplayFinger")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Replay Finger")]
	public class LeanReplayFinger : MonoBehaviour
	{
		/// <summary>The cursor used to show the recording.</summary>
		public Transform Cursor { set { cursor = value; } get { return cursor; } } [FSA("Cursor")] [SerializeField] private Transform cursor;

		/// <summary>The conversion method used to find a world point from a screen point.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.FixedDistance, Physics.DefaultRaycastLayers, 10.0f);

		/// <summary>Is the recording playing?</summary>
		public bool Playing { set { playing = value; } get { return playing; } } [FSA("Playing")] [SerializeField] private bool playing;

		/// <summary>The position of the playback in seconds.</summary>
		public float PlayTime { set { playTime = value; } get { return playTime; } } [FSA("PlayTime")] [SerializeField] private float playTime;

		// Currently recorded snapshots
		private List<LeanSnapshot> snapshots = new List<LeanSnapshot>();

		public void Replay()
		{
			playing  = true;
			playTime = 0.0f;
		}

		public void StopReplay()
		{
			playing = false;
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerUpdate += HandleFingerUpdate;
			LeanTouch.OnFingerUp     += HandleFingerUp;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
			LeanTouch.OnFingerUp     -= HandleFingerUp;
		}

		protected virtual void Update()
		{
			// Is the recording being played back?
			if (playing == true)
			{
				playTime += Time.deltaTime;

				var screenPosition = default(Vector2);

				if (LeanSnapshot.TryGetScreenPosition(snapshots, playTime, ref screenPosition) == true)
				{
					cursor.position = ScreenDepth.Convert(screenPosition, gameObject);
				}
			}
		}

		private void HandleFingerUpdate(LeanFinger finger)
		{
			if (finger.StartedOverGui == false && finger.Index != LeanTouch.HOVER_FINGER_INDEX)
			{
				playing = false;

				if (cursor != null)
				{
					cursor.position = ScreenDepth.Convert(finger.ScreenPosition, gameObject);
				}
			}
		}

		private void HandleFingerUp(LeanFinger finger)
		{
			if (finger.StartedOverGui == false)
			{
				CopySnapshots(finger);
			}
		}

		private void CopySnapshots(LeanFinger finger)
		{
			// Clear old snapshots
			snapshots.Clear();

			// Go through all new snapshots
			for (var i = 0; i < finger.Snapshots.Count; i++)
			{
				// Copy data into new snapshot
				var snapshotSrc = finger.Snapshots[i];
				var snapshotCpy = new LeanSnapshot();

				snapshotCpy.Age            = snapshotSrc.Age;
				snapshotCpy.ScreenPosition = snapshotSrc.ScreenPosition;

				// Add new snapshot to list
				snapshots.Add(snapshotCpy);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanReplayFinger;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanReplayFinger_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("cursor", "The cursor used to show the recording.");
			Draw("ScreenDepth");
			Draw("playing", "Is the recording playing?");
			Draw("playTime", "The position of the playback in seconds.");
		}
	}
}
#endif