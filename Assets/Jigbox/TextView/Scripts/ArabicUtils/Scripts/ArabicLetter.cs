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
    public static class ArabicLetter
    {
#region inner classes, enum, and structs

        /// <summary>
        /// アラビア語の文字コード
        /// http://www.unicode.org/charts/PDF/U0600.pdf
        /// https://www.compart.com/en/unicode/block/U+0600
        /// </summary>
        public enum Plain
        {
            /// <summary>Arabic Letter Hamza : ء</summary> 
            Hamza = 0x0621,

            /// <summary>Arabic Letter Alef With Madda Above : آ</summary>
            AlefMaddaAbove = 0x0622,
            /// <summary>Arabic Letter Alef With Hamza Above : أ</summary>
            AlefHamzaAbove = 0x0623,
            /// <summary>Arabic Letter Waw With Hamza Above : ؤ</summary>
            WawHamzaAbove = 0x0624,
            /// <summary>Arabic Letter Alef With Hamza Below : إ</summary>
            AlefHamzaBelow = 0x0625,
            /// <summary>Arabic Letter Yeh With Hamza Above : ئ</summary>
            YehHamzaAboove = 0x0626,

            // Alef(0x0627)～Yeh(0x064A)までがアラビア語におけるアルファベットの文字

            /// <summary>Arabic Letter Alef : ا</summary>
            Alef = 0x0627,
            /// <summary>Arabic Letter Beh : ب</summary>
            Beh = 0x0628,
            /// <summary>Arabic Letter Teh Marbuta : ة</summary>
            TehMarbuta = 0x0629,
            /// <summary>Arabic Letter Teh : ت</summary>
            Teh = 0x062A,
            /// <summary>Arabic Letter Theh : ث</summary>
            Theh = 0x062B,
            /// <summary>Arabic Letter Jeem : ج</summary>
            Jeem = 0x062C,
            /// <summary>Arabic Letter Hah : ح</summary>
            Hah = 0x062D,
            /// <summary>Arabic Letter Khah : خ</summary>
            Khah = 0x062E,
            /// <summary>Arabic Letter Dal : د</summary>
            Dal = 0x062F,
            /// <summary>Arabic Letter Thal : ذ</summary>
            Thal = 0x0630,
            /// <summary>Arabic Letter Reh : ر</summary>
            Reh = 0x0631,
            /// <summary>Arabic Letter Zain : ز</summary>
            Zain = 0x0632,
            /// <summary>Arabic Letter Seen : س</summary>
            Seen = 0x0633,
            /// <summary>Arabic Letter Sheen : ش</summary>
            Sheen = 0x0634,
            /// <summary>Arabic Letter Sad : ص</summary>
            Sad = 0x0635,
            /// <summary>Arabic Letter Dad : ض</summary>
            Dad = 0x0636,
            /// <summary>Arabic Letter Tah : ط</summary>
            Tah = 0x0637,
            /// <summary>Arabic Letter Zah : ظ</summary>
            Zah = 0x0638,
            /// <summary>Arabic Letter Ain : ع</summary>
            Ain = 0x0639,
            /// <summary>Arabic Letter Ghain : غ</summary>
            Ghain = 0x063A,
            /// <summary>Arabic Letter Feh : ف</summary>
            Feh = 0x0641,
            /// <summary>Arabic Letter Qaf : ق</summary>
            Qaf = 0x0642,
            /// <summary>Arabic Letter Kaf : ك</summary>
            Kaf = 0x0643,
            /// <summary>Arabic Letter Lam : ل</summary>
            Lam = 0x0644,
            /// <summary>Arabic Letter Meem : م</summary>
            Meem = 0x0645,
            /// <summary>Arabic Letter Noon : ن</summary>
            Noon = 0x0646,
            /// <summary>Arabic Letter Heh : ه</summary>
            Heh = 0x0647,
            /// <summary>Arabic Letter Waw : و</summary>
            Waw = 0x0648,
            /// <summary>Arabic Letter Alef Maksura : ى</summary>
            AlefMaksura = 0x0649,
            /// <summary>Arabic Letter Yeh : ي</summary>
            Yeh = 0x064A,

            // Persia Arabic

            /// <summary>Arabic Letter Peh : پ</summary>
            Peh = 0x067E,
            /// <summary>Arabic Letter Tcheh : چ</summary>
            Tcheh = 0x0686,
            /// <summary>Arabic Letter Jeh : ژ</summary>
            Jeh = 0x0698,
            /// <summary>Arabic Letter Keheh : ک</summary>
            Keheh = 0x06A9,
            /// <summary>Arabic Letter Gaf : گ</summary>
            Gaf = 0x06AF,
            /// <summary>Arabic Letter Farsi Yeh : ی</summary>
            FarasiYeh = 0x06CC,
        }

        /// <summary>
        /// アラビア語のプレゼンテーションフォームの文字コード
        /// http://www.unicode.org/charts/PDF/UFB50.pdf
        /// http://www.unicode.org/charts/PDF/UFE70.pdf
        /// https://www.compart.com/en/unicode/block/U+FB50
        /// https://www.compart.com/en/unicode/block/U+FE70
        /// </summary>
        public enum PresentationForms
        {
            /// <summary>Arabic Letter Hamza Isolated Form : ء</summary> 
            Hamza = 0xFE80,
            /// <summary>Arabic Letter Alef With Madda Above Isolated Form : آ</summary>
            AlefMaddaAbove = 0xFE81,
            /// <summary>Arabic Letter Alef With Hamza Above Isolated Form : أ</summary>
            AlefHamzaAbove = 0xFE83,
            /// <summary>Arabic Letter Waw With Hamza Above Isolated Form : ؤ</summary>
            WawHamzaAbove = 0xFE85,
            /// <summary>Arabic Letter Alef With Hamza Below : إ</summary>
            AlefHamzaBelow = 0xFE87,
            /// <summary>Arabic Letter Yeh With Hamza Above Isolated Form : ئ</summary>
            YehHamzaAboove = 0xFE89,
            /// <summary>Arabic Letter Alef Isolated Form : ا</summary>
            Alef = 0xFE8D,
            /// <summary>Arabic Letter Beh Isolated Form : ب</summary>
            Beh = 0xFE8F,
            /// <summary>Arabic Letter Teh Marbuta Isolated Form : ة</summary>
            TehMarbuta = 0xFE93,
            /// <summary>Arabic Letter Teh Isolated Form : ت</summary>
            Teh = 0xFE95,
            /// <summary>Arabic Letter Theh Isolated Form : ث</summary>
            Theh = 0xFE99,
            /// <summary>Arabic Letter Jeem Isolated Form : ج</summary>
            Jeem = 0xFE9D,
            /// <summary>Arabic Letter Hah Isolated Form : ح</summary>
            Hah = 0xFEA1,
            /// <summary>Arabic Letter Khah Isolated Form : خ</summary>
            Khah = 0xFEA5,
            /// <summary>Arabic Letter Dal Isolated Form : د</summary>
            Dal = 0xFEA9,
            /// <summary>Arabic Letter Thal Isolated Form : ذ</summary>
            Thal = 0xFEAB,
            /// <summary>Arabic Letter Reh Isolated Form : ر</summary>
            Reh = 0xFEAD,
            /// <summary>Arabic Letter Zain Isolated Form : ز</summary>
            Zain = 0xFEAF,
            /// <summary>Arabic Letter Seen Isolated Form : س</summary>
            Seen = 0xFEB1,
            /// <summary>Arabic Letter Sheen Isolated Form : ش</summary>
            Sheen = 0xFEB5,
            /// <summary>Arabic Letter Sad Isolated Form : ص</summary>
            Sad = 0xFEB9,
            /// <summary>Arabic Letter Dad Isolated Form : ض</summary>
            Dad = 0xFEBD,
            /// <summary>Arabic Letter Tah Isolated Form : ط</summary>
            Tah = 0xFEC1,
            /// <summary>Arabic Letter Zah Isolated Form : ظ</summary>
            Zah = 0xFEC5,
            /// <summary>Arabic Letter Ain Isolated Form : ع</summary>
            Ain = 0xFEC9,
            /// <summary>Arabic Letter Ghain Isolated Form : غ</summary>
            Ghain = 0xFECD,
            /// <summary>Arabic Letter Feh Isolated Form : ف</summary>
            Feh = 0xFED1,
            /// <summary>Arabic Letter Qaf Isolated Form : ق</summary>
            Qaf = 0xFED5,
            /// <summary>Arabic Letter Kaf Isolated Form : ك</summary>
            Kaf = 0xFED9,
            /// <summary>Arabic Letter Lam Isolated Form : ل</summary>
            Lam = 0xFEDD,
            /// <summary>Arabic Letter Meem Isolated Form : م</summary>
            Meem = 0xFEE1,
            /// <summary>Arabic Letter Noon Isolated Form : ن</summary>
            Noon = 0xFEE5,
            /// <summary>Arabic Letter Heh Isolated Form : ه</summary>
            Heh = 0xFEE9,
            /// <summary>Arabic Letter Waw Isolated Form : و</summary>
            Waw = 0xFEED,
            /// <summary>Arabic Letter Alef Maksura Isolated Form : ى</summary>
            AlefMaksura = 0xFEEF,
            /// <summary>Arabic Letter Yeh Isolated Form : ي</summary>
            Yeh = 0xFEF1,

            // Persia Arabic

            /// <summary>Arabic Letter Peh Isolated Form : پ</summary>
            Peh = 0xFB56,
            /// <summary>Arabic Letter Tcheh Isolated Form : چ</summary>
            Tcheh = 0xFB7A,
            /// <summary>Arabic Letter Jeh Isolated Form : ژ</summary>
            Jeh = 0xFB8A,
            /// <summary>Arabic Letter Keheh Isolated Form : ک</summary>
            Keheh = 0xFB8E,
            /// <summary>Arabic Letter Gaf Isolated Form : گ</summary>
            Gaf = 0xFB92,
            /// <summary>Arabic Letter Farsi Yeh Isolated Form : ﯼ</summary>
            FarasiYeh = 0xFBFC,

            // ラーム・アリフも変形はある

            /// <summary>Arabic Ligature Lam With Alef With Madda Above Isolated Form : ﻵ</summary>
            LamAlefMaddaAbove = 0xFEF5,
            /// <summary>Arabic Ligature Lam With Alef With Hamza Above Isolated Form : ﻷ</summary>
            LamAlefHamzaAbove = 0xFEF7,
            /// <summary>Arabic Ligature Lam With Alef With Hamza Below Isolated Form : ﻹ</summary>
            LamAlefHamzaBelow = 0xFEF9,
            /// <summary>Arabic Ligature Lam With Alef Isolated Form : ﻻ</summary>
            LamAlef = 0xFEFB,
        }

        /// <summary>
        /// シャクル
        /// </summary>
        public enum Shakl
        {
            /// <summary>Arabic Fathatan</summary>
            Fathatan = 0x064B,
            /// <summary>Arabic Dammatan</summary>
            Dammatan = 0x064C,
            /// <summary>Arabic Kasratan</summary>
            Kasratan = 0x064D,
            /// <summary>Arabic Fatha</summary>
            Fatha = 0x064E,
            /// <summary>Arabic Damma</summary>
            Damma = 0x064F,
            /// <summary>Arabic Kasra</summary>
            Kasra = 0x0650,
            /// <summary>Arabic Shadda</summary>
            Shadda = 0x0651,
            /// <summary>Arabic Sukun</summary>
            Sukun = 0x0652,
            /// <summary>Arabic Maddah Above</summary>
            MaddahAbove = 0x0653,
            /// <summary>Arabic Hamza Above</summary>
            HamzaAbove = 0x0654,
            /// <summary>Arabic Hamza Below</summary>
            HamzaBelow = 0x0655,
            /// <summary>Arabic Subscript Alef</summary>
            SubscriptAlef = 0x0656,
            /// <summary>Arabic Inverted Damma</summary>
            InvertedDamma = 0x0657,
            /// <summary>Arabic Mark Noon Ghunna</summary>
            MarkNoonGhunna = 0x0658,
            /// <summary>Arabic Zwarakay</summary>
            Zwarakay = 0x0659,
            /// <summary>Arabic Vowel Sign Small V Above</summary>
            VowelSignSmallVAbove = 0x065A,
            /// <summary>Arabic Vowel Sign Inverted Small V Above</summary>
            VowelSignInvertedSmallVAbove = 0x065B,
            /// <summary>Arabic Vowel Sign Dot Below</summary>
            VowelSignDotBelow = 0x065C,
            /// <summary>Arabic Reversed Damma</summary>
            ReversedDamma = 0x065D,
            /// <summary>Arabic Fatha With Two Dots</summary>
            FathaTwoDots = 0x065E,
            /// <summary>Arabic Wavy Hamza Below</summary>
            WavyHamzaBelow = 0x065F,
            /// <summary>Arabic Letter Superscript Alef</summary>
            SuperscriptAlef = 0x0670,
            /// <summary>Arabic Ligature Shadda With Dammatan Isolated Form</summary>
            ShaddaDammatan = 0xFC5E,
            /// <summary>Arabic Ligature Shadda With Kasratan Isolated Form</summary>
            ShaddaKasratan = 0xFC5F,
            /// <summary>Arabic Ligature Shadda With Fatha Isolated Form</summary>
            ShaddaFatha = 0xFC60,
            /// <summary>Arabic Ligature Shadda With Damma Isolated Form</summary>
            ShaddaDamma = 0xFC61,
            /// <summary>Arabic Ligature Shadda With Kasra Isolated Form</summary>
            ShaddaKasra = 0xFC62,
            /// <summary>Arabic Ligature Shadda With Superscript Alef Isolated Form</summary>
            ShaddaSuperscriptAlef = 0xFC63,

        }

#endregion

#region constants

        /// <summary>アラビア語のコードブロックの開始値</summary>
        public static readonly int BlockBegin = 0x0600;

        /// <summary>アラビア語のコードブロックの終了値</summary>
        public static readonly int BlockEnd = 0x06FF;

        /// <summary>プレゼンテーションフォームのアラビア語のコードブロックの開始値</summary>
        public static readonly int PresentationBlockBegin = 0xFB50;

        /// <summary>プレゼンテーションフォームのアラビア語のコードブロックの終了値</summary>
        public static readonly int PresentationBlockEnd = 0xFEFF;

        /// <summary>Unicodeの無効な文字コード</summary>
        public static char InvalidUnicodeCharacter = (char) 0xFFFF;

        /// <summary>アラビア文字の0</summary>
        public static int ArabicDigitZero = 0x0660;

        /// <summary>アラビア文字の9</summary>
        public static int ArabicDigitNine = 0x0669;

#endregion

#region public methods

        /// <summary>
        /// 文字列内にアラビア語の文字が含まれているかを返します。
        /// </summary>
        /// <param name="str">対象の文字列</param>
        /// <returns>アラビア語が含まれていれば、<c>true</c>を返します。</returns>
        public static bool IsExistArabic(string str)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                if (IsArabic(str[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// アラビア語かどうかを返します。
        /// </summary>
        /// <param name="character">文字</param>
        /// <returns>アラビア語として扱う文字であれば、<c>true</c>を返します。</returns>
        public static bool IsArabic(char character)
        {
            int code = (int) character;

            return (code >= BlockBegin && code <= BlockEnd)
                || (code >= PresentationBlockBegin && code <= PresentationBlockEnd);
        }

        /// <summary>
        /// アラビア語かどうかを返します。(アラビア数字を除く)
        /// </summary>
        /// <param name="character">文字</param>
        /// <returns>アラビア語として扱う文字であれば、<c>true</c>を返します。</returns>
        public static bool IsArabicWithoutNumber(char character)
        {
            int code = (int) character;

            // アラビア語のブロック内にいる
            if (code >= BlockBegin && code <= BlockEnd)
            {
                // アラビア文字の数字の場合は、記述が左から右になるので別扱い
                return code < ArabicDigitZero || code > ArabicDigitNine;
            }

            // アラビア語のプレゼンテーションフォームのブロック内にいる
            if (code >= PresentationBlockBegin && code <= PresentationBlockEnd)
            {
                return true;
            }

            return false;
        }

#endregion
    }
}
