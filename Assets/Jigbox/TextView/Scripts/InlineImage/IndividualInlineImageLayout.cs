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

namespace Jigbox.TextView
{
    /// <summary>
    /// インライン画像1枚のロードと、それ用の <c>GameObject</c> の初期化を行なう。
    /// </summary>
    public class IndividualInlineImageLayout
    {
        /// <summary>
        /// インライン画像の識別子。
        /// </summary>
        public readonly string Identifier;

        readonly IInlineImageProvider provider;

        readonly IInlineImageReceiver receiver;

        /// <param name="provider">インライン画像を実際にロードする <c>IInlineImageProvider</c></param>
        /// <param name="identifier">インライン画像の識別子</param>
        /// <param name="pool">インライン画像用 <c>GameObject</c> のプール</param>
        /// <param name="onInitializeGameObject">インライン画像が設定された <c>GameObject</c> の、その他の初期化処理</param>
        /// <param name="onCompleteLayout">ロードと <c>GameObject</c> の初期化が完了したときに通知を受ける処理</param>
        /// <param name="onError">ロードに失敗した場合に通知を受ける処理</param>
        public IndividualInlineImageLayout(
            IInlineImageProvider provider,
            string identifier,
            InlineImageGameObjectPool pool,
            Action<GameObject> onInitializeGameObject,
            Action onCompleteLayout,
            Action<string> onError)
        {
#if UNITY_EDITOR || JIGBOX_DEBUG
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentNullException("identifier");
            }
            if (pool == null)
            {
                throw new ArgumentNullException("pool");
            }
            if (onInitializeGameObject == null)
            {
                throw new ArgumentNullException("onInitializeGameObject");
            }
            if (onCompleteLayout == null)
            {
                throw new ArgumentNullException("onCompleteLayout");
            }
            if (onError == null)
            {
                throw new ArgumentNullException("onError");
            }
#endif

            Identifier = identifier;
            this.provider = provider;
            receiver = new InlineImageReciver(identifier, pool, onInitializeGameObject, onCompleteLayout, onError);
        }

        /// <summary>
        /// ロードを開始する。
        /// </summary>
        public void Load()
        {
            provider.Request(Identifier, receiver);
        }

        /// <summary>
        /// ロードを中断する。
        /// </summary>
        public void Cancel()
        {
            provider.Cancel(Identifier, receiver);
        }
    }
}
