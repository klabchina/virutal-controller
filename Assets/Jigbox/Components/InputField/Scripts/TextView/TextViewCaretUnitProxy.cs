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

using System.Collections.Generic;

namespace Jigbox.Components
{
    /// <summary>
    /// ThaiUtilsやArabicUtilsそれぞれで持ってるCaretUnitのデータを一元的に管理出来るように変換し管理するクラス
    /// タイ語やアラビア語じゃない場合もCaretUnitの生成が必要なのでここで行っている
    /// </summary>
    public static class TextViewCaretUnitProxy
    {
        /// <summary>キャレット全体</summary>
        static readonly List<CaretUnit[]> allDefaultCaretUnits = new List<CaretUnit[]>();

        /// <summary>1行のキャレットデータ</summary>
        static readonly List<CaretUnit> defaultCaretUnits = new List<CaretUnit>();

        /// <summary>キャレット全体を1次元配列に直したもの</summary>
        static readonly List<CaretUnit> allCaretUnits = new List<CaretUnit>();

        /// <summary>キャレットの初期化</summary>
        public static void Initialize()
        {
            defaultCaretUnits.Clear();
            allDefaultCaretUnits.Clear();
            AddFirstUnit();
        }

        /// <summary>
        /// キャレットのデータを取得します
        /// </summary>
        /// <returns>キャレットの配列</returns>
        public static CaretUnit[] GetAllCaretUnits()
        {
            // 文字列が空の場合も行頭データだけは必要なので、行頭データを追加している
            // 文字列が空の場合はデータを追加するタイミングがない
            if (allDefaultCaretUnits.Count == 0)
            {
                return new[]
                {
                    CaretUnit.FirstUnit(0, 0, 0)
                };
            }

            allCaretUnits.Clear();

            foreach (var lineCaretUnits in allDefaultCaretUnits)
            {
                foreach (var caretUnit in lineCaretUnits)
                {
                    allCaretUnits.Add(caretUnit);
                }
            }

            return allCaretUnits.ToArray();
        }

        /// <summary>
        /// 行頭のキャレットユニットを追加
        /// </summary>
        public static void AddFirstUnit()
        {
            AddAllDefaultCaretUnits();
            defaultCaretUnits.Clear();
            var srcIndexOffset = CalculateSrcIndexOffset();
            var dstIndexOffset = 0;
            var line = allDefaultCaretUnits.Count;
            defaultCaretUnits.Add(CaretUnit.FirstUnit(srcIndexOffset, dstIndexOffset, line));
        }

        /// <summary>
        /// 現在の行を全体管理リストに追加します
        /// </summary>
        public static void AddAllDefaultCaretUnits()
        {
            if (defaultCaretUnits.Count > 0)
            {
                allDefaultCaretUnits.Add(defaultCaretUnits.ToArray());
            }
        }

        /// <summary>
        /// タイ語のCaretUnit配列から共通CaretUnitへの変換
        /// </summary>
        public static void AddThaiCaretUnits(List<ThaiUtils.CaretUnit> caretUnits)
        {
            var srcIndexOffset = CalculateSrcIndexOffset();
            var dstIndexOffset = defaultCaretUnits[defaultCaretUnits.Count - 1].DestinationIndex;
            var line = allDefaultCaretUnits.Count;
            foreach (var caretUnit in caretUnits)
            {
                defaultCaretUnits.Add(new CaretUnit(caretUnit, srcIndexOffset, dstIndexOffset, line));
            }
        }

        /// <summary>
        /// アラビア語のCaretUnit配列から共通CaretUnitへの変換
        /// </summary>
        public static void AddArabicCaretUnits(List<ArabicUtils.CaretUnit> caretUnits)
        {
            var srcIndexOffset = CalculateSrcIndexOffset();
            var dstIndexOffset = defaultCaretUnits[defaultCaretUnits.Count - 1].DestinationIndex;
            var line = allDefaultCaretUnits.Count;
            foreach (var caretUnit in caretUnits)
            {
                defaultCaretUnits.Add(new CaretUnit(caretUnit, srcIndexOffset, dstIndexOffset, line));
            }
        }

        /// <summary>
        /// LanguageType=Defaultの時に生成する
        /// </summary>
        public static void GenerateDefaultCaretUnits(string message)
        {
            var srcIndexOffset = CalculateSrcIndexOffset();
            var dstIndexOffset = defaultCaretUnits[defaultCaretUnits.Count - 1].DestinationIndex;
            var line = allDefaultCaretUnits.Count;
            var index = 0;
            foreach (var _ in message)
            {
                defaultCaretUnits.Add(new CaretUnit(index, 1, srcIndexOffset, index, 1, dstIndexOffset, false, line));
                index++;
            }
        }

        /// <summary>文頭からのOffsetを取得します</summary>
        static int CalculateSrcIndexOffset()
        {
            var result = 0;

            if (allDefaultCaretUnits.Count > 0)
            {
                var lineCaretUnits = allDefaultCaretUnits[allDefaultCaretUnits.Count - 1];
                var lastCaretUnit = lineCaretUnits[lineCaretUnits.Length - 1];
                // 行頭は0、改行は+1する事でFirstUnitの整合性をとってる
                result = lastCaretUnit.SourceIndex + 1;
            }

            if (defaultCaretUnits.Count > 0)
            {
                var caretUnit = defaultCaretUnits[defaultCaretUnits.Count - 1];
                result = caretUnit.SourceIndex;
            }

            return result;
        }
    }
}
