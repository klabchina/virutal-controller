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

using UnityEngine;

namespace Jigbox.Examples
{
    // 元ネタ
    // https://ja.wikipedia.org/wiki/%E5%9B%BD%E9%80%A3%E3%81%AB%E3%82%88%E3%82%8B%E4%B8%96%E7%95%8C%E5%9C%B0%E7%90%86%E5%8C%BA%E5%88%86

    /// <summary>
    /// 大州
    /// </summary>
    public class ExampleContinent
    {
        /// <summary>ID</summary>
        public int Id { get; set; }

        /// <summary>名前</summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 小地域
    /// </summary>
    public class ExampleSmallArea
    {
        /// <summary>ID</summary>
        public int Id { get; set; }

        /// <summary>大州ID</summary>
        public int ContinentId { get; set; }

        /// <summary>名前</summary>
        public string Name { get; set; }

        /// <summary>詳細</summary>
        public string Detail { get; set; }
    }

    /// <summary>
    /// カラーサンプル
    /// </summary>
    public class ExampleColorItem
    {
        public int Index { get; set; }

        public Color Color { get; set; }
    }

    public class ExampleDataGenerator
    {
        public static readonly ExampleContinent[] Continents = new ExampleContinent[]
        {
            new ExampleContinent()
            {
                Id = 1,
                Name = "アフリカ州",
            },
            new ExampleContinent()
            {
                Id = 2,
                Name = "アメリカ州",
            },
            new ExampleContinent()
            {
                Id = 3,
                Name = "南極大陸",
            },
            new ExampleContinent()
            {
                Id = 4,
                Name = "アジア州",
            },
            new ExampleContinent()
            {
                Id = 5,
                Name = "ヨーロッパ州",
            },
            new ExampleContinent()
            {
                Id = 6,
                Name = "オセアニア",
            },
        };


        public static readonly ExampleSmallArea[] SmallAreas = new ExampleSmallArea[]
        {
            new ExampleSmallArea()
            {
                Id = 10,
                ContinentId = 1,
                Name = "東アフリカ",
                Detail = "東アフリカ（ひがしアフリカ、スワヒリ語: Afrika ya mashariki）は、アフリカの東部地域、おおむね大地溝帯から東側の地域を指す。",
            },
            new ExampleSmallArea()
            {
                Id = 20,
                ContinentId = 1,
                Name = "中部アフリカ",
                Detail = "中部アフリカ（ちゅうぶアフリカ）は、アフリカ大陸を5つの地域に分けた場合の1地域を指す用語である。赤道付近の南北に位置することから、赤道アフリカと呼ばれることもある。",
            },
            new ExampleSmallArea()
            {
                Id = 30,
                ContinentId = 1,
                Name = "北アフリカ",
                Detail = "北アフリカ（きたアフリカ）は、アフリカのうちサハラ砂漠より北の地域を指す。また、狭義には西端部のマグリブ地域のみを指す場合もある。エジプトやリビアを中心に中東の一部として定義されることも多い。サハラ砂漠をはじめとした砂漠地帯やステップが大部分を占めるが、地中海を挟んでEU諸国と対しており、モロッコやチュニジアのように経済が比較的発達している国が多い。住民にはアラブ系のコーカソイド（アフロ・アジア語族）が多いため、ホワイトアフリカともよばれる。",
            },
            new ExampleSmallArea()
            {
                Id = 40,
                ContinentId = 1,
                Name = "南部アフリカ",
                Detail = "南部アフリカ（なんぶアフリカ）は、アフリカ大陸を5つの地域に分けた場合の最も南に位置する地域を指す用語である。",
            },
            new ExampleSmallArea()
            {
                Id = 50,
                ContinentId = 1,
                Name = "西アフリカ",
                Detail = "東アフリカ（ひがしアフリカ、スワヒリ語: Afrika ya mashariki）は、アフリカの東部地域、おおむね大地溝帯から東側の地域を指す。",
            },
            // アメリカ州
            new ExampleSmallArea()
            {
                Id = 60,
                ContinentId = 2,
                Name = "ラテンアメリカ・カリブ海地域",
                Detail = "ラテンアメリカ（西: Latinoamérica, América Latina, 葡: América Latina, 英: Latin America, 仏: Amérique latine）は、アングロアメリカに対する概念で、アメリカ大陸の北半球中緯度から南半球にかけて存在する独立国及び非独立地域を指す総称である[1]。",
            },
            new ExampleSmallArea()
            {
                Id = 70,
                ContinentId = 2,
                Name = "北部アメリカ",
                Detail = "北部アメリカ（ほくぶアメリカ、英語: Northern America）はアメリカ州最北端の地域及び北アメリカ大陸の一部。中部アメリカのすぐ北側に位置している[1]。アメリカ合衆国とメキシコの国境が北部アメリカ及び中部アメリカの境界となっている。国連による世界地理区分による大州・小地域の区分に基づく政治地理学の分類では、北部アメリカは[2][3] バミューダ、カナダ、グリーンランド、サンピエール島・ミクロン島及びアメリカ合衆国によって構成されている。",
            },
            new ExampleSmallArea()
            {
                Id = 80,
                ContinentId = 2,
                Name = "北アメリカ",
                Detail = "北アメリカ（きたアメリカ、英: North America、西: América del Norte、仏: Amérique du Nord）は、アメリカ（米州）の北半の、北アメリカ大陸を中心とした地域である。六大州の1つ。漢字では北米（ほくべい）と表す。",
            },
            // 南極
            new ExampleSmallArea()
            {
                Id = 90,
                ContinentId = 3,
                Name = "南極",
                Detail = "南極（なんきょく、英: Antarctic）とは、地球上の南極点、もしくは南極点を中心とする南極大陸およびその周辺の島嶼・海域（南極海）などを含む地域を言う。南極点を中心に南緯66度33分までの地域については南極圏と呼ぶ。南緯50度から60度にかけて不規則な形状を描く氷塊の不連続線である南極収束線があり、これより南を南極地方とも呼ぶ。南極地方には、南極大陸を中心に南極海を含み、太平洋、インド洋、大西洋の一部も属する。",
            },
            // アジア
            new ExampleSmallArea()
            {
                Id = 100,
                ContinentId = 4,
                Name = "中央アジア",
                Detail = "中央アジア（ちゅうおうアジア）は、ユーラシア大陸またアジア中央部の内陸地域である。18世紀から19世紀にかけては一般にトルキスタンを指したが[2]、現在でも使用される。トルキスタンとは「テュルクの土地」を意味し、テュルク（突厥他）系民族が居住しており、西トルキスタンと東トルキスタンの東西に分割している。",
            },
            new ExampleSmallArea()
            {
                Id = 110,
                ContinentId = 4,
                Name = "東アジア",
                Detail = "東アジア（ひがしアジア）は、ユーラシア大陸の東部にあたるアジア地域の一部を指す地理学的な名称である。北西からモンゴル高原、中国大陸、朝鮮半島、台湾列島、日本列島などを含む[4]。北東アジア（東北アジア）、極東、東亜などと呼ぶ場合もある。",
            },
            new ExampleSmallArea()
            {
                Id = 120,
                ContinentId = 4,
                Name = "南アジア",
                Detail = "南極（なんきょく、英: Antarctic）とは、地球上の南極点、もしくは南極点を中心とする南極大陸およびその周辺の島嶼・海域（南極海）などを含む地域を言う。南極点を中心に南緯66度33分までの地域については南極圏と呼ぶ。南緯50度から60度にかけて不規則な形状を描く氷塊の不連続線である南極収束線があり、これより南を南極地方とも呼ぶ。南極地方には、南極大陸を中心に南極海を含み、太平洋、インド洋、大西洋の一部も属する。",
            },
            new ExampleSmallArea()
            {
                Id = 130,
                ContinentId = 4,
                Name = "東南アジア",
                Detail = "東南アジア（とうなんアジア）は、中国より南、インドより東のアジア地域を指す。インドシナ半島、マレー半島、インドネシア諸島、フィリピン諸島アジアと島嶼部東南アジアに分けられる。",
            },
            new ExampleSmallArea()
            {
                Id = 140,
                ContinentId = 4,
                Name = "西アジア",
                Detail = "西アジア（にしアジア）は、アジア西部を指す地理区分である。今日の欧米ではほぼ中東と同じ領域を指す場合が多い。一般的には、中央アジアおよび南アジアより西、地中海より東で、ヨーロッパとはボスポラス海峡、アフリカとはスエズ運河によって隔てられている地域を指す。",
            },
            // ヨーロッパ州
            new ExampleSmallArea()
            {
                Id = 150,
                ContinentId = 5,
                Name = "東ヨーロッパ",
                Detail = "東ヨーロッパ（ひがしヨーロッパ）は、東欧ともいい、ヨーロッパ東部の地域を指す。時代によって「東欧」の概念は大きく変わる。過去の一時期、冷戦時代においては、いわゆる「東側」「西側」という分類のそれを指す場合もあった（「東側諸国」と「西側諸国」の記事も参照）。以下では主に通時的な「東欧」という概念の大まかな変遷を説明する。",
            },
            new ExampleSmallArea()
            {
                Id = 160,
                ContinentId = 5,
                Name = "北ヨーロッパ",
                Detail = "北ヨーロッパ（きたヨーロッパ）は、ヨーロッパの北部地域である。日本では北欧（ほくおう）とも呼ばれる。具体的にどの地方や国を含めるかは、国や国際機関などにより異なる。最も広い場合は、イギリスとアイルランド、そしてドイツやロシアのバルト海沿岸部も含まれる。",
            },
            new ExampleSmallArea()
            {
                Id = 170,
                ContinentId = 5,
                Name = "南ヨーロッパ",
                Detail = "南ヨーロッパ（みなみヨーロッパ、西: Europa del Sur、葡: Europa meridional、伊: Europa meridionale）は、ヨーロッパ地域の南部を指す。南欧ともいう。西洋でも特に歴史が古く、主にラテン語系の言語を母語とする民族が住んでいる地中海沿岸の地域である。",
            },
            new ExampleSmallArea()
            {
                Id = 180,
                ContinentId = 5,
                Name = "西ヨーロッパ",
                Detail = "西ヨーロッパ（にしヨーロッパ、英: Western Europe、仏: L’europe de l'ouest、独: Westeuropa）は、西欧ともいい、ヨーロッパ地域の西部を指す。",
            },
            // オセアニア
            new ExampleSmallArea()
            {
                Id = 190,
                ContinentId = 6,
                Name = "オーストラリア",
                Detail = "オーストラリア連邦（オーストラリアれんぽう、英: Commonwealth of Australia）[2]、またはオーストラリア（英: Australia）[3][4]は、オセアニアに位置し、オーストラリア大陸本土、タスマニア島及び多数の小島から成る連邦立憲君主制国家。首都はキャンベラ。最大の都市はシドニー。",
            },
            new ExampleSmallArea()
            {
                Id = 200,
                ContinentId = 6,
                Name = "メラネシア",
                Detail = "メラネシア（Melanesia）は、オセアニアの海洋部の分類の一つ。 概ね赤道以南、東経180度以西にある島々の総称。オーストラリア大陸より北－北東に位置する。ギリシャ語で μέλας メラス「黒い」+ νῆσος ネソス「島」から「黒い（皮膚の黒い人々が住む）島々」の意味である。",
            },
            new ExampleSmallArea()
            {
                Id = 210,
                ContinentId = 6,
                Name = "ミクロネシア",
                Detail = "ミクロネシア (Micronesia) はオセアニアの海洋部の分類の一つ。カロリン諸島など4つの主要な群島から構成される地域。英語読み（マイクロニージャ）に影響され「マイクロネシア」と呼ばれる場合もある。",
            },
            new ExampleSmallArea()
            {
                Id = 220,
                ContinentId = 6,
                Name = "ポリネシア",
                Detail = "ポリネシア（Polynesia）は、オセアニアの海洋部の分類の一つである。太平洋で、概ねミッドウェー諸島（北西ハワイ諸島内）、アオテアロア（ニュージーランドのマオリ語名）、ラパ・ヌイ（イースター島）を結んだ三角形（ポリネシアン・トライアングル）の中にある諸島の総称で、2017年の人口は約700万人。",
            },
        };
    }
}
