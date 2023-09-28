using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jigbox.VirtualCursor
{
    public enum VirtualInputSystem
    {
        OLD_INPUT_SYSTEM,
        NEW_INPUT_SYSTEM
    }

    public class VirtualCursorInstance
    {
        private GameObject currentCursor;
        private BaseCursor cursor;
        public VirtualCursorInstance(VirtualInputSystem inputSystem)
        {
            string assetPath = VirtualCursorMgr.InputSystemAssetsMap[inputSystem.ToString()];
            Canvas rootCanvas = GameObject.Find("Canvas")?.GetComponentInChildren<Canvas>();
            var go = Resources.Load<GameObject>(assetPath);
            currentCursor = GameObject.Instantiate(go, rootCanvas?.transform);
            cursor = currentCursor.GetComponent<BaseCursor>();
        }

        public int TouchCount => cursor.TouchCount;

        public void Dispose()
        {
            if (currentCursor)
            {
                GameObject.Destroy(currentCursor);
                currentCursor = null;
            }
        }

        public void Show(bool isShow)
        {
            if (currentCursor)
            {
                currentCursor.SetActive(isShow);
            }
        }
    }


    public class VirtualCursorMgr
    {
        static VirtualCursorMgr _instance;
        public static VirtualCursorMgr Instance 
        { 
            get
            {
                if (_instance == null)
                {
                    _instance = new VirtualCursorMgr();
                }
                return _instance;
            } 
        }

        public static Dictionary<string, string> InputSystemAssetsMap = new()
        {
            { "OLD_INPUT_SYSTEM", "Prefabs/CursorStandalone" },
            { "NEW_INPUT_SYSTEM", "Prefabs/CursorNewInput" }
        };

        private static Dictionary<int, VirtualCursorInstance> instanceCaches = new();
        
        public int TouchCount 
        {
            get
            {
                return instanceCaches.Values.Sum((t)=>t.TouchCount);
            }
        }

        public void Launch(VirtualInputSystem inputSystem = VirtualInputSystem.NEW_INPUT_SYSTEM, int index = 0)
        {
            if (!instanceCaches.ContainsKey(index))
            {
                instanceCaches.Add(index, new VirtualCursorInstance(inputSystem));
            }
            else
            {
                instanceCaches[index].Show(true);
            }
        }

        public void Show(int index = 0)
        {
            if (instanceCaches.ContainsKey(index))
            {
                instanceCaches[index].Show(true);
            }
        }

        public void Hide(int index = 0)
        {
            if (instanceCaches.ContainsKey(index))
            {
                instanceCaches[index].Show(false);
            }
        }

        public void Dispose(int index = 0)
        {
            if (instanceCaches.ContainsKey(index))
            {
                instanceCaches[index].Dispose();
                instanceCaches.Remove(index);
            }
        }
    }
}