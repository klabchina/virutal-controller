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

namespace Jigbox.NovelKit
{
    public class AdvFadeManager : MonoBehaviour
    {
#region properties
        
        /// <summary>UIより前面のフェード制御コンポーネント</summary>
        [SerializeField]
        protected AdvFadeTransitionController front;

        /// <summary>UIより背面のフェード制御コンポーネント</summary>
        [SerializeField]
        protected AdvFadeTransitionController back;

#endregion

#region public methods

        /// <summary>
        /// フェードを開始します。
        /// </summary>
        /// <param name="time">時間</param>
        /// <param name="color">フェードの色</param>
        /// <param name="isFadeIn">フェードインかどうか</param>
        /// <param name="isFront">UIより前面かどうか</param>
        public void BeginFade(float time, Color color, bool isFadeIn, bool isFront)
        {
            AdvFadeTransitionController target = isFront ? front : back;

            target.SetColor(color);
            if (isFadeIn)
            {
                target.FadeIn(time);
            }
            else
            {
                target.FadeOut(time);
            }
        }

        /// <summary>
        /// 初期化前の状態に戻します。
        /// </summary>
        public void Uninit()
        {
            front.FadeIn(0.0f);
            back.FadeIn(0.0f);
        }

#endregion
    }
}
