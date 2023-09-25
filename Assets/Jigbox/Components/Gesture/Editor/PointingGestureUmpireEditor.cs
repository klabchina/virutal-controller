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
using Jigbox.Gesture;

namespace Jigbox.Components
{
    [CustomEditor(typeof(PointingGestureUmpire), true)]
    public class PointingGestureUmpireEditor : GestureUmpireEditor<PointingGestureType, PointingGestureEventData, PointingGestureEventHandler>
    {
#region properties

        /// <summary>長押しと判断する時間のプロパティ</summary>
        protected SerializedProperty longPressTimeProperty;

        /// <summary>ダブルクリックとしてみなされる時間のプロパティ</summary>
        protected SerializedProperty doubleClickPermissiveTimeProperty;

        /// <summary>ポインタが動いていないと判断する際の誤差範囲のプロパティ</summary>
        protected SerializedProperty toleranceProperty;

        /// <summary>ポインタが動いていないと判断する際の誤差範囲のプロパティ</summary>
        protected SerializedProperty doubleClickToleranceProperty;

#endregion

#region protected methods

        /// <summary>
        /// プロパティの編集用の表示を行います。
        /// </summary>
        protected override void DrawProperties()
        {
            base.DrawProperties();

            serializedObject.Update();

            FloatField("Long Press Time", longPressTimeProperty);
            FloatField("Double Click Permissive Time", doubleClickPermissiveTimeProperty);
            FloatField("Tolerance", toleranceProperty);
            FloatField("Double Click Tolerance", doubleClickToleranceProperty);

            serializedObject.ApplyModifiedProperties();
        }

#endregion

#region override unity methods

        protected override void OnEnable()
        {
            base.OnEnable();
            
            longPressTimeProperty = serializedObject.FindProperty("longPressTime");
            doubleClickPermissiveTimeProperty = serializedObject.FindProperty("doubleClickPermissiveTime");
            toleranceProperty = serializedObject.FindProperty("tolerance");
            doubleClickToleranceProperty = serializedObject.FindProperty("doubleClickTolerance");
        }

#endregion
    }
}
