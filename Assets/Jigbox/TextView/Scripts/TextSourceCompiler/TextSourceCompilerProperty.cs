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
    public class TextSourceCompilerProperty
    {
#region properties

        /// <summary>TextView</summary>
        protected Components.TextView textView;

        /// <summary>フォント</summary>
        public virtual Font Font { get { return textView.Font; } }

        /// <summary>フォントサイズ</summary>
        public virtual int FontSize { get { return textView.FontSize; } }

        /// <summary>文字と文字の間隔</summary>
        public virtual float CharacterSpacing { get { return textView.CharacterSpacing; } }

        /// <summary>文字の色</summary>
        public virtual Color Color { get { return textView.color; } }

        /// <summary>GlyphのX軸倍率</summary>
        public virtual float GlyphScaleX { get { return textView.GlyphScaleX; } }

        /// <summary>禁則処理ルール</summary>
        public virtual TextLineBreakRule LineBreakRule { get { return textView.LineBreakRule; } }

        /// <summary>デフォルトのフォントスタイル</summary>
        public virtual FontStyle DefaultFontStyle { get { return textView.DefaultFontStyle; } }

        /// <summary>デフォルトのレターケース</summary>
        public virtual LetterCase LetterCase { get { return textView.LetterCase; } }

        /// <summary>
        /// ALIGNタグから指定されたアライメントがある場合に格納しておくプロパティ
        /// 本来CompilerPropertyはTextViewの値を参照させるためだけに使用させたいので、例外な対応用プロパティになる
        /// </summary>
        public TextAlign? ModifyAlign;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="textView">TextView</param>
        public TextSourceCompilerProperty(Components.TextView textView)
        {
            this.textView = textView;
        }
#endregion
    }
}
