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
using System.Text;
using Jigbox.TextView;
using Jigbox.UIControl;
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// TextView_InputField用TextViewコンポーネント
    /// キャレットの情報やカリングの対応を追加で行っている
    /// </summary>
    public class InputTextView : TextView
    {
        /// <summary>空TextLine用</summary>
        protected readonly List<TextLine> emptyTextLines = new List<TextLine>();
        
        /// <summary>文字参照のエスケープ文字列を作る為のBuilder</summary>
        protected readonly StringBuilder escapeTextBuilder = new StringBuilder();

        /// <summary>キャレット操作</summary>
        protected TextViewCaretController textViewCaretController = null;

        /// <summary>
        /// InputField用RectMask2Dカリング対策処理
        /// TextViewの表示がOverflowでRectTransformのサイズより大きい場合、表示に合わせてカリングのサイズ判定を行うようにしている
        /// </summary>
        /// <param name="clipRect"></param>
        /// <param name="validRect"></param>
        public override void Cull(Rect clipRect, bool validRect)
        {
            if (layoutElement != null)
            {
                base.Cull(clipRect, validRect);
                return;
            }

            // LayoutGroupが親にいる場合
            // CanvasRendererが更新されたように見せかける処理を行い、2F目でクリッピング計算が走るようにします。
            if (dimensionChangedAfterFirstUpdate != null && dimensionChangedAfterFirstUpdate.Value)
            {
                canvasRenderer.transform.position += Vector3.zero;
            }

            // TextViewのキャンパス上のサイズを出す
            var canvasSpaceRect = RectTransformUtils.GetCanvasSpaceRect(rectTransform, canvas);

            // TextViewの範囲外に文字が出る設定の場合は文字サイズ計算を行って大きい方を採用する
            if (HorizontalOverflow == HorizontalWrapMode.Overflow)
            {
                canvasSpaceRect.width = Mathf.Max(GetPreferredWidth(PreferredWidthType.Visibled), canvasSpaceRect.width);
            }

            if (VerticalOverflow == VerticalWrapMode.Overflow)
            {
                var height = GetPreferredHeight(PreferredHeightType.Visibled);
                if (canvasSpaceRect.height < height)
                {
                    canvasSpaceRect.y -= height - canvasSpaceRect.height;
                    canvasSpaceRect.height = height;
                }
            }

            // カリングするかを決定する
            var isCull = !validRect || !clipRect.Overlaps(canvasSpaceRect, true);
            if (canvasRenderer.cull != isCull)
            {
                canvasRenderer.cull = isCull;
                onCullStateChanged.Invoke(isCull);
                OnCullingChanged();
            }
        }

        protected override void Awake()
        {
            // 自動折返しの時のwhitespaceを維持するフラグ
            isKeepTailSpace = true;
            base.Awake();
        }

        /// <summary>
        /// キャレットデータ操作クラスを取得する
        /// </summary>
        /// <returns></returns>
        public virtual TextViewCaretController GetTextViewCaretController()
        {
            if (textViewCaretController == null)
            {
                textViewCaretController = GetComponent<TextViewCaretController>();
            }

            ForceUpdate();

            textViewCaretController.UpdateCaretInfo(allTextLines, enableMirror, offsetY);
            return textViewCaretController;
        }

        /// <summary>
        /// 文字列が空の時のキャレット座標を作成する
        /// </summary>
        public virtual TextLine EmptyTextLine()
        {
            if (emptyTextLines.Count == 0)
            {
                emptyTextLines.Clear();
                emptyTextLines.Add(new TextLine()
                {
                    PlacedGlyphs = new GlyphPlacement[] { },
                    IsLastLine = true,
                });
            }

            // LineXの設定
            var textViewRect = rectTransform.rect;
            HorizontalAlignmentCalculator.Apply(emptyTextLines, Alignment, Justify, textViewRect.width);
            var baseX = textViewRect.xMin;
            foreach (var textLine in emptyTextLines)
            {
                textLine.LineX += baseX;
            }

            // LineYの設定
            var emptyTextLine = emptyTextLines[0];
            // ベースラインより上の幅を引く
            emptyTextLine.LineY = textViewRect.yMax - emptyTextLine.CalculateHeightUpperBaseLine(FontSize, IsLineHeightFixed);

            return emptyTextLine;
        }

        
        /// <summary>
        /// TextViewでパースエラーとタグを使わない文字列でテキストを設定する
        /// TextViewに設定する文字列の
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual void SetTextWithEscape(string text)
        {
            escapeTextBuilder.Length = 0;
            escapeTextBuilder.Append(text);
            escapeTextBuilder.Replace("&", "&amp;");
            escapeTextBuilder.Replace("'", "&apos;");
            escapeTextBuilder.Replace("\"", "&quot;");
            escapeTextBuilder.Replace("<", "&lt;");
            escapeTextBuilder.Replace(">", "&gt;");
            // stringBuilder.Replace("", "&shy;"); TextViewで用意されているが使用されていない為コードはコメントアウト
            Text = escapeTextBuilder.ToString();
        }
    }
}
