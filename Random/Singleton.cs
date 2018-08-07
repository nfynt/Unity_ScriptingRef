using UnityEngine;

namespace Shubham
{
    public class Singleton<T> : MonoBehaviour where T:MonoBehaviour
    {
        private static T myInstance;
        public static T Instance
        {
            get { return myInstance; }
        }

        private void Awake()
        {
            if(Instance==null)
            {
                myInstance = FindObjectOfType<T>();
                if(myInstance==null)
                {
                    myInstance = new GameObject("test").AddComponent<T>();
                }
            }
            else
            {
                Destroy(this);
            }

        }
    }
}
