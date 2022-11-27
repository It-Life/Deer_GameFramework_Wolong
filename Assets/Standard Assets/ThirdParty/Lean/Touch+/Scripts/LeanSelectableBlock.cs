using UnityEngine;
using System.Collections.Generic;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component shows you a basic implementation of a block in a match-3 style game.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanSelectableBlock")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selectable Block")]
	public class LeanSelectableBlock : LeanSelectableByFingerBehaviour
	{
		// This stores a list of all blocks
		public static List<LeanSelectableBlock> Instances = new List<LeanSelectableBlock>();

		/// <summary>Current X grid coordinate of this block.</summary>
		public int X { set { x = value; } get { return x; } } [FSA("X")] [SerializeField] private int x;

		/// <summary>Current Y grid coordinate of this block.</summary>
		public int Y { set { y = value; } get { return y; } } [FSA("Y")] [SerializeField] private int y;

		/// <summary>The size of the block in world space.</summary>
		public float BlockSize { set { blockSize = value; } get { return blockSize; } } [FSA("BlockSize")] [SerializeField] private float blockSize = 2.5f;

		/// <summary>Auto deselect this block when swapping?</summary>
		public bool DeselectOnSwap { set { deselectOnSwap = value; } get { return deselectOnSwap; } } [FSA("DeselectOnSwap")] [SerializeField] private bool deselectOnSwap = true;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Damping { set { damping = value; } get { return damping; } } [FSA("Damping")] [FSA("Dampening")] [SerializeField] private float damping = 10.0f;

		public static LeanSelectableBlock FindBlock(int x, int y)
		{
			for (var i = Instances.Count - 1; i >= 0; i--)
			{
				var block = Instances[i];

				if (block.x == x && block.y == y)
				{
					return block;
				}
			}

			return null;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			Instances.Add(this);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			Instances.Remove(this);
		}

		public static void Swap(LeanSelectableBlock a, LeanSelectableBlock b)
		{
			var tempX = a.x;
			var tempY = a.y;

			a.x = b.x;
			a.y = b.y;

			b.x = tempX;
			b.y = tempY;
		}

		protected virtual void Update()
		{
			// Is this selected and has a selecting finger?
			if (Selectable != null && Selectable.IsSelected == true)
			{
				var finger = Selectable.SelectingFinger;

				if (finger != null)
				{
					// Camera exists?
					var camera = LeanHelper.GetCamera(null);

					if (camera != null)
					{
						// Find the screen point of the finger using the depth of this block
						var screenPoint = new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, camera.WorldToScreenPoint(transform.position).z);

						// Find the world space point under the finger
						var worldPoint = camera.ScreenToWorldPoint(screenPoint);

						// Find the block coordinate at this point
						var dragX = Mathf.RoundToInt(worldPoint.x / blockSize);
						var dragY = Mathf.RoundToInt(worldPoint.y / blockSize);

						// Is this block right next to this one?
						var distX = Mathf.Abs(x - dragX);
						var distY = Mathf.Abs(y - dragY);

						if (distX + distY == 1)
						{
							// Swap blocks if one exists at this coordinate
							var block = FindBlock(dragX, dragY);

							if (block != null)
							{
								Swap(this, block);

								if (deselectOnSwap == true)
								{
									Selectable.Deselect();
								}
							}
						}
					}
				}
			}

			// Smoothly move to new position
			var targetPosition = Vector3.zero;
			var factor         = LeanHelper.GetDampenFactor(damping, Time.deltaTime);

			targetPosition.x = x * blockSize;
			targetPosition.y = y * blockSize;

			transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, factor);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanSelectableBlock;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanSelectableBlock_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("x", "Current X grid coordinate of this block.");
			Draw("y", "Current Y grid coordinate of this block.");
			Draw("blockSize", "The size of the block in world space.");
			Draw("deselectOnSwap", "Auto deselect this block when swapping?");
			Draw("damping", "If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
		}
	}
}
#endif