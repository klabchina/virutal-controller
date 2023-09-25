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
    [CustomEditor(typeof(TweenAlpha))]
    public class TweenAlphaEditor : TweenBaseEditor
    {
#region properties

        /// <summary>Tweenコンポーネント</summary>
        TweenAlpha tweenTarget;

        /// <summary>アルファの変更対象の種類</summary>
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
                targetType.enumValueIndex = (int) TweenAlpha.TargetType.Graphic;
            }

            // CanvasGroupが存在していれば、CanvasGroupを対象にする
            CanvasGroup canvasGroup = tweenTarget.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                targetType.enumValueIndex = (int) TweenAlpha.TargetType.CanvasGroup;
            }
        }

        /// <summary>
        /// 通常のプロパティのデフォルト設定を行います。
        /// </summary>
        protected override void InitProperties()
        {
            float alpha = 0.0f;

            CanvasGroup canvasGroup = tweenTarget.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                alpha = canvasGroup.alpha;
            }
            else
            {
                Graphic graphic = tweenTarget.GetComponent<Graphic>();
                if (graphic != null)
                {
                    alpha = graphic.color.a;
                }
            }

            if (alpha == 0.0f)
            {
                Renderer renderer = tweenTarget.GetComponent<Renderer>();
                if (renderer != null && renderer.sharedMaterial != null)
                {
                    alpha = renderer.sharedMaterial.color.a;
                }
            }

            tweenTarget.Tween.Begin = alpha;
            tweenTarget.Tween.Final = 1.0f;
        }

        /// <summary>
        /// Tweenの開始値のバリデーションを行います。
        /// </summary>
        protected override void ValidateBeginParameter()
        {
            if (begin.floatValue < 0.0f)
            {
                begin.floatValue = 0.0f;
            }
            else if (begin.floatValue > 1.0f)
            {
                begin.floatValue = 1.0f;
            }
        }

        /// <summary>
        /// Tweenの終了値のバリデーションを行います。
        /// </summary>
        protected override void ValidateFinalParameter()
        {
            if (final.floatValue < 0.0f)
            {
                final.floatValue = 0.0f;
            }
            else if (final.floatValue > 1.0f)
            {
                final.floatValue = 1.0f;
            }
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
            tweenTarget = target as TweenAlpha;

            targetType = serializedObject.FindProperty("type");

            base.OnEnable();
        }

#endregion
    }
}
