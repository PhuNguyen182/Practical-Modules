using UnityEngine;

namespace PracticalModules.Patterns.Singleton
{
    public abstract class MonoSingleton<TComponent> : MonoBehaviour where TComponent : Component
    {
        private static TComponent _instance;
        
        public static TComponent Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TComponent>();

                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.hideFlags = HideFlags.None;
                        _instance = obj.AddComponent<TComponent>();
                    }
                }

                return _instance;
            }
        }
        
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as TComponent;
                DontDestroyOnLoad(this.gameObject);
                OnAwake();
            }

            else
            {
                if (this != _instance)
                    Destroy(this.gameObject);
            }
        }

        protected virtual void OnAwake()
        {

        }
    }
}
