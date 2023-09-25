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
using Jigbox.TextView;

namespace Jigbox.Examples
{
    /// <summary>
    /// <c>DefaultInlineImageProvider</c> にテクチャーの同期ロード機能を足した
    /// <c>IInlineImageProvider</c> です。
    /// </summary>
    public class TestSceneInlineImageProvider : MonoBehaviour, IInlineImageProvider
    {
        const string SpritePrefix = "Sprites/";
        const string TexturePrefix = "Textures/";

        readonly IInlineImageProvider delegated = new DefaultInlineImageProvider();

        public void Request(string identifier, IInlineImageReceiver receiver)
        {
            if (identifier.StartsWith(SpritePrefix))
            {
                delegated.Request(identifier, receiver);
                return;
            }
            if (identifier.StartsWith(TexturePrefix))
            {
                var texture = Resources.Load<Texture>(identifier);

                if (texture != null)
                {
                    receiver.Send(identifier, texture);
                    return;
                }
            }
            receiver.SendError(identifier, "no such resources: " + identifier);
        }

        public void Cancel(string identifier, IInlineImageReceiver receiver)
        {
#if UNITY_EDITOR
            Debug.LogWarning("同期読み込みしているので、キャンセルできるタイミングはないため、何もしない");
#endif
        }
    }
}

