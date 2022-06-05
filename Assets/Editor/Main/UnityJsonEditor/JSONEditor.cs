using System.IO;
using System.Linq;
using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEditor;
using System;


// Adds a nice editor to edit JSON files as well as a simple text editor incase
// the editor doesn't support the types you need. It works with strings, floats
// ints and bools at the moment.
// 
// * Requires the latest version of JSON.net compatible with Unity


//If you want to edit a JSON file in the "StreammingAssets" Folder change this to DefaultAsset.
//Hacky solution to a weird problem :/
[CustomEditor(typeof(TextAsset), true)]
public class JSONEditor : Editor
{
    private string Path => AssetDatabase.GetAssetPath(target);
    private bool isCompatible => Path.EndsWith(".json");
    private bool unableToParse => !NewtonsoftExtensions.IsValidJson(rawText);

    private bool isTextMode, wasTextMode;

    private string rawText;
    private JObject jsonObject;
    private JProperty propertyToRename;
    private string propertyRename;


    private void OnEnable()
    {
        if (isCompatible)
            LoadFromJson();
    }

    private void OnDisable()
    {
        if (isCompatible)
            WriteToJson();
    }

    public override void OnInspectorGUI()
    {
        if (isCompatible)
        {
            JsonInspectorGUI();
            return;
        }
        base.OnInspectorGUI();
    }

    private void JsonInspectorGUI()
    {
        bool enabled = GUI.enabled;
        GUI.enabled = true;

        Rect subHeaderRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 2.5f);
        Rect helpBoxRect = new Rect(subHeaderRect.x, subHeaderRect.y, subHeaderRect.width - subHeaderRect.width / 6 - 5f, subHeaderRect.height);
        Rect rawTextModeButtonRect = new Rect(subHeaderRect.x + subHeaderRect.width / 6 * 5, subHeaderRect.y, subHeaderRect.width / 6, subHeaderRect.height);
        EditorGUI.HelpBox(helpBoxRect, "You edit raw text if the JSON editor isn't enough by clicking the button to the right", MessageType.Info);
        

        GUIStyle wrappedButton = new GUIStyle("Button");
        wrappedButton.wordWrap = true;
        EditorGUI.BeginChangeCheck();
        GUI.enabled = !unableToParse;
        isTextMode = GUI.Toggle(rawTextModeButtonRect, isTextMode, "Edit Text", wrappedButton);
        if(EditorGUI.EndChangeCheck())
        {
            WriteToJson();
            GUI.FocusControl("");
            LoadFromJson();
        }
        GUI.enabled = true;

        if (!isTextMode)
        {
            if (jsonObject != null)
            {
                Rect initialRect = new Rect(10, 5 + EditorGUIUtility.singleLineHeight * 6, EditorGUIUtility.currentViewWidth - 20, EditorGUIUtility.singleLineHeight);

                int fieldsHeight = RecursiveDrawField(new Rect(initialRect.x + 5, initialRect.y, initialRect.width, initialRect.height), jsonObject);
                fieldsHeight += 3;
                Rect addNewButton = new Rect(initialRect.x, initialRect.y + fieldsHeight * initialRect.height, initialRect.width, initialRect.height * 2);
                if(GUI.Button(addNewButton, "Add New Property"))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Empty Object"), false, () =>
                    {
                        AddNewProperty<JObject>(jsonObject);
                    });
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("String"), false, () =>
                    {
                        AddNewProperty<string>(jsonObject);
                    });
                    menu.AddItem(new GUIContent("Single"), false, () =>
                    {
                        AddNewProperty<float>(jsonObject);
                    });
                    menu.AddItem(new GUIContent("Integer"), false, () =>
                    {
                        AddNewProperty<int>(jsonObject);
                    });
                    menu.AddItem(new GUIContent("Boolean"), false, () =>
                    {
                        AddNewProperty<bool>(jsonObject);

                    });
                    menu.DropDown(addNewButton);
                }
            }
        }
        else
        {
            float textFieldHeight = GUI.skin.label.CalcSize(new GUIContent(rawText)).y;
            Rect textFieldRect = new Rect(subHeaderRect.x, subHeaderRect.y + subHeaderRect.height + EditorGUIUtility.singleLineHeight, subHeaderRect.width, textFieldHeight);
            rawText = EditorGUI.TextArea(textFieldRect, rawText);
            Rect errorBoxRect = new Rect(textFieldRect.x, textFieldRect.y + textFieldRect.height + EditorGUIUtility.singleLineHeight, textFieldRect.width, EditorGUIUtility.singleLineHeight * 2.5f);
            GUIStyle helpBoxRichText = new GUIStyle(EditorStyles.helpBox);
            Texture errorIcon = EditorGUIUtility.Load("icons/console.erroricon.png") as Texture2D;

            helpBoxRichText.richText = true;

            if (unableToParse)
                GUI.Label(errorBoxRect, new GUIContent("Unable to parse text into JSON. Make sure there are no mistakes! Are you missing a <b>{</b>, <b>{</b> or <b>,</b>?", errorIcon), helpBoxRichText);
        }

        wasTextMode = isTextMode;
        GUI.enabled = enabled;
    }

    private int RecursiveDrawField(Rect rect, JToken container)
    {
        int j = 0;
        for (int i = 0; i < container.Count(); i++)
        {
            JToken token = container.Values<JToken>().ToArray()[i];

            if (token.Type == JTokenType.Property)
            {
                JProperty property = token.Value<JProperty>();

                string propertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(property.Name.ToLower()) + ":";
                float propertyNameWidth = GUI.skin.label.CalcSize(new GUIContent(propertyName)).x;
                Rect propertyNameRect = new Rect(rect.x + rect.height, rect.y + EditorGUIUtility.singleLineHeight * j * 1.3f, propertyNameWidth, rect.height);

                Rect buttonRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * j * 1.3f, rect.height, rect.height);
                GUIStyle buttonContent = new GUIStyle(EditorStyles.label);
                buttonContent.normal.textColor = Color.grey;

                if (propertyToRename != property)
                {
                    if (GUI.Button(buttonRect, GUIContent.none, EditorStyles.miniButton))
                    {
                        GenericMenu menu = new GenericMenu();
                        if (property.Value.Type == JTokenType.Object)
                        {
                            JObject jObject = property.Value.Value<JObject>();
                            menu.AddItem(new GUIContent("Add/Empty Object"), false, () =>
                            {
                                AddNewProperty<JObject>(jObject);
                            });
                            menu.AddSeparator("Add/");
                            menu.AddItem(new GUIContent("Add/String"), false, () =>
                            {
                                AddNewProperty<string>(jObject);
                            });
                            menu.AddItem(new GUIContent("Add/Single"), false, () =>
                            {
                                AddNewProperty<float>(jObject);
                            });
                            menu.AddItem(new GUIContent("Add/Integer"), false, () =>
                            {
                                AddNewProperty<int>(jObject);
                            });
                            menu.AddItem(new GUIContent("Add/Boolean"), false, () =>
                            {
                                AddNewProperty<bool>(jObject);

                            });
                        }
                        menu.AddItem(new GUIContent("Remove"), false, () => {
                            token.Remove();
                        });
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Rename"), false, () => {
                            propertyToRename = property;
                            propertyRename = propertyToRename.Name;
                        });
                        menu.DropDown(buttonRect);
                    }
                    GUI.Label(propertyNameRect, propertyName);
                    GUI.Label(buttonRect, "►", buttonContent);
                }
                else
                {
                    Rect propertyTextFieldRect = new Rect(propertyNameRect.x + 2, propertyNameRect.y, propertyNameRect.width - 4, propertyNameRect.height);
                    GUI.SetNextControlName("RENAME_PROPERTY");
                    propertyRename = EditorGUI.TextField(propertyTextFieldRect, propertyRename);

                    GUI.color = new Color32(109, 135, 111, 255);
                    GUI.enabled = !string.IsNullOrEmpty(propertyRename);
                    if (GUI.Button(buttonRect, GUIContent.none, EditorStyles.miniButton))
                    {
                        property.Value.Rename(propertyRename);
                        GUI.FocusControl("");
                    }
                    GUI.color = Color.white;
                    buttonContent.normal.textColor = new Color32(133, 229, 143, 255);
                    GUI.Label(buttonRect, "✔", buttonContent);
                    GUI.enabled = true;
                }
                Rect nextRect = new Rect(rect.x + rect.height + propertyNameWidth, rect.y + EditorGUIUtility.singleLineHeight * j * 1.3f, rect.width - propertyNameWidth - rect.height, rect.height);
                j += RecursiveDrawField(nextRect, token);
            }
            else if (token.Type == JTokenType.Object)
            {
                j += RecursiveDrawField(rect, token);
            }
            else
            {
                JProperty parentProperty = token.Parent.Value<JProperty>();
                
                switch (token.Type)
                {
                    case JTokenType.String:
                        string stringValue = token.Value<string>();
                        stringValue = EditorGUI.TextField(rect, stringValue);
                        parentProperty.Value = stringValue;
                        break;
                    case JTokenType.Float:
                        float floatValue = token.Value<float>();
                        floatValue = EditorGUI.FloatField(rect, floatValue);
                        parentProperty.Value = floatValue;
                        break;
                    case JTokenType.Integer:
                        int intValue = token.Value<int>();
                        intValue = EditorGUI.IntField(rect, intValue);
                        parentProperty.Value = intValue;
                        break;
                    case JTokenType.Boolean:
                        bool boolValue = token.Value<bool>();
                        boolValue = EditorGUI.Toggle(rect, boolValue);
                        parentProperty.Value = boolValue;
                        break;
                    case JTokenType.Null:
                        float textFieldWidth = EditorStyles.helpBox.CalcSize(new GUIContent("Null")).x;
                        GUI.Label(new Rect(rect.x, rect.y, textFieldWidth, rect.height), "Null", EditorStyles.helpBox);
                        break;
                    default:
                        GUI.Label(new Rect(rect.x, rect.y, rect.width, rect.height), string.Format("Type '{0}' is not supported. Use text editor instead", token.Type.ToString()), EditorStyles.helpBox);
                        break;
                }
                j++;
            }
        }
        return Mathf.Max(j, 1);
    }

    private void LoadFromJson()
    {
        if (!string.IsNullOrWhiteSpace(Path) && File.Exists(Path))
        {
            rawText = File.ReadAllText(Path);
            jsonObject = JsonConvert.DeserializeObject<JObject>(rawText);
        }
    }

    private void WriteToJson()
    {
        if (jsonObject != null)
        {
            if (!wasTextMode)
                rawText = jsonObject.ToString();

            File.WriteAllText(Path, rawText);
        }
    }

    private void AddNewProperty<T>(JObject jObject)
    {
        string typeName = typeof(T).Name.ToLower();
        object value = default(T);

        switch (Type.GetTypeCode(typeof(T)))
        {
            case TypeCode.Boolean:
                break;
            case TypeCode.Int32:
                typeName = "integer";
                break;
            case TypeCode.Single:
                break;
            case TypeCode.String:
                value = "";
                break;
            default:
                if(typeof(T) == typeof(JObject))
                    typeName = "empty object";
                    value = new JObject();
                break;
        }

        string name = GetUniqueName(jObject, string.Format("new {0}", typeName));
        JProperty property = new JProperty(name, value);
        jObject.Add(property);
    }

    private string GetUniqueName(JObject jObject, string orignalName)
    {
        string uniqueName = orignalName;
        int suffix = 0;
        while (jObject[uniqueName] != null && suffix < 100)
        {
            suffix++;
            if (suffix >= 100)
            {
                Debug.LogError("Stop calling all your fields the same thing! Isn't it confusing?");
            }
            uniqueName = string.Format("{0} {1}", orignalName, suffix.ToString());
        }
        return uniqueName;
    }

    [MenuItem("Assets/Create/JSON File", priority = 81)]
    public static void CreateNewJsonFile()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
            path = "Assets";
        else if (System.IO.Path.GetExtension(path) != "")
            path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");

        path = System.IO.Path.Combine(path, "New JSON File.json");

        JObject jObject = new JObject();
        File.WriteAllText(path, jObject.ToString());
        AssetDatabase.Refresh();
    }
}
