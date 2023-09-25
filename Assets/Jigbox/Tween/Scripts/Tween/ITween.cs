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

using System;
using UnityEngine;

namespace Jigbox.Tween
{
    public interface ITween : IPeriodicMovement
    {
#region configuration properties

        /// <summary>
        /// トゥイーンの緩急（イージング）の種類を示します
        /// </summary>
        /// <remarks><seealso cref="Tween.MotionType.Linear"/>の場合、緩急が無い線形補間になります</remarks>
        /// <value>The type of the motion.</value>
        MotionType MotionType { get; set; }

        /// <summary>
        /// トゥイーンの緩急（イージング）の付き方を示します
        /// </summary>
        /// <value>The type of the easing.</value>
        EasingType EasingType { get; set; }

        /// <summary>
        /// トゥイーンの緩急（イージング）の付き方を示します
        /// </summary>
        /// <value>Custom type of easing.</value>
        AnimationCurve CustomMotion { get; set; }

#endregion

#region configuration utility methods

        /// <summary>
        /// トゥイーンの変化の緩急を設定します
        /// </summary>
        /// <returns>ITweenインスタンス</returns>
        /// <param name="motionType">Motion type.</param>
        /// <param name="easingType">Easing type.</param>
        ITween EasingWith(MotionType motionType, EasingType easingType);

        /// <summary>
        /// トゥイーンの変化の緩急を設定します
        /// </summary>
        /// <returns>The with.</returns>
        /// <param name="motionType">Motion type.</param>
        ITween EasingWith(MotionType motionType);

        /// <summary>
        /// トゥイーンの変化の緩急を設定します
        /// </summary>
        /// <returns>The with.</returns>
        /// <param name="easingType">Easing type.</param>
        ITween EasingWith(EasingType easingType);

#endregion

#region control method

        /// <summary>
        /// Tweenの状態を強制的に特定の状態に設定します。
        /// </summary>
        /// <param name="force">Tweenが開始してからの経過時間</param>
        void Rewind(float force = 0.0f);

#endregion
    }

    public interface ITween<T> : ITween, IPeriodicMovement<T, ITween<T>>
    {
#region event handler methods

        /// <summary>
        /// Tweenの状態を強制的に指定した際のコールバックを設定します。
        /// </summary>
        /// <param name="callback">Tweenの状態を強制的に指定した際のコールバック</param>
        /// <returns></returns>
        ITween<T> OnRewind(Action<ITween<T>> callback);

        /// <summary>
        /// Tweenの状態を強制的に指定した際のコールバックを破棄します。
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        ITween<T> RemoveOnRewind(Action<ITween<T>> callback);

        /// <summary>
        /// Tweenの状態を強制的に指定した際のコールバックを全て破棄します。
        /// </summary>
        /// <returns></returns>
        ITween<T> RemoveAllOnRewind();

#endregion

#region configuration properties

        /// <summary>
        /// トゥイーンの開始時にとるべき値を示します
        /// </summary>
        /// <value>The begin.</value>
        T Begin { get; set; }

        /// <summary>
        /// トゥイーンの終了時にとるべき値を示します
        /// </summary>
        /// <value>The final.</value>
        T Final { get; set; }

        /// <summary>
        /// トゥイーンの開始から終了までの変化量を示します
        /// </summary>
        /// <value>The change.</value>
        T Change { get; set; }

#endregion

#region configuration utility methods

        /// <summary>
        /// トゥイーンの変化の緩急を設定します
        /// </summary>
        /// <returns>ITweenインスタンス</returns>
        /// <param name="motionType">緩急の種類</param>
        /// <param name="easingType">緩急の入り抜きの指定</param>
        new ITween<T> EasingWith(MotionType motionType, EasingType easingType);

        /// <summary>
        /// トゥイーンの変化の緩急を設定します
        /// </summary>
        /// <returns>The with.</returns>
        /// <param name="motionType">Motion type.</param>
        new ITween<T> EasingWith(MotionType motionType);

        /// <summary>
        /// トゥイーンの変化の緩急を設定します
        /// </summary>
        /// <returns>The with.</returns>
        /// <param name="easingType">Easing type.</param>
        new ITween<T> EasingWith(EasingType easingType);

        /// <summary>
        /// ユーザー定義のイージング関数を元にトゥイーンの変化の緩急を設定します
        /// </summary>
        /// <returns>The with.</returns>
        /// <param name="easingFunc">Easing func.</param>
        ITween<T> EasingWith(Func<float, float> easingFunc);

        /// <summary>
        /// トゥイーンに開始値、終了値、変化時間を設定します
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="begin">Begin.</param>
        /// <param name="final">Final.</param>
        /// <param name="duration">Duration.</param>
        ITween<T> FromTo(T begin, T final, float duration);

        /// <summary>
        /// トゥイーンに終了値と変化時間を設定します
        /// </summary>
        /// <param name="final">Final.</param>
        /// <param name="duration">Duration.</param>
        ITween<T> To(T final, float duration);

        /// <summary>
        /// トゥイーンに開始値と変化量、変化時間を設定します
        /// </summary>
        /// <param name="begin">Begin.</param>
        /// <param name="change">Change.</param>
        /// <param name="duration">Duration.</param>
        ITween<T> Toward(T begin, T change, float duration);

        /// <summary>
        /// トゥイーンに変化量、変化時間を設定します
        /// </summary>
        /// <param name="change">Change.</param>
        /// <param name="duration">Duration.</param>
        ITween<T> Toward(T change, float duration);

        /// <summary>
        /// 開始値と終了値を入れ替えます
        /// </summary>
        /// <returns>ITweenインスタンス</returns>
        ITween<T> PingPong();

        /// <summary>
        /// 開始値と終了値を入れ替えます
        /// </summary>
        /// <returns>ITweenインスタンス</returns>
        ITween<T> Yoyo();

#endregion
    }
}
