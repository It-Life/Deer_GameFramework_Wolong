using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component causes the current Transform to follow the specified trail of positions.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanFollow")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Follow")]
	public class LeanFollow : MonoBehaviour
	{
		/// <summary>When this object is within this many world space units of the next point, it will be removed.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [FSA("Threshold")] [SerializeField] private float threshold = 0.001f;

		/// <summary>The speed of the following in units per seconds.</summary>
		public float Speed { set { speed = value; } get { return speed; } } [FSA("Speed")] [SerializeField] private float speed = 1.0f;

		public UnityEvent OnReachedDestination { get { if (onReachedDestination == null) onReachedDestination = new UnityEvent(); return onReachedDestination; } } [SerializeField] private UnityEvent onReachedDestination;

		[SerializeField]
		private List<Vector3> positions;

		/// <summary>This method will remove all follow positions, and stop movement.</summary>
		[ContextMenu("Clear Positions")]
		public void ClearPositions()
		{
			if (positions != null)
			{
				positions.Clear();
			}
		}

		public void SnapToNextPosition()
		{
			if (positions != null && positions.Count > 0)
			{
				transform.position = positions[0];
			}
		}

		/// <summary>This method adds a new position to the follow path.</summary>
		public void AddPosition(Vector3 newPosition)
		{
			if (positions == null)
			{
				positions = new List<Vector3>();
			}

			// Only add newPosition if it's far enough away from the last added point
			if (positions.Count == 0 || Vector3.Distance(positions[positions.Count - 1], newPosition) > threshold)
			{
				positions.Add(newPosition);
			}
		}

		protected virtual void Update()
		{
			if (positions != null)
			{
				var previousCount = positions.Count;

				TrimPositions();

				if (positions.Count > 0)
				{
					var currentPosition = transform.position;
					var targetPosition  = positions[0];

					currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);

					transform.position = currentPosition;
				}
				else if (previousCount > 0)
				{
					if (onReachedDestination != null) onReachedDestination.Invoke();
				}
			}
		}

		protected void TrimPositions()
		{
			var currentPosition = transform.position;

			while (positions.Count > 0)
			{
				var distance = Vector3.Distance(currentPosition, positions[0]);

				if (distance > threshold)
				{
					break;
				}

				positions.RemoveAt(0);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanFollow;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanFollow_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("threshold", "When this object is within this many world space units of the next point, it will be removed.");
			Draw("speed", "The speed of the following in units per seconds.");

			Separator();

			Draw("onReachedDestination");
		}
	}
}
#endif