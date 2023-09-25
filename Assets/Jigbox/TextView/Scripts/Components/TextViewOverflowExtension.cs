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

using Jigbox.TextView;
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// TextView でテキストが溢れる時の拡張機能を行うためのコンポーネント
    /// </summary>
    /// <remark>
    /// FirstLineWidthの時、Squeezeとシュリンク機能に下記の制約があります。
    /// <list type="bullet">
    ///   <item><description>考慮するのは横幅のみ</description></item>
    ///   <item><description>size タグ・属性がある場合の動作は保証しない</description></item>
    ///   <item><description></description></item>
    /// </list>
    /// </remark>
    [DisallowMultipleComponent]
    public class TextViewOverflowExtension : MonoBehaviour
    {
#region inner classes, enum, and structs

        /// <summary>
        /// AutoLineBreakで使用するEllipsisの情報
        /// </summary>
        public struct AutoLineEllipsisParameter
        {
            /// <summary>AutoLineBreak内での省略文字表示行</summary>
            public readonly int EllipsisLine;

            /// <summary>省略文字表示サイズ</summary>
            public readonly float OffsetWidth;

            public AutoLineEllipsisParameter(TextViewOverflowExtension overflowExtension, int currentAllTextLineSize)
            {
                EllipsisLine = overflowExtension.EllipsisLine - currentAllTextLineSize;
                OffsetWidth = overflowExtension.EllipsisOffsetWidth();
            }
        }

        /// <summary>
        /// Squeezeとシュリンクの基準となるサイズ
        /// </summary>
        public enum SqueezeAndShrinkCriterionSize
        {
            /// <summary>最初の1行の幅</summary>
            FirstLineWidth = 0,

            /// <summary>全行の高さ</summary>
            AllLineHeight = 1,
        }

        public interface ISqueezeParameter
        {
        }

        public class SqueezeFirstLineWidthParameter : ISqueezeParameter
        {
            public float GlyphScaleX { get; protected set; }
            
            /// <summary>表示に必要な幅</summary>
            public float NecessaryWidth { get; protected set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="glyphScaleX">フォントサイズ</param>
            /// <param name="necessaryWidth">表示に必要な幅</param>
            public SqueezeFirstLineWidthParameter(float glyphScaleX, float necessaryWidth)
            {
                GlyphScaleX = glyphScaleX;
                NecessaryWidth = necessaryWidth;
            }
        }

        public class SqueezeAllLineHeightParameter : ISqueezeParameter
        {
            /// <summary>フォントサイズ</summary>
            public float GlyphScaleX { get; protected set; }

            /// <summary>表示に必要な幅</summary>
            public float NecessaryWidth { get; protected set; }

            /// <summary>表示に必要な高さ</summary>
            public float NecessaryHeight { get; protected set; }
            
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="glyphScaleX">フォントサイズ</param>
            /// <param name="necessaryWidth">表示に必要な幅</param>
            /// <param name="necessaryHeight">表示に必要な高さ</param>
            public SqueezeAllLineHeightParameter(float glyphScaleX, float necessaryWidth, float necessaryHeight)
            {
                GlyphScaleX = glyphScaleX;
                NecessaryWidth = necessaryWidth;
                NecessaryHeight = necessaryHeight;
            }
        }

        public interface IShrinkParameter
        {
        }

        public class ShrinkFirstLineWidthParameter : IShrinkParameter
        {
            /// <summary>フォントサイズ</summary>
            public int FontSize { get; protected set; }

            /// <summary>表示に必要な幅</summary>
            public float NecessaryWidth { get; protected set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="fontSize">フォントサイズ</param>
            /// <param name="necessaryWidth">表示に必要な幅</param>
            public ShrinkFirstLineWidthParameter(int fontSize, float necessaryWidth)
            {
                FontSize = fontSize;
                NecessaryWidth = necessaryWidth;
            }
        }

        public class ShrinkAllLineHeightParameter : IShrinkParameter
        {
            /// <summary>フォントサイズ</summary>
            public int FontSize { get; protected set; }

            /// <summary>表示に必要な高さ</summary>
            public float NecessaryHeight { get; protected set; }

            /// <summary>自動改行が行われていない論理行としての高さ</summary>
            public float LogicalLineHeight { get; protected set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="fontSize">フォントサイズ</param>
            /// <param name="necessaryHeight">表示に必要な高さ</param>
            /// <param name="logicalLineHeight">自動改行が行われていない論理行としての高さ</param>
            public ShrinkAllLineHeightParameter(int fontSize, float necessaryHeight, float logicalLineHeight)
            {
                FontSize = fontSize;
                NecessaryHeight = necessaryHeight;
                LogicalLineHeight = logicalLineHeight;
            }
        }

#endregion

#region properties

        /// <summary>省略文字</summary>
        protected static readonly char EllipsisChar = '…';

        /// <summary>Ellipsisが有効かどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool ellipsisEnabled = false;

        /// <summary>Ellipsisが有効かどうか</summary>
        public bool EllipsisEnabled { get { return ellipsisEnabled; } set { ellipsisEnabled = value; } }

        /// <summary>Squeezeが有効かどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool squeezeEnabled = false;

        /// <summary>Squeezeが有効かどうか</summary>
        public bool SqueezeEnabled { get { return squeezeEnabled; } set { squeezeEnabled = value; } }

        /// <summary>Squeezeを行う最大値(0~50)</summary>
        [HideInInspector]
        [SerializeField]
        protected int squeezeMaxSize = 50;

        /// <summary>Squeezeを行う最大値(0~50)</summary>
        public int SqueezeMaxSize { get { return squeezeMaxSize; } }

        /// <summary>Squeezeを行うステップ値(0~squeezeMaxSize)</summary>
        [HideInInspector]
        [SerializeField]
        protected int squeezeStepSize = 1;

        /// <summary>Squeezeを行うステップ値(0~squeezeMaxSize)</summary>
        public int SqueezeStepSize { get { return squeezeStepSize; } }

        /// <summary>シュリンクが有効かどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool shrinkEnabled = true;

        /// <summary>シュリンクが有効かどうか</summary>
        public bool ShrinkEnabled { get { return shrinkEnabled; } set { shrinkEnabled = value; } }

        /// <summary>シュリンクの最小フォントサイズ</summary>
        [HideInInspector]
        [SerializeField]
        protected int shrinkMinFontSize = 1;

        /// <summary>シュリンクの最小フォントサイズ</summary>
        public int ShrinkMinFontSize { get { return shrinkMinFontSize; } set { shrinkMinFontSize = value; } }

        /// <summary>Squeezeとシュリンクの基準となるサイズ</summary>
        [HideInInspector]
        [SerializeField]
        protected SqueezeAndShrinkCriterionSize criterionSize = SqueezeAndShrinkCriterionSize.FirstLineWidth;

        /// <summary>Squeezeとシュリンクの基準となるサイズ</summary>
        public virtual SqueezeAndShrinkCriterionSize CriterionSize { get { return criterionSize; } }

        /// <summary>Squeeze後のGlyphScale</summary>
        protected float? cachedSqueezedSize;

        /// <summary>Squeeze後のGlyphScale</summary>
        public float SqueezedSize { get { return cachedSqueezedSize ?? 1.0f; } }

        /// <summary>シュリンク後のフォントサイズ</summary>
        protected int? cachedShrinkedSize;

        /// <summary>シュリンク後のフォントサイズ</summary>
        public int ShrinkedSize { get { return cachedShrinkedSize ?? 1; } }

        /// <summary>RectTransform</summary>
        protected RectTransform rectTransform;

        /// <summary>RectTransform</summary>
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = transform as RectTransform;
                }

                return rectTransform;
            }
        }

        /// <summary>EllipsisFlowに入ったか</summary>
        public bool IsEllipsisFlow { get { return EllipsisLine >= 0; } }

        /// <summary>allTextLinesにおける省略文字を表示するindex -1は表示なし</summary>
        public int EllipsisLine { get; private set; }

        /// <summary>横幅</summary>
        protected float characterSpacing;

        /// <summary>SpacingUnit</summary>
        protected SpacingUnit spacingUnit;

        /// <summary>省略文字のGlyphSpec</summary>
        protected FontGlyphSpec fontGlyphSpec;

#endregion

#region public methods

        /// <summary>
        /// 省略文字のFontGlyphSpecを生成する
        /// </summary>
        /// <param name="font">使用フォント</param>
        /// <param name="fontSize">フォントサイズ</param>
        /// <param name="fontStyle">フォントサイズ</param>
        /// <param name="color">色</param>
        /// <param name="glyphScaleX">グリフのX幅倍率</param>
        /// <returns>FontGlyphSpec</returns>
        public virtual FontGlyphSpec CreateFontGlyphSpec(Font font, int fontSize, FontStyle fontStyle, Color color, float glyphScaleX)
        {
            fontGlyphSpec = new FontGlyphSpec(EllipsisChar, font, fontSize, fontStyle, color, glyphScaleX);
            return fontGlyphSpec;
        }

        /// <summary>
        /// 省略文字のGlyphを取得する
        /// </summary>
        /// <returns></returns>
        public virtual Glyph GetEllipsisGlyph()
        {
            return GlyphCatalog.GetCatalog(fontGlyphSpec.Font).Get(fontGlyphSpec);
        }

        /// <summary>
        /// 省略文字表示用の領域を確保する為の横幅を返します
        /// </summary>
        /// <returns></returns>
        public virtual float EllipsisOffsetWidth()
        {
            var glyph = GetEllipsisGlyph();
            float offsetWidth = 0.0f;
            switch (spacingUnit)
            {
                case SpacingUnit.BeforeCharacterWidth:
                    // NOTE: intへのキャストは切り捨てされていた部分をそのままにして既存の挙動が変わらないようにしています
                    offsetWidth = (int) (characterSpacing * glyph.Width);
                    break;
                case SpacingUnit.FontSize:
                    // NOTE: intへのキャストは切り捨てされていた部分をそのままにして既存の挙動が変わらないようにしています
                    offsetWidth = (int) (characterSpacing * fontGlyphSpec.Size) * fontGlyphSpec.GlyphScaleX;
                    break;
            }

            return glyph.Width + offsetWidth;
        }

        /// <summary>
        /// Ellipsisを行うのに必要な設定を登録します
        /// </summary>
        /// <param name="ellipsisLine">省略文字表示行数</param>
        /// <param name="characterSpacing">文字間</param>
        /// <param name="spacingUnit">文字間のパターン</param>
        public virtual void SetEllipsisParameter(int ellipsisLine, float characterSpacing, SpacingUnit spacingUnit)
        {
            EllipsisLine = ellipsisLine;
            this.characterSpacing = characterSpacing;
            this.spacingUnit = spacingUnit;
        }

        /// <summary>
        /// Squeezeの計算を行い、適切なGlyphScaleXを返します
        /// </summary>
        /// <param name="parameter">Squeeze計算に必要なパラメータ</param>
        /// <returns></returns>
        public virtual float CalculateSqueezedSize(ISqueezeParameter parameter)
        {
            switch (CriterionSize)
            {
                case SqueezeAndShrinkCriterionSize.FirstLineWidth:
                    return CalculateAtFirstLineWidth(parameter as SqueezeFirstLineWidthParameter);
                case SqueezeAndShrinkCriterionSize.AllLineHeight:
                    return CalculateAtAllLineHeight(parameter as SqueezeAllLineHeightParameter);
                default:
                    UnityEngine.Assertions.Assert.IsTrue(false, "If you support all SqueezeAndShrinkCriterionSize, you will not reach here.");
                    return 1.0f;
            }
        }

        /// <summary>
        /// シュリンク計算を行い、適正なフォントサイズを返します。
        /// </summary>
        /// <param name="parameter">シュリンク計算に必要なパラメータ</param>
        /// <returns></returns>
        public virtual int CalculateShrinkedFontSize(IShrinkParameter parameter)
        {
            switch (CriterionSize)
            {
                case SqueezeAndShrinkCriterionSize.FirstLineWidth:
                    return CalculateAtFirstLineWidth(parameter as ShrinkFirstLineWidthParameter);
                case SqueezeAndShrinkCriterionSize.AllLineHeight:
                    return CalculateAtAllLineHeight(parameter as ShrinkAllLineHeightParameter);
                default:
                    UnityEngine.Assertions.Assert.IsTrue(false, "If you support all SqueezeAndShrinkCriterionSize, you will not reach here.");
                    return 1;
            }
        }
        
        /// <summary>
        /// Squeezeとシュリンクの再計算を行うようにします。
        /// </summary>
        public virtual void RequireCalculate()
        {
            cachedSqueezedSize = null;
            cachedShrinkedSize = null;
        }

        /// <summary>
        /// Squeezeを不要とします。
        /// </summary>
        /// <param name="glyphScaleX">Glyph倍率</param>
        public void UnnecessarySqueeze(float glyphScaleX)
        {
            cachedSqueezedSize = glyphScaleX;
        }

        /// <summary>
        /// Squeezeが必要かどうかを返します。
        /// </summary>
        /// <returns></returns>
        public virtual bool IsNeedSqueeze()
        {
            return this.squeezeEnabled && cachedSqueezedSize == null;
        }

        /// <summary>
        /// シュリンクを不要とします。
        /// </summary>
        /// <param name="fontSize">フォントサイズ</param>
        public void UnnecessaryShrink(int fontSize)
        {
            cachedShrinkedSize = fontSize;
        }
        
        /// <summary>
        /// シュリンクが必要かどうかを返します。
        /// </summary>
        /// <returns></returns>
        public virtual bool IsNeedShrink()
        {
            return this.shrinkEnabled && cachedShrinkedSize == null;
        }

#endregion

#region protected methods
        
        /// <summary>
        /// 最初の行の幅を基準にシュリンク計算を行い、適正なフォントサイズを返します。
        /// </summary>
        /// <param name="parameter">シュリンク計算に必要なパラメータ</param>
        /// <returns></returns>
        protected float CalculateAtFirstLineWidth(SqueezeFirstLineWidthParameter parameter)
        {
            if (cachedSqueezedSize == null)
            {
                cachedSqueezedSize = 1.0f;
            }

            float viewWidth = RectTransform.rect.width;

            if (viewWidth < parameter.NecessaryWidth)
            {
                var nextSqueezedSize =  parameter.GlyphScaleX - SqueezeStepSize / 100.0f;
                cachedSqueezedSize = Mathf.Max(nextSqueezedSize, 1.0f - SqueezeMaxSize / 100.0f);
            }
            else
            {
                cachedSqueezedSize = parameter.GlyphScaleX;
            }

            return cachedSqueezedSize.Value;
        }
        
        /// <summary>
        /// 全行の高さを基準にSqueeze計算を行い、適正なGlyphScaleXを返します。
        /// </summary>
        /// <param name="parameter">Squeeze計算に必要なパラメータ</param>
        /// <returns></returns>
        protected float CalculateAtAllLineHeight(SqueezeAllLineHeightParameter parameter)
        {
            float viewHeight = RectTransform.rect.height;
            float viewWidth = RectTransform.rect.width;

            // 物理行全体の高さが表示領域の高さ以下の場合はSqueezeしなくても全て表示できるはず
            if (parameter.NecessaryHeight <= viewHeight)
            {
                cachedSqueezedSize = parameter.GlyphScaleX;
            }
            // 横幅は足りているが高さが足りていない場合はSqueezeは必要ない(シュリンクは適用できる)
            else if (parameter.NecessaryWidth <= viewWidth && parameter.NecessaryHeight > viewHeight)
            {
                cachedSqueezedSize = parameter.GlyphScaleX;
            }
            else
            {
                var nextSqueezedSize =  parameter.GlyphScaleX - SqueezeStepSize / 100.0f;
                cachedSqueezedSize = Mathf.Max(nextSqueezedSize, 1.0f - SqueezeMaxSize / 100.0f);
            }

            return cachedSqueezedSize.Value;
        }
        
        /// <summary>
        /// 最初の行の幅を基準にシュリンク計算を行い、適正なフォントサイズを返します。
        /// </summary>
        /// <param name="parameter">シュリンク計算に必要なパラメータ</param>
        /// <returns></returns>
        protected int CalculateAtFirstLineWidth(ShrinkFirstLineWidthParameter parameter)
        {
            float viewWidth = RectTransform.rect.width;

            if (viewWidth < parameter.NecessaryWidth)
            {
                int shrinkedSize = Mathf.FloorToInt(viewWidth / parameter.NecessaryWidth * parameter.FontSize);
                // floatの誤差によって差が出ない場合、強制的にフォントサイズを小さくする
                if (shrinkedSize == parameter.FontSize)
                {
                    --shrinkedSize;
                }

                cachedShrinkedSize = Mathf.Max(shrinkedSize, ShrinkMinFontSize);
            }
            else
            {
                cachedShrinkedSize = parameter.FontSize;
            }

            return cachedShrinkedSize.Value;
        }

        /// <summary>
        /// 全行の高さを基準にシュリンク計算を行い、適正なフォントサイズを返します。
        /// </summary>
        /// <param name="parameter">シュリンク計算に必要なパラメータ</param>
        /// <returns></returns>
        protected int CalculateAtAllLineHeight(ShrinkAllLineHeightParameter parameter)
        {
            float viewHeight = RectTransform.rect.height;

            // 物理行全体の高さが表示領域の高さ以下の場合はシュリンクしなくても全て表示できるはず
            if (parameter.NecessaryHeight <= viewHeight)
            {
                cachedShrinkedSize = parameter.FontSize;
            }
            else
            {
                // 論理行の高さが表示領域の高さ以下の場合は、物理行にした際に
                // 自動改行が発生した結果、表示領域内に収まらなくなっている
                if (parameter.LogicalLineHeight <= viewHeight)
                {
                    // 自動改行時の禁則処理の適用条件なども考慮すると
                    // 計算で適正値まで下げるのは不可能に近いので、
                    // 単純に小さくしてリトライを繰り返す
                    cachedShrinkedSize = Mathf.Max(parameter.FontSize - 1, ShrinkMinFontSize);
                }
                // 論理行の高さが表示領域の高さより大きい場合は、単純なテキスト量が多すぎるので高さを割合で削る
                else
                {
                    int shrinkedSize = Mathf.FloorToInt(viewHeight / parameter.LogicalLineHeight * parameter.FontSize);
                    // floatの誤差によって差が出ない場合、強制的にフォントサイズを小さくする
                    if (shrinkedSize == parameter.FontSize)
                    {
                        --shrinkedSize;
                    }

                    cachedShrinkedSize = Mathf.Max(shrinkedSize, ShrinkMinFontSize);
                }
            }

            return cachedShrinkedSize.Value;
        }

#endregion
    }
}
