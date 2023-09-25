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
using UnityEditor;
using Jigbox.EditorUtils;

namespace Jigbox.Components
{
    [CustomEditor(typeof(RaycastArea))]
    [CanEditMultipleObjects]
    public class RaycastAreaEditor : Editor
    {
#region inner classes, enum, and structs

        enum SizeDefference
        {
            None,
            Width = 1,
            Height = 1 << 1,
            All = Width | Height,
        }

#endregion

#region properties

        /// <summary>RaycastArea</summary>
        protected RaycastArea area;

        /// <summary>RaycastPadding</summary>
        protected RaycastPaddingInspector raycastPadding;

#endregion

#region private methods

        /// <summary>
        /// 当たり判定の範囲を強調表示します。
        /// </summary>
        protected void DrawArea()
        {
            RectTransform rectTransform = area.RectTransform;
            Vector2 size = area.Size;
            size.x *= rectTransform.lossyScale.x;
            size.y *= rectTransform.lossyScale.y;

            Vector3 leftTop = rectTransform.position;
            leftTop.x -= size.x * rectTransform.pivot.x;
            leftTop.y += size.y * (1.0f - rectTransform.pivot.y);

            Vector3 rightTop = rectTransform.position;
            rightTop.x += size.x * (1.0f - rectTransform.pivot.x);
            rightTop.y += size.y * (1.0f - rectTransform.pivot.y);

            Vector3 rightBottom = rectTransform.position;
            rightBottom.x += size.x * (1.0f - rectTransform.pivot.x);
            rightBottom.y -= size.y * rectTransform.pivot.y;

            Vector3 leftBottom = rectTransform.position;
            leftBottom.x -= size.x * rectTransform.pivot.x;
            leftBottom.y -= size.y * rectTransform.pivot.y;

            Handles.color = Color.green;
            Handles.DrawAAPolyLine(3.0f, leftTop, rightTop, rightBottom, leftBottom, leftTop);
        }

        /// <summary>
        /// 複数選択時に各コンポーネントのサイズの状態を確認して状態を返します。
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        SizeDefference GetMultiSelectDifference(Vector2 original)
        {
            SizeDefference difference = SizeDefference.None;
            foreach (Object obj in serializedObject.targetObjects)
            {
                RaycastArea area = obj as RaycastArea;
                difference |= EditorUtilsTools.Difference(original.x, area.Size.x)
                    ? SizeDefference.Width : SizeDefference.None;
                difference |= EditorUtilsTools.Difference(original.y, area.Size.y)
                    ? SizeDefference.Height : SizeDefference.None;

                if (difference == SizeDefference.All)
                {
                    break;
                }
            }
            return difference;
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            area = target as RaycastArea;
            area.color = Color.clear;

            raycastPadding = new RaycastPaddingInspector();
        }

        public override void OnInspectorGUI()
        {
            if (area == null)
            {
                return;
            }

            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

#pragma warning disable 219
            RectTransform rectTransform = area.RectTransform;
#pragma warning restore 219

            raycastPadding.DrawInspector(targets);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("isControllableRaycastTarget"),
                new GUIContent("Controllable Raycast"));

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Size", GUILayout.Width(40.0f));

                if (serializedObject.targetObjects.Length == 1)
                {
                    Vector2 size = EditorGUILayout.Vector2Field("", area.Size);
                    size.x = size.x > 0.0f ? size.x : 0.0f;
                    size.y = size.y > 0.0f ? size.y : 0.0f;
                    if (size != area.Size)
                    {
                        area.Size = size;
                    }
                }
                // 複数選択時の処理
                else
                {
                    RaycastArea original = serializedObject.targetObject as RaycastArea;
                    Vector2 size = original.Size;
                    SizeDefference difference = GetMultiSelectDifference(size);
                    SizeDefference editStatus = SizeDefference.None;

                    EditorUtilsTools.SetLabelWidth(15.0f);

                    editStatus |= EditorUtilsTools.FloatField("X", ref size.x, (difference & SizeDefference.Width) > 0)
                        ? SizeDefference.Width : SizeDefference.None;
                    editStatus |= EditorUtilsTools.FloatField("Y", ref size.y, (difference & SizeDefference.Height) > 0)
                        ? SizeDefference.Height : SizeDefference.None;
                    size.x = size.x > 0.0f ? size.x : 0.0f;
                    size.y = size.y > 0.0f ? size.y : 0.0f;

                    if (editStatus != SizeDefference.None)
                    {
                        foreach (Object obj in serializedObject.targetObjects)
                        {
                            RaycastArea selectArea = obj as RaycastArea;
                            Vector2 areaSize = selectArea.Size;

                            areaSize.x = (editStatus & SizeDefference.Width) > 0 ? size.x : areaSize.x;
                            areaSize.y = (editStatus & SizeDefference.Height) > 0 ? size.y : areaSize.y;

                            selectArea.Size = areaSize;
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

            EditorUtilsTools.RegisterUndo("Edit Raycast Area", GUI.changed, area.rectTransform);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

        protected virtual void OnSceneGUI()
        {
            if (Selection.objects.Length > 1)
            {
                return;
            }

            Color temp = Handles.color;

            DrawArea();

            Handles.color = temp;
        }

#endregion
    }
}
