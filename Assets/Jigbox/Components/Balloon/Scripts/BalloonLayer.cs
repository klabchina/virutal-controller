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
    /// バルーンの表示に関連する動作を管理するコンポーネント
    /// </summary>
    [DisallowMultipleComponent]
    public class BalloonLayer : MonoBehaviour
    {
#region serializefield

        /// <summary>
        /// バルーンが閉じた際にこのオブジェクトを削除するかどうか
        /// </summary>
        [SerializeField]
        bool destroyOnClose;

        /// <summary>
        /// InputBlockerオブジェクトの参照
        /// </summary>
        [SerializeField]
        GameObject inputBlocker = null;

#endregion

#region property

        /// <summary>
        /// バルーンを閉じた際にこのオブジェクトを削除するかどうかのアクセサ
        /// </summary>
        public virtual bool DestroyOnClose
        {
            get { return destroyOnClose; }
            set { destroyOnClose = value; }
        }

        /// <summary>
        /// InputBlockerオブジェクトのアクセサ
        /// </summary>
        protected virtual GameObject InputBlocker
        {
            get { return inputBlocker; }
        }

        /// <summary>
        /// バルーンの表示に関連するプロパティ
        /// </summary>
        IBalloonLayerProperty balloonLayerProperty;

        /// <summary>
        /// バルーンの表示に関連するプロパティのアクセサ
        /// </summary>
        protected virtual IBalloonLayerProperty BalloonLayerProperty
        {
            get { return balloonLayerProperty; }
        }

#endregion

#region protected methods

        /// <summary>
        /// バルーンの表示領域外の入力をブロックするオブジェクトの状態を設定する
        /// </summary>
        protected virtual void SetActiveInputBlocker()
        {
            if (balloonLayerProperty.UseInputBlocker && InputBlocker != null)
            {
                InputBlocker.SetActive(true);
            }
            else if (InputBlocker != null)
            {
                InputBlocker.SetActive(false);
            }
        }

        /// <summary>
        /// バルーンの表示レイヤーを設定する
        /// </summary>
        protected virtual void SetSibling()
        {
            BalloonLayerProperty.BalloonRectTransform.SetParent(transform, true);
            BalloonLayerProperty.BalloonRectTransform.SetAsLastSibling();
        }

#endregion

#region public methods

        /// <summary>
        /// バルーンの表示に関連するプロパティを設定する
        /// </summary>
        /// <param name="balloonLayerProperty">プロパティ</param>
        public virtual void SetProperty(IBalloonLayerProperty balloonLayerProperty)
        {
#if UNITY_EDITOR
            // すでにバルーンが表示中の場合、複数のバルーンが同じレイヤーで出ているため警告を出す
            if (BalloonLayerProperty != null)
            {
                Debug.LogWarning("Already exists balloon in current layer !");
            }
#endif
            this.balloonLayerProperty = balloonLayerProperty;
        }

        /// <summary>
        /// バルーンを表示する際の処理を行う
        /// </summary>
        public virtual void OpenLayer()
        {
            gameObject.SetActive(true);

            SetActiveInputBlocker();
            SetSibling();
        }

        /// <summary>
        /// バルーンが閉じられた際の処理を行う
        /// </summary>
        public virtual void CloseLayer()
        {
            gameObject.SetActive(false);

            if (DestroyOnClose)
            {
                Destroy(gameObject);
            }
            else
            {
                balloonLayerProperty = null;
                InputBlocker.SetActive(false);
            }
        }

#endregion

#region callback

        /// <summary>
        /// 表示領域外がタップされた場合の処理
        /// </summary>
        [AuthorizedAccess]
        protected virtual void OnClickBlocker()
        {
            if (balloonLayerProperty.CloseOnClickInputBlocker)
            {
                balloonLayerProperty.NoticeOnClose();
            }
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            // InputBlockerを使用しない場合があるので最初は非アクティブにしておく
            if (InputBlocker != null)
            {
                InputBlocker.SetActive(false);
            }
        }

#endregion
    }
}
