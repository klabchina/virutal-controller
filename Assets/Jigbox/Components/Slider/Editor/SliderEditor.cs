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

using SliderFillMethod = Jigbox.Components.Slider.SliderFillMethod;

namespace Jigbox.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Slider), true)]
    public class SliderEditor : GaugeEditor
    {
#region properties

        /// <summary>ハンドルのプロパティ</summary>
        protected SerializedProperty handleProperty;

        /// <summary>柱状ゲージのView</summary>
        protected ColumnGaugeViewBase columnView;

#endregion

#region protected methods

        /// <summary>
        /// フィリング対象のコンポーネントの編集用の表示を行います。
        /// </summary>
        protected override void DrawTarget()
        {
            base.DrawTarget();

            // ハンドルはオブジェクトごとに固有のものなので、複数同時選択時は編集不可
            EditorGUI.BeginDisabledGroup(targets.Length > 1);
            {
                serializedObject.Update();

                RectTransform handle = handleProperty.objectReferenceValue as RectTransform;
                RectTransform selected = EditorGUILayout.ObjectField("Handle", handle, typeof(RectTransform), true) as RectTransform;
                if (handle != selected)
                {
                    // ハンドルの対象が更新された場合、元々シリアライズされていた
                    // ハンドルをvalue=1相当に位置に設定してから参照を更新する
                    UpdateHandle(false);
                    handleProperty.objectReferenceValue = selected;
                    UpdateHandle();
                }
            }
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// フィリング方法の編集用の表示を行います。
        /// </summary>
        protected override void DrawFillMethod()
        {
            SliderFillMethod fillMethod = GetEnumValue<SliderFillMethod>(fillMethodProperty.intValue);
            fillMethod = (SliderFillMethod) EditorGUILayout.EnumPopup("Fill Method", fillMethod);
            if (fillMethodProperty.intValue != (int) fillMethod)
            {
                fillMethodProperty.intValue = (int) fillMethod;
                fillOriginProperty.intValue = 0;

                UpdateFillTarget();
            }
        }

        /// <summary>
        /// Viewの状態を初期化します。
        /// </summary>
        protected override void InitView()
        {
            base.InitView();

            columnView = null;
            if (view != null)
            {
                columnView = view as ColumnGaugeViewBase;
                UpdateHandle();
            }
        }

        /// <summary>
        /// Viewの状態を更新します。
        /// </summary>
        /// <param name="value">ゲージの値</param>
        protected override void UpdateView(float value)
        {
            base.UpdateView(value);
            UpdateHandle();
        }

        /// <summary>
        /// ハンドルの状態を更新します。
        /// </summary>
        /// <param name="isCurrent">現在の値相当の位置にするかどうか</param>
        protected void UpdateHandle(bool isCurrent = true)
        {
            if (columnView != null)
            {
                RectTransform handle = handleProperty.objectReferenceValue as RectTransform;
                if (handle == null)
                {
                    return;
                }
                handle.position = isCurrent ? columnView.CurrentPoint : columnView.TerminatePoint;
            }
        }

#endregion

#region override unity methods

        protected override void OnEnable()
        {
            handleProperty = serializedObject.FindProperty("handle");

            base.OnEnable();
            
            // 単体選択時のみ
            // 複数選択時に実行すると参照関係がおかしくなるのでやらない
            if (targets.Length == 1)
            {
                serializedObject.Update();

                SerializedProperty buttonProperty = serializedObject.FindProperty("button");
                buttonProperty.objectReferenceValue = gauge.GetComponent<BasicButton>();

                serializedObject.ApplyModifiedProperties();
            }
        }

#endregion
    }
}
