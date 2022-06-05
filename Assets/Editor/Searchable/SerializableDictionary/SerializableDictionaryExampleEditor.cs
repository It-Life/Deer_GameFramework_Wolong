using UnityEditor;

namespace SerializableDictionary.Editor
{
    [CustomEditor(typeof(SerializableDictionaryExample))]
    public class SerializableDictionaryExampleEditor : UnityEditor.Editor
    {
        private SerializableDictionaryExample Target=> target as SerializableDictionaryExample;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}