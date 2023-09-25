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

namespace Jigbox.Tween
{
    public interface IMovement
    {
#region configuration properties

        /// <summary>開始までに遅延時間を秒単位で指定することができます</summary>
        /// <value>遅延時間</value>
        float Delay { get; set; }

        /// <summary>値の変化を<see cref="UnityEngine.Time.timeScale"/>に依存するか否かを指定することができます</summary>
        /// <value><c>true</c> if this instance is follow time scale; otherwise, <c>false</c>.</value>
        bool FollowTimeScale { get; set; }

#endregion

#region state properties

        /// <summary><see cref="Delay"/>を含め<see cref="Start"/>からの経過時間（秒）</summary>
        float DeltaTime { get; }

        /// <summary>現在の状態を示します</summary>
        TweenState State { get; }

#endregion

#region control methods

        /// <summary>有効になってる際に、フレームごとに前フレーム描画からの差分時間を累積し、状態を更新させます</summary>
        /// <remarks><see cref="Tween.TweenWorker"/>以外からこのAPIは呼び出さないようにしてください</remarks>
        /// <param name="deltaTime">seconds from last update</param>
        void Update(float deltaTime);

        /// <summary>初期状態から稼働させます</summary>
        void Start();

        /// <summary>休止状態から稼働させます</summary>
        void Resume();

        /// <summary>一時停止させます</summary>
        void Pause();

        /// <summary>最終状態にして終了します</summary>
        void Complete();

        /// <summary>内部の進行状態に関わらず、動作を終了させます</summary>
        void Kill();

#endregion
    }

    public interface IMovement<out TValue, TDerived> : IMovement where TDerived : IMovement<TValue, TDerived>
    {
#region state property

        /// <summary>参照時点での状態に応じた値を示します</summary>
        /// <value>値</value>
        TValue Value { get; }

#endregion

#region value method

        /// <summary>引数に与えられた経過時間での値を計算します</summary>
        /// <returns>経過時間での値</returns>
        /// <param name="time">秒単位の時間</param>
        TValue ValueAt(float time);

#endregion

#region event handler methods

        /// <summary>開始時のイベントハンドラーを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        TDerived OnStart(Action<TDerived> callback);

        /// <summary>開始時のイベントハンドラーを削除します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        TDerived RemoveOnStart(Action<TDerived> callback);

        /// <summary>開始時のイベントハンドラーを全て削除します</summary>
        /// <returns><c>this</c></returns>
        TDerived RemoveAllOnStart();

        /// <summary>値が変化した際のイベントハンドラーを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        TDerived OnUpdate(Action<TDerived> callback);

        /// <summary>値が変化した際のイベントハンドラーを破棄します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        TDerived RemoveOnUpdate(Action<TDerived> callback);

        /// <summary>値が変化した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        TDerived RemoveAllOnUpdate();

        /// <summary>完了時のイベントハンドラを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        TDerived OnComplete(Action<TDerived> callback);

        /// <summary>ループしていなしにおいて完了した際のイベントハンドラーを破棄します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        TDerived RemoveOnComplete(Action<TDerived> callback);

        /// <summary>ループしていなしにおいて完了した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        TDerived RemoveAllOnComplete();

        /// <summary>外部から強制的に終了した場合のイベントハンドラを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        TDerived OnKill(Action<TDerived> callback);

        /// <summary>強制的に停止した際のイベントハンドラーを破棄します</summary>
        /// <returns><c>this</c></returns>
        TDerived RemoveOnKill(Action<TDerived> callback);

        /// <summary>強制的に停止した際のイベントハンドラーを全て破棄します</summary>
        /// <returns></returns>
        TDerived RemoveAllOnKill();

        /// <summary>トゥイーンの開始、再開時のイベントハンドラを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        TDerived OnResume(Action<TDerived> callback);

        /// <summary>再開時のイベントハンドラーを削除します。</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        TDerived RemoveOnResume(Action<TDerived> callback);

        /// <summary>再開時のイベントハンドラーを全て削除します。</summary>
        /// <returns><c>this</c></returns>
        TDerived RemoveAllOnResume();

        /// <summary>一時停止時のイベントハンドラーを追加します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        TDerived OnPause(Action<TDerived> callback);

        /// <summary>一時停止時のイベントハンドラーを削除します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        TDerived RemoveOnPause(Action<TDerived> callback);

        /// <summary>一時停止時のイベントハンドラーを全て削除します。</summary>
        /// <returns><c>this</c></returns>
        TDerived RemoveAllOnPause();

        /// <summary>開始してから値の変化が始まるまでの遅延が終了した際のイベントハンドラーを設定します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        TDerived OnEndDelay(Action<TDerived> callback);

        /// <summary>開始してから値の変化が始まるまでの遅延が終了した際のイベントハンドラーを破棄します</summary>
        /// <param name="callback">イベントハンドラー</param>
        /// <returns><c>this</c></returns>
        TDerived RemoveOnEndDelay(Action<TDerived> callback);

        /// <summary>開始してから値の変化が始まるまでの遅延が終了した際のイベントハンドラーを全て破棄します</summary>
        /// <returns><c>this</c></returns>
        TDerived RemoveAllOnEndDelay();

#endregion
    }
}
