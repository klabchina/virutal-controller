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


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Jigbox.TextView;
using Jigbox.TextView.Markup;
using Jigbox.TextView.HorizontalLayout;
using Jigbox.Delegatable;
using UnityEngine.Serialization;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;
#if UNITY_EDITOR
#if UNITY_2021_1_OR_NEWER
using UnityEditor.SceneManagement;
#else
using UnityEditor.Experimental.SceneManagement;
#endif
#endif

namespace Jigbox.Components
{
    [AddComponentMenu("Jigbox/TextView", 10)]
    [ExecuteAlways]
#if UNITY_2019_4_OR_NEWER
    [RequireComponent(typeof(CanvasRenderer))]
#endif
    public class TextView : MaskableGraphic
    {
#region Inner Classes

        /// <summary>
        /// テキストが表示領域から溢れた際のデリゲート型
        /// </summary>
        public class TextOverflowDelegate : EventDelegate<TextViewOverflow>
        {
            public TextOverflowDelegate(Callback callback) : base(callback)
            {
            }
        }

        /// <summary>
        /// テキストのパースが失敗した際のデリゲート型
        /// </summary>
        public class TextParseErrorDelegate : EventDelegate<string>
        {
            public TextParseErrorDelegate(Callback callback) : base(callback)
            {
            }
        }

#endregion

#region Constants

        /// <summary>VisibleLenghtが無限扱いとなる値</summary>
        public const int UnlimitedVisibleLength = -1;

        /// <summary>TextViewで表示するGlyphScaleのデフォルト値</summary>
        public const float DefaultGlyphScaleX = 1.0f;

#endregion

#region Fields & Properties

        /// <summary>
        /// MarkupParse
        /// </summary>
        MarkupParser markupParser;

        /// <summary>
        /// TextSource
        /// </summary>
        TextSource textSource;

        /// <summary>
        /// ワークフロー管理用のモデル
        /// </summary>
        WorkflowModel model = new WorkflowModel();

        /// <summary>テキストの表示に必要な範囲を計測するモジュール</summary>
        PreferredSizeMeasurer measurer;

        /// <summary>
        /// textSourceCompilerから受け取る論理行
        /// </summary>
        List<List<SplitDenyGlyphSpecs>> logicalLines = new List<List<SplitDenyGlyphSpecs>>();

        /// <summary>
        /// 配置情報まで含めた状態の論理行
        /// </summary>
        List<LineRenderingElements> assembledLogicalLines = new List<LineRenderingElements>();

        /// <summary>
        /// グリフ要求一覧
        /// </summary>
        List<FontGlyphSpec> allGlyphSpecs = new List<FontGlyphSpec>();

        /// <summary>
        /// GlyphPlacementの配置と改行処理が入ったあとの全TextLine
        /// </summary>
        protected List<TextLine> allTextLines = new List<TextLine>();

        /// <summary>
        /// TextViewで表示するGlyphのサイズ
        /// </summary>
        public float GlyphScaleX
        {
            get
            {
                if (this.overflowExtensionComponent == null || !this.overflowExtensionComponent.SqueezeEnabled)
                {
                    return DefaultGlyphScaleX;
                }
                else
                {
                    // Squeezeが有効ですでに計算が終わっている場合は、Squeeze後の値を返す
                    return this.overflowExtensionComponent.IsNeedSqueeze() ? DefaultGlyphScaleX : this.overflowExtensionComponent.SqueezedSize;
                }
            }
        }

        /// <summary>
        /// TextViewが実際に描画するテキストのOffsetY(pivotによるY軸)の位置
        /// </summary>
        protected float offsetY;

        /// <summary>
        /// VisbleStartLineによるTextLineの描画開始補正位置
        /// </summary>
        float visibleStartLineOffsetY;

        /// <summary>
        /// Font情報を更新するかどうか
        /// 初期設定のため初期値はtrue
        /// </summary>
        bool isNeedRefreshFontFaceInfo = true;
        
        /// <summary>
        /// 現在設定されているフォントの情報
        /// </summary>
        FaceInfo fontFaceInfo = new FaceInfo();

        /// <summary>
        /// フォントの情報
        /// </summary>
        public FaceInfo FontFaceInfo
        {
            get { return this.fontFaceInfo; }
        }

        /// <summary>すでにレイアウトの計算まで終わっているかどうかを返します</summary>
        public bool AlreadyCalculatedLayout { get { return !this.model.IsNecessaryProcess(WorkflowProcess.GlyphPlacement); } }

        /// <summary>TextSourceを生成する必要があるかどうか</summary>
        [NonSerialized]
        bool isNeedRefreshTextSource = true;

        /// <summary>TextSourceを生成する必要があるかどうか</summary>
        bool IsNeedRefreshTextSource
        {
            get
            {
                if (this.textSource == null || this.markupParser == null)
                {
                    return true;
                }
                return isNeedRefreshTextSource;
            }
        }

        /// <summary>RectTransformのサイズに変化があったかどうか</summary>
        bool isNecessaryCheckSqueezeAndShrink = false;

        /// <summary>uGUIのレイアウトのリビルド中にサイズ変更が発生したかどうか</summary>
        bool hasDimensionChangedLayoutRebuilt = false;

        /// <summary>このインスタンス上での最初の更新処理後にLayoutGroup等からサイズ変更が行われたかどうか</summary>
        protected bool? dimensionChangedAfterFirstUpdate = false;

        /// <summary>レイアウトの再計算時にRectTransformのRecalculateをスキップするかどうか</summary>
        bool isSkipRecalculate = false;

        /// <summary>バッチ処理後にサイズ変更が発生したかどうか(レイアウトのリビルド中を除く)</summary>
        bool hasDimensionChangedAfterBatched = false;

        /// <summary>最後にRectTransform.rect.sizeが変化した際の値</summary>
        Vector2 lastSize = Vector2.zero;

        /// <summary>最後にRectTransform.pivotが変化した際の値</summary>
        Vector2 lastPivot = Vector2.zero;

        /// <summary>プロパティを更新中かどうか</summary>
        bool isPropertiesUpdating = false;

        /// <summary>SetVerticesDirtyが行われて頂点情報が更新されるかどうか</summary>
        public bool WillUpdateVertices { get; set; }

        /// <summary>頂点情報をVertexHelperに設定する際に利用する領域</summary>
        static UIVertex[] verticesToFill = new UIVertex[4];

        /// <summary>鏡文字表示を有効にする。アラビア語表示に使用</summary>
        protected bool enableMirror = false;

        /// <summary>折返しの余白を維持するフラグ</summary>
        protected bool isKeepTailSpace = false;

        /// <summary>Awakeが呼び出されたか</summary>
        protected bool isCalledAwake = false;

        
#endregion

#region Serialized Field & Properties

        [SerializeField]
        ExtensibilityProvider extensibilityProvider;

        /// <summary>
        /// 案件拡張用の供給スクリプトを取得、設定します。
        /// </summary>
        /// <value>The extensibility provider.</value>
        public ExtensibilityProvider ExtensibilityProvider
        {
            get { return this.extensibilityProvider; }
            set
            {
                if (this.extensibilityProvider != value)
                {
                    this.extensibilityProvider = value;
                    // ExtensibilityProviderは、MarkupParser、TextSourceCompilerを上書きできるので
                    // どちらが上書きされても大丈夫なようにテキストのパースから再処理する
                    this.markupParser = null;
                    this.isNeedRefreshTextSource = true;
                    this.RequireSqueezeAndShrink();
                    this.RequireProcessByUpdateProperty(WorkflowProcess.TextParse);
                }
            }
        }

        [SerializeField]
        MonoBehaviour inlineImageProvider;

        /// <summary>
        /// 画像の供給処理を拡張するスクリプトがアタッチされているオブジェクトを取得、設定します。
        /// (設定されるMonoBehaviourを継承しているスクリプトはIInlineImageProviderを実装しなければいけません)
        /// 設定されていなければDefaultInlineImageProviderを使用します。
        /// </summary>
        /// <value>The inline image provider.</value>
        public MonoBehaviour InlineImageProvider
        {
            get { return this.inlineImageProvider; }
            set
            {
                if (this.inlineImageProvider != value)
                {
                    this.inlineImageProvider = value;
                    this.inlineImageLayout = null;
                    this.RequireProcessByUpdateProperty(WorkflowProcess.InlineImagePlacement);
                }
            }
        }

        [SerializeField]
        Font font;

        /// <summary>
        /// 使用するフォントを取得、設定します。
        /// </summary>
        /// <value>The font.</value>
        public Font Font
        {
            get { return this.font; }
            set
            {
                if (this.font != value)
                {
                    // フォントが変わった場合は、再登録し直す必要がある
                    TextViewObserver.Unregister(this);
                    this.font = value;
                    if (isCalledAwake)
                    {
                        TextViewObserver.Register(this);    
                    }
                    this.RequireSqueezeAndShrink();
                    this.isNeedRefreshFontFaceInfo = true;
                    SetMaterialDirty();
                    this.RequireProcessByUpdateProperty(WorkflowProcess.LogicalLinesCreate);
                }
            }
        }

        [SerializeField]
        int fontSize = 24;

        /// <summary>設定元のフォントサイズ</summary>
        public int RawFontSize
        {
            get { return fontSize; }
        }

        /// <summary>
        /// 設定するフォントサイズを取得、設定します。
        /// </summary>
        /// <value>The size of the font.</value>
        public int FontSize
        {
            get
            {
                if (this.overflowExtensionComponent == null || !this.overflowExtensionComponent.ShrinkEnabled || this.overflowExtensionComponent.ShrinkMinFontSize >= this.fontSize)
                {
                    return this.fontSize;
                }
                else
                {
                    // シュリンクが有効ですでに計算が終わっている場合は、シュリンク後の値を返す
                    return this.overflowExtensionComponent.IsNeedShrink() ? this.fontSize : this.overflowExtensionComponent.ShrinkedSize;
                }
            }
            set
            {
                if (this.fontSize != value)
                {
                    this.fontSize = value;
                    this.RequireSqueezeAndShrink();
                    this.RequireProcessByUpdateProperty(WorkflowProcess.LogicalLinesCreate);
                }
            }
        }

        [SerializeField] 
        TextAlignMode alignMode;

        /// <summary>
        /// テキストの配置動作のモードを変更します。
        /// </summary>
        public virtual TextAlignMode AlignMode
        {
            get { return this.alignMode; }
            set
            {
                if (this.alignMode == value)
                {
                    return;
                }

                this.alignMode = value;

                if (this.alignMode == TextAlignMode.Font)
                {
                    // フォントベースの配置の場合、ランタイムで変更されるとフォント情報が更新されないためここで更新をするように明示
                    this.isNeedRefreshFontFaceInfo = true;
                }
                
                // オフセット座標計算を行うようにプロセスを要求する
                this.RequireProcessByUpdateProperty(WorkflowProcess.LinesOffsetYCalculate);
                // HeightMeasurerのキャッシュを消す必要があるため無効化する
                if (this.measurer != null)
                {
                    this.measurer.InvalidateCache();
                }
            }
        }

        [SerializeField]
        TextAlign alignment;

        /// <summary>
        /// テキストの横配置についての設定を取得、設定します。
        /// 実際の表示ではviewAlignmentが使用されることに注意(アラビア語対応のため)
        /// </summary>
        /// <value>The alignment.</value>
        public TextAlign Alignment
        {
            get { return alignment; }
            set
            {
                if (this.alignment != value)
                {
                    if (languageType == TextLanguageType.Arabic &&
                        !this.model.IsNecessaryProcess(WorkflowProcess.LogicalLinesCreate) &&
                        GetHasArabic())
                    {
                        // languateTypeがArabicかつアラビア語が含まれている場合、viewAlignmentの更新はアラビア語用のAlignmentになるようにしないといけない
                        // CreateLogicalLinesの処理が走る場合はここで更新しなくてよい
                        viewAlignment = GetArabicAlign(value);
                    }
                    else
                    {
#if UNITY_EDITOR
                        if (languageType == TextLanguageType.Arabic &&
                            (value == TextAlign.Justify ||
                            value == TextAlign.JustifyAll))
                        {
                            Debug.LogErrorFormat("Can't use TextAlign {0} for Arabic.", value);
                        }
#endif
                        // viewAlignmentとalignmentがずれない場合においてもアラビア語が含まれる場合はあるが
                        // その場合はviewAlignmentとalignmentを同じにして良い
                        viewAlignment = value;
                    }

                    this.alignment = value;
                    this.RequireProcessByUpdateProperty(WorkflowProcess.TextLinesPositionXCalculate);
                }
            }
        }

        /// <summary>
        /// 実際に使用されるテキストの横配置についての設定を取得、設定します。
        /// プロパティのAlignmentと差がでる条件はテキストにアラビア語が含まれていて、かつAlignmentの設定が左端もしくは右端の場合となる
        /// アラビア語は左端設定だと右端に、右端設定だと左端で表示させる必要があるため設定用と表示用のAlignmentを分ける必要があった
        /// </summary>
        TextAlign viewAlignment;

        [SerializeField]
        TextVerticalAlign verticalAlignment;

        /// <summary>
        /// テキストの縦配置についての設定を取得、設定します。
        /// </summary>
        /// <value>The vertical alignment.</value>
        public TextVerticalAlign VerticalAlignment
        {
            get { return verticalAlignment; }
            set
            {
                if (this.verticalAlignment != value)
                {
                    this.verticalAlignment = value;
                    this.RequireProcessByUpdateProperty(WorkflowProcess.CurrentOffsetYCalculate);
                }
            }
        }

        [SerializeField]
        TextJustify justify;

        /// <summary>
        /// テキストの均等割付の形式を取得、設定します。
        /// (Alignmentが両端揃え[Justify, JustifyAll]の際にしか使いません)
        /// </summary>
        /// <value>The justify.</value>
        public TextJustify Justify
        {
            get { return this.justify; }
            set
            {
                if (this.justify != value)
                {
                    this.justify = value;
                    this.RequireProcessByUpdateProperty(WorkflowProcess.TextLinesPositionXCalculate);
                }
            }
        }

        [SerializeField]
        string text = string.Empty;

        /// <summary>
        /// パースするテキストを取得、設定します。
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return this.text; }
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.isNeedRefreshTextSource = true;
                    this.RequireSqueezeAndShrink();
                    this.RequireProcessByUpdateProperty(WorkflowProcess.TextParse);
                }
            }
        }

        [SerializeField]
        float characterSpacing;

        /// <summary>
        /// ベースとなる、文字と文字との間隔をフォントサイズに掛かる係数で取得、設定します
        /// </summary>
        /// <remarks>デフォルト値は0,文字間に隙間なく表示する設定です</remarks>
        /// <value>The character spacing.</value>
        public float CharacterSpacing
        {
            get { return this.characterSpacing; }
            set
            {
                if (this.characterSpacing != value)
                {
                    this.characterSpacing = value;
                    this.RequireSqueezeAndShrink();
                    this.RequireProcessByUpdateProperty(WorkflowProcess.LogicalLinesCreate);
                }
            }
        }

        [SerializeField]
        SpacingUnit spacingUnit;

        /// <summary>
        /// スペーシング単位を取得、設定します
        /// </summary>
        /// <value>スペーシング単位</value>
        public SpacingUnit SpacingUnit
        {
            get { return this.spacingUnit; }
            set
            {
                if (this.spacingUnit != value)
                {
                    this.spacingUnit = value;
                    this.RequireSqueezeAndShrink();
                    this.RequireProcessByUpdateProperty(WorkflowProcess.LogicalLinesCreate);
                }
            }
        }

        [SerializeField]
        bool trimLineTailSpacing;

        /// <summary>
        /// 行末のスペースを取り除くかどうかを指定します
        /// </summary>
        /// <value>trueの場合は取り除きます</value>
        public bool TrimLineTailSpacing
        {
            get { return this.trimLineTailSpacing; }
            set
            {
                if (this.trimLineTailSpacing != value)
                {
                    this.trimLineTailSpacing = value;
                    this.RequireSqueezeAndShrink();
                    this.RequireProcessByUpdateProperty(WorkflowProcess.Shrink);
                }
            }
        }

        [SerializeField]
        bool isLineHeightFixed = false;

        /// <summary>
        /// 一行の高さを固定値をするかどうかを取得、設定います。
        /// </summary>
        /// <value><c>true</c> if this instance is line height fixed; otherwise, <c>false</c>.</value>
        public bool IsLineHeightFixed
        {
            get { return this.isLineHeightFixed; }
            set
            {
                if (this.isLineHeightFixed != value)
                {
                    this.isLineHeightFixed = value;
                    this.RequireProcessByUpdateProperty(WorkflowProcess.LinesOffsetYCalculate);
                }
            }
        }

        [SerializeField]
        float lineHeight = 1.5f;

        /// <summary>
        /// 一行の高さを取得、設定します。<c>1</c>はフォントの高さと等しくなります。
        /// </summary>
        /// <value>The height of the line.</value>
        public float LineHeight
        {
            get { return this.lineHeight; }
            set
            {
                if (this.lineHeight != value)
                {
                    this.lineHeight = value;
                    if (this.overflowExtensionComponent != null && this.overflowExtensionComponent.EllipsisEnabled)
                    {
                        this.RequireProcessByUpdateProperty(WorkflowProcess.GlyphPlacement);
                    }
                    else
                    {
                        this.RequireProcessByUpdateProperty(WorkflowProcess.LinesOffsetYCalculate);
                    }
                }
            }
        }

        [SerializeField]
        int visibleLength = UnlimitedVisibleLength;

        /// <summary>
        /// 表示する文字数を取得、設定します。
        /// -1の場合は全文字を表示します。
        /// </summary>
        /// <value>The length of the visible.</value>
        public int VisibleLength
        {
            get { return this.visibleLength; }
            set
            {
                if (this.visibleLength != value)
                {
                    this.visibleLength = value;
                    this.RequireProcessByUpdateProperty(WorkflowProcess.DisplayableElementIndexCalculate);
                }
            }
        }

        [SerializeField]
        HorizontalWrapMode horizontalOverflow = HorizontalWrapMode.Wrap;

        /// <summary>
        /// テキストの表示が横方向に溢れた場合に折り返すかどうか
        /// </summary>
        /// <value>The horizontal overflow.</value>
        public HorizontalWrapMode HorizontalOverflow
        {
            get { return this.horizontalOverflow; }
            set
            {
                if (this.HorizontalOverflow != value)
                {
                    this.horizontalOverflow = value;
                    this.RequireProcessByUpdateProperty(WorkflowProcess.GlyphPlacement);
                }
            }
        }

        [SerializeField]
        VerticalWrapMode verticalOverflow = VerticalWrapMode.Overflow;

        /// <summary>
        /// テキストの表示が縦方向に溢れた場合に隠すかどうか
        /// </summary>
        /// <value>The vertical overflow.</value>
        public VerticalWrapMode VerticalOverflow
        {
            get { return this.verticalOverflow; }
            set
            {
                if (this.verticalOverflow != value)
                {
                    this.verticalOverflow = value;
                    if (this.overflowExtensionComponent != null && this.overflowExtensionComponent.EllipsisEnabled)
                    {
                        this.RequireProcessByUpdateProperty(WorkflowProcess.GlyphPlacement);
                    }
                    else
                    {
                        this.RequireProcessByUpdateProperty(WorkflowProcess.DisplayableElementIndexCalculate);
                    }
                }
            }
        }

        [SerializeField]
        bool treatNewLineAsLineBreak = true;

        /// <summary>
        /// 改行文字をそのまま表示でも改行として扱うか否かを取得、設定します
        /// </summary>
        /// <remarks><c>false</c>に指定した場合、テキストの改行には br タグを使用してください</remarks>
        /// <value><c>true</c> if treat new line as line break; otherwise, <c>false</c>.</value>
        public bool TreatNewLineAsLineBreak
        {
            get { return this.treatNewLineAsLineBreak; }
            set
            {
                if (this.treatNewLineAsLineBreak != value)
                {
                    this.treatNewLineAsLineBreak = value;
                    this.isNeedRefreshTextSource = true;
                    this.RequireProcessByUpdateProperty(WorkflowProcess.TextParse);
                }
            }
        }

        [SerializeField]
        TextLineBreakRule lineBreakRule;

        /// <summary>
        /// 禁則処理ルールを取得、設定します
        /// </summary>
        /// <value>禁則処理ルール</value>
        public TextLineBreakRule LineBreakRule
        {
            get { return this.lineBreakRule; }
            set
            {
                if (this.lineBreakRule != value)
                {
                    this.lineBreakRule = value;
                    this.RequireProcessByUpdateProperty(WorkflowProcess.GlyphPlacement);
                }
            }
        }

        [SerializeField]
        bool useBurasage;

        /// <summary>
        /// ぶら下げを行うかを取得、設定します
        /// </summary>
        /// <value>trueの場合、ぶら下げを行う</value>
        public bool UseBurasage
        {
            get { return this.useBurasage; }
            set
            {
                if (this.useBurasage != value)
                {
                    this.useBurasage = value;
                    this.RequireProcessByUpdateProperty(WorkflowProcess.GlyphPlacement);
                }
            }
        }

        /// <summary>テキストが溢れる時の拡張機能用コンポーネント</summary>
        [FormerlySerializedAs("shrinkComponent")]
        [HideInInspector]
        [SerializeField]
        TextViewOverflowExtension overflowExtensionComponent = null;

        /// <summary>レイアウト用コンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected TextViewLayoutElement layoutElement = null;

        InlineImageLayout inlineImageLayout;

        InlineImageLayout InlineImageLayout
        {
            get
            {
                if (this.inlineImageLayout == null)
                {
                    IInlineImageProvider provider = this.InlineImageProvider as IInlineImageProvider;
                    if (provider == null)
                    {
                        provider = new DefaultInlineImageProvider();
                    }

                    this.inlineImageLayout = new InlineImageLayout(this.gameObject, provider);
                }

                return this.inlineImageLayout;
            }
        }

        HorizontalLayout horizontalLayout = new HorizontalLayout();
        
        /// <summary>
        /// 行間を返します
        /// </summary>
        public float LineGap { get { return 0.5f * this.FontSize * (this.LineHeight - 1); } }

#if UNITY_EDITOR
        [SerializeField]
        bool heightAdjustment;

        [SerializeField]
        bool heightAdjustmentWithRuby;
#endif

        /// <summary>長文用拡張コンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected TextViewLongTextExtension longTextExtension;

        [HideInInspector]
        [SerializeField] 
        bool isBold = false;
        
        /// <summary>
        /// TextViewに設定した文字列全体をボールドにするか設定します
        /// </summary>
        /// <value>trueの場合はボールドに設定します</value>
        public bool IsBold
        {
            get { return this.isBold; }
            set
            {
                if (this.isBold != value)
                {
                    this.isBold = value;
                    this.isNeedRefreshTextSource = true;
                    this.RequireProcessByUpdateProperty(WorkflowProcess.TextParse);
                }
            }
        }
        
        [HideInInspector]
        [SerializeField] 
        bool isItalic = false;
        
        /// <summary>
        /// TextViewに設定した文字列全体をイタリックにするか設定します
        /// </summary>
        /// <value>trueの場合はイタリックに設定します</value>
        public bool IsItalic
        {
            get { return this.isItalic; }
            set
            {
                if (this.isItalic != value)
                {
                    this.isItalic = value;
                    this.isNeedRefreshTextSource = true;
                    this.RequireProcessByUpdateProperty(WorkflowProcess.TextParse);
                }
            }
        }

        /// <summary>
        /// デフォルトのフォントスタイルの取得
        /// </summary>
        public FontStyle DefaultFontStyle
        {
            get
            {
                if (this.IsBold && this.IsItalic)
                {
                    return FontStyle.BoldAndItalic;
                }
            
                if (this.IsItalic)
                {
                    return FontStyle.Italic;
                }
            
                if (this.IsBold)
                {
                    return FontStyle.Bold;
                }
            
                return FontStyle.Normal;
            }
        }
        
        [HideInInspector]
        [SerializeField] 
        LetterCase letterCase = LetterCase.None;
        
        /// <summary>
        /// デフォルトのフォントスタイルの取得
        /// </summary>
        public LetterCase LetterCase
        {
            set
            {
                if (this.letterCase != value)
                {
                    this.letterCase = value;
                    this.isNeedRefreshTextSource = true;
                    this.RequireSqueezeAndShrink();
                    this.RequireProcessByUpdateProperty(WorkflowProcess.TextParse);
                }
            }
            get
            {
                return this.letterCase;
            }
        }
        
        /// <summary>
        /// Ellipsis機能を有効化されているか
        /// </summary>
        /// <value>trueの場合はEllipsis機能を有効化</value>
        public bool EllipsisEnabled
        {
            get
            {
                if (this.overflowExtensionComponent != null)
                {
                    return this.overflowExtensionComponent.EllipsisEnabled;
                }
                
                return false;
            }
            set
            {
                if (this.overflowExtensionComponent != null)
                {
                    if (this.overflowExtensionComponent.EllipsisEnabled != value)
                    {
                        this.overflowExtensionComponent.EllipsisEnabled = value;
                        this.RequireProcess(WorkflowProcess.LogicalLinesCreate);
                    }
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogWarning("The ellipsis function cannot be enabled because the TextViewOverflowExtension is not attached.");
                }
#endif
            }
        }
        
        /// <summary>
        /// Squeeze機能を有効化されているか
        /// </summary>
        /// <value>trueの場合はSqueeze機能を有効化</value>
        public bool SqueezeEnabled
        {
            get
            {
                if (this.overflowExtensionComponent != null)
                {
                    return this.overflowExtensionComponent.SqueezeEnabled;
                }
                
                return false;
            }
            set
            {
                if (this.overflowExtensionComponent != null)
                {
                    if (this.overflowExtensionComponent.SqueezeEnabled != value)
                    {
                        this.overflowExtensionComponent.SqueezeEnabled = value;
                        this.RequireProcess(WorkflowProcess.LogicalLinesCreate);
                        this.overflowExtensionComponent.RequireCalculate();
                    }
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogWarning("The squeeze function cannot be enabled because the TextViewOverflowExtension is not attached.");
                }
#endif
            }
        }

        /// <summary>
        /// Shrink機能を有効化されているか
        /// </summary>
        /// <value>trueの場合はShrink機能を有効化</value>
        public bool ShrinkEnabled
        {
            get
            {
                if (this.overflowExtensionComponent != null)
                {
                    return this.overflowExtensionComponent.ShrinkEnabled;
                }
                
                return false;
            }
            set
            {
                if (this.overflowExtensionComponent != null)
                {
                    if (this.overflowExtensionComponent.ShrinkEnabled != value)
                    {
                        this.overflowExtensionComponent.ShrinkEnabled = value;
                        this.RequireProcess(WorkflowProcess.LogicalLinesCreate);
                        this.overflowExtensionComponent.RequireCalculate();
                    }
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogWarning("The shrink function cannot be enabled because the TextViewOverflowExtension is not attached.");
                }
#endif
            }
        }

        [SerializeField]
        [HideInInspector]
        TextLanguageType languageType = TextLanguageType.Default;
        
        /// <summary>TextViewの言語の扱い</summary>
        public TextLanguageType LanguageType
        {
            get
            {
                return this.languageType;
            }
            set
            {
                if (this.languageType != value)
                {
                    this.languageType = value;

                    this.isNeedRefreshTextSource = true;
                    this.RequireProcessByUpdateProperty(WorkflowProcess.TextParse);    
                }
            }
        }

        /// <summary>行頭約物半角処理を行うフラグ</summary>
        [SerializeField, HideInInspector]
        bool isHalfPunctuationOfLineHead;

        /// <summary>行頭約物半角処理を行うフラグ</summary>
        public bool IsHalfPunctuationOfLineHead
        {
            get { return isHalfPunctuationOfLineHead; }
            set
            {
                if (isHalfPunctuationOfLineHead != value)
                {
                    isHalfPunctuationOfLineHead = value;
                    this.RequireProcessByUpdateProperty(WorkflowProcess.CreateGlyph);
                }
            }
        }

#endregion

#region Visibility

        int maxIndex;

        /// <summary>
        /// 整形済みテキストの最後のインデックスを取得します
        /// </summary>
        /// <value>The index of the max.</value>
        public int MaxIndex
        {
            get
            {
                EarlyUpdateIfNeeded();
                return this.maxIndex;
            }
        }

        int visibleMinIndex;

        /// <summary>
        /// 表示できるグリフの最初のインデックスを取得します
        /// </summary>
        /// <value>The index of the visible minimum.</value>
        public int VisibleMinIndex
        {
            get
            {
                EarlyUpdateIfNeeded();
                return this.visibleMinIndex;
            }
        }

        int visibleMaxIndex;

        /// <summary>
        /// 表示できるグリフの最後のインデックスを取得します
        /// </summary>
        /// <value>The index of the visible max.</value>
        public int VisibleMaxIndex
        {
            get
            {
                EarlyUpdateIfNeeded();
                return this.visibleMaxIndex;
            }
        }

        [SerializeField]
        int visibleLineStart;

        /// <summary>
        /// 整形済みテキストの何行目から表示するかを取得、設定します
        /// </summary>
        /// <value>The visible line start.</value>
        public int VisibleLineStart
        {
            get { return this.visibleLineStart; }
            set
            {
                var i = Math.Max(value, 0);
                if (i != this.visibleLineStart)
                {
                    this.visibleLineStart = i;
                    if (overflowExtensionComponent != null && overflowExtensionComponent.EllipsisEnabled)
                    {
                        this.RequireProcessByUpdateProperty(WorkflowProcess.GlyphPlacement);
                    }
                    else
                    {
                        // 表示する行が変更になるという事は高さが変更になる可能性があるためOffset位置を再計算する必要がある
                        this.RequireProcessByUpdateProperty(WorkflowProcess.DisplayableElementIndexCalculate, WorkflowProcess.CurrentOffsetYCalculate);
                    }
                }
            }
        }

        /// <summary>
        /// バッチングで処理する内容を先行して走らせます。
        /// </summary>
        void EarlyUpdateIfNeeded()
        {
            if (this.font == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Can't update TextView, because Font is null!");
#endif
                this.maxIndex = 0;
                this.visibleMaxIndex = 0;
                this.visibleMinIndex = 0;
                return;
            }

            // すでに表示する文字の計算まで終わっている場合は、先行させる必要はなくなるので何もしない
            if (!this.model.IsNecessaryProcess(WorkflowProcess.DisplayableElementIndexCalculate))
            {
                return;
            }

            ForceUpdate();
        }

        /// <summary>
        /// バッチングで処理する内容を強制的に実行します。
        /// </summary>
        protected void ForceUpdate()
        {
            if (this.font == null)
            {
                return;
            }

            this.ParseText();

            // フォントのリビルドやSqueeze、シュリンクが発生した場合、フォントを生成し直す必要があるので
            // 生成が完了するまでは繰り返す
            while (this.model.IsNecessaryProcess(WorkflowProcess.CreateGlyph))
            {
                this.RequestFontAndCreateGlyph();
            }

            this.CalculateLayout();
        }

#endregion

#region Event Handler

        /// <summary>
        /// テキストが表示領域からはみ出した際のイベント.
        /// </summary>
        /// <value>The on overflow text lines.</value>
        [HideInInspector]
        [SerializeField]
        protected DelegatableList onOverflowTextLines = new DelegatableList();

        /// <summary>
        /// テキストが表示領域からはみ出した際のイベント.
        /// </summary>
        /// <value>The on overflow text lines.</value>
        public DelegatableList OnOverflowTextLines { get { return onOverflowTextLines; } }

        /// <summary>
        /// テキストが表示領域から溢れた際のコールバックを追加します。
        /// </summary>
        /// <param name="callback">void Func(int, TextLine[])のメソッド</param>
        public void AddOverflowEvent(TextOverflowDelegate.Callback callback)
        {
            onOverflowTextLines.Add(new TextOverflowDelegate(callback));
        }

        /// <summary>
        /// テキストのパースが失敗した際のイベント.
        /// </summary>
        /// <value>The on text parse errors.</value>
        [HideInInspector]
        [SerializeField]
        protected DelegatableList onTextParseError = new DelegatableList();

        /// <summary>
        /// テキストのパースが失敗した際のイベント.
        /// </summary>
        /// <value>The on text parse errors.</value>
        public DelegatableList OnTextParseError { get { return onTextParseError; } }

        /// <summary>
        /// テキストのパースが失敗した際のコールバックを追加します。
        /// </summary>
        /// <param name="callback">void Func(string)のメソッド</param>
        public void AddTextParseErrorEvent(TextParseErrorDelegate.Callback callback)
        {
            onTextParseError.Add(new TextParseErrorDelegate(callback));
        }

        /// <summary>
        /// 全てのTextViewで共通の初期化用コールバック
        /// </summary>
        protected static System.Action<TextView> onInitialize = null;

        /// <summary>
        /// TextViewの共通の初期化用コールバックを設定します。
        /// </summary>
        /// <param name="onInitialize">初期化用コールバック</param>
        public static void SetInitializer(System.Action<TextView> onInitialize)
        {
            TextView.onInitialize = onInitialize;
        }

        /// <summary>Font設定がnullのTextViewに使用されるデフォルトフォント</summary>
        protected static Font defaultFont = null;

        /// <summary>
        /// Font設定がnullのTextViewに使用するデフォルトフォントを設定します
        /// </summary>
        /// <param name="font"></param>
        public static void SetDefaultFont(Font font)
        {
            TextView.defaultFont = font;
        }

        /// <summary>
        /// アラビア語の状態でのアライメントを取得します。
        /// </summary>
        /// <param name="align">元のアライメント</param>
        /// <returns>変換後のアライメント</returns>
        protected static TextAlign GetArabicAlign(TextAlign align)
        {
#if UNITY_EDITOR
            if (align == TextAlign.Justify ||
                align == TextAlign.JustifyAll)
            {
                Debug.LogErrorFormat("Can't use TextAlign {0} for Arabic.", align);
            }
#endif
            switch (align)
            {
                case TextAlign.Left:
                    return TextAlign.Right;
                case TextAlign.Right:
                    return TextAlign.Left;
                default:
                    return align;
            }
        }

        /// <summary>
        /// アラビア語の表示時の鏡描画の影響によるオフセットを取得します。
        /// アラビア語以外では利用しません。
        /// </summary>
        /// <returns></returns>
        protected float GetArabicMirrorOffset()
        {
            if (enableMirror)
            {
                return rectTransform.rect.width * (0.5f - rectTransform.pivot.x) * 2.0f;
            }

            return 0f;
        }

#endregion

#region Unity Method

        public override Texture mainTexture
        {
            get
            {
                if (this.Font != null && this.Font.material != null && this.Font.material.mainTexture != null)
                {
                    return this.Font.material.mainTexture;
                }
                if (this.m_Material != null)
                {
                    return this.m_Material.mainTexture;
                }
                return base.mainTexture;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // Fontがnullの場合はデフォルトフォントを適用する
            if (font == null && defaultFont != null)
            {
                font = defaultFont;
            }

#if UNITY_EDITOR || JIGBOX_DEBUG
            // fontとdefaultFontがnullの場合TextViewは表示されないので警告を出す
            // 実行時にfontが設定される場合もあるためEditor再生中のみ出力する
            if (font == null && Application.isPlaying)
            {
                Debug.LogWarningFormat(gameObject, "[{0}] : font is null.", name);
            }
#endif

            TextViewObserver.Register(this);

            // バッチ処理が既に終わってしまっている場合は、
            // 状態の計算が正しくできていない可能性があるので、
            // その状態でレンダリングが始まると表示が崩れる可能性があるため
            // 自身で更新処理を行う
#if UNITY_EDITOR
            // エディタの場合は、再生状態切替中はObserverのインスタンスが存在しない状態となるため、
            // 切り替え中以外のタイミングでのみチェックを行う
            if (!TextViewObserver.EditorPlayModeChanging && TextViewObserver.Instance.AlreadyBatched)
#else
            if (TextViewObserver.Instance.AlreadyBatched)
#endif
            {
                ForceUpdate();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (inlineImageLayout != null)
            {
                if (inlineImageLayout.CancelRequests())
                {
                    RequireProcess(WorkflowProcess.InlineImagePlacement, false);
                }
            }
#if UNITY_EDITOR

            // 以降の処理はプレハブモード時用の処理のため、それ以外では実行させない
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                return;
            }

            // Editor上だとOnDestroyが呼ばれずに削除される場合がある
            // プレハブモード中に消されたインラインイメージ付きのオブジェクトの画像を削除する
            if (inlineImageLayout != null)
            {
                this.InlineImageLayout.ClearReservedImages();
                this.InlineImageLayout.ClearInlineImageGameObjects();
                this.inlineImageLayout.PoolDestroyAll();
            }
            
            // OnDisableでTextViewObserverの登録解除をしておく
            TextViewObserver.Unregister(this);
#endif
        }

        protected override void Awake()
        {
            base.Awake();

            this.model.RequireProcess(WorkflowProcess.TextParse);

            onCullStateChanged.AddListener(OnCullStateChanged);

            isCalledAwake = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            TextViewObserver.Unregister(this);
        }

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            this.WillUpdateVertices = true;

            // TextViewLayoutElementがアタッチされている場合は、
            // VeticesDirtyをする(=何かしら表示に更新が必要)タイミングで
            // 一緒にLayoutDirtyも行うようにする
            if (layoutElement != null && !CanvasUpdateRegistry.IsRebuildingLayout())
            {
                base.SetLayoutDirty();
            }

            // SetVeticesDarty = 頂点の更新が必要 = 再レンダリングが必要
            this.model.RequireProcess(WorkflowProcess.Rendering);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            this.isNecessaryCheckSqueezeAndShrink = true;

            bool isLayoutRebuilding = CanvasUpdateRegistry.IsRebuildingLayout();
            if (isLayoutRebuilding)
            {
                this.hasDimensionChangedLayoutRebuilt = true;

                // LayoutGroupによってリビルドが走った際に、TextViewの座標が1Fだけ崩れてしまうため
                // 同一フレーム内で再計算をする
                // 最初の1F目ではレンダリングがスキップされるため処理を行わない
                if (this.dimensionChangedAfterFirstUpdate == null)
                {
                    this.isSkipRecalculate = true;
                    this.model.RequireProcess(WorkflowProcess.GlyphPlacement);
                    this.CalculateLayout();
                }

                // このTextViewのインスタンスにおける最初のフレームの処理で
                // uGUIのリビルド中にリサイズが発生した場合のみ
                if (this.dimensionChangedAfterFirstUpdate != null)
                {
                    this.dimensionChangedAfterFirstUpdate = true;
                    // レンダリングがスキップされる際、InlineImageは別オブジェクトとして描画されてしまうため
                    // 配置されているInlineImageの情報を全て削除して次のフレームの再計算時に再度描画を行う
                    this.InlineImageLayout.ClearReservedImages();
                    this.InlineImageLayout.ClearInlineImageGameObjects();
                }

                return;
            }

            // TextViewの文字は変更されずにRectTransformのサイズだけ変更された場合に備えた対応
#if UNITY_EDITOR
            // エディタの場合は、再生状態切替中はObserverのインスタンスが存在しない状態となるため、
            // 切り替え中以外のタイミングでのみチェックを行う
            if (!TextViewObserver.EditorPlayModeChanging && TextViewObserver.Instance.AlreadyBatched)
#else
            if (TextViewObserver.Instance.AlreadyBatched)
#endif
            {
                // OnRectTransformDimensionsChange以外のタイミングでCalculateLayoutが行われることを待つ
                // 基本的には次のフレームのLateUpdateで呼ばれるCalculateLayoutまで待つため、このフレームでは描画もされない
                // サイズ変更だけであればCalculateLayoutをここで実行してもよかったが、
                // 親の変更も同時に行われていた場合への対応としてインラインイメージの削除をしている
                // 親の変更から呼ばれるOnRectTransformDimensionsChangeの中ではインラインイメージ用GameObjectの位置が変更できないため
                this.InlineImageLayout.ClearReservedImages();
                this.InlineImageLayout.ClearInlineImageGameObjects();
                hasDimensionChangedAfterBatched = true;
            }
        }

        /// <summary>
        /// マテリアルを更新します
        /// </summary>
        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();

            if (!IsActive())
            {
                return;
            }

            if (longTextExtension != null)
            {
                longTextExtension.UpdateMaterial(materialForRendering, mainTexture);
            }
        }

        /// <summary>
        /// カリングの状態が変更された際に呼ばれるコールバックです
        /// </summary>
        /// <param name="cull">カリングを行うかどうか</param>
        protected virtual void OnCullStateChanged(bool cull)
        {
            if (longTextExtension != null)
            {
                longTextExtension.UpdateCull(cull);
            }

            // カリング状態の際にフォントリビルドが走っていた場合、レンダリングを再度行わないと文字が化けて表示されるため
            // DirtyをつけてOnPopulateMeshが呼ばれるようにする
            if (!cull)
            {
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// クリッピング領域を設定します
        /// </summary>
        /// <param name="clipRect">クリッピング領域</param>
        /// <param name="validRect">クリッピングを行うかどうか</param>
        public override void SetClipRect(Rect clipRect, bool validRect)
        {
            base.SetClipRect(clipRect, validRect);

            if (longTextExtension != null)
            {
                longTextExtension.SetClipRect(clipRect, validRect);
            }
        }

        /// <summary>
        /// カリング設定を行います。
        /// </summary>
        /// <param name="clipRect">クリッピング領域</param>
        /// <param name="validRect">クリッピングを行うかどうか</param>
        public override void Cull(Rect clipRect, bool validRect)
        {
            // LayoutGroupが親にいる場合
            // CanvasRendererが更新されたように見せかける処理を行い、2F目でクリッピング計算が走るようにします。
            if (dimensionChangedAfterFirstUpdate != null && dimensionChangedAfterFirstUpdate.Value)
            {
                canvasRenderer.transform.position += Vector3.zero;
            }

             base.Cull(clipRect, validRect);
        }

#endregion

        /// <summary>
        /// プロセスの実行を要求します。
        /// </summary>
        /// <param name="process">実行しようとしているプロセス</param>
        /// <param name="requireLowerProcesses">下位のプロセスも有効化するかどうか</param>
        public virtual void RequireProcess(WorkflowProcess process, bool requireLowerProcesses = true)
        {
            this.model.RequireProcess(process, requireLowerProcesses);
            this.SetVerticesDirty();
        }

        /// <summary>
        /// <para>
        /// プロパティの更新を開始することを設定します。
        /// プロパティを更新し終えたら、EndUpdateProperties()を呼び出して下さい。
        /// </para>
        /// <para>※ このメソッドは、LateUpdate中にTextViewのプロパティを更新する際に、処理を最適化するためのものです。
        /// このメソッドを呼び出してから、プロパティを更新することで余剰処理を抑え、パフォーマンスが向上する場合があります。
        /// 通常のUpdate中にこのメソッドを利用しても弊害はありません。</para>
        /// </summary>
        public void BeginUpdateProperties()
        {
            this.isPropertiesUpdating = true;
        }

        /// <summary>
        /// <para>プロパティの更新を終了したことを設定します。</para>
        /// <para>※ このメソッドは、LateUpdate中にTextViewのプロパティを更新する際に、処理を最適化するためのものです。
        /// このメソッドを呼び出してから、プロパティを更新することで余剰処理を抑え、パフォーマンスが向上する場合があります。
        /// 通常のUpdate中にこのメソッドを利用しても弊害はありません。</para>
        /// </summary>
        public void EndUpdateProperties()
        {
            if (this.isPropertiesUpdating)
            {
                // バッチ処理が既に終わってしまっている場合のみ、自身で処理する
                if (TextViewObserver.Instance.AlreadyBatched)
                {
                    this.ForceUpdate();
                }
                this.isPropertiesUpdating = false;
            }
#if UNITY_EDITOR
            else
            {
                // BeginUpdatePropertiesを呼び出さないで呼び出した場合、
                // もしくは、呼び出した回数が間違っている場合
                Debug.LogWarning("TextView.EndUpdateProperties : Can't be processed before call to \"BeginUpdateProperties\"!");
            }
#endif
        }
#if UNITY_EDITOR
        /// <summary>
        /// プレハブを保存した時に全ての処理をやり直すために必要なフラグなどを戻す
        /// </summary>
        public void PrefabSavedInit()
        {
            this.isNeedRefreshFontFaceInfo = true;
            this.isNeedRefreshTextSource = true;
            this.RequireSqueezeAndShrink();
            this.RequireProcessByUpdateProperty(WorkflowProcess.TextParse);
            
            if (this.measurer != null)
            {
                this.measurer.InvalidateCache();
            }

            this.SetMaterialDirty();
            this.ForceUpdate();
        }
#endif

        /// <summary>
        /// プロパティが変化した場合のプロセスの実行を要求します。
        /// </summary>
        /// <param name="processes">実行しようとしているプロセス</param>
        /// <remarks>
        /// バッチ処理終了済みの場合、要求したプロセスは即時実行されます。
        /// 合わせて実行を要求したいプロセスがある(例えばシュリンク)場合、このメソッドを呼び出す前に実行を要求しておいてください。
        /// </remarks>
        protected virtual void RequireProcessByUpdateProperty(params WorkflowProcess[] processes)
        {
            foreach (WorkflowProcess process in processes)
            {
                this.model.RequireProcess(process);
            }
            this.SetVerticesDirty();

            // バッチ処理が既に終わってしまっている場合は、
            // レンダリングまでに計算が完了している状態にするために
            // 自身で更新処理を行う
            if (TextViewObserver.Instance.AlreadyBatched && !this.isPropertiesUpdating)
            {
                this.ForceUpdate();
            }
        }

        /// <summary>
        /// OverflowExtensionコンポーネントにSqueezeとシュリンクの再計算を行うように要求します。
        /// </summary>
        protected virtual void RequireSqueezeAndShrink()
        {
            if (overflowExtensionComponent != null && (overflowExtensionComponent.ShrinkEnabled || overflowExtensionComponent.SqueezeEnabled))
            {
                overflowExtensionComponent.RequireCalculate();
            }
        }

        /// <summary>
        /// 現在利用されているフォントの情報を更新します
        /// </summary>
        protected virtual void RefreshFontFaceInfo()
        {
            if (!this.isNeedRefreshFontFaceInfo)
            {
                return;
            }

            if (AlignMode != TextAlignMode.Font)
            {
                return;
            }

            // フォント情報をロードします。
            var fontEngineError = FontEngine.LoadFontFace(font);
            this.isNeedRefreshFontFaceInfo = false;
            
            // FontEngineではUnity標準のArialフォントがLoadできないため、ログを出して処理を終了する
            if (fontEngineError != FontEngineError.Success)
            {
#if UNITY_EDITOR
                Debug.LogError("このフォントではAlignMode.Fontは使用できません。");
#endif
                return;
            }

            this.fontFaceInfo = FontEngine.GetFaceInfo();
        }

        /// <summary>
        /// MarkupParserを生成して返します
        /// </summary>
        /// <returns>The markup parser.</returns>
        protected virtual MarkupParser CreateMarkupParser()
        {
            if (this.ExtensibilityProvider != null)
            {
                return this.ExtensibilityProvider.CreateMarkupParser();
            }
            return new MarkupParser();
        }

        /// <summary>
        /// 指定したTextLineのリストから、指定したindexから後の要素を抽出して配列として返します
        /// </summary>
        /// <param name="index">抽出する先頭行のindex</param>
        /// <param name="textLines">indexでした要素以降ののTextLineの配列</param>
        /// <returns></returns>
        TextLine[] SplitTextLines(int index, List<TextLine> textLines)
        {
            var list = new List<TextLine>();
            for (int i = index; i < textLines.Count; i++)
            {
                list.Add(textLines[i]);
            }
            
            return list.ToArray();
        }

        /// <summary>
        /// 表示領域からはみ出る行があった場合に実行されます
        /// </summary>
        /// <param name="line"></param>
        /// <param name="remain"></param>
        /// <param name="isEllipsis"></param>
        void OnOverflowTextLine(int line, TextLine[] remain, bool isEllipsis)
        {
            if (this.onOverflowTextLines.Count > 0)
            {
                this.onOverflowTextLines.Execute(new TextViewOverflow(
                    line,
                    this.visibleMinIndex,
                    this.visibleMaxIndex,
                    remain,
                    isEllipsis
                ));
            }
        }

        /// <summary>
        /// テキストのパースがエラーだった時に呼ばれます
        /// </summary>
        void TextParseError()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (this.onTextParseError.Count > 0)
            {
                this.onTextParseError.Execute(textSource.ErrorMessage);
            }
            else
            {
                Debug.LogError(textSource.ErrorMessage);
            }
        }

        /// <summary>
        /// 初期化用コールバックを処理する必要がある場合のみコールバックを発火します。
        /// </summary>
        void OnInitializeIfNeeded()
        {
            if (!this.model.CanExecuteProcess(WorkflowProcess.Initialize))
            {
                return;
            }

            // onInitializeの処理で、TextViewにForceUpdateがかかるような処理をした場合無限ループになってしまう。
            // また、onInitializeがnullの場合にもプロセスは処理する必要があるので
            // ここで先にプロセスのフラグは折っておく
            this.model.ProcessExecuted(WorkflowProcess.Initialize);

#if UNITY_EDITOR
            // 非実行状態で発火するとシリアライズ情報が書き換わってしまうため、
            // エディタでは実行中のみ処理する
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            if (onInitialize != null)
            {
                onInitialize(this);
            }
        }

        /// <summary>
        /// テキストをパースしてTextSourceを生成します
        /// </summary>
        void CreateTextSource()
        {
            if (IsNeedRefreshTextSource)
            {
                this.markupParser = this.markupParser ?? this.CreateMarkupParser();
                this.markupParser.TreatNewLineAsLineBreak = this.TreatNewLineAsLineBreak;
                this.textSource = this.markupParser.ParseTextSource(this.text, LanguageType);
                this.VisibleLineStart = 0;
                this.isNeedRefreshTextSource = false;

                if (this.textSource.HasError)
                {
                    TextParseError();
                }
            }
        }

        /// <summary>
        /// 論理行を生成します
        /// </summary>
        void CreateLogicalLines()
        {
            if (this.textSource == null || string.IsNullOrEmpty(this.text))
            {
                this.logicalLines.Clear();
                return;
            }

            // TextSourceから表示に必要なすべてのグリフ要求と表示制御データを集める
            var compilerProperty = new TextSourceCompilerProperty(this);
            var textSourceCompiler = (this.extensibilityProvider == null) ?
                new TextSourceCompiler(compilerProperty) :
                this.extensibilityProvider.CreateTextSourceCompiler(compilerProperty);

            // logicalLinesを保持
            this.logicalLines = textSourceCompiler.Compile(this.textSource);
            // ALIGNタグによってアライメントの指定が入った場合はModifiedAlignプロパティに値が格納されているので適用する
            if (compilerProperty.ModifyAlign.HasValue)
            {
                alignment = compilerProperty.ModifyAlign.Value;
            }

            // languageTypeがArabicの場合は特別対応が必要
            if (languageType == TextLanguageType.Arabic)
            {
                // 文字列にアラビア語が含まれているかどうかで設定を変える
                bool hasArabic = GetHasArabic();
                viewAlignment = hasArabic ? GetArabicAlign(alignment) : alignment;
                enableMirror = hasArabic;
                return;
            }

            // アラビア語出ない場合は
            enableMirror = false;
            viewAlignment = alignment;
        }

        /// <summary>
        /// 要求するグリフの一覧を作成します
        /// </summary>
        void CreateRequestGlyphSpecs()
        {
            // 初期化
            this.allGlyphSpecs.Clear();

            // 要求するグリフの一覧を作成する
            // Linq使うと重たくなるので見づらくなるけどベタにループで処理
            foreach (List<SplitDenyGlyphSpecs> line in logicalLines)
            {
                foreach (SplitDenyGlyphSpecs specs in line)
                {
                    foreach (IGlyphSpecGroup group in specs.Groups)
                    {
                        foreach (MainGlyphPlacementSpec main in group.Mains)
                        {
                            if (main.GlyphSpec is FontGlyphSpec)
                            {
                                this.allGlyphSpecs.Add(main.GlyphSpec as FontGlyphSpec);
                            }
                            else if (main.GlyphSpec is ImageGlyphSpec)
                            {
                                ImageGlyphSpec imageGlyphSpec = main.GlyphSpec as ImageGlyphSpec;
                                this.allGlyphSpecs.Add(imageGlyphSpec.SizeBaseGlyphSpec);
                            }
                        }

                        if (group.RubiesCount > 0)
                        {
                            this.allGlyphSpecs.AddRange(group.Rubies);
                        }
                    }
                }
            }

            if (overflowExtensionComponent != null && overflowExtensionComponent.EllipsisEnabled)
            {
                var ellipsisGlyphSpec = overflowExtensionComponent.CreateFontGlyphSpec(font, FontSize, DefaultFontStyle, color, GlyphScaleX);
                this.allGlyphSpecs.Add(ellipsisGlyphSpec);
            }
        }

        /// <summary>
        /// Squeezeを行う必要がある場合、Squeezeを行います。
        /// </summary>
        /// <returns>Squeezeが行われて、GlyphScaleが変わった場合、trueを返します</returns>
        bool SqueezeIfNeeded()
        {
            if (this.overflowExtensionComponent == null || !this.overflowExtensionComponent.SqueezeEnabled)
            {
                return false;
            }

            if (this.logicalLines == null || this.logicalLines.Count == 0)
            {
                this.overflowExtensionComponent.UnnecessarySqueeze(1.0f);
                return false;
            }

            float glyphScaleX = this.GlyphScaleX;
            TextViewOverflowExtension.ISqueezeParameter parameter = null;

            switch (overflowExtensionComponent.CriterionSize)
            {
                case TextViewOverflowExtension.SqueezeAndShrinkCriterionSize.FirstLineWidth:
                    // Squeezeの計算に必要なので、先に1行分だけ論理行を生成する
                    if (assembledLogicalLines.Count == 0)
                    {
                        var option = new LineRenderingElementsAssembly.AssembleOption(this);
                        assembledLogicalLines.Add(LineRenderingElementsAssembly.Assemble(this.logicalLines[0], GlyphCatalog.GetCatalog(font), option));
                    }
                    parameter = new TextViewOverflowExtension.SqueezeFirstLineWidthParameter(glyphScaleX, this.assembledLogicalLines[0].Width);
                    break;
                case TextViewOverflowExtension.SqueezeAndShrinkCriterionSize.AllLineHeight:
                    // 高さを計算するために必要なグリフの配置等のプロセスは、
                    // Squeezeとシュリンクが終わっている状態でしか実行できない用になっているので、
                    // 一旦終わったという扱いにして処理ができるようにする
                    this.model.ProcessExecuted(WorkflowProcess.Squeeze);
                    this.model.ProcessExecuted(WorkflowProcess.Shrink);
                    parameter = new TextViewOverflowExtension.SqueezeAllLineHeightParameter(
                        glyphScaleX,
                        GetPreferredWidth(PreferredWidthType.AllLogicalLine),
                        GetPreferredHeight(PreferredHeightType.AllLine));
                    
                    this.model.RequireProcess(WorkflowProcess.Shrink);
                    break;
                default:
                    UnityEngine.Assertions.Assert.IsTrue(false, "If you support all SqueezeAndShrinkCriterionSize, you will not reach here.");
                    break;
            }

            // GlyphScaleXとSqueeze後のサイズに差異があればSqueezeが実行されている
            return !Mathf.Approximately(glyphScaleX, overflowExtensionComponent.CalculateSqueezedSize(parameter));
        }

        /// <summary>
        /// シュリンクを行う必要がある場合、シュリンクを行います。
        /// </summary>
        /// <returns>シュリンクが行われて、フォントサイズが変わった場合、<c>true</c>を返します。</returns>
        bool ShrinkIfNeeded()
        {
            // 必要であればシュリンクを行う
            if (this.overflowExtensionComponent == null || !this.overflowExtensionComponent.ShrinkEnabled || this.overflowExtensionComponent.ShrinkMinFontSize >= this.fontSize)
            {
                return false;
            }
            // 入力がない状態ではシュリンクも発生しない
            if (this.logicalLines == null || this.logicalLines.Count == 0)
            {
                this.overflowExtensionComponent.UnnecessaryShrink(this.fontSize);
                return false;
            }

            int fontSize = this.FontSize;
            TextViewOverflowExtension.IShrinkParameter parameter = null;

            switch (overflowExtensionComponent.CriterionSize)
            {
                case TextViewOverflowExtension.SqueezeAndShrinkCriterionSize.FirstLineWidth:
                    // シュリンクの計算に必要なので、先に1行分だけ論理行を生成する
                    if (assembledLogicalLines.Count == 0)
                    {
                        var option = new LineRenderingElementsAssembly.AssembleOption(this);
                        assembledLogicalLines.Add(LineRenderingElementsAssembly.Assemble(this.logicalLines[0], GlyphCatalog.GetCatalog(font), option));
                    }
                    parameter = new TextViewOverflowExtension.ShrinkFirstLineWidthParameter(fontSize, this.assembledLogicalLines[0].Width);
                    break;
                case TextViewOverflowExtension.SqueezeAndShrinkCriterionSize.AllLineHeight:
                    // 高さを計算するために必要なグリフの配置等のプロセスは、
                    // シュリンクが終わっている状態でしか実行できない用になっているので、
                    // 一旦終わったという扱いにして処理ができるようにする
                    this.model.ProcessExecuted(WorkflowProcess.Shrink);
#pragma warning disable 618
                    parameter = new TextViewOverflowExtensionMultiLine.ShrinkAllLineHeightParameter(
                        fontSize,
                        GetPreferredHeight(PreferredHeightType.AllLine),
                        GetPreferredHeight(PreferredHeightType.AllLogicalLine));
#pragma warning restore 618
                    break;
                default:
                    UnityEngine.Assertions.Assert.IsTrue(false, "If you support all SqueezeAndShrinkCriterionSize, you will not reach here.");
                    break;
            }

            // フォントサイズとシュリンク後のサイズに差異があればシュリンクが実行されている
            return overflowExtensionComponent.CalculateShrinkedFontSize(parameter) != fontSize;
        }

        /// <summary>
        /// RectTransformのサイズが変化したことによって、シュリンクを再度行う必要があるかどうか確認します。
        /// </summary>
        void CheckSqueezeAndShrinkByRectTransform()
        {
            if (!this.isNecessaryCheckSqueezeAndShrink)
            {
                return;
            }

            this.isNecessaryCheckSqueezeAndShrink = false;

            if (this.overflowExtensionComponent != null && (this.overflowExtensionComponent.SqueezeEnabled || this.overflowExtensionComponent.ShrinkEnabled))
            {
                if (Mathf.Approximately(DefaultGlyphScaleX, GlyphScaleX) && this.fontSize == this.FontSize)
                {
                    switch (this.overflowExtensionComponent.CriterionSize)
                    {
                        case TextViewOverflowExtension.SqueezeAndShrinkCriterionSize.FirstLineWidth:
                            // シュリンク後のサイズと設定されているフォントサイズに違いがない場合は
                            // 表示に必要な幅を下回っていない限り、再計算をする必要はない
                            float lineWidth = 0.0f;
                            if (this.assembledLogicalLines.Count > 0)
                            {
                                lineWidth = assembledLogicalLines[0].Width;
                            }

                            float width = Mathf.Max(rectTransform.rect.width, 0.0f);
                            if (width >= lineWidth)
                            {
                                return;
                            }
                            break;
                        case TextViewOverflowExtension.SqueezeAndShrinkCriterionSize.AllLineHeight:
                            // 複数行の場合は、実際に配置してみないと
                            // シュリンクが必要かの正確な判別ができないため、
                            // サイズ変更があったら再計算するようにする
                            break;
                        default:
                            UnityEngine.Assertions.Assert.IsTrue(false, "If you support all SqueezeAndShrinkCriterionSize, you will not reach here.");
                            break;
                    }
                }
                
                // 一度Squeezeやシュリンクが行われると、TextViewが返す値はSqueezeとシュリンクされた値となり、
                // それによってTextModifierが返す値もSqueezeとシュリンク後の数値となるため、
                // 一度それらをクリアして再計算しないと正しい状態が取得できないため
                // 論理行の生成まで戻ってやり直す
                this.overflowExtensionComponent.RequireCalculate();
                this.RequireProcessByUpdateProperty(WorkflowProcess.LogicalLinesCreate);
            }
        }

        /// <summary>
        /// RectTransformの状態の変化によって、レイアウトの再計算を行う必要があるかどうか確認します。
        /// </summary>
        void CheckRecalculateByRectTransform()
        {
            if (this.isSkipRecalculate)
            {
                this.isSkipRecalculate = false;
                return;
            }

            Vector2 size = rectTransform.rect.size;
            Vector2 pivot = rectTransform.pivot;

            // HasLayoutRebuiltは、レイアウトのリビルド中によってサイズが変化した場合のみ、
            // trueとなるため、1フレーム目はこの条件は絶対に成立しない
            if (this.hasDimensionChangedLayoutRebuilt)
            {
                this.hasDimensionChangedLayoutRebuilt = false;
                this.dimensionChangedAfterFirstUpdate = null;
                this.lastSize = size;
                this.lastPivot = pivot;

                this.model.RequireProcess(WorkflowProcess.GlyphPlacement);
                RefreshInlineImageIfNeeded();
                // レイアウトのリビルドが発生した場合の再計算では、
                // この時点でDirtyフラグが立っていない状態がありえるので明示的に立てる
                SetVerticesDirty();
                return;
            }

            UnityEngine.Assertions.Assert.IsFalse(this.dimensionChangedAfterFirstUpdate != null && this.dimensionChangedAfterFirstUpdate.Value);

            // RectTransformの状態が変化した際に、
            // レイアウトを再計算しなければいけないのは、
            // rect.sizeとpivotの2パターン
            // 他は変化しても、直接的に計算には影響しない

            if (this.lastSize != size)
            {
                this.lastSize = size;
                this.lastPivot = pivot;

                this.model.RequireProcess(WorkflowProcess.GlyphPlacement);
                RefreshInlineImageIfNeeded();
                return;
            }

            if (this.lastPivot != pivot)
            {
                this.lastPivot = pivot;

                // Pivotが変化した場合、文字を表示するオフセット位置を変更しないと
                // 表示位置がズレてしまうので、位置の再計算から行う
                this.model.RequireProcess(WorkflowProcess.LinesOffsetYCalculate);
                this.model.RequireProcess(WorkflowProcess.TextLinesPositionXCalculate);

                return;
            }
        }

        /// <summary>
        /// 必要な場合のみ、インライン画像のレイアウト情報が再計算されるように設定します。
        /// </summary>
        void RefreshInlineImageIfNeeded()
        {
            // アライメントをCenterにしている状態かつPivotが中央の場合は、
            // サイズが変更された際にインライン画像の表示位置がズレる
            if (this.viewAlignment == TextAlign.Center && this.rectTransform.pivot.x == 0.5f)
            {
                this.InlineImageLayout.SetLayoutDirty();
            }
            else if (this.verticalAlignment == TextVerticalAlign.Center && this.rectTransform.pivot.y == 0.5f)
            {
                this.InlineImageLayout.SetLayoutDirty();
            }
        }

        /// <summary>
        /// GlyphPlacementの生成および改行処理を行ってTextLineのリストを生成します
        /// </summary>
        /// <param name="width"></param>
        void CreateAllTextLines(float width)
        {
            GlyphCatalog catalog = GlyphCatalog.GetCatalog(font);
            var option = new LineRenderingElementsAssembly.AssembleOption(this);

            // 初期化
            this.allTextLines.Clear();

            // シュリンクを行っている場合は、先に1行分作っている
            // すでに一度物理行の生成を行っている場合は、論理行は全て作成済みなので再生成はしない
            for (int i = this.assembledLogicalLines.Count; i < this.logicalLines.Count; ++i)
            {
                assembledLogicalLines.Add(LineRenderingElementsAssembly.Assemble(this.logicalLines[i], catalog, option));
            }

            foreach (LineRenderingElements assembledLogicalLine in this.assembledLogicalLines)
            {
                // 自動改行処理を適用する
                var physicalLines = AutoLineBreakRule.Apply(
                    assembledLogicalLine,
                    width,
                    Jigbox.TextView.LineBreakRule.GetLineBreakRule(this.LineBreakRule),
                    this.UseBurasage,
                    null,
                    isKeepTailSpace
                );

                // 2行目以降がある場合は、自動改行された行としてマークする
                for (int i = 1; i < physicalLines.Count; ++i)
                {
                    physicalLines[i].IsAutoLineBreak = true;
                }

                this.allTextLines.AddRange(physicalLines);
            }

            if (this.allTextLines.Count > 0)
            {
                // 最終行のTextLineに最終行であることを示すフラグを立てる
                this.allTextLines[this.allTextLines.Count - 1].IsLastLine = true;
            }

            this.maxIndex = 0;
            for (var i = this.allTextLines.Count - 1; i >= 0; i--)
            {
                var tempMaxIndex = this.allTextLines[i].MaxIndex();
                if (tempMaxIndex != -1)
                {
                    this.maxIndex = tempMaxIndex;
                    break;
                }
            }

            this.visibleMinIndex = 0;
            this.visibleMaxIndex = this.maxIndex;
        }

        /// <summary>
        /// 省略文字を含めて改行処理を行ったTextLineのリストを生成します
        /// </summary>
        /// <param name="width"></param>
        void CreateTextLinesIncludeEllipsis(float width)
        {
            int skipLogicalLineCount = 0;
            int clearStartLine = -1;
            for (var i = overflowExtensionComponent.EllipsisLine; i >= 0; i--)
            {
                if (!allTextLines[i].IsAutoLineBreak)
                {
                    if (clearStartLine == -1)
                    {
                        clearStartLine = i;
                    }
                    else
                    {
                        skipLogicalLineCount++;
                    }
                }
            }

            var removeCount = allTextLines.Count - clearStartLine;
            for (var i = 0; i < removeCount; i++)
            {
                allTextLines.RemoveAt(allTextLines.Count - 1);
            }

            int skip = 0;
            foreach (LineRenderingElements assembledLogicalLine in this.assembledLogicalLines)
            {
                if (skip < skipLogicalLineCount)
                {
                    skip++;
                    continue;
                }

                // 自動改行処理を適用する
                var physicalLines = AutoLineBreakRule.Apply(
                    assembledLogicalLine,
                    width,
                    Jigbox.TextView.LineBreakRule.GetLineBreakRule(this.LineBreakRule),
                    this.UseBurasage,
                    new TextViewOverflowExtension.AutoLineEllipsisParameter(overflowExtensionComponent, allTextLines.Count),
                    isKeepTailSpace
                );

                // 2行目以降がある場合は、自動改行された行としてマークする
                for (int i = 1; i < physicalLines.Count; ++i)
                {
                    physicalLines[i].IsAutoLineBreak = true;
                }

                this.allTextLines.AddRange(physicalLines);
            }

            if (this.allTextLines.Count > 0)
            {
                // 最終行のTextLineに最終行であることを示すフラグを立てる
                this.allTextLines[this.allTextLines.Count - 1].IsLastLine = true;
            }

            this.maxIndex = 0;
            for (var i = this.allTextLines.Count - 1; i >= 0; i--)
            {
                var tempMaxIndex = this.allTextLines[i].MaxIndex();
                if (tempMaxIndex != -1)
                {
                    this.maxIndex = tempMaxIndex;
                    break;
                }
            }
            
            AddEllipsisGlyphPlacement();

            this.visibleMinIndex = 0;
            this.visibleMaxIndex = this.maxIndex;
        }

        /// <summary>
        /// 省略文字のGlyphPlacementを追加します
        /// </summary>
        protected virtual void AddEllipsisGlyphPlacement()
        {
            // 省略文字を対象のTextLineに追加する
            var textLine = allTextLines[overflowExtensionComponent.EllipsisLine];
            var glyphIndex = 0;
            var glyphX = 0.0f;
            var textLineGlyphLength = textLine.PlacedGlyphs.Length;
            // Glyphが1文字でもある場合はこちらを基準にする
            if (textLineGlyphLength > 0)
            {
                // Glyphサイズがマイナスになって取得できない場合があるので最後のGlyphPlacementを入れる
                GlyphPlacement lastGlyphPlacement = textLine.PlacedGlyphs[textLineGlyphLength - 1];
                foreach (var glyphPlacement in textLine.PlacedGlyphs)
                {
                    var tempGlyphX = (glyphPlacement.X + glyphPlacement.Width);
                    if (glyphX < tempGlyphX)
                    {
                        lastGlyphPlacement = glyphPlacement;
                        glyphX = tempGlyphX;
                    }
                }

                switch (SpacingUnit)
                {
                    case SpacingUnit.BeforeCharacterWidth:
                        // NOTE: intへのキャストは切り捨てされていた部分をそのままにして既存の挙動が変わらないようにしています
                        glyphX += (int) (CharacterSpacing * lastGlyphPlacement.Width);
                        break;
                    case SpacingUnit.FontSize:
                        // NOTE: intへのキャストは切り捨てされていた部分をそのままにして既存の挙動が変わらないようにしています
                        glyphX += (int) (CharacterSpacing * FontSize) * GlyphScaleX;
                        break;
                }

                glyphIndex = lastGlyphPlacement.Index + 1;
            }
            else
            {
                for (int i = overflowExtensionComponent.EllipsisLine; i >= 0; i--)
                {
                    var maxIndex = allTextLines[i].MaxIndex();
                    if (maxIndex != -1)
                    {
                        glyphIndex = maxIndex + 1;
                        break;
                    }
                }
            }

            var glyph = overflowExtensionComponent.GetEllipsisGlyph();
            var glyphPlace = new GlyphPlacement(glyph, glyphX, 0, glyphIndex, true, null, false, PunctuationHalfType.None);
            textLine.AddGlyphPlacement(glyphPlace);
        }

        /// <summary>
        /// インライン画像配置のプロセスを実行します
        /// </summary>
        void PlacementInlineImage()
        {
            this.InlineImageLayout.ClearReservedImages();

            // アラビア語で使用するための値
            // アラビア語でなければ使用しないがループ内で計算させたくないためここで計算させておく
            float mirrorOffsetX = GetArabicMirrorOffset();

            foreach (var element in this.horizontalLayout)
            {
                if (element is HorizontalLayoutedImage)
                {
                    var image = (HorizontalLayoutedImage) element;
                    var imageGlyph = (InlineImageGlyph) image.GlyphPlacement.Glyph;
                    if (imageGlyph.Source == null)
                    {
                        continue;
                    }

                    var x = image.OffsetX + image.GlyphPlacement.X;
                    // アラビア語の場合は鏡表示をしているためxの座標位置が変わる
                    if (enableMirror)
                    {
                        x = -(image.OffsetX + image.GlyphPlacement.X + imageGlyph.Width) + mirrorOffsetX;
                    }
                    this.InlineImageLayout.Add(x,
                        image.OffsetY - image.GlyphPlacement.Y - this.offsetY - this.visibleStartLineOffsetY,
                        imageGlyph.Width, imageGlyph.Height, imageGlyph.Source, imageGlyph.Name);
                }
            }

            this.InlineImageLayout.Place();
        }
        

        /// <summary>
        /// レンダリングのプロセスを実行します
        /// </summary>
        /// <param name="vertexHelper"></param>
        void FillVertices(VertexHelper vertexHelper)
        {
            float verticalOffset = -this.offsetY - this.visibleStartLineOffsetY;

            int divideLength = longTextExtension != null ? longTextExtension.DivideLength : int.MaxValue;
            int index = 0;

            if (longTextExtension != null)
            {
                longTextExtension.SetLength(this.horizontalLayout.DisplayableElementEndIndex);
                longTextExtension.BeginFill();
            }

            // アラビア語で使用するための値
            // アラビア語でなければ使用しないがループ内で計算させたくないためここで計算させておく
            float mirrorOffsetX = GetArabicMirrorOffset();

            foreach (var element in this.horizontalLayout)
            {
                // InlineImageは別コンポーネントで描画するのでTextViewで描画は処理しない
                if (element is HorizontalLayoutedGlyph)
                {
                    HorizontalLayoutedGlyph layoutedGlyph = element as HorizontalLayoutedGlyph;
                    if (!layoutedGlyph.GlyphPlacement.Glyph.IsWhiteSpaceOrControl)
                    {
                        if (!enableMirror)
                        {
                            layoutedGlyph.GetVertices(ref verticesToFill, verticalOffset, color, layoutedGlyph.GlyphPlacement.PunctuationHalfType);
                        }
                        else
                        {
                            layoutedGlyph.GetMirrorVertices(ref verticesToFill, verticalOffset, color, mirrorOffsetX);
                        }

                        if (index < divideLength)
                        {
                            vertexHelper.AddUIVertexQuad(verticesToFill);
                        }
                        else
                        {
                            longTextExtension.AddUIVertexQuad(verticesToFill);
                        }
                        ++index;
                    }
                }
            }

            if (longTextExtension != null)
            {
                longTextExtension.EndFill();
            }

            if (this.horizontalLayout.OverflowLine != null && this.horizontalLayout.OverflowRemainText != null)
            {
                var isEllipsis = overflowExtensionComponent != null && overflowExtensionComponent.IsEllipsisFlow;
                OnOverflowTextLine((int) this.horizontalLayout.OverflowLine, this.horizontalLayout.OverflowRemainText, isEllipsis);
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            // 描画に参照すべきフォントが無い
            if (Font == null)
            {
                return;
            }

            // LateUpdate以降でフォントのリビルドが発生した場合は、
            // このタイミングで情報を更新する
            if (this.model.CanExecuteProcess(WorkflowProcess.FontTextureUVUpdate))
            {
                this.UpdateGlyph();
                this.model.ProcessExecuted(WorkflowProcess.FontTextureUVUpdate);
            }

            // 最初のフレームでuGUIのリビルドが始まってからRectTransformのサイズが変更された場合は
            // LayoutGroup等からサイズの変更が入っているという状況で、
            // この場合は、LateUpdate時点ではRectTransformのサイズが0の状態となっており
            // 正しい状態に計算できていないので、レンダリングをスキップする
            if (this.dimensionChangedAfterFirstUpdate != null && this.dimensionChangedAfterFirstUpdate.Value)
            {
                // フォントのリビルドが行われる可能性があるため、
                // この時点ではフラグを変更しない
                return;
            }

            // バッチ処理後にサイズ変更が行われCalculateLayoutが必要な場合は描画を行わない
            // この処理は上記のLayoutGroupからのサイズ変更とは別のケースのサイズ変更に備えている
            // ユーザ側もしくはJigboxの別コンポーネントの仕様により、文字列は変更されずにサイズだけ変更される場合がある
            // その処理がTextViewObserverのLateUpdateより遅い場合に文字列が正しく表示されないためCalculateLayoutが行われるのを待つ
            if (this.hasDimensionChangedAfterBatched)
            {
                return;
            }

            // レンダリング
            if (this.model.CanExecuteProcess(WorkflowProcess.Rendering))
            {
                this.FillVertices(vh);
                this.model.ProcessExecuted(WorkflowProcess.Rendering);
            }

            // 一度処理された後は、使用しないのでnullにする
            if (this.dimensionChangedAfterFirstUpdate != null)
            {
                this.dimensionChangedAfterFirstUpdate = null;
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// UnityEditorのデフォルト機能でインスペクタ上で右クリックされた時に出てくる「Reset」から呼ばれる
        /// </summary>
        protected override void Reset()
        {
            this.Font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

#endif

#region Calculate Itself Propotion

        /// <summary>
        /// TextViewがテキストを表示するために必要な幅を取得します。
        /// </summary>
        /// <param name="type">必要な幅を求める方法</param>
        public float GetPreferredWidth(PreferredWidthType type = PreferredWidthType.FirstLine)
        {
            if (this.font == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Can't calculate preferred width, because Font is null!");
#endif
                return 0.0f;
            }

            ForceUpdate();

            if (measurer == null)
            {
                this.measurer = new PreferredSizeMeasurer(this);
            }

            this.measurer.CalculatePreferredWidth(this.assembledLogicalLines, type);
            return this.measurer.PreferredWidth;
        }

        /// <summary>
        /// PreferredSizeのキャッシュを行います
        /// </summary>
        protected virtual void CachePreferredSize()
        {
            if (layoutElement == null)
            {
                return;
            }

            if (measurer == null)
            {
                this.measurer = new PreferredSizeMeasurer(this);
            }

            if (measurer.HasCache)
            {
                return;
            }

            this.measurer.CalculatePreferredWidth(this.assembledLogicalLines, layoutElement.PreferredWidthType);
            this.measurer.CalculatePreferredHeight(this.allTextLines, layoutElement.PreferredHeightType);
        }

        /// <summary>
        /// Textにアラビア語を含めているか判定します
        /// </summary>
        /// <returns></returns>
        protected bool GetHasArabic()
        {
            foreach (TextRun run in textSource.TextRuns)
            {
                if (run is ArabicTextCharacters)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// TextViewがテキストを表示するために必要な高さを取得します。
        /// </summary>
        /// <param name="type">必要な高さを求める方法</param>
        /// <returns></returns>
        public float GetPreferredHeight(PreferredHeightType type = PreferredHeightType.AllLine)
        {
            if (this.font == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Can't calculate preferred height, because Font is null!");
#endif
                return 0.0f;
            }

            ForceUpdate();

            if (measurer == null)
            {
                this.measurer = new PreferredSizeMeasurer(this);
            }

            this.measurer.CalculatePreferredHeight(this.allTextLines, type);
            return this.measurer.PreferredHeight;
        }

        /// <summary>
        /// TextViewがテキストを表示するために必要な高さを取得します。
        /// </summary>
        /// <param name="isHeightDependingOnVisibleLength">高さを表示されているテキストを元に算出するかどうか</param>
        /// <returns></returns>
        public float GetPreferredHeight(bool isHeightDependingOnVisibleLength)
        {
            if (this.font == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Can't calculate preferred height, because Font is null!");
#endif
                return 0.0f;
            }

            ForceUpdate();

            if (measurer == null)
            {
                this.measurer = new PreferredSizeMeasurer(this);
            }

            PreferredHeightType type = isHeightDependingOnVisibleLength ? PreferredHeightType.Visibled : PreferredHeightType.AllLine;
            this.measurer.CalculatePreferredHeight(this.allTextLines, type);
            return this.measurer.PreferredHeight;
        }

#endregion

#region Call by Observer

        /// <summary>
        /// テキスト情報をパースして、TextViewとして処理するための情報を生成します。
        /// </summary>
        public void ParseText()
        {
            OnInitializeIfNeeded();

            // テキストパースのプロセス
            if (this.model.CanExecuteProcess(WorkflowProcess.TextParse))
            {
                CreateTextSource();
                this.model.ProcessExecuted(WorkflowProcess.TextParse);
            }
        }

        /// <summary>
        /// <para>フォントのリクエストに必要な情報を生成し、Glyphを生成します。</para>
        /// <para>シュリンクが必要な場合は、Glyph生成後にシュリンクを行います。</para>
        /// </summary>
        /// <returns>シュリンクが行われて、フォントサイズが変わった場合、<c>true</c>を返します。</returns>
        public bool RequestFontAndCreateGlyph()
        {
            CheckSqueezeAndShrinkByRectTransform();
            this.RefreshFontFaceInfo();

            bool shrinking = false;
            // 論理行生成のプロセス
            if (this.model.CanExecuteProcess(WorkflowProcess.LogicalLinesCreate))
            {
                CreateLogicalLines();
                this.model.ProcessExecuted(WorkflowProcess.LogicalLinesCreate);
            }

            // 要求グリフ一覧生成のプロセス
            if (this.model.CanExecuteProcess(WorkflowProcess.RequestGlyphSpecsCreate))
            {
                CreateRequestGlyphSpecs();
                this.model.ProcessExecuted(WorkflowProcess.RequestGlyphSpecsCreate);
            }

            // グリフ生成のプロセス
            if (this.model.CanExecuteProcess(WorkflowProcess.CreateGlyph))
            {
                if (!CreateGlyphs())
                {
                    // 生成途中でフォントのリビルドが発生した場合は処理を中断して戻る
                    return shrinking;
                }

                this.model.ProcessExecuted(WorkflowProcess.CreateGlyph);

                // すでにレイアウト情報の計算が終わっている場合は
                // 計算された情報のグリフ情報を新しい情報で上書きする
                if (this.AlreadyCalculatedLayout)
                {
                    UpdateGlyph();
                }
            }

            // Squeezeのプロセス
            if (this.model.CanExecuteProcess(WorkflowProcess.Squeeze))
            {
                shrinking = SqueezeIfNeeded();
                if (shrinking)
                {
                    this.model.RequireProcess(WorkflowProcess.LogicalLinesCreate);
                    return shrinking;
                }
                else
                {
                    this.model.ProcessExecuted(WorkflowProcess.Squeeze);
                }
            }

            // シュリンクのプロセス
            if (this.model.CanExecuteProcess(WorkflowProcess.Shrink))
            {
                shrinking = ShrinkIfNeeded();
                if (shrinking)
                {
                    // シュリンクが発生した場合は、それに合わせて再計算を行えるようにフラグを調整
                    this.model.RequireProcess(WorkflowProcess.LogicalLinesCreate);
                }
                else
                {
                    this.model.ProcessExecuted(WorkflowProcess.Shrink);
                }
            }

            return shrinking;
        }

        /// <summary>
        /// 描画に必要なレイアウト位置などの情報を計算します。
        /// </summary>
        public void CalculateLayout(bool isPlacedGlyph = false)
        {
            if (!this.model.CanExecuteProcess(WorkflowProcess.EllipsisCalculate))
            {
                CheckRecalculateByRectTransform();

                // グリフ配置と改行処理(GlyphPlacementの生成とTextLineの生成)のプロセス
                if (this.model.CanExecuteProcess(WorkflowProcess.GlyphPlacement))
                {
                    if (this.overflowExtensionComponent != null)
                    {
                        this.overflowExtensionComponent.SetEllipsisParameter(-1, 0, SpacingUnit.BeforeCharacterWidth);
                    }

                    var paragraphWidth = this.HorizontalOverflow == HorizontalWrapMode.Wrap ? Mathf.Max(this.rectTransform.rect.width, 0.0f) : float.MaxValue;

                    CreateAllTextLines(paragraphWidth);
                    // 一旦全てのIHorizontalLayoutedElementを生成します
                    CreateHorizontalLayoutedElements();
                    this.model.ProcessExecuted(WorkflowProcess.GlyphPlacement);

                    if (this.measurer != null)
                    {
                        this.measurer.InvalidateCache();
                    }

                    isPlacedGlyph = true;
                }
            }
            else
            {
                var paragraphWidth = this.HorizontalOverflow == HorizontalWrapMode.Wrap ? Mathf.Max(this.rectTransform.rect.width, 0.0f) : float.MaxValue;

                CreateTextLinesIncludeEllipsis(paragraphWidth);
                // 一旦全てのIHorizontalLayoutedElementを生成します
                CreateHorizontalLayoutedElements();

                if (this.measurer != null)
                {
                    this.measurer.InvalidateCache();
                }

                this.model.RequireProcess(WorkflowProcess.LinesOffsetYCalculate);
                this.model.RequireProcess(WorkflowProcess.TextLinesPositionXCalculate);

                this.model.ProcessExecuted(WorkflowProcess.EllipsisCalculate);
            }

            // 行毎のY軸位置計算のプロセス
            if (this.model.CanExecuteProcess(WorkflowProcess.LinesOffsetYCalculate))
            {
                CalculateLinesOffsetY();

                // 値を反映
                var index = 0;
                foreach (var textLine in this.allTextLines)
                {
                    int length = textLine.PlacedGlyphs.Length;
                    for (int i = 0; i < length; ++i)
                    {
                        var element = this.horizontalLayout.GetElementAtIndex(index);
                        element.TextLineOffsetY = textLine.LineY;
                        index++;
                    }
                }

                // HorizontalLayoutedElementのYが変化しているのでY軸のオフセット位置を再計算する必要がある
                this.model.RequireProcess(WorkflowProcess.DisplayableElementIndexCalculate);
                this.model.RequireProcess(WorkflowProcess.CurrentOffsetYCalculate);

                this.model.ProcessExecuted(WorkflowProcess.LinesOffsetYCalculate);
            }

            // X軸のオフセット計算のプロセス
            if (this.model.CanExecuteProcess(WorkflowProcess.TextLinesPositionXCalculate))
            {
                CalculateTextLinesPositionX();

                // 値を反映
                var index = 0;
                foreach (var textLine in this.allTextLines)
                {
                    foreach (var p in textLine.PlacedGlyphs)
                    {
                        var element = this.horizontalLayout.GetElementAtIndex(index);
                        element.TextLineOffsetX = textLine.LineX;
                        element.JustifyShiftOffsetX = p.JustifyShiftOffsetX;
                        index++;
                    }
                }

                this.model.RequireProcess(WorkflowProcess.DisplayableElementIndexCalculate);

                this.model.ProcessExecuted(WorkflowProcess.TextLinesPositionXCalculate);
            }

            // 描画するHorizontalLayoutedElementのStartIndexとEndIndex計算のプロセス
            if (this.model.CanExecuteProcess(WorkflowProcess.DisplayableElementIndexCalculate))
            {
                this.model.ProcessExecuted(WorkflowProcess.EllipsisCalculate);

                // 実際に画面に表示するIHorizontalLayoutedElementの登録
                PickUpDisplayableHorizontalLayoutedElements();
                if (this.model.CanExecuteProcess(WorkflowProcess.EllipsisCalculate))
                {
                    CalculateLayout(isPlacedGlyph);

                    return;
                }

                this.model.ProcessExecuted(WorkflowProcess.DisplayableElementIndexCalculate);
            }

            // Y軸のオフセット計算のプロセス
            if (this.model.CanExecuteProcess(WorkflowProcess.CurrentOffsetYCalculate))
            {
                // 必要な場合はフォント情報を更新する
                // ランタイム中にAlignModeが変更された場合ここを通る
                this.RefreshFontFaceInfo();
                // HorizontalLayoutElementの登録が終了したらオフセット計算を行う
                this.offsetY = CalculateCurrentOffsetY();
                this.model.ProcessExecuted(WorkflowProcess.CurrentOffsetYCalculate);
            }

            // インライン画像配置のプロセス
            if (this.model.CanExecuteProcess(WorkflowProcess.InlineImagePlacement))
            {
                if (!CanvasUpdateRegistry.IsRebuildingGraphics())
                {
                    PlacementInlineImage();
                    this.model.ProcessExecuted(WorkflowProcess.InlineImagePlacement);
                }
            }

            // LayoutGroupとTextViewLayoutElementを使用しているときにFontのリビルドが起きると正しいサイズが取れなくなるときがある場合への対応
            CachePreferredSize();

            // グリフの配置処理を行っている場合は、HorizontalLayoutが最新状態となるため、
            // 必然的に内部のデータは全て正しいものとなっているはずなので、
            // 頂点情報を更新する必要はないと判断して、処理は行わずにフラグを折る
            if (isPlacedGlyph)
            {
                this.model.ProcessExecuted(WorkflowProcess.FontTextureUVUpdate);
            }
            else
            {
                if (this.model.CanExecuteProcess(WorkflowProcess.FontTextureUVUpdate))
                {
                    this.UpdateGlyph();
                    this.model.ProcessExecuted(WorkflowProcess.FontTextureUVUpdate);
                }
            }

            // EndUpdatePropertiesを呼び出し忘れた場合などにフラグが残ってしまうので、
            // 更新処理の最後まで到達した時点でフラグを折る
            this.isPropertiesUpdating = false;

            // バッチ処理後にサイズ変更がされた場合に以下の処理を行う
            if (this.hasDimensionChangedAfterBatched)
            {
                // バッチ処理後にサイズ変更されたフレームでは表示がずれる可能性があるため描画をさせないようにしている
                // CalculateLayoutが終わった後にOnPopulateMeshが呼ばれるようにDirtyをたてておく
                SetVerticesDirty();
                // バッチ処理後のサイズ変更に伴うCalculateLayoutの処理が終わったのでフラグはおる
                this.hasDimensionChangedAfterBatched = false;
            }
        }

        /// <summary>
        /// Glyphの生成を行います。
        /// </summary>
        /// <returns>生成中にフォントのリビルドが発生した場合、<c>false</c>を返します。</returns>
        public bool CreateGlyphs()
        {
            // assembledLogicalLinesがキャッシュされている場合、新しく生成したGlyphと
            // データの食い違いが起きる可能性があるので、キャッシュをクリアする
            this.assembledLogicalLines.Clear();

            var glyphCatalog = GlyphCatalog.GetCatalog(this.Font);
            return glyphCatalog.CreateGlyphs(this.allGlyphSpecs);
        }

        /// <summary>
        /// OnPopulateが完了した後に、頂点情報が変化した場合に頂点情報を再設定します。
        /// </summary>
        public void Repopulate()
        {
            // OnPopulateMeshで頂点情報を更新している最中にTextからフォントのリビルドが発生した場合のみ
            if (CanvasUpdateRegistry.IsRebuildingGraphics() || CanvasUpdateRegistry.IsRebuildingLayout())
            {
                if (!canvasRenderer.cull)
                {
                    bool alreadyRendered = !this.model.IsNecessaryProcess(WorkflowProcess.Rendering);
                    this.model.RequireProcess(WorkflowProcess.FontTextureUVUpdate);
                    // すでに頂点情報が設定されている場合は、もう一度設定し直す必要があるので
                    // 強制的に頂点情報の更新メソッドを呼び出す
                    if (alreadyRendered)
                    {
                        UpdateGeometry();
                    }
                }
                // カリング状態にある場合は、Graphic.Rebuildが呼び出されても、
                // ガード節によってすぐに抜けてしまうため、OnPopulateMeshが実行されない
                // それによって、本来行われるはずの計算が行われなくなってしまうため、
                // カリングされている場合は、直接処理を行うようにする
                else
                {
                    this.UpdateGlyph();
                }
            }
        }

        /// <summary>
        /// 配置済みのGlyph情報を更新します。
        /// </summary>
        public void UpdateLayoutedGlyph()
        {
            if (!canvasRenderer.cull)
            {
                RequireProcess(WorkflowProcess.FontTextureUVUpdate);
            }
            // カリング状態にある場合は、OnPopulateMeshが呼び出されないため、
            // Glyphを更新するプロセスが実行されないので、直接更新する
            // Observerのバッチ処理が終わっているタイミングでフォントリビルドが発生し
            // AlreadyCalculatedLayoutのフラグが立っていない場合
            // ForceUpdateによってフォント再生成処理が走っている状態
            // ForceUpdateの後続の処理にUpdateGlyphを任せるため、処理は行わない
            else if(AlreadyCalculatedLayout)
            {
                // Dirtyはカリング状態が切り替わった際に設定されているので、
                // Glyphの更新ができていれば、クリピング範囲内に入った時点で正しくレンダリングされる
                UpdateGlyph();
            }
        }

#endregion

#region Process Methods

        /// <summary>
        /// HorizontalLayoutに全てのIHorizontalLayoutedElementを生成します(Offsetの値は未設定)
        /// </summary>
        void CreateHorizontalLayoutedElements()
        {
            this.horizontalLayout.Clear();

            foreach (var textLine in this.allTextLines)
            {
                foreach (var p in textLine.PlacedGlyphs)
                {
                    if (p.Glyph is InlineImageGlyph)
                    {
                        this.horizontalLayout.Add(new HorizontalLayoutedImage(p));
                    }
                    else
                    {
                        this.horizontalLayout.Add(new HorizontalLayoutedGlyph(p));
                    }
                }
            }
        }

        /// <summary>
        /// 各行のY軸位置の計算を行ってTextLineに反映させます
        /// </summary>
        void CalculateLinesOffsetY()
        {
            // 開始位置
            var y = rectTransform.rect.yMax;
            // 行毎に空けるYの間隔
            var lineGap = LineGap;
            // 各行のOffset計算
            for (int i = 0; i < this.allTextLines.Count; ++i)
            {
                var textLine = this.allTextLines[i];
                // ベースラインより上の幅
                float upperHeight = 0.0f;
                // ベースラインより下の幅
                float underHeight = 0.0f;
                
                if (AlignMode == TextAlignMode.Placement)
                {
                    upperHeight = textLine.CalculateHeightUpperBaseLine(this.FontSize, this.isLineHeightFixed);
                    underHeight = textLine.CalculateHeightUnderBaseLine(this.font, upperHeight, this.isLineHeightFixed);
                }
                if (AlignMode == TextAlignMode.Font)
                { 
                    textLine.CalculateHeightFromFontInfo(
                        FontSize,
                        this.fontFaceInfo.pointSize, 
                        this.fontFaceInfo.scale, 
                        this.fontFaceInfo.ascentLine,
                        this.fontFaceInfo.descentLine,
                        isLineHeightFixed);

                    upperHeight = textLine.UpperHeightFromFont;
                    underHeight = textLine.UnderHeightFromFont;
                }
                
                y -= upperHeight;

                // TextLineにY軸を設定する
                textLine.LineY = y;

                y -= underHeight + lineGap;
            }
        }

        /// <summary>
        /// 行のX軸位置の計算を行ってTextLineに反映させます
        /// </summary>
        void CalculateTextLinesPositionX()
        {
            HorizontalAlignmentCalculator.Apply(this.allTextLines, this.viewAlignment, this.Justify, this.rectTransform.rect.width);

            var baseX = this.rectTransform.rect.xMin;
            foreach (var textLine in this.allTextLines)
            {
                textLine.LineX += baseX;
            }
        }

        /// <summary>
        /// 実際に画面に表示するIHorizontalLayoutedElementをHorizontalLayoutに設定します
        /// </summary>
        void PickUpDisplayableHorizontalLayoutedElements()
        {
            // 開始行
            var lineStart = Math.Max(VisibleLineStart, 0);

            // 開始行がそもそもallTextLinesよりも多ければ表示する行を0行とする
            if (lineStart >= this.allTextLines.Count)
            {
                this.horizontalLayout.DisplayableElementStartIndex = 0;
                this.horizontalLayout.DisplayableElementEndIndex = 0;
                return;
            }

            // allHorizontalLayoutedElementListから読み出すGlyphsのCount
            var startIndex = 0;

            // lineStartより前のGlyphはスキップするため、その数をカウントする
            for (int i = 0; i < lineStart; i++)
            {
                // スキップした分だけdisplayableGlyphsCountを進める
                startIndex += this.allTextLines[i].PlacedGlyphs.Length;
            }

            // VisibleMinIndexの設定
            this.visibleMinIndex = this.allTextLines[lineStart].MinIndex();
            this.visibleStartLineOffsetY = 0;
            // 最初の行ではない時だけstartLineYの設定が必要になる
            if (lineStart > 0)
            {
                // lineStartで指定されているTextLineのベースラインに高さを足した位置が開始位置となる
                var firstLine = this.allTextLines[lineStart];
                if (AlignMode == TextAlignMode.Placement)
                {
                    this.visibleStartLineOffsetY = 
                        rectTransform.rect.yMax + 
                        (firstLine.LineY + firstLine.CalculateHeightUpperBaseLine(this.FontSize, this.isLineHeightFixed));
                }

                if (AlignMode == TextAlignMode.Font)
                {
                    this.visibleStartLineOffsetY =
                        rectTransform.rect.yMax + (firstLine.LineY + firstLine.UpperHeightFromFont);
                }
            }

            // StartIndexを反映
            this.horizontalLayout.DisplayableElementStartIndex = startIndex;

            // VisibleLength設定時に必要になる最終行のIndex
            var lastLineIndex = lineStart;
            // VisibleLength設定時に必要になる最終行のPlacedGlyphsの数
            var lastLinePlacedGlyphsCount = 0;
            // 表示する行のPlacedGlyphsのCount
            var placedGlyphsCount = 0;
            // Fontモード時に使用するTextLineの高さの合計
            var totalTextLineHeight = 0.0f;

            var overedVisibleLength = false;

            // horizontalLayoutに残っているOverflowTextの情報をクリアする
            this.horizontalLayout.ClearOverflowText();

            this.visibleMaxIndex = 0;
            for (int i = lineStart; i < this.allTextLines.Count; i++)
            {
                var textLine = this.allTextLines[i];
                bool isOverflowFromRect = false;

                if (VerticalOverflow == VerticalWrapMode.Truncate)
                {
                    // PlacementとFontで異なるOverflowまでの計算をするためifで分ける
                    if (AlignMode == TextAlignMode.Placement)
                    {
                        // TextLineの配置位置がrectの最底部よりより小さい場合はoverflowしている判定
                        isOverflowFromRect = rectTransform.rect.yMin > textLine.LineY - this.visibleStartLineOffsetY;
                    }
                    if (AlignMode == TextAlignMode.Font)
                    {
                        // TextLine一行の高さを加算
                        totalTextLineHeight += this.allTextLines[i].CalculateHeightFromFontInfo(
                            FontSize,
                            this.fontFaceInfo.pointSize, 
                            this.fontFaceInfo.scale, 
                            this.fontFaceInfo.ascentLine,
                            this.fontFaceInfo.descentLine,
                            isLineHeightFixed);
                        // 二行目以降の場合は行間を加算
                        if (i > lineStart)
                        {
                            totalTextLineHeight += LineGap;
                        }

                        // TextLineの全体の高さがrect.heightより大きい場合は大きい場合はoverflowしている判定
                        isOverflowFromRect = totalTextLineHeight > rectTransform.rect.height;
                    }
                }

                if (isOverflowFromRect ||
                    overflowExtensionComponent != null && 
                    overflowExtensionComponent.IsEllipsisFlow &&
                    overflowExtensionComponent.EllipsisLine == i - 1)
                {
                    if (overflowExtensionComponent != null && 
                        overflowExtensionComponent.EllipsisEnabled && 
                        !overflowExtensionComponent.IsEllipsisFlow && 
                        i > 0) // iが0の場合は何も表示されていないので省略文字を表示する必要がない
                    {
                        overflowExtensionComponent.SetEllipsisParameter(i - 1, CharacterSpacing, SpacingUnit);

                        RequireProcess(WorkflowProcess.EllipsisCalculate);
                        return;
                    }

                    this.horizontalLayout.SetOverflowText(i, SplitTextLines(i, this.allTextLines));
                    // この時点で描画に必要な座標計算は終わるのでループから抜ける
                    break;
                }

                // 「描画できる文字のインデックス」を更新
                var maxIndex = textLine.MaxIndex();
                if (maxIndex != -1)
                {
                    this.visibleMaxIndex = maxIndex;
                }

                if (!overedVisibleLength)
                {
                    // 最後の行のGlyphPlacementの数を保持しておく
                    lastLinePlacedGlyphsCount = textLine.PlacedGlyphs.Length;
                    // 表示する行のPlacedGlyphsの数を足し込む
                    placedGlyphsCount += lastLinePlacedGlyphsCount;
                    // 最終行のindexを保持しておく
                    lastLineIndex = i;
                    // VisibleLengthの値をMaxIndexが越えた場合は終了する
                    if (this.visibleLength >= 0 && maxIndex >= this.visibleLength)
                    {
                        overedVisibleLength = true;
                    }
                }
            }

            var endIndex = startIndex + placedGlyphsCount;
            // VisibleLengthが設定されている場合はそれを考慮した値を設定する
            if (this.visibleLength != UnlimitedVisibleLength)
            {
                // 前の行の数に戻して表示する最終行の有効数を足す
                endIndex = startIndex + placedGlyphsCount - lastLinePlacedGlyphsCount + this.allTextLines[lastLineIndex].GetPlacedGlyphCountAtVisibleIndex(this.visibleLength);
            }

            // EndIndexを反映
            this.horizontalLayout.DisplayableElementEndIndex = endIndex;
        }

        /// <summary>
        /// PirovによるY軸のオフセット計算を行います(ただしHorizontalLayoutに必要なものが全て配置されている状態でないと正しい値が出ない)
        /// </summary>
        /// <returns></returns>
        float CalculateCurrentOffsetY()
        {
            // 補正値
            var remainderY = 0.0f;

            // 配置された文字ベースでの計算の場合は、horizontalLayout.Heightを利用して計算を行う
            if (AlignMode == TextAlignMode.Placement)
            {
                // pivotの設定によって配置されたElementの位置のyMinの値から, pivotから見た時のRectTransform.rect.yMax(Elementの基準点)の距離までをコンテンツ高さとして計算する.
                // (純粋にRectTransform.rect.heightを使ってしまうと, VerticalAlignmentがTop以外の時pivot.yが1の時のElementのyMin分だけずれてしまう)
                remainderY = Mathf.Max(rectTransform.rect.height, 0.0f) * rectTransform.pivot.y - this.visibleStartLineOffsetY;
                remainderY += this.horizontalLayout.yMin;
                remainderY -= this.horizontalLayout.Height;
            }

            // Font情報ベースでの計算の場合は、FontSizeとフォント情報の比率を計算し直してからBaseLine分y座標を補正する
            if (AlignMode == TextAlignMode.Font)
            {
                // 行が存在しない場合は計算を行わない
                if (this.allTextLines.Count == 0)
                {
                    return 0;
                }
                
                // Font情報ベースでの計算の場合は、配置情報を1から計算するためpivotの値を使わずheightをそのまま扱う
                remainderY = rectTransform.rect.height;

                // 末尾に改行のみの空白行が存在する場合、計算に適用しないように計算に適用される行を変更する
                int existTextLinesLimit = this.allTextLines.Count;
                
                // 末尾が空白行でない場合は計算処理を行わない
                if (this.allTextLines[this.allTextLines.Count - 1].PlacedGlyphs.Length == 0)
                {
                    for (int i = this.allTextLines.Count - 1; i >= this.visibleLineStart; i--)
                    {
                        if (this.allTextLines[i].PlacedGlyphs.Length != 0)
                        {
                            existTextLinesLimit = i + 1;
                            break;
                        }
                    }
                }

                for (int i = this.visibleLineStart; i < existTextLinesLimit; i++)
                {
                    // テキストのOverflow設定がWrap状態だった場合、溢れ出た文字列の高さを加味しないように修正
                    if (this.horizontalLayout.OverflowLine.HasValue)
                    {
                        if (this.horizontalLayout.OverflowLine.Value == i &&
                            this.verticalOverflow == VerticalWrapMode.Truncate)
                        {
                            break;
                        }
                    }

                    // 一行の高さを計算して補正する
                    remainderY -= this.allTextLines[i].CalculateHeightFromFontInfo(
                        FontSize,
                        this.fontFaceInfo.pointSize, 
                        this.fontFaceInfo.scale, 
                        this.fontFaceInfo.ascentLine,
                        this.fontFaceInfo.descentLine,
                        isLineHeightFixed);
                    
                    // 2行目以降の場合は行間を加味する
                    if (i > this.visibleLineStart)
                    {
                        remainderY -= LineGap;
                    }
                    
                    // VisibleLengthの設定より行のIndex値が大きい場合、次の行は表示されないためループを終了する
                    if (this.allTextLines[i].MaxIndex() >= this.visibleLength - 1 && 
                        this.visibleLength != UnlimitedVisibleLength)
                    {
                        break;
                    }
                }
            }

            switch (this.VerticalAlignment)
            {
                case TextVerticalAlign.Top:
                    return 0f;
                case TextVerticalAlign.Center:
                    return remainderY / 2;
                case TextVerticalAlign.Bottom:
                    return remainderY;
                default:
                    Debug.LogWarning("TextView.VerticalAlignment has unexpected value " + this.VerticalAlignment + ".");
                    return 0f;
            }
        }

        /// <summary>
        /// HorizontalLayout内のGlyph情報を更新します。
        /// </summary>
        void UpdateGlyph()
        {
            this.horizontalLayout.UpdateGlyph(this.font);
        }

#endregion
    }
}
