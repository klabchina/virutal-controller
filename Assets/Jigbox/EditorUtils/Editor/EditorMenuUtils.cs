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
using UnityEngine.EventSystems;
#if UNITY_2021_1_OR_NEWER
using UnityEditor.SceneManagement;
#else
using UnityEditor.Experimental.SceneManagement;
#endif
namespace Jigbox.EditorUtils
{
    public static class EditorMenuUtils
    {
#region public methods
        
        /// <summary>
        /// オブジェクトをHierarchy上に作成します。
        /// </summary>
        /// <param name="target">作成するGameObject</param>
        public static void CreateObject(GameObject target)
        {
            Undo.RegisterCreatedObjectUndo(target, "Create Object - " + target.name);

            GameObject selectedObject = Selection.activeGameObject;
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (selectedObject == null && prefabStage != null)
            {
                selectedObject = prefabStage.prefabContentsRoot;
            }

            if (selectedObject != null)
            {
                target.name = GameObjectUtility.GetUniqueNameForSibling(selectedObject.transform, target.name);
                target.transform.SetParent(selectedObject.transform, false);
                SetLayer(target, selectedObject.layer);
            }
            else
            {
                string name = target.name;
                // この時点でHierarchy上にすでに生成されているので、必ず"name (N)"の形式で返ってくる
                target.name = GameObjectUtility.GetUniqueNameForSibling(null, target.name);
                // Hierarchyのルートで元の名前のオブジェクトを探して、存在していなければ、名前を戻す
                GameObject sameNameObject = GameObject.Find('/' + name);
                if (sameNameObject == null)
                {
                    target.name = name;
                }
            }
            
            Selection.activeGameObject = target;
        }

        /// <summary>
        /// UIオブジェクトをHierarchy上に作成します。
        /// </summary>
        /// <param name="target">作成するGameObject</param>
        public static void CreateUIObject(GameObject target)
        {
            GameObject selectedObject = null;
            string undoName = "Create UI - " + target.name;

            if (Selection.gameObjects.Length > 0)
            {
                selectedObject = Selection.gameObjects[0];
                // 親にCanvasを持っていないオブジェクトが選択されいている場合は、選択オブジェクトを対象として扱わない
                if (selectedObject.GetComponentInParent<Canvas>() == null)
                {
                    selectedObject = null;
                }
            }
            
            // オブジェクト未選択
            if (selectedObject == null)
            {
                Canvas canvas = null;
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage == null)
                {
                    canvas = GameObject.FindObjectOfType<Canvas>();
                    if (canvas != null)
                    {
                        selectedObject = canvas.gameObject;
                    }
                }

                // Canvasが存在しない場合、Canvasを作成
                if (selectedObject == null)
                {
                    GameObject canvasObject = new GameObject("Canvas");
                    canvas = canvasObject.AddComponent<Canvas>();
                    canvasObject.AddComponent<CanvasScaler>();
                    canvasObject.AddComponent<GraphicRaycaster>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    RectTransform transform = canvasObject.transform as RectTransform;
                    transform.sizeDelta = new Vector2(Screen.width, Screen.height);
                    canvasObject.layer = LayerMask.NameToLayer("UI");
                    if (prefabStage != null && !prefabStage.IsPartOfPrefabContents(selectedObject))
                    {
                        Undo.SetTransformParent(canvasObject.transform, prefabStage.prefabContentsRoot.transform, "prefabStageCanvas");
                    }

                    Undo.RegisterCreatedObjectUndo(canvasObject, undoName);

                    selectedObject = canvasObject;
                }
            }

            // EventSystemが存在していなければ作成
            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.AddComponent<EventSystem>();
                eventSystemObject.AddComponent<StandaloneInputModule>();

                Undo.RegisterCreatedObjectUndo(eventSystemObject, undoName);
            }

            Undo.RegisterCreatedObjectUndo(target, undoName);

            target.name = GameObjectUtility.GetUniqueNameForSibling(selectedObject.transform, target.name);
            target.transform.SetParent(selectedObject.transform, false);
            SetLayer(target, selectedObject.layer);
            
            Selection.activeGameObject = target;
        }

#endregion

#region private methods

        /// <summary>
        /// 対象オブジェクト以下のレイヤーを設定します。
        /// </summary>
        /// <param name="target">対象オブジェクト</param>
        /// <param name="layer">レイヤー</param>
        static void SetLayer(GameObject target, int layer)
        {
            target.layer = layer;

            foreach (Transform child in target.transform)
            {
                SetLayer(child.gameObject, layer);
            }
        }

#endregion
    }
}
