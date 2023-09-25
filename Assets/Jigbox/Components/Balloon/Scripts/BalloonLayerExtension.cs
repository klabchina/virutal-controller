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

namespace Jigbox.Components
{
    /// <summary>
    /// バルーンレイヤーを使用する際のExtensionクラス
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Balloon))]
    public class BalloonLayerExtension : MonoBehaviour, IBalloonLayerProperty
    {
#region serializefield

        /// <summary>
        /// バルーンの親になるLayerクラスの参照
        /// </summary>
        [SerializeField]
        BalloonLayer balloonLayer;

        /// <summary>
        /// InputBlockerを使用するかどうか
        /// </summary>
        [SerializeField]
        bool useInputBlocker;

        /// <summary>
        /// InputBlockerを使用した際、バルーン領域外をタップした時にバルーンを閉じるかどうか
        /// </summary>
        [SerializeField]
        bool closeOnClickInputBlocker;

#endregion

#region property

        /// <summary>
        /// バルーンのRectTransform
        /// </summary>
        public virtual RectTransform BalloonRectTransform
        {
            get { return transform as RectTransform; }
        }

        /// <summary>
        /// バルーンの親になるLayerクラスのアクセサ
        /// </summary>
        public virtual BalloonLayer BalloonLayer
        {
            get { return balloonLayer; }
            set { balloonLayer = value; }
        }

        /// <summary>
        /// InputBlockerを使用するかどうかのアクセサ
        /// </summary>
        public virtual bool UseInputBlocker
        {
            get { return useInputBlocker; }
            set { useInputBlocker = value; }
        }

        /// <summary>
        /// バルーン領域外をタップした時にバルーンを閉じるかどうかのアクセサ
        /// </summary>
        public virtual bool CloseOnClickInputBlocker
        {
            get { return closeOnClickInputBlocker; }
            set { closeOnClickInputBlocker = value; }
        }

        /// <summary>
        /// バルーンのハンドラ
        /// </summary>
        protected IBalloonLayerHandler Handler;

#endregion

#region public methods

        /// <summary>
        /// バルーンのハンドラのセットを行う
        /// </summary>
        /// <param name="handler">ハンドラ</param>
        public virtual void SetHandler(IBalloonLayerHandler handler)
        {
            if (Handler != null)
            {
                return;
            }

            this.Handler = handler;
        }

        /// <summary>
        /// バルーンの表示を行う際に、親オブジェクトを指定して子供になる
        /// Canvas内での表示順番の制御を行う際に使用される
        /// </summary>
        public virtual void OpenBalloonLayer()
        {
            if (BalloonLayer != null)
            {
                BalloonLayer.SetProperty(this);
                BalloonLayer.OpenLayer();
            }
        }

        /// <summary>
        /// バルーンのクローズから呼ばれる
        /// レイヤーをクローズ状態にする
        /// </summary>
        public virtual void CloseBalloonLayer()
        {
            if (BalloonLayer != null)
            {
                BalloonLayer.CloseLayer();
            }
        }

        /// <summary>
        /// レイヤーから呼ばれる
        /// バルーンを閉じる
        /// </summary>
        public virtual void NoticeOnClose()
        {
            Handler.Close();
        }

#endregion
    }
}
