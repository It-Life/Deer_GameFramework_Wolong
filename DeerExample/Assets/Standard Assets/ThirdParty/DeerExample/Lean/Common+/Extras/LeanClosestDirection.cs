using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Lean.Common
{
	/// <summary>This component will compare the direction of the current Transform against a list of Transforms, and tell you which is closest.</summary>
	[DefaultExecutionOrder(200)]
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanClosestDirection")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Closest Direction")]
	public class LeanClosestDirection : MonoBehaviour
	{
		[System.Serializable] public class IntEvent : UnityEvent<int> {}

		/// <summary>This allows you to specify which local direction is considered forward on this GameObject.
		/// Leave this as the default (0,0,1) if you're not sure.</summary>
		public Vector3 Forward { set { forward = value; } get { return forward; } } [SerializeField] private Vector3 forward = Vector3.forward;

		/// <summary>This allows you to specify the different direction options we want to check between.</summary>
		public List<Transform> Targets { get { if (targets == null) targets = new List<Transform>(); return targets; } } [SerializeField] private List<Transform> targets;

		/// <summary>This event will be invoked when the <b>CurrentIndex</b> value changes.</summary>
		public IntEvent OnCurrentIndex { get { if (onCurrentIndex == null) onCurrentIndex = new IntEvent(); return onCurrentIndex; } } [SerializeField] private IntEvent onCurrentIndex;

		/// <summary>This stores the current closest <b>Targets</b> index.</summary>
		public int CurrentIndex
		{
			set
			{
				if (currentIndex != value)
				{
					currentIndex = value;

				}
			}

			get
			{
				return currentIndex;
			}
		}

		[SerializeField]
		private int currentIndex = -1;

		/// <summary>This method will update the <b>CurrentIndex</b> value now.</summary>
		[ContextMenu("Update Index")]
		public void UpdateIndex()
		{
			var bestIndex = -1;
			var bestAngle = float.PositiveInfinity;

			if (targets != null)
			{
				var positionA  = transform.position;
				var directionA = transform.TransformDirection(forward);

				for (var i = 0; i < targets.Count; i++)
				{
					var target = targets[i];

					if (target != null)
					{
						var directionB = target.position - positionA;
						var angle       = Vector3.Angle(directionA, directionB);

						if (angle < bestAngle)
						{
							bestIndex = i;
							bestAngle = angle;
						}
					}
				}
			}

			CurrentIndex = bestIndex;
		}

		protected virtual void LateUpdate()
		{
			UpdateIndex();
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			if (forward != Vector3.zero)
			{
				var fwd = transform.TransformPoint(forward);

				Gizmos.DrawLine(transform.position, fwd);
			}

			if (targets != null)
			{
				Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

				foreach (var target in targets)
				{
					Gizmos.DrawLine(transform.position, target.position);
				}
			}
		}
#endif
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanClosestDirection;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanClosestDirection_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("forward", "This allows you to specify which local direction is considered forward on this GameObject.\n\nLeave this as the default (0,0,1) if you're not sure.");
			Draw("targets", "This allows you to specify the different direction options we want to check between.");

			Separator();

			Draw("onCurrentIndex");
		}
	}
}
#endif