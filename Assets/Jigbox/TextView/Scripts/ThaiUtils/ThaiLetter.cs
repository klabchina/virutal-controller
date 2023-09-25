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
    /// タイ語の文字
    /// </summary>
    public static class ThaiLetter
    {
        // タイ語コードブロック
        // https://www.compart.com/en/unicode/block/U+0e00

        /// <summary>
        /// 子音
        /// </summary>
        public enum Consonants
        {
            /// <summary>Thai Character Ko Kai : ก</summary>
            KoKai = 0x0e01,
            /// <summary>Thai Character Kho Khai : ข</summary>
            KhoKhai = 0x0e02,
            /// <summary>Thai Character Kho Khuat : ฃ</summary>
            KhoKhuat = 0x0e03,
            /// <summary>Thai Character Kho Khwai : ค</summary>
            KhoKhwai = 0x0e04,
            /// <summary>Thai Character Kho Khon : ฅ</summary>
            KhoKhon = 0x0e05,
            /// <summary>Thai Character Kho Rankhang : ฆ</summary>
            KhoRankhang = 0x0e06,
            /// <summary>Thai Character Ngo Ngu : ง</summary>
            NgoNgu = 0x0e07,
            /// <summary>Thai Character Cho Chan : จ</summary>
            ChoChan = 0x0e08,
            /// <summary>Thai Character Cho Ching : ฉ</summary>
            ChoChing = 0x0e09,
            /// <summary>Thai Character Cho Chang : ช</summary>
            ChoChang = 0x0e0a,
            /// <summary>Thai Character So So : ซ</summary>
            SoSo = 0x0e0b,
            /// <summary>Thai Character Cho Choe : ฌ</summary>
            ChoChoe = 0x0e0c,
            /// <summary>Thai Character Yo Ying : ญ</summary>
            YoYing = 0x0e0d,
            /// <summary>Thai Character Do Chada : ฎ</summary>
            DoChada = 0x0e0e,
            /// <summary>Thai Character To Patak : ฏ</summary>
            ToPatak = 0x0e0f,
            /// <summary>Thai Character Tho Than : ฐ</summary>
            ThoThan = 0x0e10,
            /// <summary>Thai Character Tho Nangmontho : ฑ</summary>
            ThoNangmontho = 0x0e11,
            /// <summary>Thai Character Tho Phuthao : ฒ</summary>
            ThoPhuthao = 0x0e12,
            /// <summary>Thai Character No Nen : ณ</summary>
            NoNen = 0x0e13,
            /// <summary>Thai Character Do Dek : ด</summary>
            DoDek = 0x0e14,
            /// <summary>Thai Character To Tao : ต</summary>
            ToTao = 0x0e15,
            /// <summary>Thai Character Tho Thung : ถ</summary>
            ThoThung = 0x0e16,
            /// <summary>Thai Character Tho Thahan : ท</summary>
            ThoThahan = 0x0e17,
            /// <summary>Thai Character Tho Thong : ธ</summary>
            ThoThong = 0x0e18,
            /// <summary>Thai Character No Nu : น</summary>
            NoNu = 0x0e19,
            /// <summary>Thai Character Bo Baimai : บ</summary>
            BoBaimai = 0x0e1a,
            /// <summary>Thai Character Po Pla : ป</summary>
            PoPla = 0x0e1b,
            /// <summary>Thai Character Pho Phung : ผ</summary>
            PhoPhung = 0x0e1c,
            /// <summary>Thai Character Fo Fa : ฝ</summary>
            FoFa = 0x0e1d,
            /// <summary>Thai Character Pho Phan : พ</summary>
            PhoPhan = 0x0e1e,
            /// <summary>Thai Character Fo Fan : ฟ</summary>
            FoFan = 0x0e1f,
            /// <summary>Thai Character Pho Samphao : ภ</summary>
            PhoSamphao = 0x0e20,
            /// <summary>Thai Character Mo Ma : ม</summary>
            MoMa = 0x0e21,
            /// <summary>Thai Character Ro Rua : ร</summary>
            RoRua = 0x0e23,
            /// <summary>Thai Character Ru : ฤ</summary>
            Ru = 0x0e24,
            /// <summary>Thai Character Lo Ling : ล</summary>
            LoLing = 0x0e25,
            /// <summary>Thai Character Lu : ฦ</summary>
            Lu = 0x0e26,
            /// <summary>Thai Character So Sala : ศ</summary>
            SoSala = 0x0e28,
            /// <summary>Thai Character So Rusi : ษ</summary>
            SoRusi = 0x0e29,
            /// <summary>Thai Character So Sua : ส</summary>
            SoSua = 0x0e2a,
            /// <summary>Thai Character Ho Hip : ห</summary>
            HoHip = 0x0e2b,
            /// <summary>Thai Character Lo Chula : ฬ</summary>
            LoChula = 0x0e2c,
            /// <summary>Thai Character Ho Nokhuk : ฮ</summary>
            HoNokhuk = 0x0e2e,
        }

        public enum ExConstants
        {
            /// <summary>ญ(YoYing)の第四層部分を削除した文字</summary>
            YoYing = 0xe001,
            /// <summary>ฐ(ThoThan)の第四層部分を削除した文字</summary>
            ThoThan = 0xe002,
        }

        /// <summary>
        /// 母音になる可能性のある子音
        /// </summary>
        public enum VagueConsonants
        {
            /// <summary>Thai Character Yo Yak : ย</summary>
            YoYak = 0x0e22,
            /// <summary>Thai Character Wo Waen : ว</summary>
            WoWaen = 0x0e27,
            /// <summary>Thai Character O Ang : อ</summary>
            OAng = 0x0e2d,
        }

        // 母音がどこに付くか
        // http://el.minoh.osaka-u.ac.jp/flc/tha/pandl/05.html

        /// <summary>
        /// 子音の左に付く母音
        /// </summary>
        public enum LeftVowels
        {
            /// <summary>Thai Character Sara E : เ</summary>
            SaraE = 0x0e40,
            /// <summary>Thai Character Sara Ae : แ</summary>
            SaraAe = 0x0e41,
            /// <summary>Thai Character Sara O : โ</summary>
            SaraO = 0x0e42,
            /// <summary>Thai Character Sara Ai Maimuan : ใ</summary>
            SaraAiMaimuan = 0x0e43,
            /// <summary>Thai Character Sara Ai Maimulai : ไ</summary>
            SaraAiMaimulai = 0x0e44,
        }

        /// <summary>
        /// 子音の右に付く母音
        /// </summary>
        public enum RightVowels
        {
            /// <summary>Thai Character Sara A : ะ</summary>
            SaraA = 0x0e30,
            /// <summary>Thai Character Sara Aa : า</summary>
            SaraAa = 0x0e32,
            /// <summary>
            /// Thai Character Lakkhangyao : ๅ
            /// ฤ(Ru)、ฦ(Lu) に า(Sara Aa) が付く場合はこちらを使う
            /// </summary>
            Lakkhangyao = 0x0e45,
        }

        /// <summary>
        /// 子音の右に付く母音で上に付く母音を含む特殊なもの
        /// </summary>
        public enum SaraAm
        {
            /// <summary>
            /// Thai Character Sara Am : ำ
            /// 子音によって、ํ(Nikhahit) と า(Sara Aa) に分解してレイアウトする
            /// </summary>
            SaraAm = 0x0e33,
        }

        /// <summary>
        /// 子音の上に付く母音
        /// </summary>
        public enum TopVowels
        {
            /// <summary>Thai Character Mai Han-Akat : ั</summary>
            MaiHanAkat = 0x0e31,
            /// <summary>Thai Character Sara I : ิ</summary>
            SaraI = 0x0e34,
            /// <summary>Thai Character Sara Ii : ี</summary>
            SaraIi = 0x0e35,
            /// <summary>Thai Character Sara Ue : ึ</summary>
            SaraUe = 0x0e36,
            /// <summary>Thai Character Sara Uee : ื</summary>
            SaraUee = 0x0e37,
            /// <summary>Thai Character Maitaikhu : ็</summary>
            Maitaikhu = 0x0e47,
            /// <summary>
            /// Thai Character Nikhahit : ํ
            /// 分類上、鼻音記号というものになるけど、レイアウト的には母音扱いなのでこの分類にする
            /// </summary>
            Nikhahit = 0x0e4d,
            /// <summary>
            /// Thai Character Yamakkan : ๎
            /// 母音ではないが、母音と同じレイアウト領域を使うため、重複しないのでこの分類とする
            /// </summary>
            Yamakkan = 0x0e4e,
        }

        /// <summary>
        /// 子音の下に付く母音
        /// </summary>
        public enum BottomVowels
        {
            /// <summary>Thai Character Sara U : ุ</summary>
            SaraU = 0x0e38,
            /// <summary>Thai Character Sara Uu : ู</summary>
            SaraUu = 0x0e39,
            /// <summary>Thai Character Phinthu : ฺ</summary>
            Phinthu = 0x0e3a,
        }

        /// <summary>
        /// 声調記号
        /// </summary>
        public enum ToneMarks
        {
            /// <summary>Thai Character Mai Ek : ่</summary>
            MaiEk = 0x0e48,
            /// <summary>Thai Character Mai Tho : ้</summary>
            MaiTho = 0x0e49,
            /// <summary>Thai Character Mai Tri : ๊</summary>
            MaiTri = 0x0e4a,
            /// <summary>Thai Character Mai Chattawa : ๋</summary>
            MaiChattawa = 0x0e4b,
            /// <summary>
            /// Thai Character Thanthakhat : ์
            /// 黙字記号なので、声調記号ではないが、声調記号と同じレイアウト領域を使うため、重複しないのでこの分類とする
            /// </summary>
            Thanthakhat = 0x0e4c,
        }
    }
}
