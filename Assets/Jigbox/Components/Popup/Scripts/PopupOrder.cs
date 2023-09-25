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
using System;

namespace Jigbox.Components
{
    public abstract class PopupOrder : IInstanceProvider<PopupBase>
    {
#region properties

        /// <summary>ポップアップの初期化時に呼び出されるコールバック</summary>
        public Action<PopupBase> OnInit { get; set; }

        /// <summary>ポップアップを開くトランジションを開始する際のコールバック</summary>
        public Action<PopupBase> OnBeginOpen { get; set; }

        /// <summary>ポップアップを開くトランジションが完了した際のコールバック</summary>
        public Action<PopupBase> OnCompleteOpen { get; set; }

        /// <summary>ポップアップを閉じるトランジションを開始する際のコールバック</summary>
        public Action<PopupBase> OnBeginClose { get; set; }

        /// <summary>ポップアップを閉じるトランジションが完了した際のコールバック</summary>
        public Action<PopupBase> OnCompleteClose { get; set; }

        /// <summary>エスケープキーが(Androidのバックキー)が押された際のコールバック</summary>
        public Action<PopupBase> OnEscape { get; set; }

#endregion

#region public methods

        /// <summary>
        /// ポップアップを生成します。
        /// </summary>
        /// <returns></returns>
        public abstract PopupBase Generate();

#endregion

#region protected methods

        /// <summary>
        /// パスからポップアップを読み込みます。
        /// </summary>
        /// <param name="path">パス</param>
        /// <returns></returns>
        protected virtual PopupBase LoadFromPath(string path)
        {
            GameObject obj = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            return obj.GetComponent<PopupBase>();
        }

#endregion
    }
}
