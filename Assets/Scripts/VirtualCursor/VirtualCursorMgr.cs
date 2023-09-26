using System.Collections.Generic;
using UnityEngine;

namespace Jigbox.VirtualCursor
{
    public enum VirtualInputSystem
    {
        OLD_INPUT_SYSTEM,
        NEW_INPUT_SYSTEM
    }

    public class VirtualCursorMgr
    {
        private GameObject currentCursor;

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

        static Dictionary<string, string> _inputSystemAssetsMap = new Dictionary<string, string>()
        {
            { "OLD_INPUT_SYSTEM", "Prefabs/CursorStandalone" },
            { "NEW_INPUT_SYSTEM", "Prefabs/CursorNewInput" }
        };
        

        public void Launch(VirtualInputSystem inputSystem)
        {
            string assetPath = _inputSystemAssetsMap[inputSystem.ToString()];
            Canvas rootCanvas = GameObject.Find("Canvas").GetComponentInChildren<Canvas>();
            var go = Resources.Load<GameObject>(assetPath);
            currentCursor = GameObject.Instantiate(go, rootCanvas.transform);
        }

        public void Dispose()
        {
            GameObject.Destroy(currentCursor);
            currentCursor = null;
        }
    }
}