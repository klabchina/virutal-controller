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
    [CustomEditor(typeof(SliderScaler))]
    public class SliderScalerEditor : Editor
    {
#region override unity methods

        protected virtual void OnEnable()
        {
            // 単体選択時のみ
            // 複数選択時に実行すると参照関係がおかしくなるのでやらない
            if (targets.Length == 1)
            {
                SliderScaler scaler = target as SliderScaler;
                serializedObject.Update();

                SerializedProperty targetProperty = serializedObject.FindProperty("target");
                targetProperty.objectReferenceValue = scaler.GetComponent<Slider>();

                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            // Inspector上で表示する必要があるものがないので、
            // 何も表示しないようにする
        }

#endregion
    }
}
