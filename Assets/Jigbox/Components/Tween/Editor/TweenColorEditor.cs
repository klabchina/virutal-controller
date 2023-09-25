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
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TweenColor))]
    public class TweenColorEditor : TweenBaseEditor
    {
#region properties

        /// <summary>Tweenコンポーネント</summary>
        TweenColor tweenTarget;

        /// <summary>色の変更対象の種類</summary>
        SerializedProperty targetType;

#endregion

#region protected methods
        
        /// <summary>
        /// SerializedPropertyのデフォルト設定を行います。
        /// </summary>
        protected override void InitSerializedProperties()
        {
            // uGUI関連のオブジェクトの場合、デフォルトの対象をuGUIコンポーネントにする
            RectTransform transform = tweenTarget.transform as RectTransform;
            if (transform != null)
            {
                targetType.enumValueIndex = (int) TweenColor.TargetType.Graphic;
            }
        }

        /// <summary>
        /// 通常のプロパティのデフォルト設定を行います。
        /// </summary>
        protected override void InitProperties()
        {
            Color color = Color.white;

            Graphic graphic = tweenTarget.GetComponent<Graphic>();
            if (graphic != null)
            {
                color = graphic.color;
            }
            else
            {
                Renderer renderer = tweenTarget.GetComponent<Renderer>();
                if (renderer != null && renderer.sharedMaterial != null)
                {
                    color = renderer.sharedMaterial.color;
                }
            }
            
            tweenTarget.Tween.Begin = color;
            tweenTarget.Tween.Final = color;
        }
        
        /// <summary>
        /// SerializedPropertyの編集用フィールドを表示します。
        /// </summary>
        protected override void DrawSerializedProperties()
        {
            base.DrawSerializedProperties();
            EditorGUILayout.PropertyField(targetType, new GUIContent("Target Type"));
        }

#endregion

#region override unity methods

        protected override void OnEnable()
        {
            tweenTarget = target as TweenColor;

            targetType = serializedObject.FindProperty("type");

            base.OnEnable();
        }

#endregion
    }
}
