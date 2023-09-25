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

using UnityEditor;

namespace Jigbox.Components
{
    [CustomEditor(typeof(TextViewOverflowExtension), true)]
    public class TextViewOverflowExtensionEditor : Editor
    {
#region properties
        /// <summary>同じオブジェクトについているTextView</summary>
        protected TextView textView;
#endregion
#region override unity methods

        protected virtual void OnEnable()
        {
            var textViewOverflowExtension = target as TextViewOverflowExtension;
            textView = textViewOverflowExtension.GetComponent<TextView>();
        }
        
        public override void OnInspectorGUI()
        {
            // TextViewがついてない
            if (textView == null)
            {
                EditorGUILayout.HelpBox("TextViewと同じGameObjectにアタッチして下さい。", MessageType.Warning);
                return;
            }
            else
            {
                EditorGUILayout.HelpBox("プロパティはTextViewに移動しました。\n編集はTextViewのInspector上で行って下さい。", MessageType.Info);
            }

            base.OnInspectorGUI();
        }
#endregion
    }
}
