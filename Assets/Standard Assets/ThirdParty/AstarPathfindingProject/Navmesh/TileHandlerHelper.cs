using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding {
	/// <summary>
	/// Helper for navmesh cut objects.
	///
	/// Deprecated: Use <see cref="AstarPath.navmeshUpdates"/> instead
	/// </summary>
	[System.Obsolete("Use AstarPath.navmeshUpdates instead. You can safely remove this component.")]
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_tile_handler_helper.php")]
	public class TileHandlerHelper : VersionedMonoBehaviour {
		/// <summary>How often to check if an update needs to be done (real seconds between checks).</summary>
		public float updateInterval {
			get { return AstarPath.active.navmeshUpdates.updateInterval; }
			set { AstarPath.active.navmeshUpdates.updateInterval = value; }
		}

		/// <summary>Use the specified handler, will create one at start if not called</summary>
		[System.Obsolete("All navmesh/recast graphs now use navmesh cutting")]
		public void UseSpecifiedHandler (TileHandler newHandler) {
			throw new System.Exception("All navmesh/recast graphs now use navmesh cutting");
		}

		/// <summary>Discards all pending updates caused by moved or modified navmesh cuts</summary>
		public void DiscardPending () {
			AstarPath.active.navmeshUpdates.DiscardPending();
		}

		/// <summary>Checks all NavmeshCut instances and updates graphs if needed.</summary>
		public void ForceUpdate () {
			AstarPath.active.navmeshUpdates.ForceUpdate();
		}
	}
}
