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
    public class TextModifier : TextRun
    {
        public override int Length { get { return 0; } }

        /// <summary>
        /// 文字間隔
        /// </summary>
        /// <value>The spacing.</value>
        public float? Spacing { get; set; }

        /// <summary>
        /// ルビのオフセット位置
        /// </summary>
        /// <value>The spacing.</value>
        public float? RubyOffset { get; set; }

        /// <summary>
        /// ルビの文字カラー
        /// </summary>
        /// <value>The color of the ruby.</value>
        public Color? RubyColor { get; set; }

        /// <summary>
        /// ルビのフォントスケール
        /// </summary>
        /// <value>The color of the ruby.</value>
        public float? RubyFontScale { get; set; }

        /// <summary>
        /// ルビのフォントスタイル
        /// </summary>
        /// <value>The ruby font style.</value>
        public FontStyle? RubyFontStyle { get; set; }

        /// <summary>
        /// 本文のフォントサイズ
        /// </summary>
        /// <value>The size of the font.</value>
        public int? FontSize { get; set; }

        /// <summary>
        /// 本文のフォントスタイル
        /// </summary>
        /// <value>The size of the font.</value>
        public FontStyle? FontStyle { get; set; }

        /// <summary>
        /// 本文の文字カラー
        /// </summary>
        /// <value>The color.</value>
        public Color? Color { get; set; }
        
        /// <summary>
        /// 本文のレターケース
        /// </summary>
        /// <value>The letterCase.</value>
        public LetterCase? LetterCase { get; set; }
    }
}
