/**
 * Additional Language Utility Library
 * Copyright(c) 2018 KLab, Inc. All Rights Reserved.
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

namespace ArabicUtils
{
    public static class BracketLetter
    {
#region inner classes, enum, and structs

        // wiki : https://ja.wikipedia.org/wiki/%E6%8B%AC%E5%BC%A7

        /// <summary>
        /// 左括弧
        /// </summary>
        public enum Left
        {
            /// <summary>(</summary>
            Parenthesis = 0x0028,
            /// <summary>（</summary>
            FullParenthesis = 0xFF08,
            /// <summary>﹙</summary>
            SmallParenthesis = 0xFE59,
            /// <summary>❨</summary>
            MediumParenthesis = 0x2768,
            /// <summary>❪</summary>
            MediumFattenedParenthesis = 0x276A,

            /// <summary>⦅</summary>
            WhiteParenthesis = 0x2985,
            /// <summary>｟</summary>
            FullWhiteParenthesis = 0xFF5F,

            /// <summary>「</summary>
            CornerBracket = 0x300C,
            /// <summary>｢</summary>
            HalfCornerBracket = 0xFF62,

            /// <summary>『</summary>
            WhiteCornerBracket = 0x300E,

            /// <summary>[</summary>
            SquareBracket = 0x005B,
            /// <summary>［</summary>
            FullSquareBracket = 0xFF3B,

            /// <summary>〚</summary>
            WhiteSquareBracket = 0x301A,

            /// <summary>{</summary>
            CurlyBracket = 0x007B,
            /// <summary>｛</summary>
            FullCurlyBracket = 0xFF5B,
            /// <summary>﹛</summary>
            SmallCurlyBracket = 0xFE5B,
            /// <summary>❴</summary>
            MediumCurlyBracket = 0x2774,

            /// <summary>〔</summary>
            TortoiseShellBracket = 0x3014,
            /// <summary>❲</summary>
            TortoiseShellBracketOrnament = 0x2772,

            /// <summary>〘</summary>
            WhiteTortoiseShellBracket = 0x3018,

            /// <summary>〈</summary>
            AngleBracket = 0x3008,
            /// <summary>〈</summary>
            PointingAngleBracket = 0x2329,
            /// <summary>❬</summary>
            MediumPointingAngleBracket = 0x276C,
            /// <summary>❰</summary>
            HeavyPointingAngleBracket = 0x2770,

            /// <summary>《</summary>
            DoubleAngleBracket = 0x300A,

            /// <summary>«</summary>
            DoubleAngleQuotation = 0x00AB,
            /// <summary>‹</summary>
            SingleAngleQuotation = 0x2039,

            /// <summary>&lt;</summary>
            LessThanSign = 0x003C,

            /// <summary>【</summary>
            BlackLenticularBracket = 0x3010,
        }

        /// <summary>
        /// 右括弧
        /// </summary>
        public enum Right
        {
            /// <summary>)</summary>
            Parenthesis = 0x0029,
            /// <summary>）</summary>
            FullParenthesis = 0xFF09,
            /// <summary>﹚</summary>
            SmallParenthesis = 0xFE5A,
            /// <summary>❩</summary>
            MediumParenthesis = 0x2769,
            /// <summary>❫</summary>
            MediumFattenedParenthesis = 0x276B,

            /// <summary>⦆</summary>
            WhiteParenthesis = 0x2986,
            /// <summary>｠</summary>
            FullWhiteParenthesis = 0xFF60,

            /// <summary>」</summary>
            CornerBracket = 0x300D,
            /// <summary>｣</summary>
            HalfCornerBracket = 0xFF63,

            /// <summary>』</summary>
            WhiteCornerBracket = 0x300F,

            /// <summary>]</summary>
            SquareBracket = 0x005D,
            /// <summary>］</summary>
            FullSquareBracket = 0xFF3D,

            /// <summary>〛</summary>
            WhiteSquareBracket = 0x301B,

            /// <summary>}</summary>
            CurlyBracket = 0x007D,
            /// <summary>｝</summary>
            FullCurlyBracket = 0xFF5D,
            /// <summary>﹜</summary>
            SmallCurlyBracket = 0xFE5C,
            /// <summary>❵</summary>
            MediumCurlyBracket = 0x2775,

            /// <summary>〕</summary>
            TortoiseShellBracket = 0x3015,
            /// <summary>❳</summary>
            TortoiseShellBracketOrnament = 0x2773,

            /// <summary>〙</summary>
            WhiteTortoiseShellBracket = 0x3019,

            /// <summary>〉</summary>
            AngleBracket = 0x3009,
            /// <summary>〉</summary>
            PointingAngleBracket = 0x232A,
            /// <summary>❭</summary>
            MediumPointingAngleBracket = 0x276D,
            /// <summary>❱</summary>
            HeavyPointingAngleBracket = 0x2771,

            /// <summary>》</summary>
            DoubleAngleBracket = 0x300B,

            /// <summary>»</summary>
            DoubleAngleQuotation = 0x00BB,
            /// <summary>›</summary>
            SingleAngleQuotation = 0x203A,

            /// <summary>&gt;</summary>
            GreaterThanSign = 0x003E,

            /// <summary>】</summary>
            BlackLenticularBracket = 0x3011,
        }

#endregion
    }
}
