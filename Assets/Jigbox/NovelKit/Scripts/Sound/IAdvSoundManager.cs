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

namespace Jigbox.NovelKit
{
    public interface IAdvSoundManager
    {
#region public methods

        /// <summary>
        /// サウンドを再生させます。
        /// </summary>
        /// <param name="sound">サウンド情報</param>
        void Post(string sound);

        /// <summary>
        /// サウンドを再生させます。
        /// </summary>
        /// <param name="sound">サウンド情報</param>
        /// <param name="endCallbak">終了コールバック</param>
        /// <param name="markerCallback">サウンド内にマーカーが含まれていた場合のコールバック</param>
        void Post(string sound, Action endCallbak, Action markerCallback);

        /// <summary>
        /// クリック時等、ボイスを停止させる際に呼び出されます。
        /// </summary>
        void StopVoice();

#endregion
    }
}
