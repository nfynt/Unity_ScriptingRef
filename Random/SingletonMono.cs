using UnityEngine;

namespace nfynt
{
    public class SingletonMono < T > : MonoBehaviour where T : MonoBehaviour
    {
        private static T s_Instance;

        public static T Instance
        {
            get
            {
                if ( s_Instance == null )
                {
                    s_Instance = FindObjectOfType < T >();
                    if(s_Instance == null)
                    {
                        s_Instance = new GameObject("SingletonMono").AddComponent<T>();
                    }
                }

                return s_Instance;
            }
        }

        protected void RegisterInstanceOrDestroy( T instance )
        {
            if ( s_Instance == null )
            {
                s_Instance = instance;
            }
            else if ( s_Instance != this )
            {
                Destroy( instance );
            }
        }
    }
}