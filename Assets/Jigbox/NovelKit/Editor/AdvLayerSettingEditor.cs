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

namespace Jigbox.NovelKit
{
    [CustomEditor(typeof(AdvLayerSetting), true)]
    public sealed class AdvLayerSettingEditor : Editor
    {
#region properties

        /// <summary>前面プレーンのレイヤーのプロパティ</summary>
        SerializedProperty planeFrontLayerProperty;
        /// <summary>前面プレーンの写すカメラのレイヤーカリングのプロパティ</summary>
        SerializedProperty frontCameraCulilngMaskProperty;

        /// <summary>背面プレーンのレイヤーのプロパティ</summary>
        SerializedProperty planeBackLayerProperty;
        /// <summary>背面プレーンを写すカメラのレイヤーカリングのプロパティ</summary>
        SerializedProperty backCameraCullingMaskProperty;

        /// <summary>拡張用プレーンのレイヤーのプロパティ</summary>
        SerializedProperty planeOptionalLayerProperty;
        /// <summary>拡張用プレーンを写すカメラのレイヤーカリングのレイヤーのプロパティ</summary>
        SerializedProperty optionalCameraCullingMaskProperty;

        /// <summary>UIのレイヤーのプロパティ</summary>
        SerializedProperty uiLayerProperty;
        /// <summary>UIを写すカメラのレイヤーカリングのプロパティ</summary>
        SerializedProperty uiCameraCullingMaskProperty;

        /// <summary>拡張用補助カメラのレイヤーのプロパティ</summary>
        SerializedProperty subCameraLayerProperty;
        /// <summary>拡張用補助カメラのレイヤーカリングのプロパティ</summary>
        SerializedProperty subCameraCullingMaskProperty;

        /// <summary>レイヤーの一覧</summary>
        string[] layers;

#endregion

#region protected methods
        
        /// <summary>
        /// レイヤーを編集します。
        /// </summary>
        /// <param name="label">ラベル</param>
        /// <param name="property">SerializedProperty</param>
        void EditLayer(string label, SerializedProperty property)
        {
            int index = LayerMask.NameToLayer(property.stringValue);
            int selected = EditorGUILayout.LayerField(label, index);
            if (selected != index)
            {
                property.stringValue = LayerMask.LayerToName(selected);
            }
        }

        /// <summary>
        /// レイヤーマスクを編集します。
        /// </summary>
        /// <param name="label">ラベル</param>
        /// <param name="property">SerializedProperty</param>
        void EditMask(string label, SerializedProperty property)
        {
            int editMask = GetEditMask(property.intValue);
            int edited = EditorGUILayout.MaskField(label, editMask, layers);
            if (edited != editMask)
            {
                property.intValue = GetMask(edited);
            }
        }

        /// <summary>
        /// 実際のマスクから編集用のマスクを取得します。
        /// </summary>
        /// <param name="mask">実際のマスク</param>
        /// <returns></returns>
        int GetEditMask(int mask)
        {
            int editMask = 0;
            for (int i = 0; i < layers.Length; ++i)
            {
                if ((mask & 1 << LayerMask.NameToLayer(layers[i])) > 0)
                {
                    editMask |= 1 << i;
                }
            }
            return editMask;
        }

        /// <summary>
        /// 編集用のマスクから実際のマスクを取得します。
        /// </summary>
        /// <param name="editMask">編集用のマスク</param>
        /// <returns></returns>
        int GetMask(int editMask)
        {
            int mask = 0;
            for (int i = 0; i < layers.Length; ++i)
            {
                if ((editMask & 1 << i) > 0)
                {
                    mask |= 1 << LayerMask.NameToLayer(layers[i]);
                }
            }
            return mask;
        }

#endregion

#region override unity methods

        void OnEnable()
        {
            planeFrontLayerProperty = serializedObject.FindProperty("planeFrontLayer");
            frontCameraCulilngMaskProperty = serializedObject.FindProperty("frontCameraCulilngMask");

            planeBackLayerProperty = serializedObject.FindProperty("planeBackLayer");
            backCameraCullingMaskProperty = serializedObject.FindProperty("backCameraCullingMask");

            planeOptionalLayerProperty = serializedObject.FindProperty("planeOptionalLayer");
            optionalCameraCullingMaskProperty = serializedObject.FindProperty("optionalCameraCullingMask");

            uiLayerProperty = serializedObject.FindProperty("uiLayer");
            uiCameraCullingMaskProperty = serializedObject.FindProperty("uiCameraCullingMask");

            subCameraLayerProperty = serializedObject.FindProperty("subCameraLayer");
            subCameraCullingMaskProperty = serializedObject.FindProperty("subCameraCullingMask");

            layers = UnityEditorInternal.InternalEditorUtility.layers;
        }

        public override void OnInspectorGUI()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            EditLayer("UI Layer", uiLayerProperty);
            EditMask("UI Camera Culling Mask", uiCameraCullingMaskProperty);

            EditorGUILayout.Space();

            EditLayer("Plane Front Layer", planeFrontLayerProperty);
            EditMask("Front Camera Culling Mask", frontCameraCulilngMaskProperty);

            EditorGUILayout.Space();

            EditLayer("Plane Back Layer", planeBackLayerProperty);
            EditMask("Back Camera Culling Mask", backCameraCullingMaskProperty);

            EditorGUILayout.Space();

            EditLayer("Plane Optional Layer", planeOptionalLayerProperty);
            EditMask("Optional Camera Culling Mask", optionalCameraCullingMaskProperty);

            EditorGUILayout.Space();

            EditLayer("Sub Camera Layer", subCameraLayerProperty);
            EditMask("Sub Camera Culling Mask", subCameraCullingMaskProperty);

            serializedObject.ApplyModifiedProperties();

            EditorUtils.EditorUtilsTools.RegisterUndo("Edit Layer Setting", GUI.changed, target);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
