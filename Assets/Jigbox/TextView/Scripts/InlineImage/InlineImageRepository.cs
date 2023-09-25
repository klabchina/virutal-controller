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

using System.Collections.Generic;
using UnityEngine;

namespace Jigbox.TextView
{
    /// <summary>
    /// インライン画像の読み込みを効率化するためのプロキシクラスです。
    ///
    /// 現在は以下の機能を実装しています。
    /// - 複数のIInlineImageReceiverから同じ画像がリクエストされた場合、1つのリクエストに集約する
    ///     1. receiverAがImage1をリクエスト -> リクエスト元を記録。originalProvider.Requestを呼び出し
    ///     2. receiverBがImage1をリクエスト -> リクエスト元を記録
    ///     3. originalProviderから画像を受信 -> receiverA.Send および receiverB.Sendを呼び出し
    ///
    /// プロジェクトからのフィードバック次第で以下の機能も実装可能です。
    /// - 一度受信した画像をキャッシュする
    /// </summary>
    public class InlineImageRepository : IInlineImageProvider, IInlineImageReceiver
    {
#region internal class

        /// <summary>
        /// リクエストのidentifierの一意性を決定するKey.
        /// </summary>
        /// <remarks>
        /// インライン画像のidentifierに空文字を入れても
        /// Dictionary.ContainsKeyが呼べるようにこのようにしています。
        /// </remarks>
        internal class RequestKey
        {
            public string Identifier { get; private set; }

            public RequestKey(string identifier)
            {
                this.Identifier = identifier;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }
                var other = obj as RequestKey;
                if (other == null)
                {
                    return false;
                }
                return other.Identifier == this.Identifier;
            }

            public override int GetHashCode()
            {
                if (string.IsNullOrEmpty(this.Identifier))
                {
                    return 0;
                }
                return this.Identifier.GetHashCode();
            }
        }

#endregion internal class

#region Fields

        private IInlineImageProvider originalProvider;
        private Dictionary<RequestKey, HashSet<IInlineImageReceiver>> requests;

#endregion

#region Constructor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        ///
        /// <param name="provider">実際に画像の提供を行うプロバイダ</param>
        public InlineImageRepository(IInlineImageProvider provider)
        {
            this.originalProvider = provider;
            this.requests = new Dictionary<RequestKey, HashSet<IInlineImageReceiver>>();
        }

#endregion

#region IInlineImageProvider

        public void Request(string identifier, IInlineImageReceiver receiver)
        {
            var key = new RequestKey(identifier);
            if (this.requests.ContainsKey(key))
            {
                var receivers = this.requests[key];
                receivers.Add(receiver);
            }
            else
            {
                var receivers = new HashSet<IInlineImageReceiver> { receiver };
                this.requests.Add(key, receivers);

                this.originalProvider.Request(key.Identifier, this);
            }
        }

        public void Cancel(string identifier, IInlineImageReceiver receiver)
        {
            var key = new RequestKey(identifier);
            if (this.requests.ContainsKey(key) == false)
            {
                return;
            }

            var receivers = this.requests[key];
            if (receivers.Contains(receiver) == false)
            {
                return;
            }

            receivers.Remove(receiver);
            if (receivers.Count == 0)
            {
                this.originalProvider.Cancel(key.Identifier, this);
                this.requests.Remove(key);
            }
        }

#endregion

#region IInlineImageReceiver

        public void Send(string identifier, Sprite sprite)
        {
            var key = new RequestKey(identifier);
            if (this.requests.ContainsKey(key) == false)
            {
                return;
            }

            foreach (var receiver in this.requests[key])
            {
                receiver.Send(key.Identifier, sprite);
            }

            this.requests.Remove(key);
        }

        public void Send(string identifier, Texture texture)
        {
            var key = new RequestKey(identifier);
            if (this.requests.ContainsKey(key) == false)
            {
                return;
            }

            foreach (var receiver in this.requests[key])
            {
                receiver.Send(key.Identifier, texture);
            }

            this.requests.Remove(key);
        }

        public void SendError(string identifier, string errorMessage)
        {
            var key = new RequestKey(identifier);
            if (this.requests.ContainsKey(key) == false)
            {
                return;
            }

            foreach (var receiver in this.requests[key])
            {
                receiver.SendError(key.Identifier, errorMessage);
            }

            this.requests.Remove(key);
        }

#endregion
    }
}
