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
using UnityEngine.UI;

namespace Jigbox.TextView
{
    public class InlineImageReciver : IInlineImageReceiver
    {
        readonly string identifier;

        readonly InlineImageGameObjectPool pool;

        readonly Action<GameObject> onInitializeGameObject;

        readonly Action onCompleteLayout;

        readonly Action<string> onError;

        /// <summary><c>Send*()</c> が複数回呼ばれたときは無視するようにするため。</summary>
        bool firstSendCalling = true;

        public InlineImageReciver(
            string identifier,
            InlineImageGameObjectPool pool,
            Action<GameObject> onInitializeGameObject,
            Action onCompleteLayout,
            Action<string> onError)
        {
            this.identifier = identifier;
            this.pool = pool;
            this.onInitializeGameObject = onInitializeGameObject;
            this.onCompleteLayout = onCompleteLayout;
            this.onError = onError;
        }

        /// <summary>
        /// Spriteのロード終了時に設定されたコールバックを呼びます
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="sprite"></param>
        public virtual void Send(string identifier, Sprite sprite)
        {
            if (!Validate(identifier))
            {
                return;
            }

            var image = pool.Take<Image>();
            image.sprite = sprite;
            onInitializeGameObject(image.gameObject);

            firstSendCalling = false;
            onCompleteLayout();
        }

        /// <summary>
        /// Textureのロード終了時に設定されたコールバックを呼びます
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="texture"></param>
        public virtual void Send(string identifier, Texture texture)
        {
            if (!Validate(identifier))
            {
                return;
            }

            var rawImage = pool.Take<RawImage>();
            rawImage.texture = texture;
            onInitializeGameObject(rawImage.gameObject);

            firstSendCalling = false;
            onCompleteLayout();
        }

        /// <summary>
        /// ロード中にエラーが発生した場合のコールバックを呼びます
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="errorMessage"></param>
        public virtual void SendError(string identifier, string errorMessage)
        {
            if (!Validate(identifier))
            {
                return;
            }

            firstSendCalling = false;
            onError(errorMessage);
        }

        protected bool Validate(string identifier)
        {
            if (identifier != this.identifier)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat(
                    "unexpected identifier: actual: {0}, expected: {1}",
                    identifier,
                    this.identifier
                );
#endif
                return false;
            }

            if (!firstSendCalling)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat(
                    "this is not the first calling Send*() function: identifier: {0}",
                    identifier
                );
#endif
                return false;
            }

            return true;
        }
    }
}
