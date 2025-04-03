using UnityEngine;
using System.Collections;

namespace Kud.Common
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogWarning("GameObjectが設定されていません");
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<T>();
                }
                return instance;
            }
        }

        protected SingletonMonoBehaviour()
        {
            if(instance == null)
            {
                instance = this as T;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else
            {
                if (instance != this as T)
                {
                    Destroy(this as T);
                }
            }
        }
    }
}