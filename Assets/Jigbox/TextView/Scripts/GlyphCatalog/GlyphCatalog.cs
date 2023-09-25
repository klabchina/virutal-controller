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
using UnityEngine;
using System.Text;

namespace Jigbox.TextView
{
    /// <summary>
    /// 入力に対応するGlyphをすべて含む辞書を管理するクラス
    /// </summary>
    public class GlyphCatalog
    {
#region constants

        /// <summary>
        /// <para>一度にリクエストできる文字の最大数</para>
        /// <para>TextGeneratorが16375文字以上を一度にリクエストするとエラーを吐くため</para>
        /// </summary>
        protected static readonly int RequestStringLengthLimit = 16000;

#endregion

#region properties

        /// <summary>現在利用されているフォント毎のプロバイダ</summary>
        static readonly Dictionary<Font, GlyphCatalog> catalogs = new Dictionary<Font, GlyphCatalog>();
        
        /// <summary>StringBuilder</summary>
        static StringBuilder stringBuilder = new StringBuilder();

        /// <summary>フォント</summary>
        public Font Font { get; protected set; }

        /// <summary>TextGenerator</summary>
        readonly TextGenerator generator;

        /// <summary>グリフの一覧</summary>
        Dictionary<FontGlyphSpec, Glyph> glyphs = new Dictionary<FontGlyphSpec, Glyph>();

        /// <summary>フォントテクスチャのリビルドが行われたかどうか</summary>
        bool hasFontRebuilt = false;

#endregion

#region public methods

        /// <summary>
        /// カタログを取得します。
        /// </summary>
        /// <param name="font">利用するフォント</param>
        /// <returns>フォントに合ったカタログのインスタンスを返します。</returns>
        public static GlyphCatalog GetCatalog(Font font)
        {
            GlyphCatalog catalog;

            if (catalogs.TryGetValue(font, out catalog))
            {
                return catalog;
            }

            catalog = new GlyphCatalog(font);
            catalogs.Add(font, catalog);

            return catalog;
        }

        /// <summary>
        /// Glyphを生成します。
        /// </summary>
        /// <param name="specs">フォントスペック一覧</param>
        /// <returns>生成途中でフォントのリビルドが発生した場合、処理を中断して<c>false</c>を返します。</returns>
        public bool CreateGlyphs(IList<FontGlyphSpec> specs)
        {
            var specsCount = specs.Count;
            if (specsCount == 0)
            {
                return true;
            }

            // このメソッド以外のタイミングで発生したものに関してはここでの処理には考慮しない
            hasFontRebuilt = false;

            stringBuilder.Length = 0;
            var baseSpecIndex = -1;
            FontGlyphSpec baseSpec = null;

            for (var i = 0; i < specsCount; i++)
            {
                stringBuilder.Append(specs[i].Character);

                // リクエストまとめ中の文字がない場合、現在の文字をまとめの基準文字にする
                if (baseSpecIndex == -1)
                {
                    baseSpecIndex = i;
                    baseSpec = specs[baseSpecIndex];
                }

                // 一度にリクエストできる文字の最大数を超えてなければ、まとめられるかの判断をする
                // 最大数を超える場合は、判定を行わずに一度リクエストを行う
                if (i - baseSpecIndex < RequestStringLengthLimit)
                {
                    // 最後の文字でない限り、次の文字とまとめてリクエストできるか確認する
                    var nextSpecIndex = i + 1;
                    if (nextSpecIndex < specsCount)
                    {
                        var nextSpec = specs[nextSpecIndex];

                        if (baseSpec.Size == nextSpec.Size && baseSpec.Style == nextSpec.Style)
                        {
                            continue;
                        }
                    }
                }

                var requestString = stringBuilder.ToString();
                var setting = GetTextGeneratorSetting(this.Font, baseSpec.Size, baseSpec.Style);
                // 同じ内容をリクエストし続ける場合は、Invalidate()しない方がいいが
                // 違う内容をリクエストする際は、余計な内部比較を発生させる分パフォーマンスが低下するので
                // 比較はしないようにInvalidate()を呼び出している
                // また、フォントのリビルドが発生した場合には、内容如何に関わらず、キャッシュが使えなくなるので
                // 強制的にキャッシュを無効化する
                this.generator.Invalidate();
                this.generator.Populate(requestString, setting);

                if (hasFontRebuilt)
                {
                    return false;
                }

                var requestStringLength = requestString.Length;
                for (var j = 0; j < requestStringLength; j++)
                {
                    var spec = specs[baseSpecIndex + j];
                    if (glyphs.ContainsKey(spec))
                    {
                        continue;
                    }

                    // 本来TextGeneratorから渡されたUIVertexだけで十分だが、
                    // TextViewでは元々CharacterInfoからUIVertexを生成していた関係で、
                    // TextGeneratorのUIVertexでレンダリングするとズレが発生する。
                    // そのため、多少無駄なコストは発生するがレンダリング状態を保持するために
                    // CharacterInfoを取得して、その情報を使ってUIVertexを加工するようにしている
                    CharacterInfo characterInfo;
                    Font.GetCharacterInfo(spec.Character, out characterInfo, spec.Size, spec.Style);

                    glyphs.Add(spec, new Glyph(spec, characterInfo));
                }

                // リクエストを行ったので、リクエストまとめ中の文字をクリア
                stringBuilder.Length = 0;
                baseSpecIndex = -1;
                baseSpec = null;
            }

            return true;
        }

        /// <summary>
        /// 保持しているGlyphをクリアします。
        /// </summary>
        public void Clear()
        {
            glyphs.Clear();
        }

        /// <summary>
        /// <para>Glyphを取得します。</para>
        /// <para>CreateGlyphで予め生成Glyphが生成されていない場合は、nullを返します。</para>
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public Glyph Get(FontGlyphSpec spec)
        {
            Glyph glyph;
            glyphs.TryGetValue(spec, out glyph);
#if UNITY_EDITOR
            if (glyph == null)
            {
                Debug.LogError("That FontGlyphSpec did not be requested CreateGlyph!");
            }
#endif
            return glyph;
        }

#endregion

#region private methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="font">フォント</param>
        private GlyphCatalog(Font font)
        {
            this.Font = font;
            this.generator = new TextGenerator();
            Font.textureRebuilt += OnFontRebuilt;
        }

        /// <summary>
        /// ダイナミックフォントのリビルドが発生した際に呼び出されます。
        /// </summary>
        /// <param name="font">リビルドが発生したフォント</param>
        private void OnFontRebuilt(Font font)
        {
            if (this.Font == font)
            {
                hasFontRebuilt = true;
            }
        }

        /// <summary>
        /// TextGeneratorで利用する設定を取得します。
        /// </summary>
        /// <param name="font">フォント</param>
        /// <param name="fontSize">フォントサイズ</param>
        /// <param name="fontStyle">フォントスタイル</param>
        /// <returns>引数で指定された値を適用した設定を返します。</returns>
        private static TextGenerationSettings GetTextGeneratorSetting(
            Font font,
            int fontSize,
            FontStyle fontStyle
        )
        {
            var setting = new TextGenerationSettings();

            setting.font = font;
            setting.fontSize = fontSize;
            setting.fontStyle = fontStyle;

            // UnityEngine.UI.Textと同じようなレンダリング結果を得る場合は設定する必要がある
            // setting.generationExtents = new Vector2(0, fontSize);
            // 5.4.2p4で確認している限りは問題ないが、
            // 5.3.4p2では、TextGenerationSettings.generateOutOfBoundsが正しく効かないようで、
            // 範囲が狭いと頂点が生成されなくなる
            setting.generationExtents = new Vector2(float.MaxValue, float.MaxValue);

            setting.color = Color.white;
            setting.generateOutOfBounds = true;
            setting.alignByGeometry = false;
            setting.resizeTextForBestFit = false;
            setting.updateBounds = false;
            setting.scaleFactor = 1.0f;
            setting.textAnchor = TextAnchor.UpperLeft;

            return setting;
        }

#endregion
    }
}
