using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minimalist.Bar.Utility
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject();

                        singletonObject.name = typeof(T).ToString();

                        _instance = singletonObject.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        public virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);

                return;
            }

            else
            {
                _instance = GetComponent<T>();
            }

            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}