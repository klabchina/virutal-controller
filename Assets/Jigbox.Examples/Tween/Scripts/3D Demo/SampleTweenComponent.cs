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
using Jigbox.Tween;

/// <summary>
/// シンプルな<c>GameObject</c>の移動をインスペクタで設定し、トゥイーンアニメーションで制御するサンプルです
/// </summary>
public class SampleTweenComponent : MonoBehaviour
{
#region Fields

    /// <summary>
    /// 予めインスペクタからトゥイーンを設定する場合はシリアライズフィールドで保持させ、エディタ上で設定しておきます
    /// </summary>
    [SerializeField]
    TweenVector3 tween;

    /// <summary>
    /// インスタンス生成後に自動的にトゥイーンを稼働させるフラグの例です
    /// </summary>
    [SerializeField]
    bool isAutoPlay;

#endregion

#region Properties

    /// <summary>
    /// 外部のオブジェクトから制御する為のトゥイーンへのアクセサです
    /// </summary>
    /// <value>The tween.</value>
    public ITween<Vector3> Tween { get { return tween ?? (tween = new TweenVector3()); } }

    /// <summary>
    /// トゥイーンがインスタンス生成後から稼働するか否かを示します
    /// </summary>
    /// <value><c>true</c> if this instance is auto play; otherwise, <c>false</c>.</value>
    public bool IsAutoPlay
    {
        get { return isAutoPlay; }
        set { isAutoPlay = value; }
    }

#endregion

#region Unity Method

    void Start()
    {
        // トゥイーンの開始時の初期化に行いたいイベントをコールバックで渡します
        Tween.OnStart(tween =>
        {
            if (transform)
            {
                // トゥイーンの設定から、transformを合わせたい場合は Startイベント でこのようにします
                transform.localPosition = tween.Begin;
            }
        });

        // トゥイーンが稼働している間に、Updateイベントで同期する処理をコールバックで渡します
        Tween.OnUpdate(tween =>
        {
            // トゥイーンの変化とGameObject, MonoBehaviourの状態を同期させる処理を記述します
            if (transform)
            {
                transform.localPosition = tween.Value;
            }
        });

        // インスタンス生成後すぐに起動させる場合は、StartイベントでTween.Start()を呼びます
        if (IsAutoPlay)
        {
            Tween.Start();
        }
    }

    void Update()
    {
        // Startイベントで ITween.OnUpdate() にイベントハンドラを追加していた場合
        // Updateイベントで特にトゥイーンをコントロールさせる必要はありません
    }


    // 以下の実装は、このGameObjectのアクティベートをtrue/falseと切り替える可能性がある場合のコードです
    // Tweenインスタンスの状態変化は、TweenWorkerインスタンスが更新をさせていくので
    // 仮にこのスクリプトがアタッチされたGameObjectがdisableとなっても状態更新は継続されます

    void OnEnable()
    {
        Tween.Resume();
    }

    void OnDisable()
    {
        Tween.Pause();
    }

#endregion
}
