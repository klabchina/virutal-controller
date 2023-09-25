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

using Jigbox.Delegatable;
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// バルーンコンポーネントクラス
    /// </summary>
    [DisallowMultipleComponent]
    public class Balloon : MonoBehaviour, IBalloonTransitionHandler, IBalloonLayerHandler
    {
#region inner class

        /// <summary>
        /// バルーンのコールバッククラス
        /// </summary>
        public class BalloonDelegate : EventDelegate<Balloon>
        {
            public BalloonDelegate(Callback callback) : base(callback)
            {
            }
        }

#endregion

#region serializefield

        /// <summary>
        /// バルーンで使用するモデルクラス
        /// </summary>
        [SerializeField]
        [HideInInspector]
        BalloonModel balloonModel = new BalloonModel();

        /// <summary>
        /// バルーンのトランジションクラス
        /// </summary>
        [SerializeField]
        [HideInInspector]
        BalloonTransitionBase balloonTransition;

        /// <summary>
        /// バルーンレイヤーを管理するクラス
        /// </summary>
        [SerializeField]
        [HideInInspector]
        BalloonLayerExtension balloonLayerExtension;

        /// <summary>
        /// Start()メソッドで自動でOpen()メソッドを呼ぶかどうか
        /// </summary>
        [SerializeField]
        [HideInInspector]
        bool openOnStart = false;

        /// <summary>
        /// OnCompleteCloseのタイミングでGameObjectを削除するかどうか
        /// </summary>
        [SerializeField]
        [HideInInspector]
        bool destroyOnClose;

        /// <summary>
        /// Open()の開始時に呼ばれる
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onBeginOpen = new DelegatableList();

        /// <summary>
        /// Open()が終了した時に呼ばれる
        /// トランジションがある場合はトランジション終了時に呼ばれる
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onCompleteOpen = new DelegatableList();

        /// <summary>
        /// Close()の開始時に呼ばれる
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onBeginClose = new DelegatableList();

        /// <summary>
        /// Close()が終了した時に呼ばれる
        /// トランジションがある場合はトランジション終了時に呼ばれる
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onCompleteClose = new DelegatableList();

#endregion

#region property

        /// <summary>
        /// モデルクラスへのアクセサ
        /// </summary>
        protected virtual BalloonModel BalloonModel
        {
            get { return balloonModel; }
        }

        /// <summary>
        /// トランジションクラスのアクセサ
        /// </summary>
        protected virtual BalloonTransitionBase BalloonTransition
        {
            get
            {
                if (balloonTransition == null)
                {
                    balloonTransition = GetComponent<BalloonTransitionBase>();
                }

                if (balloonTransition != null)
                {
                    balloonTransition.SetHandler(this);
                }

                return balloonTransition;
            }
        }

        /// <summary>
        /// バルーンレイヤーを管理するクラスのアクセサ
        /// </summary>
        protected virtual BalloonLayerExtension BalloonLayerExtension
        {
            get
            {
                if (balloonLayerExtension == null)
                {
                    balloonLayerExtension = GetComponent<BalloonLayerExtension>();
                }

                if (balloonLayerExtension != null)
                {
                    balloonLayerExtension.SetHandler(this);
                }

                return balloonLayerExtension;
            }
        }

        /// <summary>
        /// バルーンの位置を計算するクラスのアクセサ
        /// </summary>
        public virtual BalloonLayoutCalculator BalloonLayoutCalculator
        {
            get { return BalloonModel.Calculator; }
            set { BalloonModel.Calculator = value; }
        }

        /// <summary>
        /// バルーンの親になるLayerクラスのアクセサ
        /// </summary>
        public virtual BalloonLayer BalloonLayer
        {
            get { return BalloonLayerExtension.BalloonLayer; }
            set { BalloonLayerExtension.BalloonLayer = value; }
        }

        /// <summary>
        /// InputBlockerを使用するかどうかのアクセサ
        /// </summary>
        public virtual bool UseInputBlocker
        {
            get { return BalloonLayerExtension.UseInputBlocker; }
            set { BalloonLayerExtension.UseInputBlocker = value; }
        }

        /// <summary>
        /// バルーン領域外をタップした時にバルーンを閉じるかどうかのアクセサ
        /// </summary>
        public virtual bool CloseOnClickInputBlocker
        {
            get { return BalloonLayerExtension.CloseOnClickInputBlocker; }
            set { BalloonLayerExtension.CloseOnClickInputBlocker = value; }
        }

        /// <summary>
        /// モデルのAutoLayoutAreaへのプロパティ参照
        /// </summary>
        public virtual RectTransform AutoLayoutArea
        {
            get { return BalloonModel.AutoLayoutArea; }
            set { BalloonModel.AutoLayoutArea = value; }
        }

        /// <summary>
        /// モデルのBalloonLayoutへのプロパティ参照
        /// バルーンのレイアウト位置をどこにするか決める列挙
        /// </summary>
        public virtual BalloonLayout BalloonLayout
        {
            get { return BalloonModel.BalloonLayout; }
            set { BalloonModel.BalloonLayout = value; }
        }

        /// <summary>
        /// モデルのBalloonLayoutPositionRateへのプロパティ参照
        /// バルーンの基準座標と隣り合うContentの辺のどこを中心とするか
        /// </summary>
        public virtual float BalloonLayoutPositionRate
        {
            get { return BalloonModel.BalloonLayoutPositionRate; }
            set { BalloonModel.BalloonLayoutPositionRate = value; }
        }

        /// <summary>
        /// モデルのSpacingへのプロパティ参照
        /// 基準座標とバルーンの間にどれだけ空間を空けるか
        /// </summary>
        public virtual float Spacing
        {
            get { return BalloonModel.Spacing; }
            set { BalloonModel.Spacing = value; }
        }

        /// <summary>
        /// モデルのBasePositionへのプロパティ参照
        /// </summary>
        public virtual Vector2 BasePosition
        {
            get { return BalloonModel.BasePosition; }
            set { BalloonModel.BasePosition = value; }
        }

        /// <summary>
        /// モデルのBasePositionRectTransformへのプロパティ参照
        /// </summary>
        public virtual RectTransform BasePositionRectTransform
        {
            get { return BalloonModel.BasePositionRectTransform; }
            set { BalloonModel.BasePositionRectTransform = value; }
        }

        /// <summary>
        /// バルーンを閉じた際にこのオブジェクトを削除するかどうかのアクセサ
        /// </summary>
        public virtual bool DestroyOnClose
        {
            get { return destroyOnClose; }
            set { destroyOnClose = value; }
        }

        /// <summary>
        /// Open()の開始時に呼ばれるデリゲートのアクセサ
        /// </summary>
        public DelegatableList OnBeginOpenDelegates
        {
            get { return onBeginOpen; }
        }

        /// <summary>
        /// Open()が終了した時に呼ばれるデリゲートのアクセサ
        /// </summary>
        public DelegatableList OnCompleteOpenDelegates
        {
            get { return onCompleteOpen; }
        }

        /// <summary>
        /// Close()の開始時に呼ばれるデリゲートのアクセサ
        /// </summary>
        public DelegatableList OnBeginCloseDelegates
        {
            get { return onBeginClose; }
        }

        /// <summary>
        /// Close()が終了した時に呼ばれるデリゲートのアクセサ
        /// </summary>
        public DelegatableList OnCompleteCloseDelegates
        {
            get { return onCompleteClose; }
        }

#endregion

#region protected field

        /// <summary>
        /// バルーンが開いている最中かどうか
        /// </summary>
        protected bool IsWorkingOpen = false;

        /// <summary>
        /// バルーンが閉じている最中かどうか
        /// </summary>
        protected bool IsWorkingClose = false;

#endregion

#region protected methods

        /// <summary>
        /// トランジションのOpenを行う
        /// トランジションが存在しない場合、コールバックを発火する
        /// </summary>
        protected virtual void OpenTransition()
        {
            if (BalloonTransition != null)
            {
                BalloonTransition.OpenTransition();
            }
            else
            {
                OnBeginOpen();
                OnCompleteOpen();
            }
        }

        /// <summary>
        /// トランジションのCloseを行う
        /// トランジションが存在しない場合、コールバックを発火する
        /// </summary>
        protected virtual void CloseTransition()
        {
            if (BalloonTransition != null)
            {
                BalloonTransition.CloseTransition();
            }
            else
            {
                OnBeginClose();
                OnCompleteClose();
            }
        }

        /// <summary>
        /// レイヤーのOpenを行う
        /// </summary>
        protected virtual void OpenLayer()
        {
            if (BalloonLayerExtension != null)
            {
                BalloonLayerExtension.OpenBalloonLayer();
            }
        }

        /// <summary>
        /// レイヤーのCloseを行う
        /// </summary>
        protected virtual void CloseLayer()
        {
            if (BalloonLayerExtension != null)
            {
                BalloonLayerExtension.CloseBalloonLayer();
            }
        }

#endregion

#region public methods

        /// <summary>
        /// バルーンとして表示するオブジェクトを変更する
        /// </summary>
        /// <param name="balloonContent">表示するオブジェクト</param>
        /// <param name="isRelayout">バルーンのレイアウトを更新するかどうか</param>
        public virtual void SetBalloonContent(RectTransform balloonContent, bool isRelayout = true)
        {
            BalloonModel.BalloonContent = balloonContent;

            if (isRelayout)
            {
                UpdateLayout();
            }
        }

        /// <summary>
        /// バルーンの位置を更新する
        /// </summary>
        public virtual void UpdateLayout()
        {
            BalloonModel.UpdateContentPosition();
        }

        /// <summary>
        /// バルーンを表示する
        /// </summary>
        public virtual void Open()
        {
            if (IsWorkingOpen)
            {
                return;
            }

            IsWorkingOpen = true;

            OpenLayer();
            UpdateLayout();
            OpenTransition();
        }

        /// <summary>
        /// バルーンを閉じる
        /// </summary>
        public virtual void Close()
        {
            if (IsWorkingClose)
            {
                return;
            }

            IsWorkingClose = true;

            CloseTransition();
        }

        /// <summary>
        /// Open()開始時に呼ばれる
        /// </summary>
        public virtual void OnBeginOpen()
        {
            if (!IsWorkingOpen)
            {
                return;
            }

            if (onBeginOpen.Count > 0)
            {
                onBeginOpen.Execute(this);
            }
        }

        /// <summary>
        /// Open()終了時に呼ばれる
        /// </summary>
        public virtual void OnCompleteOpen()
        {
            if (!IsWorkingOpen)
            {
                return;
            }

            IsWorkingOpen = false;

            if (onCompleteOpen.Count > 0)
            {
                onCompleteOpen.Execute(this);
            }
        }

        /// <summary>
        /// Close()開始時に呼ばれる
        /// </summary>
        public virtual void OnBeginClose()
        {
            if (!IsWorkingClose)
            {
                return;
            }

            if (onBeginClose.Count > 0)
            {
                onBeginClose.Execute(this);
            }
        }

        /// <summary>
        /// Close()終了時に呼ばれる
        /// </summary>
        public virtual void OnCompleteClose()
        {
            if (!IsWorkingClose)
            {
                return;
            }

            CloseLayer();

            IsWorkingClose = false;

            if (onCompleteClose.Count > 0)
            {
                onCompleteClose.Execute(this);
            }

            if (DestroyOnClose)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 外部からOpen()開始時のコールバックを設定する
        /// </summary>
        /// <param name="callback">コールバック</param>
        public virtual void AddBeginOpenEvent(BalloonDelegate.Callback callback)
        {
            onBeginOpen.Add(new BalloonDelegate(callback));
        }

        /// <summary>
        /// 外部からOpen()終了時のコールバックを設定する
        /// </summary>
        /// <param name="callback">コールバック</param>
        public virtual void AddCompleteOpenEvent(BalloonDelegate.Callback callback)
        {
            onCompleteOpen.Add(new BalloonDelegate(callback));
        }

        /// <summary>
        /// 外部からClose()開始時のコールバックを設定する
        /// </summary>
        /// <param name="callback">コールバック</param>
        public virtual void AddBeginCloseEvent(BalloonDelegate.Callback callback)
        {
            onBeginClose.Add(new BalloonDelegate(callback));
        }

        /// <summary>
        /// 外部からClose()終了時のコールバックを設定する
        /// </summary>
        /// <param name="callback">コールバック</param>
        public virtual void AddCompleteCloseEvent(BalloonDelegate.Callback callback)
        {
            onCompleteClose.Add(new BalloonDelegate(callback));
        }

#endregion

#region override unity methods

        protected virtual void Start()
        {
            if (openOnStart)
            {
                Open();
            }
        }

#endregion
    }
}
