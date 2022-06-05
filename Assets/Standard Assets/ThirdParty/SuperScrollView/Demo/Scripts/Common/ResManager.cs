using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace SuperScrollView
{
    public class ResManager : MonoBehaviour
    {
        public Sprite[] spriteObjArray;
        // Use this for initialization
        static ResManager instance = null;

        string[] mWordList;

        Dictionary<string, Sprite> spriteObjDict = new Dictionary<string, Sprite>();

        public static ResManager Get
        {
            get
            {
                if (instance == null)
                {
                    instance = Object.FindObjectOfType<ResManager>();
                }
                return instance;
            }

        }


        void InitData()
        {
            spriteObjDict.Clear();
            foreach (Sprite sp in spriteObjArray)
            {
                spriteObjDict[sp.name] = sp;
            }
        }

        void Awake()
        {
            instance = null;
            InitData();
        }

        public Sprite GetSpriteByName(string spriteName)
        {
            Sprite ret = null;
            if (spriteObjDict.TryGetValue(spriteName, out ret))
            {
                return ret;
            }
            return null;
        }


        public string GetRandomSpriteName()
        {
            int count = spriteObjArray.Length;
            int index = Random.Range(0, count);
            return spriteObjArray[index].name;
        }

        public int SpriteCount
        {
            get
            {
                return spriteObjArray.Length;
            }
        }

        public Sprite GetSpriteByIndex(int index)
        {
            if (index < 0 || index >= spriteObjArray.Length)
            {
                return null;
            }
            return spriteObjArray[index];
        }

        public string GetSpriteNameByIndex(int index)
        {
            if (index < 0 || index >= spriteObjArray.Length)
            {
                return "";
            }
            return spriteObjArray[index].name;
        }
    }
}
