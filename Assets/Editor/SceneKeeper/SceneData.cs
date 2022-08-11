using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrunoMikoski.SceneHierarchyKeeper
{
    [Serializable]
    internal class SceneData
    {
        [SerializeField]
        internal List<HierarchyData> scenesHierarchy = new List<HierarchyData>();

        [SerializeField]
        internal List<SelectionData> selectionData = new List<SelectionData>();

        [SerializeField]
        internal List<string> alwaysExpanded = new List<string>();
        
        public HierarchyData GetOrAddSceneData(string scenePath)
        {
            if (TryGetSceneData(scenePath, out HierarchyData resultData))
                return resultData;

            resultData = new HierarchyData {scenePath = scenePath};
            scenesHierarchy.Add(resultData);
            return resultData;
        }

        public SelectionData GetOrAddSceneSelectionData(string scenePath)
        {
            if (TryGetSceneSelectionData(scenePath, out SelectionData resultData))
                return resultData;

            resultData = new SelectionData(scenePath);
            
            selectionData.Add(resultData);
            return resultData;
        }

        public bool TryGetSceneSelectionData(string scenePath, out SelectionData resultSelectionData)
        {
            for (int i = 0; i < selectionData.Count; i++)
            {
                SelectionData data = selectionData[i];
                if (data.scenePath.Equals(scenePath, StringComparison.OrdinalIgnoreCase))
                {
                    resultSelectionData = data;
                    return true;
                }
            }

            resultSelectionData = null;
            return false;
        }

        public bool TryGetSceneData(string scenePath, out HierarchyData resultHierarchyData)
        {
            for (int i = 0; i < scenesHierarchy.Count; i++)
            {
                HierarchyData hierarchyData = scenesHierarchy[i];
                if (hierarchyData.scenePath.Equals(scenePath, StringComparison.InvariantCulture))
                {
                    resultHierarchyData = hierarchyData;
                    return true;
                }
            }

            resultHierarchyData = null;
            return false;
        }
    }

    [Serializable]
    internal class SelectionData
    {
        [SerializeField]
        internal string scenePath;
        [SerializeField]
        internal List<string> itemPath = new List<string>();

        public SelectionData(string scenePath)
        {
            this.scenePath = scenePath;
        }
    }
            
    [Serializable]
    internal class HierarchyData
    {
        [SerializeField]
        internal string scenePath;
        [SerializeField]
        internal List<string> itemsPath = new List<string>();
    }
}
