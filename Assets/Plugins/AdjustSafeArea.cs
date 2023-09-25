using System;
using UnityEngine;
using UnityEngine.UI;


namespace Jigbox.Examples
{
    public class AdjustSafeArea : MonoBehaviour
    {
        [SerializeField]
        RectTransform safeAreaRectTransform = null;

        void Awake()
        {
            // safeArea対応
#if UNITY_2017_2_OR_NEWER
            var safeArea = Screen.safeArea;
#else
            var safeArea = SafeAreaUtils.GetSafeArea();
#endif
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            safeAreaRectTransform.anchorMin = anchorMin;
            safeAreaRectTransform.anchorMax = anchorMax;
        }
    }
}
