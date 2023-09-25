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

using Jigbox.Collection;
using Jigbox.EditorUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Components
{
    [CustomEditor(typeof(VirtualCollectionHeaderFooter), true)]
    public class VirtualCollectionHeaderFooterEditor : Editor
    {
        /// <summary>VirtualCollectionHeaderFooter</summary>
        protected VirtualCollectionHeaderFooter headerFooter;

        /// <summary>IVirtualCollectionView</summary>
        IVirtualCollectionView view;

        /// <summary>ヘッダの外周の余白のプロパティ</summary>
        protected SerializedPadding headerPaddingProperty;

        /// <summary>フッタの外周の余白のプロパティ</summary>
        protected SerializedPadding footerPaddingProperty;

        protected virtual void OnEnable()
        {
            headerFooter = target as VirtualCollectionHeaderFooter;
            view = headerFooter.gameObject.GetComponent<IVirtualCollectionView>();
            // IVirtualCollectionViewがとれないときはなにもしない
            if (view == null)
            {
                return;
            }
            // ヘッダとフッタ用paddingプロパティを取得
            headerPaddingProperty = new SerializedPadding(serializedObject.FindProperty("headerPadding"));
            footerPaddingProperty = new SerializedPadding(serializedObject.FindProperty("footerPadding"));
            SetProperty();
        }

        public override void OnInspectorGUI()
        {
            if (view == null)
            {
                EditorGUILayout.HelpBox("ListViewかTileViewと同じGameObjectにアタッチして下さい。", MessageType.Warning);
                return;
            }

            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            // Header Padding
            headerPaddingProperty.EditProperty("Header Padding");

            GUILayout.Space(10f);

            // Footer Padding
            footerPaddingProperty.EditProperty("Footer Padding");

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                view.RelayoutHeaderFooter();
            }

            EditorUtilsTools.RegisterUndo("Edit VirtualCollectionHeaderFooter", GUI.changed, targets);
            
            EditorGUI.EndChangeCheck();
        }

        protected void SetProperty()
        {
            serializedObject.Update();
            // ScrollRectからContentの参照をとりだしcontentTransフィールドへつける
            var scrollRect = headerFooter.GetComponent<ScrollRect>();
            var contentTrans = scrollRect.content;
            var serializedContentTrans = serializedObject.FindProperty("contentTrans");
            serializedContentTrans.objectReferenceValue = contentTrans;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
