using UnityEditor;

namespace Pathfinding.Legacy {
	[CustomEditor(typeof(LegacyRichAI))]
	[CanEditMultipleObjects]
	public class LegacyRichAIEditor : BaseAIEditor {
		protected override void Inspector () {
			base.Inspector();
			LegacyEditorHelper.UpgradeDialog(targets, typeof(RichAI));
		}
	}
}
