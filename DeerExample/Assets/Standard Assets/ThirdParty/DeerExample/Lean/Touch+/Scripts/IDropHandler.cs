using UnityEngine;

namespace Lean.Touch
{
	/// <summary>Any component implementing this interface will make it compatible with LeanSelectableDrop, allowing you to perform a specific action when the current object is dropped on something.</summary>
	public interface IDropHandler
	{
		void HandleDrop(GameObject droppedGameObject, LeanFinger finger);
	}
}