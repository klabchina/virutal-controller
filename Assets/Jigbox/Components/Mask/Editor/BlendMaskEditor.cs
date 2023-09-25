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
using UnityEditorInternal;
using System;
using System.Reflection;
using Jigbox.EditorUtils;

namespace Jigbox.Components
{
    [CustomEditor(typeof(BlendMask), true)]
    public class BlendMaskEditor : Editor
    {
#region constants

        /// <summary>情報を更新した際に呼び出すメソッドを取得するためのフラグ情報</summary>
        protected static readonly BindingFlags MethodBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

#endregion

#region properties

        /// <summary>マスクコンポーネント</summary>
        protected BlendMask mask;

        /// <summary>マテリアルを設定するためのメソッド情報</summary>
        protected MethodInfo setTargetMaterialMethod;

        /// <summary>マスキングが有効かどうか</summary>
        protected SerializedProperty isEnableProperty;

        /// <summary>マスキングに使用するマテリアル</summary>
        protected SerializedProperty maskMaterialProperty;

        /// <summary>マスキングを行う際に基準となるオブジェクト</summary>
        protected SerializedProperty maskedTargetRootProperty;

        /// <summary>自動的にマスキング対象の更新を行うかどうか</summary>
        protected SerializedProperty isAutoUpdateMaskedTargetsProperty;

        /// <summary>マスキング用のマテリアルを更新する際に同時に更新するマテリアル</summary>
        protected SerializedProperty updateTogetherMaterialsProperty;

        /// <summary>確認用にマスクを表示するかどうか</summary>
        protected SerializedProperty showMaskProperty;

        /// <summary>マスキング用のマテリアルを更新する際に同時に更新するマテリアルの編集用リスト</summary>
        protected ReorderableList updateTogetherMaterialList;

        /// <summary>マテリアルを設定する必要があるかどうか</summary>
        protected bool needSetMaterial = false;

        /// <summary>マテリアルを更新する必要があるかどうか</summary>
        protected bool needUpdateMaterial = false;

        /// <summary>Undo用のDescription</summary>
        protected virtual string UndoDescription { get { return "Edit Mask"; } }

        /// <summary>合成したGUI変更フラグ。</summary>
        protected bool compositedGUIChanged;

#endregion

#region protected methods

        /// <summary>
        /// 各プロパティを表示します。
        /// </summary>
        protected virtual void DrawSerializedProperties()
        {
            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;
            EditorGUILayout.PropertyField(isEnableProperty);
            needSetMaterial |= Application.isPlaying && GUI.changed;

            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;
            EditorGUILayout.PropertyField(maskMaterialProperty);
            // マテリアル自体を変更した場合は、マテリアルの設定も必要
            bool isUpdateMaskMaterial = Application.isPlaying && GUI.changed;
            needUpdateMaterial |= isUpdateMaskMaterial;
            needSetMaterial |= isUpdateMaskMaterial;

            RectTransform lastTargetRoot = maskedTargetRootProperty.objectReferenceValue as RectTransform;
            EditorGUILayout.PropertyField(maskedTargetRootProperty);
            RectTransform currentTargetRoot = maskedTargetRootProperty.objectReferenceValue as RectTransform;
            if (lastTargetRoot != currentTargetRoot && currentTargetRoot != null)
            {
                // 子オブジェクト以外は対象として設定された場合に不適切である可能性があるのでブロックする
                bool isChild = false;
                Transform parent = currentTargetRoot.parent;
                while (parent != null)
                {
                    if (mask.transform == parent)
                    {
                        isChild = true;
                        break;
                    }
                    parent = parent.parent;
                }

                if (!isChild)
                {
                    maskedTargetRootProperty.objectReferenceValue = null;
                    Debug.LogWarning("Masked Target Root is valid only for child objects!");
                }
            }
            
            EditorGUILayout.PropertyField(isAutoUpdateMaskedTargetsProperty);

            EditorGUILayout.PropertyField(showMaskProperty);

            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;
            updateTogetherMaterialList.DoLayoutList();
            needUpdateMaterial |= Application.isPlaying && GUI.changed;
        }

        /// <summary>
        /// マスクコンポーネントを更新します。
        /// </summary>
        protected virtual void UpdateComponent()
        {
            if (needSetMaterial)
            {
                setTargetMaterialMethod.Invoke(mask, null);
                needSetMaterial = false;
            }

            if (needUpdateMaterial)
            {
                mask.UpdateMaskMaterial();
                mask.rectTransform.hasChanged = true;
                needUpdateMaterial = false;
            }
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            mask = target as BlendMask;

            Type maskType = mask.GetType();

            setTargetMaterialMethod = maskType.GetMethod("SetTargetMaterial", MethodBindingFlags, null, new Type[0], null);

            isEnableProperty = serializedObject.FindProperty("isEnable");
            maskMaterialProperty = serializedObject.FindProperty("maskMaterial");
            maskedTargetRootProperty = serializedObject.FindProperty("maskedTargetRoot");
            isAutoUpdateMaskedTargetsProperty = serializedObject.FindProperty("isAutoUpdateMaskedTargets");
            updateTogetherMaterialsProperty = serializedObject.FindProperty("updateTogetherMaterials");

            showMaskProperty = serializedObject.FindProperty("showMask");

            updateTogetherMaterialList = new ReorderableList(serializedObject, updateTogetherMaterialsProperty);
            updateTogetherMaterialList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Update Together Materials");
            updateTogetherMaterialList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty material = updateTogetherMaterialsProperty.GetArrayElementAtIndex(index);
                UnityEngine.Object edited = EditorGUI.ObjectField(rect, "Material " + (index + 1), material.objectReferenceValue, typeof(Material), true);
                if (material.objectReferenceValue != edited)
                {
                    material.objectReferenceValue = edited;
                }
            };

            // マスク自体に当たり判定は存在しない
            mask.raycastTarget = false;
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            compositedGUIChanged = false;
            
            serializedObject.Update();

            DrawSerializedProperties();

            serializedObject.ApplyModifiedProperties();

            // serializedObject.ApplyModifiedProperties()では、実際に値は適用されていないので、
            // 編集が終わってからマスクの状態を更新する
            UpdateComponent();

            compositedGUIChanged |= GUI.changed;
            EditorUtilsTools.RegisterUndo(UndoDescription, compositedGUIChanged, target);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
