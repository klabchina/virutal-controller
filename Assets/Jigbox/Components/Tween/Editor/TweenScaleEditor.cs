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

namespace Jigbox.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TweenScale))]
    public class TweenScaleEditor : TweenVector3Editor
    {
#region properties

        /// <summary>Tweenコンポーネント</summary>
        TweenScale tweenTarget;

        /// <summary>リセットボタンに表示する文言</summary>
        protected override string ResetButtonLabel { get { return "S"; } }

        /// <summary>リセット時の値</summary>
        protected override Vector3 ResetValue { get { return Vector3.one; } }

#endregion

#region protected methods

        /// <summary>
        /// 通常のプロパティのデフォルト設定を行います。
        /// </summary>
        protected override void InitProperties()
        {
            tweenTarget.Tween.Begin = tweenTarget.transform.localScale;
            tweenTarget.Tween.Final = tweenTarget.transform.localScale;
        }

#endregion

#region override unity methods

        protected override void OnEnable()
        {
            tweenTarget = target as TweenScale;

            base.OnEnable();
        }

#endregion
    }
}
