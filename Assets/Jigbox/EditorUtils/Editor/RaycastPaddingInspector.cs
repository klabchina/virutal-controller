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

using System.Linq;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.EditorUtils
{
    /// <summary>
    /// Graphicコンポーネントに追加されたRaycastPaddingのインスペクター表示を行う為のクラス<br/>
    /// <see cref="GraphicEditor"/>の実装とほぼ同じになっています
    /// </summary>
    public class RaycastPaddingInspector
    {
        /// <summary>RaycastPaddingのGUIContent</summary>
        protected readonly GUIContent paddingContent;

        /// <summary>leftのGUIContent</summary>
        protected readonly GUIContent leftContent;

        /// <summary>rightのGUIContent</summary>
        protected readonly GUIContent rightContent;

        /// <summary>topのGUIContent</summary>
        protected readonly GUIContent topContent;

        /// <summary>bottomのGUIContent</summary>
        protected readonly GUIContent bottomContent;

        /// <summary>RaycastPaddingが展開されているか</summary>
        protected static bool isShowPadding = false;

        public RaycastPaddingInspector()
        {
            paddingContent = EditorGUIUtility.TrTextContent("Raycast Padding");
            leftContent = EditorGUIUtility.TrTextContent("Left");
            rightContent = EditorGUIUtility.TrTextContent("Right");
            topContent = EditorGUIUtility.TrTextContent("Top");
            bottomContent = EditorGUIUtility.TrTextContent("Bottom");
        }

        /// <summary>RaycastPaddingのインスペクター表示を行う</summary>
        public virtual void DrawInspector(Object[] targets)
        {
#if UNITY_2020_3_OR_NEWER

            var graphics = targets.Select(t => t as Graphic).ToArray();
            isShowPadding = EditorGUILayout.Foldout(isShowPadding, paddingContent);

            if (isShowPadding)
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.indentLevel++;
                    Vector4 newPadding = graphics[0].raycastPadding;

                    EditorGUI.showMixedValue = graphics.Select(g => g.raycastPadding.x).Distinct().Count() >= 2;
                    newPadding.x = EditorGUILayout.FloatField(leftContent, newPadding.x);
                    EditorGUI.showMixedValue = graphics.Select(g => g.raycastPadding.z).Distinct().Count() >= 2;
                    newPadding.z = EditorGUILayout.FloatField(rightContent, newPadding.z);
                    EditorGUI.showMixedValue = graphics.Select(g => g.raycastPadding.w).Distinct().Count() >= 2;
                    newPadding.w = EditorGUILayout.FloatField(topContent, newPadding.w);
                    EditorGUI.showMixedValue = graphics.Select(g => g.raycastPadding.y).Distinct().Count() >= 2;
                    newPadding.y = EditorGUILayout.FloatField(bottomContent, newPadding.y);
                    EditorGUI.showMixedValue = false;

                    if (check.changed)
                    {
                        foreach (var graphic in graphics)
                        {
                            graphic.raycastPadding = newPadding;
                        }
                    }

                    EditorGUI.indentLevel--;
                }
            }

#endif
        }
    }
}
