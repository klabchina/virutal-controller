/**
 * Jigbox
 * Copyright(c) 2016 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 *
 * Subject to the prior written consent of KLab, Inc(Licensor) and its terms and
 * conditions, Licensor grants to you, and you hereby accept nontransferable,
 * nonexclusive limited right to access, obtain, use, copy and/or download
 * a copy of this product only for requirement purposes. You may not rent,
 * lease, loan, time share, sublicense, transfer, make generally available,
 * license, disclose, disseminate, distribute or otherwise make accessible or
 * available this product to any third party without the prior written approval
 * of Licensor. Unauthorized copying of this product, including modifications 
 * of this product or programs in which this product has been merged or included
 * with other software products is expressly forbidden.
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Jigbox.EditorUtils;

namespace Jigbox.Components
{
    public static class PopupViewMenuEditor
    {
#region public methods

        [MenuItem("GameObject/UI/Jigbox/Popup/PopupView")]
        public static void Create()
        {
            // PopupViewのルートオブジェクト作る
            GameObject popupViewObject = new GameObject("PopupView");
            PopupView popupView = popupViewObject.AddComponent<PopupView>();
            SetupPopupView(popupViewObject, popupView);
            
            EditorMenuUtils.CreateObject(popupViewObject);
        }

        [MenuItem("GameObject/UI/Jigbox/Popup/PopupGroupView")]
        public static void CreateGroup()
        {
            // PopupViewのルートオブジェクト作る
            GameObject popupViewObject = new GameObject("PopupGroupView");
            PopupView popupView = popupViewObject.AddComponent<PopupGroupView>();
            SetupPopupView(popupViewObject, popupView);
            
            PopupGroupViewSorter popupSorter = popupViewObject.AddComponent<PopupGroupViewSorter>();
            SetupPopupViewSorter(popupViewObject, popupSorter);

            EditorMenuUtils.CreateObject(popupViewObject);
        }

        static void SetupPopupView(GameObject popupViewObject, PopupView popupView)
        {
            var camera = CreateCamera(popupViewObject.transform);
            var canvas = CreateCanvas(popupViewObject.transform, camera);
            var popupContainer = CreatePopupContainer(canvas.transform);
            var blockerBack = CreateBlockerBack(popupContainer.transform);
            var blockerTransition = CreateBlockerTransition(canvas.transform);

            // 参照を設定する
            SerializedObject serializedObject = new SerializedObject(popupView);
            SerializedProperty canvasProperty = serializedObject.FindProperty("canvas");
            SerializedProperty popupContainerProperty = serializedObject.FindProperty("popupContainer");
            SerializedProperty inputBlockerProperty = serializedObject.FindProperty("inputBlocker");
            SerializedProperty blockerBackProperty = inputBlockerProperty.FindPropertyRelative("back");
            SerializedProperty blockerTransitionProperty = inputBlockerProperty.FindPropertyRelative("transition");
            popupContainerProperty.objectReferenceValue = popupContainer;
            blockerBackProperty.objectReferenceValue = blockerBack;
            blockerTransitionProperty.objectReferenceValue = blockerTransition;
            if(canvasProperty != null)
            {
                canvasProperty.objectReferenceValue = canvas;
            }
            
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        static void SetupPopupViewSorter(GameObject popupViewObject, PopupGroupViewSorter popupSorter)
        {
            SerializedObject serializedObject = new SerializedObject(popupSorter);
            SerializedProperty canvasProperty = serializedObject.FindProperty("canvas");
            canvasProperty.objectReferenceValue = popupViewObject.GetComponentInChildren<Canvas>();
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

#endregion

#region private methods

        static Camera CreateCamera(Transform parent)
        {
            // Camera用意する
#if UNITY_2017_1_OR_NEWER
            GameObject cameraObject = new GameObject("Camera", typeof(Camera), typeof(FlareLayer));
#else
            GameObject cameraObject = new GameObject("Camera", typeof(Camera), typeof(GUILayer), typeof(FlareLayer));
#endif
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Depth;
            camera.orthographic = true;
            cameraObject.transform.SetParent(parent, false);

            return camera;
        }

        static Canvas CreateCanvas(Transform parent, Camera camera)
        {
            // Canvas用意する
            GameObject canvasObject = new GameObject("Canvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvasObject.layer = LayerMask.NameToLayer("UI");
            canvasObject.transform.SetParent(parent, false);
            {
                RectTransform transform = canvasObject.transform as RectTransform;
                transform.sizeDelta = new Vector2(Screen.width, Screen.height);
            }

            return canvas;
        }

        static GameObject CreatePopupContainer(Transform parent)
        {
            // Popupを置くためのContainerを作る
            GameObject popupContainer = new GameObject("PopupContainer", typeof(RectTransform));
            popupContainer.transform.SetParent(parent, false);
            popupContainer.layer = LayerMask.NameToLayer("UI");
            {
                RectTransform transform = popupContainer.transform as RectTransform;
                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.sizeDelta = Vector2.zero;
            }

            return popupContainer;
        }

        static GameObject CreateBlockerBack(Transform parent)
        {
            // 最前面のポップアップより後ろへの入力をブロックするためのオブジェクトを作る
            GameObject blockerBack = new GameObject("InputBlocker Back", typeof(RectTransform));
            blockerBack.transform.SetParent(parent, false);
            blockerBack.layer = LayerMask.NameToLayer("UI");
            {
                RectTransform transform = blockerBack.transform as RectTransform;
                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.sizeDelta = Vector2.zero;

                Image image = blockerBack.AddComponent<Image>();
                image.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
            }

            return blockerBack;
        }

        static GameObject CreateBlockerTransition(Transform parent)
        {
            // トランジション中の入力をブロックするためのオブジェクトを作る
            GameObject blockerTransition = new GameObject("InputBlocker Transition", typeof(RectTransform));
            blockerTransition.transform.SetParent(parent, false);
            blockerTransition.layer = LayerMask.NameToLayer("UI");
            {
                RectTransform transform = blockerTransition.transform as RectTransform;
                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.sizeDelta = Vector2.zero;

                Image image = blockerTransition.AddComponent<Image>();
                image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }

            return blockerTransition;
        }

#endregion
    }
}