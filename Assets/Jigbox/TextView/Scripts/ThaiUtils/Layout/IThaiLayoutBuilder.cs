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
using ThaiUtils.UnitParser;

namespace ThaiUtils.Layout
{
    public interface IThaiLayoutBuilder
    {
        /// <summary>
        /// レイアウト情報込みの構成を作成します。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="textIndex">テキストの開始インデックス</param>
        /// <param name="unit">タイ語の最小構成情報</param>
        /// <param name="characterTypes">文字の種類</param>
        /// <param name="characterTypeIndex">文字の種類の開始インデックス</param>
        /// <param name="isCreateCaretUnit">キャレット参照単位の情報を生成するかどうか</param>
        /// <param name="caretUnits">キャレット参照単位の情報</param>
        /// <param name="convertedCharacterLength">変換処理が済んだ変換後のテキストの文字数</param>
        /// <returns>作成した構成情報を返します。</returns>
        TextUnit Build(string text,
            int textIndex,
            ThaiUnit unit,
            List<CharacterType> characterTypes,
            int characterTypeIndex,
            bool isCreateCaretUnit,
            List<CaretUnit> caretUnits,
            int convertedCharacterLength);
    }
}
