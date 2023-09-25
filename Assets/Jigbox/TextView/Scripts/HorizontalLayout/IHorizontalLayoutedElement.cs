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

namespace Jigbox.TextView.HorizontalLayout
{
    public interface IHorizontalLayoutedElement
    {
        /// <summary>
        /// 配置するグリフ情報
        /// </summary>
        /// <value>The glyph placement.</value>
        GlyphPlacement GlyphPlacement { get; }

        /// <summary>
        /// 配置するグリフのx軸のオフセット
        /// </summary>
        /// <value>The offset x.</value>
        float OffsetX { get; }

        /// <summary>
        /// 配置するグリフのy軸のオフセット
        /// </summary>
        /// <value>The offset y.</value>
        float OffsetY { get; }

        /// <summary>
        /// TextLine単位で見た時のこの文字のX位置
        /// </summary>
        float TextLineOffsetX { get; set; }

        /// <summary>
        /// TextLine単位で見た時のこの文字のY位置
        /// </summary>
        float TextLineOffsetY { get; set; }

        /// <summary>
        /// アラインメント設定でTextAlign.JustifyもしくはTextAlign.JustifyAllを設定している時に発生するオフセット位置
        /// </summary>
        float JustifyShiftOffsetX { get; set; }

        /// <summary>
        /// 配置するグリフのx軸の最小値
        /// </summary>
        /// <value>The x minimum.</value>
        float xMin { get; }

        /// <summary>
        /// 配置するグリフのx軸の最大値
        /// </summary>
        /// <value>The x max.</value>
        float xMax { get; }

        /// <summary>
        /// 配置するグリフのy軸の最小値
        /// </summary>
        /// <value>The y minimum.</value>
        float yMin { get; }

        /// <summary>
        /// 配置するグリフのy軸の最大値
        /// </summary>
        /// <value>The y max.</value>
        float yMax { get; }

        /// <summary>
        /// 特殊な補正を加えたy軸の最小値
        ///
        /// タグによるy軸のオフセットが
        /// マイナス(画面上方向への補正)の場合、そのまま補正を行う
        /// プラス(画面下方向への補正)の場合、補正を行わない
        ///
        /// 本来はどちらも同じように扱うべきであるが、過去経緯により異なる扱いとなっている
        ///
        /// 実際のグリフ配置座標を取得したい場合はこのプロパティではなくyMinを参照すること
        /// </summary>
        float yMinOffsetSpecialAdjusted { get; }

        /// <summary>
        /// 特殊な補正を加えたy軸の最大値
        ///
        /// タグによるy軸のオフセットが
        /// マイナス(画面上方向への補正)の場合、そのまま補正を行う
        /// プラス(画面下方向への補正)の場合、補正を行わない
        ///
        /// 本来はどちらも同じように扱うべきであるが、過去経緯により異なる扱いとなっている
        ///
        /// 実際のグリフ配置座標を取得したい場合はこのプロパティではなくyMaxを参照すること
        /// </summary>
        float yMaxOffsetSpecialAdjusted { get; }
    }
}
