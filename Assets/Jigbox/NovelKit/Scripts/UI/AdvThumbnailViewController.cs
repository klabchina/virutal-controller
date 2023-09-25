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
using UnityEngine.UI;

namespace Jigbox.NovelKit
{
    public class AdvThumbnailViewController : MonoBehaviour
    {
#region properties

        /// <summary>
        /// サムネイル表示用のImage
        /// </summary>
        [SerializeField]
        protected Image image;

#endregion

#region public methods

        /// <summary>
        /// リソースを読み込みます。
        /// </summary>
        /// <param name="loader">Loader</param>
        /// <param name="resourcePath">リソースのパス</param>
        public virtual void LoadResource(IAdvResourceLoader loader, string resourcePath)
        {
            Sprite sprite = loader.Load<Sprite>(resourcePath);

            if (sprite == null)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("対象のリソースが存在しない、もしくは、Spriteではありません。"
                    + "\nパス : " + resourcePath);
#endif
                return;
            }

            image.sprite = sprite;
        }

        /// <summary>
        /// サムネイルを表示します。
        /// </summary>
        public virtual void Show()
        {
            if (!image.gameObject.activeSelf)
            {
                image.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// サムネイルを非表示します。
        /// </summary>
        public virtual void Hide()
        {
            if (image.gameObject.activeSelf)
            {
                image.gameObject.SetActive(false);
            }
        }

#endregion
    }
}
