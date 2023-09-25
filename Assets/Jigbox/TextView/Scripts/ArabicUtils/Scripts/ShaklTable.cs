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

using System.Collections.Generic;
using System.Text;

namespace ArabicUtils
{
    public static class ShaklTable
    {
#region properties

        /// <summary>シャクルの一覧</summary>
        static HashSet<char> shakleMap;

#endregion

#region public methods

        /// <summary>
        /// シャクルかどうかを返します。
        /// </summary>
        /// <param name="character">文字</param>
        /// <returns>シャクルであれば、<c>true</c>を返します。</returns>
        public static bool IsShakl(char character)
        {
            return shakleMap.Contains(character);
        }

#endregion

#region private methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static ShaklTable()
        {
            shakleMap = new HashSet<char>();

            shakleMap.Add((char) ArabicLetter.Shakl.Fathatan);
            shakleMap.Add((char) ArabicLetter.Shakl.Dammatan);
            shakleMap.Add((char) ArabicLetter.Shakl.Kasratan);
            shakleMap.Add((char) ArabicLetter.Shakl.Fatha);
            shakleMap.Add((char) ArabicLetter.Shakl.Damma);
            shakleMap.Add((char) ArabicLetter.Shakl.Kasra);
            shakleMap.Add((char) ArabicLetter.Shakl.Shadda);
            shakleMap.Add((char) ArabicLetter.Shakl.Sukun);
            shakleMap.Add((char) ArabicLetter.Shakl.MaddahAbove);
            shakleMap.Add((char) ArabicLetter.Shakl.HamzaAbove);
            shakleMap.Add((char) ArabicLetter.Shakl.HamzaBelow);
            shakleMap.Add((char) ArabicLetter.Shakl.SubscriptAlef);
            shakleMap.Add((char) ArabicLetter.Shakl.InvertedDamma);
            shakleMap.Add((char) ArabicLetter.Shakl.MarkNoonGhunna);
            shakleMap.Add((char) ArabicLetter.Shakl.Zwarakay);
            shakleMap.Add((char) ArabicLetter.Shakl.VowelSignSmallVAbove);
            shakleMap.Add((char) ArabicLetter.Shakl.VowelSignInvertedSmallVAbove);
            shakleMap.Add((char) ArabicLetter.Shakl.VowelSignDotBelow);
            shakleMap.Add((char) ArabicLetter.Shakl.ReversedDamma);
            shakleMap.Add((char) ArabicLetter.Shakl.FathaTwoDots);
            shakleMap.Add((char) ArabicLetter.Shakl.WavyHamzaBelow);
            shakleMap.Add((char) ArabicLetter.Shakl.SuperscriptAlef);
            shakleMap.Add((char) ArabicLetter.Shakl.ShaddaDammatan);
            shakleMap.Add((char) ArabicLetter.Shakl.ShaddaKasratan);
            shakleMap.Add((char) ArabicLetter.Shakl.ShaddaFatha);
            shakleMap.Add((char) ArabicLetter.Shakl.ShaddaDamma);
            shakleMap.Add((char) ArabicLetter.Shakl.ShaddaKasra);
            shakleMap.Add((char) ArabicLetter.Shakl.ShaddaSuperscriptAlef);
        }

#endregion
    }
}
