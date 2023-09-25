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

namespace ThaiUtils
{
    /// <summary>
    /// 文字の種類
    /// </summary>
    public enum CharacterType
    {
        /// <summary>子音</summary>
        Consonants,
        /// <summary>母音になる可能性のある子音</summary>
        VagueConsonants,
        /// <summary>子音の左に付く母音</summary>
        LeftVowels,
        /// <summary>子音の右に付く母音</summary>
        RightVowels,
        /// <summary>子音の右に付く母音で上に付く母音を含む特殊なもの</summary>
        SaraAm,
        /// <summary>子音の上に付く母音</summary>
        TopVowels,
        /// <summary>子音の下に付く母音</summary>
        BottomVowels,
        /// <summary>声調記号</summary>
        ToneMarks,
        /// <summary>その他</summary>
        Other
    }
}
