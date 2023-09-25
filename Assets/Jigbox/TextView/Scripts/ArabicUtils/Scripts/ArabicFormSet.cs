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

namespace ArabicUtils
{
    public sealed class ArabicFormSet
    {
        /// <summary>単独形</summary>
        public readonly int isolated;

        /// <summary>尾字</summary>
        public readonly int final;

        /// <summary>頭字</summary>
        public readonly int initial;

        /// <summary>中字</summary>
        public readonly int medial;

        /// <summary>頭字、中字を持つかどうか</summary>
        public readonly bool hasInitialAndMedial;

        /// <summary>尾字を持つかどうか(基本的にHamza用の例外)</summary>
        public readonly bool hasFinal;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="isolated">単独形の文字コード</param>
        ArabicFormSet(ArabicLetter.PresentationForms isolated)
        {
            this.isolated = (int) isolated;
            final = ArabicLetter.InvalidUnicodeCharacter;
            initial = ArabicLetter.InvalidUnicodeCharacter;
            medial = ArabicLetter.InvalidUnicodeCharacter;
            hasInitialAndMedial = false;
            hasFinal = false;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="isolated">単独形の文字コード</param>
        /// <param name="hasInitialAndMedial">頭字、中字を持つかどうか</param>
        ArabicFormSet(ArabicLetter.PresentationForms isolated, bool hasInitialAndMedial)
        {
            this.isolated = (int) isolated;
            // Unicode上で尾字は単独形から1つ、頭字は2つ、中字は3つズレた位置に格納されている
            // 補足 : 頭字、中字を持たない文字の場合は、中抜けになるわけではなく、次の文字が詰めて配置されている
            final = this.isolated + 1;
            initial = hasInitialAndMedial ? this.isolated + 2 : ArabicLetter.InvalidUnicodeCharacter;
            medial = hasInitialAndMedial ? this.isolated + 3 : ArabicLetter.InvalidUnicodeCharacter;
            this.hasInitialAndMedial = hasInitialAndMedial;
            hasFinal = true;
        }

        /// <summary>
        /// アラビア文字セットを生成します。
        /// </summary>
        /// <param name="isolated">単独形の文字コード</param>
        /// <param name="hasInitialAndMedial">頭字、中字を持つかどうか</param>
        /// <returns></returns>
        public static ArabicFormSet Create(ArabicLetter.PresentationForms isolated, bool hasInitialAndMedial = true)
        {
            return new ArabicFormSet(isolated, hasInitialAndMedial);
        }

        /// <summary>
        /// 単独形のみのアラビア文字セットを生成します。
        /// </summary>
        /// <param name="isolated">単独系の文字コード</param>
        /// <returns></returns>
        public static ArabicFormSet CreateIsolatedOnly(ArabicLetter.PresentationForms isolated)
        {
            return new ArabicFormSet(isolated);
        }
    }
}
