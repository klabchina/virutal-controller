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

using System;
using System.Collections.Generic;

namespace ArabicUtils
{
    public static class ArabicTable
    {
#region constants

        /// <summary>変形に対応している文字数(現状)</summary>
        static readonly int MapCount = 36 + 6;

        /// <summary>変形する文字数</summary>
        static readonly int TableCount = 36 + 6 + 4;

#endregion

#region properties

        /// <summary>入力されたアラビア文字とプレゼンテーションフォームの対応表</summary>
        static Dictionary<int, char> presentationFormMap;

        /// <summary>アラビア文字の変形テーブル</summary>
        static Dictionary<int, ArabicFormSet> formSetTable;

#endregion

#region public methods

        /// <summary>
        /// プレゼンテーションフォームを取得します。
        /// </summary>
        /// <param name="character">文字</param>
        /// <returns>プレゼンテーションフォームが存在しない場合、渡された文字をそのまま返します。</returns>
        public static char GetPresentationForm(char character)
        {
            char presentationForm;
            if (presentationFormMap.TryGetValue((int) character, out presentationForm))
            {
                return presentationForm;
            }

            return character;
        }

        /// <summary>
        /// アラビア文字の文字セットを取得します。
        /// </summary>
        /// <param name="character">文字</param>
        /// <returns>文字セットが存在しない場合、<c>null</c>を返します。</returns>
        public static ArabicFormSet GetFormSet(char character)
        {
            ArabicFormSet formSet;
            if (formSetTable.TryGetValue((int) character, out formSet))
            {
                return formSet;
            }
            return null;
        }

#endregion

#region private methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static ArabicTable()
        {
            // wiki
            // https://ja.wikipedia.org/wiki/%E3%82%A2%E3%83%A9%E3%83%93%E3%82%A2%E6%96%87%E5%AD%97#%E5%9F%BA%E6%9C%AC%E6%96%87%E5%AD%97%E8%A1%A8

            CreatePresentationFormMap();
            CreateConvertTable();
        }

        /// <summary>
        /// 入力されたアラビア文字とプレゼンテーションフォームの対応表を作成します。
        /// </summary>
        static void CreatePresentationFormMap()
        {
            presentationFormMap = new Dictionary<int, char>(MapCount);

            presentationFormMap.Add((int) ArabicLetter.Plain.Hamza, (char) ArabicLetter.PresentationForms.Hamza);
            presentationFormMap.Add((int) ArabicLetter.Plain.AlefMaddaAbove, (char) ArabicLetter.PresentationForms.AlefMaddaAbove);
            presentationFormMap.Add((int) ArabicLetter.Plain.AlefHamzaAbove, (char) ArabicLetter.PresentationForms.AlefHamzaAbove);
            presentationFormMap.Add((int) ArabicLetter.Plain.WawHamzaAbove, (char) ArabicLetter.PresentationForms.WawHamzaAbove);
            presentationFormMap.Add((int) ArabicLetter.Plain.AlefHamzaBelow, (char) ArabicLetter.PresentationForms.AlefHamzaBelow);
            presentationFormMap.Add((int) ArabicLetter.Plain.YehHamzaAboove, (char) ArabicLetter.PresentationForms.YehHamzaAboove);
            presentationFormMap.Add((int) ArabicLetter.Plain.Alef, (char) ArabicLetter.PresentationForms.Alef);
            presentationFormMap.Add((int) ArabicLetter.Plain.Beh, (char) ArabicLetter.PresentationForms.Beh);
            presentationFormMap.Add((int) ArabicLetter.Plain.TehMarbuta, (char) ArabicLetter.PresentationForms.TehMarbuta);
            presentationFormMap.Add((int) ArabicLetter.Plain.Teh, (char) ArabicLetter.PresentationForms.Teh);
            presentationFormMap.Add((int) ArabicLetter.Plain.Theh, (char) ArabicLetter.PresentationForms.Theh);
            presentationFormMap.Add((int) ArabicLetter.Plain.Jeem, (char) ArabicLetter.PresentationForms.Jeem);
            presentationFormMap.Add((int) ArabicLetter.Plain.Hah, (char) ArabicLetter.PresentationForms.Hah);
            presentationFormMap.Add((int) ArabicLetter.Plain.Khah, (char) ArabicLetter.PresentationForms.Khah);
            presentationFormMap.Add((int) ArabicLetter.Plain.Dal, (char) ArabicLetter.PresentationForms.Dal);
            presentationFormMap.Add((int) ArabicLetter.Plain.Thal, (char) ArabicLetter.PresentationForms.Thal);
            presentationFormMap.Add((int) ArabicLetter.Plain.Reh, (char) ArabicLetter.PresentationForms.Reh);
            presentationFormMap.Add((int) ArabicLetter.Plain.Zain, (char) ArabicLetter.PresentationForms.Zain);
            presentationFormMap.Add((int) ArabicLetter.Plain.Seen, (char) ArabicLetter.PresentationForms.Seen);
            presentationFormMap.Add((int) ArabicLetter.Plain.Sheen, (char) ArabicLetter.PresentationForms.Sheen);
            presentationFormMap.Add((int) ArabicLetter.Plain.Sad, (char) ArabicLetter.PresentationForms.Sad);
            presentationFormMap.Add((int) ArabicLetter.Plain.Dad, (char) ArabicLetter.PresentationForms.Dad);
            presentationFormMap.Add((int) ArabicLetter.Plain.Tah, (char) ArabicLetter.PresentationForms.Tah);
            presentationFormMap.Add((int) ArabicLetter.Plain.Zah, (char) ArabicLetter.PresentationForms.Zah);
            presentationFormMap.Add((int) ArabicLetter.Plain.Ain, (char) ArabicLetter.PresentationForms.Ain);
            presentationFormMap.Add((int) ArabicLetter.Plain.Ghain, (char) ArabicLetter.PresentationForms.Ghain);
            presentationFormMap.Add((int) ArabicLetter.Plain.Feh, (char) ArabicLetter.PresentationForms.Feh);
            presentationFormMap.Add((int) ArabicLetter.Plain.Qaf, (char) ArabicLetter.PresentationForms.Qaf);
            presentationFormMap.Add((int) ArabicLetter.Plain.Kaf, (char) ArabicLetter.PresentationForms.Kaf);
            presentationFormMap.Add((int) ArabicLetter.Plain.Lam, (char) ArabicLetter.PresentationForms.Lam);
            presentationFormMap.Add((int) ArabicLetter.Plain.Meem, (char) ArabicLetter.PresentationForms.Meem);
            presentationFormMap.Add((int) ArabicLetter.Plain.Noon, (char) ArabicLetter.PresentationForms.Noon);
            presentationFormMap.Add((int) ArabicLetter.Plain.Heh, (char) ArabicLetter.PresentationForms.Heh);
            presentationFormMap.Add((int) ArabicLetter.Plain.Waw, (char) ArabicLetter.PresentationForms.Waw);
            presentationFormMap.Add((int) ArabicLetter.Plain.AlefMaksura, (char) ArabicLetter.PresentationForms.AlefMaksura);
            presentationFormMap.Add((int) ArabicLetter.Plain.Yeh, (char) ArabicLetter.PresentationForms.Yeh);

            // Persia Arabic
            presentationFormMap.Add((int) ArabicLetter.Plain.Peh, (char) ArabicLetter.PresentationForms.Peh);
            presentationFormMap.Add((int) ArabicLetter.Plain.Tcheh, (char) ArabicLetter.PresentationForms.Tcheh);
            presentationFormMap.Add((int) ArabicLetter.Plain.Jeh, (char) ArabicLetter.PresentationForms.Jeh);
            presentationFormMap.Add((int) ArabicLetter.Plain.Keheh, (char) ArabicLetter.PresentationForms.Keheh);
            presentationFormMap.Add((int) ArabicLetter.Plain.Gaf, (char) ArabicLetter.PresentationForms.Gaf);
            presentationFormMap.Add((int) ArabicLetter.Plain.FarasiYeh, (char) ArabicLetter.PresentationForms.FarasiYeh);
        }

        /// <summary>
        /// アラビア文字の変形テーブルを作成します。
        /// </summary>
        static void CreateConvertTable()
        {
            formSetTable = new Dictionary<int, ArabicFormSet>(TableCount);

            formSetTable.Add((int) ArabicLetter.PresentationForms.Hamza, ArabicFormSet.CreateIsolatedOnly(ArabicLetter.PresentationForms.Hamza));
            formSetTable.Add((int) ArabicLetter.PresentationForms.AlefMaddaAbove, ArabicFormSet.Create(ArabicLetter.PresentationForms.AlefMaddaAbove, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.AlefHamzaAbove, ArabicFormSet.Create(ArabicLetter.PresentationForms.AlefHamzaAbove, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.WawHamzaAbove, ArabicFormSet.Create(ArabicLetter.PresentationForms.WawHamzaAbove, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.AlefHamzaBelow, ArabicFormSet.Create(ArabicLetter.PresentationForms.AlefHamzaBelow, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.YehHamzaAboove, ArabicFormSet.Create(ArabicLetter.PresentationForms.YehHamzaAboove));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Alef, ArabicFormSet.Create(ArabicLetter.PresentationForms.Alef, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Beh, ArabicFormSet.Create(ArabicLetter.PresentationForms.Beh));
            formSetTable.Add((int) ArabicLetter.PresentationForms.TehMarbuta, ArabicFormSet.Create(ArabicLetter.PresentationForms.TehMarbuta, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Teh, ArabicFormSet.Create(ArabicLetter.PresentationForms.Teh));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Theh, ArabicFormSet.Create(ArabicLetter.PresentationForms.Theh));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Jeem, ArabicFormSet.Create(ArabicLetter.PresentationForms.Jeem));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Hah, ArabicFormSet.Create(ArabicLetter.PresentationForms.Hah));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Khah, ArabicFormSet.Create(ArabicLetter.PresentationForms.Khah));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Dal, ArabicFormSet.Create(ArabicLetter.PresentationForms.Dal, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Thal, ArabicFormSet.Create(ArabicLetter.PresentationForms.Thal, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Reh, ArabicFormSet.Create(ArabicLetter.PresentationForms.Reh, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Zain, ArabicFormSet.Create(ArabicLetter.PresentationForms.Zain, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Seen, ArabicFormSet.Create(ArabicLetter.PresentationForms.Seen));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Sheen, ArabicFormSet.Create(ArabicLetter.PresentationForms.Sheen));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Sad, ArabicFormSet.Create(ArabicLetter.PresentationForms.Sad));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Dad, ArabicFormSet.Create(ArabicLetter.PresentationForms.Dad));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Tah, ArabicFormSet.Create(ArabicLetter.PresentationForms.Tah));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Zah, ArabicFormSet.Create(ArabicLetter.PresentationForms.Zah));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Ain, ArabicFormSet.Create(ArabicLetter.PresentationForms.Ain));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Ghain, ArabicFormSet.Create(ArabicLetter.PresentationForms.Ghain));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Feh, ArabicFormSet.Create(ArabicLetter.PresentationForms.Feh));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Qaf, ArabicFormSet.Create(ArabicLetter.PresentationForms.Qaf));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Kaf, ArabicFormSet.Create(ArabicLetter.PresentationForms.Kaf));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Lam, ArabicFormSet.Create(ArabicLetter.PresentationForms.Lam));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Meem, ArabicFormSet.Create(ArabicLetter.PresentationForms.Meem));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Noon, ArabicFormSet.Create(ArabicLetter.PresentationForms.Noon));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Heh, ArabicFormSet.Create(ArabicLetter.PresentationForms.Heh));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Waw, ArabicFormSet.Create(ArabicLetter.PresentationForms.Waw, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.AlefMaksura, ArabicFormSet.Create(ArabicLetter.PresentationForms.AlefMaksura, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Yeh, ArabicFormSet.Create(ArabicLetter.PresentationForms.Yeh));

            // Persia Arabic
            formSetTable.Add((int) ArabicLetter.PresentationForms.Peh, ArabicFormSet.Create(ArabicLetter.PresentationForms.Peh));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Tcheh, ArabicFormSet.Create(ArabicLetter.PresentationForms.Tcheh));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Jeh, ArabicFormSet.Create(ArabicLetter.PresentationForms.Jeh, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Keheh, ArabicFormSet.Create(ArabicLetter.PresentationForms.Keheh));
            formSetTable.Add((int) ArabicLetter.PresentationForms.Gaf, ArabicFormSet.Create(ArabicLetter.PresentationForms.Gaf));
            formSetTable.Add((int) ArabicLetter.PresentationForms.FarasiYeh, ArabicFormSet.Create(ArabicLetter.PresentationForms.FarasiYeh));

            formSetTable.Add((int) ArabicLetter.PresentationForms.LamAlefMaddaAbove, ArabicFormSet.Create(ArabicLetter.PresentationForms.LamAlefMaddaAbove, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.LamAlefHamzaAbove, ArabicFormSet.Create(ArabicLetter.PresentationForms.LamAlefHamzaAbove, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.LamAlefHamzaBelow, ArabicFormSet.Create(ArabicLetter.PresentationForms.LamAlefHamzaBelow, false));
            formSetTable.Add((int) ArabicLetter.PresentationForms.LamAlef, ArabicFormSet.Create(ArabicLetter.PresentationForms.LamAlef, false));
        }

#endregion
    }
}
