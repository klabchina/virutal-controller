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

namespace ArabicUtils
{
    public static class BracketTable
    {
#region constants

        /// <summary>括弧の数</summary>
        static readonly int BracketCount = 29;

#endregion

#region properties

        /// <summary>左括弧(開き括弧)の対応表</summary>
        static Dictionary<char, char> leftBracketMap;

        /// <summary>右括弧(閉じ括弧)の対応表</summary>
        static Dictionary<char, char> rightBracketMap;

#endregion

#region public methods

        /// <summary>
        /// 括弧かどうかを返します。
        /// </summary>
        /// <param name="character">文字</param>
        /// <returns>括弧であれば、<c>true</c>を返します。</returns>
        public static bool IsBracket(char character)
        {
            return leftBracketMap.ContainsKey(character) || rightBracketMap.ContainsKey(character);
        }

        /// <summary>
        /// 左括弧(開き括弧)かどうかを返します。
        /// </summary>
        /// <param name="character">文字</param>
        /// <returns>左括弧(開き括弧)であれば、<c>true</c>を返します。</returns>
        public static bool IsLeftBracket(char character)
        {
            return leftBracketMap.ContainsKey(character);
        }

        /// <summary>
        /// 右括弧(閉じ括弧)かどうかを返します。
        /// </summary>
        /// <param name="character">文字</param>
        /// <returns>右括弧(閉じ括弧)であれば、<c>true</c>を返します。</returns>
        public static bool IsRightBracket(char character)
        {
            return rightBracketMap.ContainsKey(character);
        }

        /// <summary>
        /// 左括弧(開き括弧)に対応する右括弧(閉じ括弧)を取得します。
        /// </summary>
        /// <param name="bracket">括弧</param>
        /// <returns>対応する括弧があれば、対応する括弧を返します。</returns>
        public static char GetRightBracket(char bracket)
        {
            char rightBracket = bracket;
            if (leftBracketMap.TryGetValue(bracket, out rightBracket))
            {
                return rightBracket;
            }
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning("Not exist pair! : " + bracket);
#endif
            return bracket;
        }

        /// <summary>
        /// 右括弧(閉じ括弧)に対応する左括弧(開き括弧)を取得します。
        /// </summary>
        /// <param name="bracket">括弧</param>
        /// <returns>対応する括弧があれば、対応する括弧を返します。</returns>
        public static char GetLeftBracket(char bracket)
        {
            char leftBracket = bracket;
            if (rightBracketMap.TryGetValue(bracket, out leftBracket))
            {
                return leftBracket;
            }
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning("Not exist pair! : " + bracket);
#endif
            return bracket;
        }

#endregion

#region private methods

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static BracketTable()
        {
            CreateLeftBracketMap();
            CreateRightBracketMap();
        }

        /// <summary>
        /// 左括弧(開き括弧)の対応表を作成します。
        /// </summary>
        static void CreateLeftBracketMap()
        {
            leftBracketMap = new Dictionary<char, char>(BracketCount);

            leftBracketMap.Add((char) BracketLetter.Left.Parenthesis, (char) BracketLetter.Right.Parenthesis);
            leftBracketMap.Add((char) BracketLetter.Left.FullParenthesis, (char) BracketLetter.Right.FullParenthesis);
            leftBracketMap.Add((char) BracketLetter.Left.SmallParenthesis, (char) BracketLetter.Right.SmallParenthesis);
            leftBracketMap.Add((char) BracketLetter.Left.MediumParenthesis, (char) BracketLetter.Right.MediumParenthesis);
            leftBracketMap.Add((char) BracketLetter.Left.MediumFattenedParenthesis, (char) BracketLetter.Right.MediumFattenedParenthesis);
            leftBracketMap.Add((char) BracketLetter.Left.WhiteParenthesis, (char) BracketLetter.Right.WhiteParenthesis);
            leftBracketMap.Add((char) BracketLetter.Left.FullWhiteParenthesis, (char) BracketLetter.Right.FullWhiteParenthesis);
            leftBracketMap.Add((char) BracketLetter.Left.CornerBracket, (char) BracketLetter.Right.CornerBracket);
            leftBracketMap.Add((char) BracketLetter.Left.HalfCornerBracket, (char) BracketLetter.Right.HalfCornerBracket);
            leftBracketMap.Add((char) BracketLetter.Left.WhiteCornerBracket, (char) BracketLetter.Right.WhiteCornerBracket);
            leftBracketMap.Add((char) BracketLetter.Left.SquareBracket, (char) BracketLetter.Right.SquareBracket);
            leftBracketMap.Add((char) BracketLetter.Left.FullSquareBracket, (char) BracketLetter.Right.FullSquareBracket);
            leftBracketMap.Add((char) BracketLetter.Left.WhiteSquareBracket, (char) BracketLetter.Right.WhiteSquareBracket);
            leftBracketMap.Add((char) BracketLetter.Left.CurlyBracket, (char) BracketLetter.Right.CurlyBracket);
            leftBracketMap.Add((char) BracketLetter.Left.FullCurlyBracket, (char) BracketLetter.Right.FullCurlyBracket);
            leftBracketMap.Add((char) BracketLetter.Left.SmallCurlyBracket, (char) BracketLetter.Right.SmallCurlyBracket);
            leftBracketMap.Add((char) BracketLetter.Left.MediumCurlyBracket, (char) BracketLetter.Right.MediumCurlyBracket);
            leftBracketMap.Add((char) BracketLetter.Left.TortoiseShellBracket, (char) BracketLetter.Right.TortoiseShellBracket);
            leftBracketMap.Add((char) BracketLetter.Left.TortoiseShellBracketOrnament, (char) BracketLetter.Right.TortoiseShellBracketOrnament);
            leftBracketMap.Add((char) BracketLetter.Left.WhiteTortoiseShellBracket, (char) BracketLetter.Right.WhiteTortoiseShellBracket);
            leftBracketMap.Add((char) BracketLetter.Left.AngleBracket, (char) BracketLetter.Right.AngleBracket);
            leftBracketMap.Add((char) BracketLetter.Left.PointingAngleBracket, (char) BracketLetter.Right.PointingAngleBracket);
            leftBracketMap.Add((char) BracketLetter.Left.MediumPointingAngleBracket, (char) BracketLetter.Right.MediumPointingAngleBracket);
            leftBracketMap.Add((char) BracketLetter.Left.HeavyPointingAngleBracket, (char) BracketLetter.Right.HeavyPointingAngleBracket);
            leftBracketMap.Add((char) BracketLetter.Left.DoubleAngleBracket, (char) BracketLetter.Right.DoubleAngleBracket);
            leftBracketMap.Add((char) BracketLetter.Left.DoubleAngleQuotation, (char) BracketLetter.Right.DoubleAngleQuotation);
            leftBracketMap.Add((char) BracketLetter.Left.SingleAngleQuotation, (char) BracketLetter.Right.SingleAngleQuotation);
            leftBracketMap.Add((char) BracketLetter.Left.LessThanSign, (char) BracketLetter.Right.GreaterThanSign);
            leftBracketMap.Add((char) BracketLetter.Left.BlackLenticularBracket, (char) BracketLetter.Right.BlackLenticularBracket);
        }

        /// <summary>
        /// 右括弧(閉じ括弧)の対応表を作成します。
        /// </summary>
        static void CreateRightBracketMap()
        {
            rightBracketMap = new Dictionary<char, char>(BracketCount);

            rightBracketMap.Add((char) BracketLetter.Right.Parenthesis, (char) BracketLetter.Left.Parenthesis);
            rightBracketMap.Add((char) BracketLetter.Right.FullParenthesis, (char) BracketLetter.Left.FullParenthesis);
            rightBracketMap.Add((char) BracketLetter.Right.SmallParenthesis, (char) BracketLetter.Left.SmallParenthesis);
            rightBracketMap.Add((char) BracketLetter.Right.MediumParenthesis, (char) BracketLetter.Left.MediumParenthesis);
            rightBracketMap.Add((char) BracketLetter.Right.MediumFattenedParenthesis, (char) BracketLetter.Left.MediumFattenedParenthesis);
            rightBracketMap.Add((char) BracketLetter.Right.WhiteParenthesis, (char) BracketLetter.Left.WhiteParenthesis);
            rightBracketMap.Add((char) BracketLetter.Right.FullWhiteParenthesis, (char) BracketLetter.Left.FullWhiteParenthesis);
            rightBracketMap.Add((char) BracketLetter.Right.CornerBracket, (char) BracketLetter.Left.CornerBracket);
            rightBracketMap.Add((char) BracketLetter.Right.HalfCornerBracket, (char) BracketLetter.Left.HalfCornerBracket);
            rightBracketMap.Add((char) BracketLetter.Right.WhiteCornerBracket, (char) BracketLetter.Left.WhiteCornerBracket);
            rightBracketMap.Add((char) BracketLetter.Right.SquareBracket, (char) BracketLetter.Left.SquareBracket);
            rightBracketMap.Add((char) BracketLetter.Right.FullSquareBracket, (char) BracketLetter.Left.FullSquareBracket);
            rightBracketMap.Add((char) BracketLetter.Right.WhiteSquareBracket, (char) BracketLetter.Left.WhiteSquareBracket);
            rightBracketMap.Add((char) BracketLetter.Right.CurlyBracket, (char) BracketLetter.Left.CurlyBracket);
            rightBracketMap.Add((char) BracketLetter.Right.FullCurlyBracket, (char) BracketLetter.Left.FullCurlyBracket);
            rightBracketMap.Add((char) BracketLetter.Right.SmallCurlyBracket, (char) BracketLetter.Left.SmallCurlyBracket);
            rightBracketMap.Add((char) BracketLetter.Right.MediumCurlyBracket, (char) BracketLetter.Left.MediumCurlyBracket);
            rightBracketMap.Add((char) BracketLetter.Right.TortoiseShellBracket, (char) BracketLetter.Left.TortoiseShellBracket);
            rightBracketMap.Add((char) BracketLetter.Right.TortoiseShellBracketOrnament, (char) BracketLetter.Left.TortoiseShellBracketOrnament);
            rightBracketMap.Add((char) BracketLetter.Right.WhiteTortoiseShellBracket, (char) BracketLetter.Left.WhiteTortoiseShellBracket);
            rightBracketMap.Add((char) BracketLetter.Right.AngleBracket, (char) BracketLetter.Left.AngleBracket);
            rightBracketMap.Add((char) BracketLetter.Right.PointingAngleBracket, (char) BracketLetter.Left.PointingAngleBracket);
            rightBracketMap.Add((char) BracketLetter.Right.MediumPointingAngleBracket, (char) BracketLetter.Left.MediumPointingAngleBracket);
            rightBracketMap.Add((char) BracketLetter.Right.HeavyPointingAngleBracket, (char) BracketLetter.Left.HeavyPointingAngleBracket);
            rightBracketMap.Add((char) BracketLetter.Right.DoubleAngleBracket, (char) BracketLetter.Left.DoubleAngleBracket);
            rightBracketMap.Add((char) BracketLetter.Right.DoubleAngleQuotation, (char) BracketLetter.Left.DoubleAngleQuotation);
            rightBracketMap.Add((char) BracketLetter.Right.SingleAngleQuotation, (char) BracketLetter.Left.SingleAngleQuotation);
            rightBracketMap.Add((char) BracketLetter.Right.GreaterThanSign, (char) BracketLetter.Left.LessThanSign);
            rightBracketMap.Add((char) BracketLetter.Right.BlackLenticularBracket, (char) BracketLetter.Left.BlackLenticularBracket);
        }

#endregion
    }
}
