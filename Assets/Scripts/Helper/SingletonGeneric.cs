using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon2D.Helper
{
    public abstract class SingletonGeneric<T> : MonoBehaviour where T : SingletonGeneric<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();
                if (_instance == null)
                    Debug.LogError("[SingletonGeneric] This component doesn't attached to a gameObject: " + typeof(T).Name);
                return _instance;
            }
        }

        public static bool IsInitialized
        {
            get => _instance != null;
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Debug.LogError("[SingletonGeneric] There is an gameObject already exist, no need to extra one, destroying extra now: " + typeof(T).Name);
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this as T)
                _instance = null;
        }
    }
}
