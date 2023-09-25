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
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Jigbox.Components;
using Jigbox.TextView;

namespace Jigbox.Examples
{
    public sealed class TextViewScreenShotCapture : MonoBehaviour
    {
#region inner classes, enum, and structs

        class TextViewModifier
        {
            /// <summary>変更内容</summary>
            public string Description { get; private set; }

            /// <summary>TextViewの設定を変更する処理</summary>
            public Action<Components.TextView> Modifier { get; private set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="description">変更内容</param>
            /// <param name="modifier">TextViewの設定を変更する処理</param>
            public TextViewModifier(string description, Action<Components.TextView> modifier)
            {
                Description = description;
                Modifier = modifier;
            }
        }

#endregion

#region constants

        /// <summary>画面をキャプチャするテクスチャの横幅</summary>
        static readonly int CaptureTextureWidth = 1136;

        /// <summary>画面をキャプチャするテクスチャの縦幅</summary>
        static readonly int CaptureTextureHeight = 640;

        /// <summary>スクリーンショットを出力するテクスチャの横幅</summary>
        static readonly int EncodeTextureWidth = TextViewScreenShotConstants.TextureWidth;

        /// <summary>スクリーンショットを出力するテクスチャの縦幅</summary>
        static readonly int EncodeTextureHeight = TextViewScreenShotConstants.TextureHeight;

        /// <summary>キャプチャしたテクスチャから色をフェッチするx位置</summary>
        static readonly int FetchPixelX = 0;

        /// <summary>キャプチャしたテクスチャから色をフェッチするy位置</summary>
        static readonly int FetchPixelY = 124;

        /// <summary>画面からスクリーンショットとして出力するテクスチャを切り出す範囲(iPhone5相当)</summary>
        static readonly Rect CaptureRect = new Rect(0.0f, 0.0f, CaptureTextureWidth, CaptureTextureHeight);

        /// <summary>TextViweの設定を変更する内容</summary>
        static readonly List<TextViewModifier> Modifiers = new List<TextViewModifier>()
        {
            new TextViewModifier("AlignMode=Placement", textView => textView.AlignMode = TextAlignMode.Placement),
            new TextViewModifier("TreatNewLineAsLineBreak=true", textView => textView.TreatNewLineAsLineBreak = true),
            new TextViewModifier("LineBreakRule=Korean", textView => textView.LineBreakRule = TextView.TextLineBreakRule.Korean),
            new TextViewModifier("LineBreakRule=SimplifiedChinese", textView => textView.LineBreakRule = TextView.TextLineBreakRule.SimplifiedChinese),
            new TextViewModifier("LineBreakRule=TraditionalChinese", textView => textView.LineBreakRule = TextView.TextLineBreakRule.TraditionalChinese),
            new TextViewModifier("Alignment=Left VerticalAlignment=Top", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Left;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Top;
                }),
            new TextViewModifier("Alignment=Center VerticalAlignment=Top", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Center;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Top;
                }),
            new TextViewModifier("Alignment=Right VerticalAlignment=Top", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Right;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Top;
                }),
            new TextViewModifier("Alignment=Left VerticalAlignment=Center", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Left;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Center;
                }),
            new TextViewModifier("Alignment=Center VerticalAlignment=Center", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Center;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Center;
                }),
            new TextViewModifier("Alignment=Right VerticalAlignment=Center", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Right;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Center;
                }),
            new TextViewModifier("Alignment=Left VerticalAlignment=Bottom", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Left;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Bottom;
                }),
            new TextViewModifier("Alignment=Center VerticalAlignment=Bottom", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Center;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Bottom;
                }),
            new TextViewModifier("Alignment=Right VerticalAlignment=Bottom", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Right;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Bottom;
                }),
            new TextViewModifier("SpacingUnit=BeforeCharacterWidth CharacterSpacing=0.2", textView =>
                {
                    textView.SpacingUnit = TextView.SpacingUnit.BeforeCharacterWidth;
                    textView.CharacterSpacing = 0.2f;
                }),
            new TextViewModifier("SpacingUnit=FontSize CharacterSpacing=0.2", textView =>
                {
                    textView.SpacingUnit = TextView.SpacingUnit.FontSize;
                    textView.CharacterSpacing = 0.2f;
                }),
            new TextViewModifier("LineHeight=0.8", textView => textView.LineHeight = 0.8f),
            new TextViewModifier("LineHeight=1.2", textView => textView.LineHeight = 1.2f),
            new TextViewModifier("IsLineHeightFixed=true", textView => textView.IsLineHeightFixed = true),
            new TextViewModifier("HorizontalOverflow=Overflow", textView => textView.HorizontalOverflow = HorizontalWrapMode.Overflow),
            new TextViewModifier("VerticalOverflow=Overflow", textView => textView.VerticalOverflow = VerticalWrapMode.Overflow),
            // AlignMode = Font
            new TextViewModifier("AlignMode=Font", textView => textView.AlignMode = TextAlignMode.Font),
            new TextViewModifier("AlignMode=Font TreatNewLineAsLineBreak=true", textView => textView.TreatNewLineAsLineBreak = true),
            new TextViewModifier("AlignMode=Font LineBreakRule=Korean", textView => textView.LineBreakRule = TextView.TextLineBreakRule.Korean),
            new TextViewModifier("AlignMode=Font LineBreakRule=SimplifiedChinese", textView => textView.LineBreakRule = TextView.TextLineBreakRule.SimplifiedChinese),
            new TextViewModifier("AlignMode=Font LineBreakRule=TraditionalChinese", textView => textView.LineBreakRule = TextView.TextLineBreakRule.TraditionalChinese),
            new TextViewModifier("AlignMode=Font Alignment=Left VerticalAlignment=Top", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Left;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Top;
                }),
            new TextViewModifier("AlignMode=Font Alignment=Center VerticalAlignment=Top", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Center;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Top;
                }),
            new TextViewModifier("AlignMode=Font Alignment=Right VerticalAlignment=Top", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Right;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Top;
                }),
            new TextViewModifier("AlignMode=Font Alignment=Left VerticalAlignment=Center", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Left;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Center;
                }),
            new TextViewModifier("AlignMode=Font Alignment=Center VerticalAlignment=Center", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Center;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Center;
                }),
            new TextViewModifier("AlignMode=Font Alignment=Right VerticalAlignment=Center", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Right;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Center;
                }),
            new TextViewModifier("AlignMode=Font Alignment=Left VerticalAlignment=Bottom", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Left;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Bottom;
                }),
            new TextViewModifier("AlignMode=Font Alignment=Center VerticalAlignment=Bottom", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Center;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Bottom;
                }),
            new TextViewModifier("AlignMode=Font Alignment=Right VerticalAlignment=Bottom", textView =>
                {
                    textView.Alignment = TextView.TextAlign.Right;
                    textView.VerticalAlignment = TextView.TextVerticalAlign.Bottom;
                }),
            new TextViewModifier("AlignMode=Font SpacingUnit=BeforeCharacterWidth CharacterSpacing=0.2", textView =>
                {
                    textView.SpacingUnit = TextView.SpacingUnit.BeforeCharacterWidth;
                    textView.CharacterSpacing = 0.2f;
                }),
            new TextViewModifier("AlignMode=Font SpacingUnit=FontSize CharacterSpacing=0.2", textView =>
                {
                    textView.SpacingUnit = TextView.SpacingUnit.FontSize;
                    textView.CharacterSpacing = 0.2f;
                }),
            new TextViewModifier("AlignMode=Font LineHeight=0.8", textView => textView.LineHeight = 0.8f),
            new TextViewModifier("AlignMode=Font LineHeight=1.2", textView => textView.LineHeight = 1.2f),
            new TextViewModifier("AlignMode=Font IsLineHeightFixed=true", textView => textView.IsLineHeightFixed = true),
            new TextViewModifier("AlignMode=Font HorizontalOverflow=Overflow", textView => textView.HorizontalOverflow = HorizontalWrapMode.Overflow),
            new TextViewModifier("AlignMode=Font VerticalOverflow=Overflow", textView => textView.VerticalOverflow = VerticalWrapMode.Overflow),
            new TextViewModifier("AlignMode=Font IsHalfPunctuationOfLineHead=true", textView =>
            {
                textView.IsHalfPunctuationOfLineHead = true;
            }),
        };

#endregion

#region properties

        /// <summary>Exampleシーンの制御用コンポーネント</summary>
        TextViewSample exampleController;

        /// <summary>キャプチャを開始するボタン</summary>
        BasicButton captureButton;

        /// <summary>サンプルテキストの総数</summary>
        int textCount = 0;

        /// <summary>現在表示しているサンプルテキストのインデックス</summary>
        int currentTextIndex = 0;

        /// <summary>テキストのオーバーフローのコールバックを受け取れるかどうか</summary>
        bool canRecieveOverflow = false;

        /// <summary>ページング時に設定するTextViewの開始行</summary>
        int nextLineIndex = 0;

        /// <summary>現在のTextViewの開始行</summary>
        int currentLineIndex = 0;

        /// <summary>ページング回数</summary>
        int pagingCount = 0;

        /// <summary>設定の変更内容のリストのインデックス</summary>
        int modifierIndex = 0;

        /// <summary>現在のTextViewの設定の変更内容</summary>
        TextViewModifier modifier = null;

        /// <summary>キャプチャ中かどうか</summary>
        bool isCapturing = false;

        /// <summary>キャプチャ用のテクスチャ</summary>
        Texture2D captureTexture = null;

        /// <summary>スクリーンショットのエンコード用のテクスチャ</summary>
        Texture2D encodeTexture = null;

        /// <summary>StringBuilder</summary>
        StringBuilder builder = new StringBuilder();

#endregion

#region public methods

        /// <summary>
        /// スクリーンショット撮影機能の初期化を行います。
        /// </summary>
        /// <param name="exampleController">Exampleシーンの制御用コンポーネント</param>
        /// <param name="captureButton">キャプチャを開始するボタン</param>
        /// <param name="textCount">サンプルテキストの総数</param>
        public void Init(TextViewSample exampleController, BasicButton captureButton, int textCount)
        {
            this.exampleController = exampleController;
            this.textCount = textCount;

            currentTextIndex = 0;
            nextLineIndex = 0;
            currentLineIndex = 0;
            pagingCount = 0;
            modifierIndex = 0;
            modifier = Modifiers[modifierIndex];

            this.exampleController.SetTextViewOverflow(OnOverflowTextLine);

            this.captureButton = captureButton;
            this.captureButton.AddEvent(InputEventType.OnClick, StartCapture);

            captureTexture = new Texture2D(CaptureTextureWidth, CaptureTextureHeight, TextureFormat.ARGB32, false);
            encodeTexture = new Texture2D(EncodeTextureWidth, EncodeTextureHeight, TextureFormat.ARGB32, false);

            string directoryPath = GetSaveDirectory();
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

#endregion

#region private methods

        /// <summary>
        /// 画像を保存するパスを取得します。
        /// </summary>
        /// <returns>スクリーンショットを保存するためのファイル名を含むパスを返します。</returns>
        string GetSavePath()
        {
            builder.Length = 0;
            builder.AppendFormat("{0}/SampleText{1:D2}", GetSaveDirectory(), currentTextIndex);
            builder.AppendFormat(" {0:D2}", pagingCount + 1);
            if (!string.IsNullOrEmpty(modifier.Description))
            {
                builder.Append(" " + modifier.Description);
            }
            builder.Append(".png");
            return builder.ToString();
        }

        /// <summary>
        /// スクリーンショットを保存するディレクトリのパスを取得します。
        /// </summary>
        /// <returns>ユーザーのピクチャディレクトリ内の特定ディレクトリのパスを返します。</returns>
        string GetSaveDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "/Jigbox_TextView_SS_" + Application.unityVersion;
        }

        /// <summary>
        /// 設定を次の状態に変更します。
        /// </summary>
        void ChangeNextSetting()
        {
            canRecieveOverflow = false;
            ++modifierIndex;
            if (modifierIndex < Modifiers.Count)
            {
                modifier = Modifiers[modifierIndex];
                exampleController.ModifyTextViewOptions(ModifyTextViewOptions);
                exampleController.ModifyTextViewOptions(textView => textView.VisibleLineStart = currentLineIndex);
                return;
            }

            if (nextLineIndex > 0)
            {
                modifierIndex = 0;
                modifier = Modifiers[modifierIndex];
                exampleController.ModifyTextViewOptions(ModifyTextViewOptions);
                canRecieveOverflow = true;
                currentLineIndex = nextLineIndex;
                exampleController.ModifyTextViewOptions(textView => textView.VisibleLineStart = currentLineIndex);
                nextLineIndex = 0;                
                ++pagingCount;
                return;
            }

            ++currentTextIndex;
            nextLineIndex = 0;
            currentLineIndex = 0;
            pagingCount = 0;
            modifierIndex = 0;
            modifier = Modifiers[modifierIndex];

            if (currentTextIndex >= textCount)
            {
                currentTextIndex = 0;
                isCapturing = false;
                captureButton.Clickable = true;
                Debug.Log("全ての画像の出力が完了しました。");
                return;
            }

            canRecieveOverflow = true;
            exampleController.SetText(currentTextIndex);
            exampleController.ModifyTextViewOptions(ModifyTextViewOptions);
            exampleController.ModifyTextViewOptions(textView => textView.VisibleLineStart = currentLineIndex);
        }

        /// <summary>
        /// TextViewに設定したテキストがオーバーフローした際に呼び出されます。
        /// </summary>
        /// <param name="overflow">オーバーフローしたテキストの情報</param>
        void OnOverflowTextLine(TextViewOverflow overflow)
        {
            // 設定変更中にもオーバーフローは発生するが、テキスト設定時以外でオーバーフローした際は、
            // デフォルト設定における状態と異なる可能性があるため、オーバーフローしても無視する
            if (!canRecieveOverflow)
            {
                return;
            }
            canRecieveOverflow = false;
            nextLineIndex = overflow.OverflowTextLineIndex;
        }

        /// <summary>
        /// TextViewの設定を初期化した後に、設定を変更します。
        /// </summary>
        /// <param name="textView">TextView</param>
        void ModifyTextViewOptions(Components.TextView textView)
        {
            textView.TreatNewLineAsLineBreak = false;
            textView.LineBreakRule = TextView.TextLineBreakRule.Japanese;

            textView.Alignment = TextView.TextAlign.Left;
            textView.VerticalAlignment = TextView.TextVerticalAlign.Top;

            textView.SpacingUnit = TextView.SpacingUnit.BeforeCharacterWidth;
            textView.CharacterSpacing = 0.0f;

            textView.LineHeight = 1.0f;
            textView.IsLineHeightFixed = false;
            textView.HorizontalOverflow = HorizontalWrapMode.Wrap;
            textView.VerticalOverflow = VerticalWrapMode.Truncate;
            textView.IsHalfPunctuationOfLineHead = false;

            modifier.Modifier(textView);
        }

        /// <summary>
        /// キャプチャを開始します。
        /// </summary>
        [AuthorizedAccess]
        void StartCapture()
        {
            isCapturing = true;
            captureButton.Clickable = false;

            exampleController.TextWindowOutline.enabled = false;
            exampleController.TextWindowImage.enabled = false;

            canRecieveOverflow = true;
            exampleController.SetText(currentTextIndex);
            exampleController.ModifyTextViewOptions(ModifyTextViewOptions);
            exampleController.ModifyTextViewOptions(textView => textView.VisibleLineStart = currentLineIndex);
        }

#endregion

#region override unity methods

        void OnPostRender()
        {
            if (isCapturing)
            {
                // 本当はReadPixelsの時点でトリミングしてデータを取得できるが、
                // グラフィックライブラリの状態などによって、テクスチャ座標が変化する影響によって、
                // ReadPixels時点では、座標の上下が入れ替わっている場合があるため、
                // 一度全画面をテクスチャ化してから必要な領域を抽出する
                captureTexture.ReadPixels(CaptureRect, 0, 0, false);
                encodeTexture.SetPixels(captureTexture.GetPixels(FetchPixelX, FetchPixelY, EncodeTextureWidth, EncodeTextureHeight));
                byte[] bytes = encodeTexture.EncodeToPNG();
                File.WriteAllBytes(GetSavePath(), bytes);

                ChangeNextSetting();
            }
        }

#endregion
    }
}
