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
    [CustomEditor(typeof(SwipeGestureUmpire), true)]
    public class SwipeGestureUmpireEditor : GestureUmpireEditor<SwipeGestureType, SwipeGestureEventData, SwipeGestureEventHandler>
    {
#region properties

        /// <summary>無効扱いとなる移動量のプロパティ</summary>
        protected SerializedProperty invalidMovementProperty;

#endregion

#region protected methods

        /// <summary>
        /// プロパティの編集用の表示を行います。
        /// </summary>
        protected override void DrawProperties()
        {
            base.DrawProperties();

            serializedObject.Update();

            FloatField("Invalid Movement", invalidMovementProperty);

            serializedObject.ApplyModifiedProperties();
        }

#endregion

#region override unity methods

        protected override void OnEnable()
        {
            base.OnEnable();

            invalidMovementProperty = serializedObject.FindProperty("invalidMovement");
        }

#endregion
    }
}
