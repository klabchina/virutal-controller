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

#if UNITY_EDITOR || NOVELKIT_EDITOR
using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    public interface IAdvScriptDataCompressor
    {
#region public methods

        /// <summary>
        /// 渡された文字列情報を圧縮したバイト列に変換して返します。
        /// </summary>
        /// <param name="stream">文字列情報</param>
        /// <param name="srcLength">元のバイト列の長さ</param>
        /// <returns>圧縮後のバイト列</returns>
        byte[] Compression(List<string> stream, ref int srcLength);

#endregion
    }
}
#endif
