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

namespace Jigbox.TextView
{
    /// <summary>
    /// JigboxデフォルトのInlineImageProvider実装です。
    /// Resourcesからのスプライト(シンプル・マルチプル)の同期読み込みを行います。
    ///
    /// identiferには以下のどちらかを渡してください
    /// - シンプルスプライトのパス
    /// - マルチプルスプライトのパス#スプライトの名前
    /// </summary>
    public class DefaultInlineImageProvider : IInlineImageProvider
    {
        private const string MultipleSpriteAnchor = "#";

        public void Request(string identifier, IInlineImageReceiver receiver)
        {
            if (identifier.Contains(MultipleSpriteAnchor))
            {
                var splitedIdentifer = identifier.Split(MultipleSpriteAnchor.ToCharArray(), 2);
                this.LoadMultipleSprite(identifier, receiver, splitedIdentifer[0], splitedIdentifer[1]);
            }
            else
            {
                this.LoadSimpleSprite(identifier, receiver);
            }
        }

        private void LoadMultipleSprite(
            string identifier,
            IInlineImageReceiver receiver,
            string resourceName,
            string spriteName)
        {
            // NOTE: resourceNameが見付からない場合、nullではなく空配列が返る
            var sprites = Resources.LoadAll<Sprite>(resourceName);

            foreach (var sprite in sprites)
            {
                if (sprite.name == spriteName)
                {
                    receiver.Send(identifier, sprite);
                    return;
                }
            }

            receiver.SendError(identifier,
                "Multiple Sprite " + resourceName + " that contains " + spriteName + " is not found in Resources.");
        }

        private void LoadSimpleSprite(string identifier, IInlineImageReceiver receiver)
        {
            var sprite = Resources.Load<Sprite>(identifier);

            if (sprite == null)
            {
                receiver.SendError(identifier, "Sprite " + identifier + " is not found in Resources.");
            }
            else
            {
                receiver.Send(identifier, sprite);
            }
        }

        public void Cancel(string identifier, IInlineImageReceiver receiver)
        {
#if UNITY_EDITOR
            Debug.LogWarning("同期読み込みしているので、キャンセルできるタイミングはないため、何もしない");
#endif
        }
    }
}
