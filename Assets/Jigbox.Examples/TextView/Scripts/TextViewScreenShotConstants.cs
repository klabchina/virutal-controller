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

namespace Jigbox.Examples
{
    public static class TextViewScreenShotConstants
    {
#region constants

        // スクリーンショットのサイズを変更する場合は、DevTools側にも同名のcsファイルが存在するので
        // 同じ内容の変更を適用すること(DevToolsはJigboxの内部的な開発支援ツール等なので、外部に出力されないため)

        /// <summary>スクリーンショットの保存ディレクトリ 1の保存用キー</summary>
        public static readonly string ScreenShotDirectoryKey1 = "TEXTVIEW_SCREENSHOT_DIRECTORY_1";

        /// <summary>スクリーンショットの保存ディレクトリ 2の保存用キー</summary>
        public static readonly string ScreenShotDirectoryKey2 = "TEXTVIEW_SCREENSHOT_DIRECTORY_2";

        /// <summary>不一致だったスクリーンショットのリストの保存用キー</summary>
        public static readonly string ScreenShotMismatchListKey = "TEXTVIEW_SCREENSHOT_MISMATCH_LIST";

        /// <summary>スクリーンショットのテクスチャの横幅</summary>
        public static readonly int TextureWidth = 970;

        /// <summary>スクリーンショットのテクスチャの縦幅</summary>
        public static readonly int TextureHeight = 340;

        /// <summary>不一致だったスクリーンショットのリストの区切り文字</summary>
        public static readonly char ScreenShotMismatchListDelimiter = ';';

#endregion
    }
}
