using UnityEditor;
using UnityEngine;
using static tackor.UIExtension.HorizontalSelector;

namespace tackor.UIExtension
{
    [CustomEditor(typeof(HorizontalSelector))]
    public class HorizontalSelectorEditor : Editor
	{
        private GUISkin customSkin;
        private HorizontalSelector hsTarget;
        private int currentTab;

        private void OnEnable()
        {
            hsTarget = (HorizontalSelector)target;
            customSkin = (GUISkin)Resources.Load("UIExtensionGUISkin");

            if (hsTarget.defaultIndex > hsTarget.items.Count - 1)
                hsTarget.defaultIndex = 0;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            GUIContent[] toolbarTabs = new GUIContent[3];
			toolbarTabs[0] = new GUIContent("Content");
			toolbarTabs[1] = new GUIContent("Resources");
			toolbarTabs[2] = new GUIContent("Settings");

            //Tabs----
            currentTab = EditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

			if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
				currentTab = 0;
			if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab Resources")))
				currentTab = 1;
			if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
				currentTab = 2;
			GUILayout.EndHorizontal();

            var items = serializedObject.FindProperty("items");
            var onValueChanged = serializedObject.FindProperty("onValueChanged");
			var animator = serializedObject.FindProperty("m_Animator");

			var label = serializedObject.FindProperty ("label");
            var labelHelper = serializedObject.FindProperty("labelHelper");
            var labelIcon = serializedObject.FindProperty("labelIcon");
			var labelIconHelper = serializedObject.FindProperty("labelIconHelper");

			var indicatorParent = serializedObject.FindProperty("indicatorParent");
			var indicatorObject = serializedObject.FindProperty("indicatorObject");

			var indicatorText = serializedObject.FindProperty("indicatorText");

			var enableIcon = serializedObject.FindProperty("enableIcon");
			var saveSelected = serializedObject.FindProperty("saveSelected");
			var saveKey = serializedObject.FindProperty("saveKey");

			var indicatorType = serializedObject.FindProperty("indicatorType");

			var invokeAtStart = serializedObject.FindProperty("invokeAtStart");
			var invertAnimation = serializedObject.FindProperty("invertAnimation");
			var loopSelection = serializedObject.FindProperty("loopSelection");
			var defaultIndex = serializedObject.FindProperty("defaultIndex");
			var iconScale = serializedObject.FindProperty("iconScale");
			var contentSpacing = serializedObject.FindProperty("contentSpacing");
			var contentLayout = serializedObject.FindProperty("contentLayout");
			var contentLayoutHelper = serializedObject.FindProperty("contentLayoutHelper");
			var enableUIManager = serializedObject.FindProperty("enableUIManager");


			switch (currentTab)
            {
				case 0:
					//Content Header ---------------------------
					EditorHandler.DrawHeader(customSkin, "Content Header", 6);

					//»¬¶¯ÌõÉèÖÃ defaultIndex
					if (hsTarget.items.Count > 0)
					{
						if (Application.isPlaying)
						{
							GUILayout.BeginVertical(EditorStyles.helpBox);
							GUILayout.BeginHorizontal();
							GUI.enabled = false;

							EditorGUILayout.LabelField(new GUIContent("Current Item:"), customSkin.FindStyle("Text"), GUILayout.Width(74));
							EditorGUILayout.LabelField(new GUIContent(hsTarget.items[hsTarget.index].itemTitle), customSkin.FindStyle("Text"));

							GUILayout.EndHorizontal();
							GUILayout.Space(2);

							EditorGUILayout.IntSlider(hsTarget.index, 0, hsTarget.items.Count - 1);

							GUI.enabled = true;
							GUILayout.EndVertical();
						}
						else
						{
							GUILayout.BeginVertical(EditorStyles.helpBox);
							GUILayout.BeginHorizontal();
							GUI.enabled = false;
							EditorGUILayout.LabelField(new GUIContent("Selected Item: "), customSkin.FindStyle("Text"), GUILayout.Width(78));
							GUI.enabled = true;
							EditorGUILayout.LabelField(new GUIContent(hsTarget.items[defaultIndex.intValue].itemTitle), customSkin.FindStyle("Text"));
							GUILayout.EndHorizontal();

							GUILayout.Space(2);

							defaultIndex.intValue = EditorGUILayout.IntSlider(defaultIndex.intValue, 0, hsTarget.items.Count - 1);
							GUILayout.EndVertical();
						}
					}
					else
					{
						EditorGUILayout.HelpBox("There is no item in the list.", MessageType.Warning);
					}

					//Selector Items
					GUILayout.BeginVertical();
					EditorGUI.indentLevel = 1;
					EditorGUILayout.PropertyField(items, new GUIContent("Selector Items"), true);
					EditorGUI.indentLevel = 1;
					GUILayout.EndVertical();

					//Events Header ---------------------------
					EditorHandler.DrawHeader(customSkin, "Events Header", 10);
					EditorGUILayout.PropertyField(onValueChanged, new GUIContent("On Value Changed"), true);
					break;
				case 1:
					//Core Header ---------------------------
					EditorHandler.DrawHeader(customSkin, "Core Header", 6);

					EditorHandler.DrawProperty(label, customSkin, "Label");
					EditorHandler.DrawProperty(labelHelper, customSkin, "Label Helper");
					EditorHandler.DrawProperty(labelIcon, customSkin, "Label Icon");
					EditorHandler.DrawProperty(labelIconHelper, customSkin, "Label Icon Helper");
					EditorHandler.DrawProperty(indicatorParent, customSkin, "Indicator Parent");
					EditorHandler.DrawProperty(indicatorObject, customSkin, "Indicator Object");
					EditorHandler.DrawProperty(indicatorText, customSkin, "Indicator Text");
					EditorHandler.DrawProperty(contentLayout, customSkin, "Content Layout");
					EditorHandler.DrawProperty(contentLayoutHelper, customSkin, "Content Layout Helper");
					EditorHandler.DrawProperty(animator, customSkin, "Animator");
					break;
				case 2:
					//Customization Header ---------------------------
					EditorHandler.DrawHeader(customSkin, "Customization Header", 6);

					//EnableIcon
					GUILayout.BeginVertical(EditorStyles.helpBox);
					GUILayout.Space(-3);
					enableIcon.boolValue = EditorHandler.DrawTogglePlain(enableIcon.boolValue, customSkin, "Enable Icon");
					GUILayout.Space(3);
					if (enableIcon.boolValue == true && hsTarget.labelIcon == null) { EditorGUILayout.HelpBox("'Enable Icon' is enabled but 'Label Icon' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error); }
					else if (enableIcon.boolValue == true && hsTarget.labelIcon != null) { hsTarget.labelIcon.gameObject.SetActive(true); }
					else if (enableIcon.boolValue == false && hsTarget.labelIcon != null) { hsTarget.labelIcon.gameObject.SetActive(false); }
					GUILayout.EndVertical();

					//IndicatorType
					GUILayout.BeginVertical(EditorStyles.helpBox);
					GUILayout.Space(-3);
					EditorHandler.DrawProperty(indicatorType, customSkin, "Indicator Type");
					GUILayout.Space(3);
					GUILayout.BeginHorizontal();
					switch ((IndicatorType)indicatorType.enumValueIndex)
					{
						case IndicatorType.Point:
							if (hsTarget.indicatorObject == null)
								EditorGUILayout.HelpBox("'Enable Indicators' is enabled but 'Indicator Object' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
							if (hsTarget.indicatorParent == null)
								EditorGUILayout.HelpBox("'Enable Indicators' is enabled but 'Indicator Parent' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
							else
								hsTarget.indicatorParent.gameObject.SetActive(true);
							
							if (hsTarget.indicatorText != null)
								hsTarget.indicatorText.gameObject.SetActive(false);
							break;
						case IndicatorType.Text:
							hsTarget.indicatorParent.gameObject.SetActive(false);
							if (hsTarget.indicatorText != null)
								hsTarget.indicatorText.gameObject.SetActive(true);
							break;
						case IndicatorType.None:
						default:
							hsTarget.indicatorParent.gameObject.SetActive(false);
							if (hsTarget.indicatorText != null)
								hsTarget.indicatorText.gameObject.SetActive(false);
							break;
					}
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();

					//IconScale
					EditorHandler.DrawProperty(iconScale, customSkin, "Icon Scale");
					//ContentSpacing
					EditorHandler.DrawProperty(contentSpacing, customSkin, "Content Spacing");
					hsTarget.UpdateContentLayout();

					//Options Header ---------------------------
					EditorHandler.DrawHeader(customSkin, "Options Header", 10);

					//Invoke At Start
					invokeAtStart.boolValue = EditorHandler.DrawToggle(invokeAtStart.boolValue, customSkin, "Invoke At Start");
					//Invert Animation
					invertAnimation.boolValue = EditorHandler.DrawToggle(invertAnimation.boolValue, customSkin, "Invert Animation");
					//Loop Selection
					loopSelection.boolValue = EditorHandler.DrawToggle(loopSelection.boolValue, customSkin, "Loop Selection");
					GUI.enabled = true;

					//Save Selected
					GUILayout.BeginVertical(EditorStyles.helpBox);
					GUILayout.Space(-3);
					saveSelected.boolValue = EditorHandler.DrawTogglePlain(saveSelected.boolValue, customSkin, "Save Selected");
					GUILayout.Space(3);
					if (saveSelected.boolValue)
					{
						EditorHandler.DrawPropertyCW(saveKey, customSkin, "Save Key:", 90);
						EditorGUILayout.HelpBox("Each selector should has its own unique save key.", MessageType.Info);
					}
					GUILayout.EndVertical();
					break;
				default:
                    break;
            }

			this.Repaint();
			serializedObject.ApplyModifiedProperties();
		}
    }
}