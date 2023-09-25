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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jigbox.EditorUtils;
using Jigbox.UIControl;
using UnityEditor;
using UnityEngine;

namespace Jigbox.UnityEditorExtension
{
    /// <summary>
    /// RectTransformのインスペクター表示に機能を追加した
    /// Canvasの左上を原点とした座標入力
    /// </summary>
    [CustomEditor(typeof(RectTransform))]
    [CanEditMultipleObjects]
    public class RectTransformExtensionEditor : Editor
    {
        /// <summary>機能有効化メニューのパス</summary>
        protected const string MenuPath = "Tools/Jigbox/RectTransformEditorAddOn";

        /// <summary>EditorUserSettingとMenuによる機能の有効化管理</summary>
        [MenuItem(MenuPath)]
        static void RectTransformEditorCheckbox()
        {
            var isChecked = Menu.GetChecked(MenuPath);
            Menu.SetChecked(MenuPath, !isChecked);
            EditorUserSettings.SetConfigValue(MenuPath, (!isChecked).ToString());
        }

        /// <summary>機能有効化</summary>
        [InitializeOnLoadMethod]
        static void AwakeEditor()
        {
            bool parseResult;
            bool.TryParse(EditorUserSettings.GetConfigValue(MenuPath), out parseResult);
            EditorApplication.delayCall += () => { Menu.SetChecked(MenuPath, parseResult); };
        }

        /// <summary>Editorのアセンブリ情報</summary>
        protected static readonly Assembly editorAssembly = Assembly.GetAssembly(typeof(Editor));

        /// <summary>アセンブリの型の名前</summary>
        protected static readonly string AssemblyTypeName = "RectTransformEditor";

        /// <summary>ラベル名</summary>
        protected static readonly string Label = "CanvasPos";

        /// <summary>ラベルの幅</summary>
        protected static readonly float LabelWidth = 50.0f;

        /// <summary>プロパティのラベルの幅</summary>
        protected static readonly float PropertyLabelWidth = 15.0f;

        /// <summary>標準のEditor</summary>
        protected Editor defaultEditor;

        /// <summary>drivenPropertyのリフレクション呼び出し用</summary>
        protected PropertyInfo drivenProperty;

        /// <summary>RectTransform</summary>
        protected RectTransform rectTransform = null;

        /// <summary>Canvas上左上からの座標</summary>
        protected Vector2 canvasPos;

        /// <summary>キャンパスのRectTransformか</summary>
        protected bool isCanvas = false;

        /// <summary>ルートキャンパス</summary>
        protected Canvas rootCanvas;

        /// <summary>キャンパスプロパティ</summary>
        protected Canvas RootCanvas
        {
            get
            {
                if (rootCanvas == null)
                {
                    CacheRootCanvas();
                }

                return rootCanvas;
            }
        }

        /// <summary>親のCanvasを探す用のリスト</summary>
        protected static readonly List<Canvas> parentCanvasList = new List<Canvas>();

        /// <summary>ルートキャンパスを探しキャッシュする</summary>
        protected void CacheRootCanvas()
        {
            parentCanvasList.Clear();
            rectTransform.gameObject.GetComponentsInParent(false, parentCanvasList);
            if (parentCanvasList.Count > 0)
            {
                for (int i = 0; i < parentCanvasList.Count; ++i)
                {
                    if (parentCanvasList[i].isActiveAndEnabled)
                    {
                        rootCanvas = parentCanvasList[i].rootCanvas;
                        break;
                    }

                    if (i == parentCanvasList.Count - 1)
                    {
                        rootCanvas = null;
                    }
                }
            }
            else
            {
                rootCanvas = null;
            }
        }

        /// <summary>標準のエディタの情報を取得</summary>
        protected virtual void GetDefaultEditor()
        {
            Type defaultEditorType = editorAssembly.GetTypes().FirstOrDefault(t => t.Name == AssemblyTypeName);
            defaultEditor = CreateEditor(targets, defaultEditorType);

            var type = rectTransform.GetType();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic).ToArray();
            drivenProperty = properties.FirstOrDefault(prop => prop.Name == "drivenProperties");
        }

        protected virtual void OnEnable()
        {
            rectTransform = target as RectTransform;
            GetDefaultEditor();

            // 自身がCanvasかチェック
            if (rectTransform != null)
            {
                isCanvas = rectTransform.GetComponent<Canvas>() != null;
            }
        }

        public override void OnInspectorGUI()
        {
            if (!isCanvas && RootCanvas != null && Menu.GetChecked(MenuPath))
            {
                // Disableチェック
                var xDisable = targets.Any(x => ((DrivenTransformProperties) drivenProperty.GetValue(x as RectTransform, null) & DrivenTransformProperties.AnchoredPositionX) != 0);
                var yDisable = targets.Any(x => ((DrivenTransformProperties) drivenProperty.GetValue(x as RectTransform, null) & DrivenTransformProperties.AnchoredPositionY) != 0);

                var screenPositions = targets.Select(t => RectTransformUtils.GetScreenPoint(RootCanvas, t as RectTransform)).Distinct().ToArray();
                Vector2 result;
                if (EditorUtilsTools.Vector2Field(Label, LabelWidth, PropertyLabelWidth, xDisable, yDisable, screenPositions, out result))
                {
                    canvasPos = result;
                    foreach (var o in targets)
                    {
                        var gui = (RectTransform) o;
                        var worldPoint = RectTransformUtils.LeftTopScreenPointToWorldPoint(RootCanvas, gui, canvasPos);
                        gui.position = worldPoint;
                        gui.anchoredPosition = new Vector2(Mathf.Round(gui.anchoredPosition.x), Mathf.Round(gui.anchoredPosition.y));
                    }
                }
            }

            // デフォルトのインスペクターを表示する
            defaultEditor.OnInspectorGUI();

            EditorUtilsTools.RegisterUndo("RectTransform Changed", GUI.changed, targets);
        }

        protected virtual void OnDisable()
        {
            DestroyImmediate(defaultEditor);
        }
    }
}
