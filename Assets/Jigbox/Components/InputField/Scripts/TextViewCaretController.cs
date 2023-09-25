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
using Jigbox.TextView;
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// TextViewで表示できるキャレット操作のロジックを管理するクラス
    /// </summary>
    [RequireComponent(typeof(InputTextView))]
    [RequireComponent(typeof(InputExtensibilityProvider))]
    [DisallowMultipleComponent]
    public class TextViewCaretController : MonoBehaviour
    {
        [SerializeField]
        protected InputTextView textView = null;

        /// <summary>InputTextViewの参照</summary>
        public InputTextView TextView
        {
            get { return textView; }
            set { textView = value; }
        }

        /// <summary>双方向言語かのフラグ</summary>
        protected bool isBidirectionalText;

        /// <summary>キャレットの一覧</summary>
        protected CaretUnit[] allCaretUnits;

        /// <summary>選択範囲のRect</summary>
        protected readonly List<Rect> selectRects = new List<Rect>();

        /// <summary>行単位のグリフ情報</summary>
        protected readonly List<LineGlyphCaret> lineCaretGlyphs = new List<LineGlyphCaret>();

        /// <summary>TextLineから変換する時の一時管理変数</summary>
        protected readonly List<GlyphCaret> tempGlyphCarets = new List<GlyphCaret>();

        /// <summary>
        /// 情報の更新
        /// </summary>
        /// <param name="allTextLines">TextLine</param>
        /// <param name="isMirror">鏡文字の有効フラグ</param>
        /// <param name="offsetY">TextViewの縦方向アライメントのオフセット</param>
        public void UpdateCaretInfo(List<TextLine> allTextLines, bool isMirror, float offsetY)
        {
            this.isBidirectionalText = isMirror && allTextLines.Count > 0 && textView.LanguageType == TextLanguageType.Arabic;
            this.allCaretUnits = TextViewCaretUnitProxy.GetAllCaretUnits();

            ConvertAllTextLine(allTextLines, offsetY);
        }

        /// <summary>
        /// 文字列が空の時のVerticalAlignOffsetの計算
        /// </summary>
        /// <param name="emptyTextLine">空のTextLine</param>
        /// <returns></returns>
        protected virtual float CalculateEmptyTextOffsetY(TextLine emptyTextLine)
        {
            switch (textView.VerticalAlignment)
            {
                case TextVerticalAlign.Top:
                    return 0.0f;
                case TextVerticalAlign.Center:
                    return textView.rectTransform.rect.height * 0.5f - LineHeight(emptyTextLine) * 0.5f;
                case TextVerticalAlign.Bottom:
                    return textView.rectTransform.rect.height - LineHeight(emptyTextLine);
            }

            return 0.0f;
        }

        /// <summary>
        /// キャレットを考慮した配列データに変換する 
        /// </summary>
        /// <returns></returns>
        protected virtual void ConvertAllTextLine(List<TextLine> allTextLines, float offsetY)
        {
            lineCaretGlyphs.Clear();
            tempGlyphCarets.Clear();
            // 折返し行数のカウンター
            var wrapLineCount = 0;

            // TextViewの文字列が空の場合の対応
            if (allTextLines.Count == 0)
            {
                // 空のTextLineを作る
                var emptyTextLine = textView.EmptyTextLine();
                // 文頭だけ追加する
                tempGlyphCarets.Add(new GlyphCaret()
                {
                    GlyphPlacement = null,
                    LineX = emptyTextLine.LineX,
                    LineY = emptyTextLine.LineY - CalculateEmptyTextOffsetY(emptyTextLine),
                    TextLine = emptyTextLine,
                    WrapIndex = wrapLineCount,
                    WrapMarker = false,
                });

                lineCaretGlyphs.Add(new LineGlyphCaret()
                {
                    GlyphCaretData = tempGlyphCarets.ToArray(),
                    WrapLineCount = wrapLineCount
                });

                return;
            }

            // 自動折返しを解除しつつCaretUnitから参照できる形に変換する
            for (var i = 0; i <= allTextLines.Count; i++)
            {
                // 折返し前の1行になったGlyph情報を配列に詰め直す
                if (i == allTextLines.Count || !allTextLines[i].IsAutoLineBreak && tempGlyphCarets.Count > 0)
                {
                    lineCaretGlyphs.Add(new LineGlyphCaret()
                    {
                        GlyphCaretData = tempGlyphCarets.ToArray(),
                        WrapLineCount = wrapLineCount
                    });
                    tempGlyphCarets.Clear();
                    wrapLineCount = 0;
                }

                // 配列外なので終了
                if (i == allTextLines.Count)
                {
                    break;
                }

                var textLine = allTextLines[i];

                // 自動折返しではないものが通常の改行で作られたTextLineの為、行頭用の情報を作る
                if (!textLine.IsAutoLineBreak)
                {
                    tempGlyphCarets.Add(new GlyphCaret()
                    {
                        GlyphPlacement = null,
                        LineX = textLine.LineX,
                        LineY = textLine.LineY - offsetY,
                        TextLine = textLine,
                        WrapIndex = wrapLineCount,
                    });
                }
                else if (tempGlyphCarets.Count > 0)
                {
                    // 自動改行のマーカーをつけておく
                    tempGlyphCarets[tempGlyphCarets.Count - 1].WrapMarker = true;
                }

                // 文字
                foreach (var gp in textLine.PlacedGlyphs)
                {
                    tempGlyphCarets.Add(new GlyphCaret()
                    {
                        GlyphPlacement = gp,
                        LineX = textLine.LineX,
                        LineY = textLine.LineY - offsetY,
                        TextLine = textLine,
                        WrapIndex = wrapLineCount,
                    });
                }

                wrapLineCount++;
            }
        }

        /// <summary>
        /// InputFieldのCaretPositionから対象のCaretUnitを取得する
        /// </summary>
        /// <param name="caretPosition">CaretPosition</param>
        /// <param name="caretUnitIndex">CaretUnitを示すインデックス</param>
        /// <returns>CaretUnit</returns>
        protected virtual CaretUnit FindCaretUnit(int caretPosition, out int caretUnitIndex)
        {
            caretUnitIndex = 0;
            foreach (var caretUnit in allCaretUnits)
            {
                if (caretPosition <= caretUnit.SourceIndex)
                {
                    return caretUnit;
                }

                caretUnitIndex++;
            }

            // 見つからなかった場合は最後のキャレット
            caretUnitIndex = allCaretUnits.Length - 1;
            return allCaretUnits[caretUnitIndex];
        }

        /// <summary>
        /// キャレット情報を取得する
        /// </summary>
        /// <param name="caretPosition"></param>
        /// <returns></returns>
        public virtual CaretDrawInfo GetCaretInfo(int caretPosition)
        {
            int caretUnitIndex;
            var caretUnit = FindCaretUnit(caretPosition, out caretUnitIndex);
            var caretVector2 = GetCaretVector2(caretUnit, caretUnitIndex, false);
            var textLine = GetGlyphCaret(caretUnit).TextLine;
            var upperBaseHeight = UpperBaseLineHeight(textLine);
            var underBaseHeight = UnderBaseLineHeight(textLine, upperBaseHeight);
            var result = new CaretDrawInfo(new Vector2(caretVector2.x, caretVector2.y - underBaseHeight), upperBaseHeight + underBaseHeight);
            return result;
        }

        /// <summary>
        /// キャレットの表示座標を求める
        /// </summary>
        /// <param name="caretUnit">キャレットの情報</param>
        /// <param name="caretUnitIndex">caretUnit単位のindex</param>
        /// <param name="isPosCheck">座標チェックで使用するか?</param>
        /// <returns></returns>
        protected virtual Vector2 GetCaretVector2(CaretUnit caretUnit, int caretUnitIndex, bool isPosCheck)
        {
            var glyphCaret = GetGlyphCaret(caretUnit);
            // 文頭しかない場合
            if (allCaretUnits.Length == 1 && caretUnit.IsBeginningOfLine())
            {
                var posX = isBidirectionalText ? -glyphCaret.LineX : glyphCaret.LineX;
                return new Vector2(posX, glyphCaret.LineY);
            }

            // 基本の表示位置を入れる
            var x = glyphCaret.X + glyphCaret.Width;
            var y = glyphCaret.LineY;
            var isRTL = caretUnit.isRightToLeft;

            // 双方向のLTRは特殊挙動になる為、それ以外は次の文字を確認してキャレットの表示位置を変える
            // 通常の場合
            if (glyphCaret.WrapMarker && (!isBidirectionalText || caretUnit.isRightToLeft) && !isPosCheck)
            {
                var nextCaretUnit = allCaretUnits[caretUnitIndex + 1];
                // タイ語の場合destination.lengthが2以上になる可能性があるので強制で1にしている
                var nextGlyphCaret = lineCaretGlyphs[nextCaretUnit.lineIndex].GlyphCaretData[nextCaretUnit.destination.offset + nextCaretUnit.destination.index + 1];
                x = nextGlyphCaret.X;
                y = nextGlyphCaret.LineY;
            }

            // 現在rtlで次がltrの場合ltrの文字の左側をキャレット位置にする
            if ((caretUnit.isRightToLeft || isBidirectionalText && caretUnit.IsBeginningOfLine()) && !isPosCheck && caretUnitIndex < allCaretUnits.Length - 1)
            {
                var nextCaretUnit = allCaretUnits[caretUnitIndex + 1];
                // 次の文字がltr 改行は無視
                if (!nextCaretUnit.isRightToLeft && !nextCaretUnit.IsBeginningOfLine())
                {
                    var nextGlyphCaret = GetGlyphCaret(nextCaretUnit);
                    x = nextGlyphCaret.X + nextGlyphCaret.Width;
                    y = nextGlyphCaret.LineY;
                }
            }

            // 現在ltrで次がrtl or 改行 or 文末の場合ltrが始まった箇所の左側にする
            if (isBidirectionalText && !caretUnit.isRightToLeft && !caretUnit.IsBeginningOfLine() && !isPosCheck)
            {
                // 文末かどうかを判定する
                var isGoBackLTR = caretUnitIndex == allCaretUnits.Length - 1;
                if (!isGoBackLTR)
                {
                    // rtlか改行の場合
                    var nextCaretUnit = allCaretUnits[caretUnitIndex + 1];
                    if (nextCaretUnit.isRightToLeft || nextCaretUnit.IsBeginningOfLine())
                    {
                        isGoBackLTR = true;
                    }
                }

                if (isGoBackLTR)
                {
                    var previousCaretUnit = allCaretUnits[0];
                    for (var i = caretUnitIndex - 1; i >= 0; i--)
                    {
                        // rtl か 行頭 のCaretUnitを探し見つかったら1つ前がltrの始まり
                        if (allCaretUnits[i].isRightToLeft || allCaretUnits[i].IsBeginningOfLine())
                        {
                            previousCaretUnit = allCaretUnits[i + 1];
                            break;
                        }
                    }

                    var nextGlyphCaret = GetGlyphCaret(previousCaretUnit);
                    x = nextGlyphCaret.X + nextGlyphCaret.Width;
                    y = nextGlyphCaret.LineY;
                    isRTL = true;
                }
            }

            // RTLの鏡文字対応
            if (isBidirectionalText)
            {
                x *= -1;
                if (!isRTL)
                {
                    x += glyphCaret.Width;
                }
            }

            return new Vector2(x, y);
        }

        /// <summary>
        /// ベースラインより上を返す
        /// </summary>
        protected virtual float UpperBaseLineHeight(TextLine textLine)
        {
            return textLine.CalculateHeightUpperBaseLine(textView.FontSize, textView.IsLineHeightFixed, textView.VisibleLength);
        }

        /// <summary>
        /// ベースラインより下を返す
        /// </summary>
        protected virtual float UnderBaseLineHeight(TextLine textLine, float startLineUpperHeight)
        {
            return textLine.CalculateHeightUnderBaseLine(textView.Font, startLineUpperHeight, textView.IsLineHeightFixed, textView.VisibleLength);
        }

        /// <summary>
        /// 行の高さを返す
        /// TextLineに入っている文字などによって高さは変わる
        /// </summary>
        protected virtual float LineHeight(TextLine textLine)
        {
            var startLineUpperHeight = UpperBaseLineHeight(textLine);
            var endLineUnderHeight = UnderBaseLineHeight(textLine, startLineUpperHeight);
            return startLineUpperHeight + endLineUnderHeight;
        }

        /// <summary>
        /// CaretUnitからGlyphCaretに変換する
        /// </summary>
        /// <param name="caretUnit"></param>
        /// <returns></returns>
        protected virtual GlyphCaret GetGlyphCaret(CaretUnit caretUnit)
        {
            return lineCaretGlyphs[caretUnit.lineIndex].GlyphCaretData[caretUnit.DestinationIndex];
        }

        /// <summary>
        /// 選択中の矩形一覧を作成する
        /// </summary>
        /// <param name="start">開始index</param>
        /// <param name="end">終了index</param>
        /// <returns>矩形配列</returns>
        public virtual Rect[] SelectRect(int start, int end)
        {
            selectRects.Clear();

            int caretUnitIndex;
            FindCaretUnit(start, out caretUnitIndex);

            for (var i = caretUnitIndex + 1; i < allCaretUnits.Length; i++)
            {
                var caretUnit = allCaretUnits[i];
                if (caretUnit.SourceIndex > end)
                {
                    break;
                }

                float minX = float.MaxValue, minY = float.MaxValue, width = 0, height = 0;
                // 複数文字の場合があるので、複数の文字を1つのRectで表す為の計算
                for (var j = 1; j <= caretUnit.destination.length; j++)
                {
                    var glyphCaretIndex = caretUnit.destination.offset + caretUnit.destination.index + j;
                    var glyphCaret = lineCaretGlyphs[caretUnit.lineIndex].GlyphCaretData[glyphCaretIndex];
                    var upperBaseHeight = UpperBaseLineHeight(glyphCaret.TextLine);
                    var underBaseHeight = UnderBaseLineHeight(glyphCaret.TextLine, upperBaseHeight);
                    minX = Mathf.Min(glyphCaret.X, minX);
                    minY = Mathf.Min(glyphCaret.LineY - underBaseHeight, minY);
                    width += glyphCaret.Width;
                    height = Mathf.Max(upperBaseHeight + underBaseHeight, height);
                }

                if (isBidirectionalText)
                {
                    minX *= -1;
                    minX -= width;
                }

                selectRects.Add(
                    new Rect(minX, minY, width, height)
                );
            }

            return selectRects.ToArray();
        }

        /// <summary>
        /// 座標からキャレット番号を取得する
        /// </summary>
        /// <param name="pos">クリック座標</param>
        /// <returns>対象のCaretPosition</returns>
        public virtual int GetCaretPositionFromPosition(Vector2 pos)
        {
            // どの行か確認し、どのX座標か確認する
            var caretLine = int.MaxValue;
            var wrapLine = int.MaxValue;
            var checkCaretUnitIndex = 0;
            // キャレット配列を逆に遡るフラグ
            var isGoBack = false;
            // RTLからLTRに自動折返しのタイミングで切り替わった場合、RTLの行の高さを保持しておく
            var rtlWrapY = float.MaxValue;

            // 行を確定する
            for (var i = 0; i < allCaretUnits.Length; i++)
            {
                var caretUnit = allCaretUnits[i];
                var glyphCaret = GetGlyphCaret(caretUnit);
                var caretVec2 = GetCaretVector2(caretUnit, i, !glyphCaret.WrapMarker);
                var checkY = caretVec2.y;

                // 1文字目より上をクリックしている場合は文頭
                if (i == 0 && checkY + LineHeight(glyphCaret.TextLine) < pos.y)
                {
                    return caretUnit.SourceIndex;
                }

                // 最後のキャレットまで探しても見つからない場合は文末
                if (i == allCaretUnits.Length - 1)
                {
                    return caretUnit.SourceIndex;
                }

                // 行の確認は行頭か自動折返しで確認する
                if (!(caretUnit.IsBeginningOfLine() || glyphCaret.WrapMarker))
                {
                    continue;
                }

                // 対象の行を探す
                // 座標が上でいいのは、双方向言語かつltrの折返しではない場合
                // 自動折返しがある場合は下から探す
                // 自動折返し文字がRTLかつ折り返し後の文字もRTLの場合は通常の判定で問題ない
                var nextCaretUnit = allCaretUnits[i + 1];
                // LTRの文字列がきて逆の探索が必要になっているかどうかのフラグ
                var isBidirectionalLTRStart = !(glyphCaret.WrapMarker && isBidirectionalText && (!caretUnit.isRightToLeft || caretUnit.isRightToLeft && !nextCaretUnit.isRightToLeft));
                if (pos.y >= checkY && (caretUnit.IsBeginningOfLine() || isBidirectionalLTRStart))
                {
                    caretLine = caretUnit.lineIndex;
                    wrapLine = glyphCaret.WrapIndex + (glyphCaret.WrapMarker ? 1 : 0);
                    checkCaretUnitIndex = i;
                    break;
                }

                // 双方向言語の自動折返しかつ、次の文字がLTR文字の場合RTL下の行から座標判定を行う為、それ以外はスキップ
                if (isBidirectionalLTRStart)
                {
                    rtlWrapY = float.MaxValue;
                    continue;
                }

                // 前のキャレットのlineYを確認する
                var prevCaretUnit = allCaretUnits[i - 1];
                var prevY = GetCaretVector2(prevCaretUnit, i - 1, false).y;

                // 座標チェック
                // 下からの行の範囲に入っている
                if (prevY <= pos.y && pos.y <= checkY)
                {
                    caretLine = caretUnit.lineIndex;
                    wrapLine = glyphCaret.WrapIndex + 1;
                    checkCaretUnitIndex = i - 1;
                    isGoBack = true;
                    break;
                }

                // 自動折返しの文字がRTLで次の文字がLTRの場合の対応
                if (caretUnit.isRightToLeft && !nextCaretUnit.isRightToLeft)
                {
                    // RTL行のY座標を保持しておく
                    rtlWrapY = GetCaretVector2(caretUnit, i, true).y;
                    // 次のキャレット文字に置き換え、LTRのキャレット参照に移行する
                    caretUnit = allCaretUnits[i + 1];
                    glyphCaret = GetGlyphCaret(caretUnit);
                }

                // LTRの行と、次の行の高さを求めて行を決める
                // 最終的にはRTLの行に辿りつく為、最初にRTLからLTRになった時に保存しておいたY座標を使用する
                for (var j = i + 1; j < allCaretUnits.Length; j++)
                {
                    var searchCaretUnit = allCaretUnits[j];
                    var searchGlyphCaret = GetGlyphCaret(searchCaretUnit);

                    // 折返し、文末、RTLが途中できた場合に座標チェックを行う
                    if (searchGlyphCaret.WrapMarker || j == allCaretUnits.Length - 1 || searchCaretUnit.isRightToLeft)
                    {
                        float searchY;
                        // ltrが終了か行末の場合で終了
                        if (searchCaretUnit.isRightToLeft || j == allCaretUnits.Length - 1)
                        {
                            searchY = rtlWrapY;
                        }
                        else
                        {
                            searchY = GetCaretVector2(searchCaretUnit, j, false).y;
                        }

                        // 行の範囲に入っている
                        if (checkY <= pos.y && pos.y <= searchY)
                        {
                            caretLine = caretUnit.lineIndex;
                            wrapLine = glyphCaret.WrapIndex;
                            // 現在の確認中キャレットは別の行のキャレットを見ているので1つ前を確認する
                            checkCaretUnitIndex = j - 1;
                            isGoBack = true;
                        }

                        break;
                    }
                }

                // 折返し行が見つかったら終了
                if (wrapLine != int.MaxValue)
                {
                    break;
                }
            }

            // 双方向言語のltr文字列で自動折返しの場合遡りの検証を行う
            if (isGoBack)
            {
                for (var i = checkCaretUnitIndex; i >= 0; i--)
                {
                    var caretUnit = allCaretUnits[i];
                    var glyphCaret = GetGlyphCaret(caretUnit);

                    if (glyphCaret.WrapIndex != wrapLine)
                    {
                        continue;
                    }

                    // 折返しが変わったら終了
                    if (glyphCaret.WrapIndex != wrapLine)
                    {
                        // 折返しが終了
                        break;
                    }

                    var caretVec2 = GetCaretVector2(caretUnit, i, true);
                    var prevCaretUnit = allCaretUnits[i + 1];
                    var prevGlyphCaret = GetGlyphCaret(prevCaretUnit);

                    // 自動折返しの1つ後ろは右側を全てカバーする
                    if (prevGlyphCaret.WrapIndex != glyphCaret.WrapIndex)
                    {
                        if (caretVec2.x <= pos.x)
                        {
                            return caretUnit.SourceIndex;
                        }
                    }

                    // 自動折返しマーカーの場合左側を全てカバーする
                    if (glyphCaret.WrapMarker)
                    {
                        if (caretVec2.x >= pos.x)
                        {
                            return caretUnit.SourceIndex;
                        }
                    }

                    // どの文字の間か確認する
                    var nextCaretUnit = allCaretUnits[i - 1];
                    var nextVec2 = GetCaretVector2(nextCaretUnit, i - 1, false);
                    if (nextVec2.x <= pos.x && pos.x <= caretVec2.x)
                    {
                        return caretUnit.SourceIndex;
                    }
                }
            }

            // 対象の文字を決定する
            for (var i = checkCaretUnitIndex; i < allCaretUnits.Length; i++)
            {
                var caretUnit = allCaretUnits[i];
                var glyphCaret = GetGlyphCaret(caretUnit);

                // 改行されたら終了
                if (caretLine != caretUnit.lineIndex)
                {
                    return allCaretUnits[i - 1].SourceIndex;
                }

                // 改行した箇所が文末の可能性がある為、文末判定は改行判定の後行う
                if (i == allCaretUnits.Length - 1)
                {
                    return caretUnit.SourceIndex;
                }

                // 同じ行と最初のキャレットが折返しキャレットの可能性がある為最初のキャレットだけ通す
                if (wrapLine != glyphCaret.WrapIndex && i != checkCaretUnitIndex)
                {
                    continue;
                }

                // 双方向言語のLTRかつ自動折返しではない場合、同じ行の折返しが来たら終了
                // 双方向言語のLTRかつ自動折返しの場合は、上のキャレット配列の遡りで対応している
                if (!(isBidirectionalText && !caretUnit.isRightToLeft) && wrapLine == glyphCaret.WrapIndex && glyphCaret.WrapMarker)
                {
                    return allCaretUnits[i - 1].SourceIndex;
                }

                if (!isBidirectionalText)
                {
                    var caretVec2 = GetCaretVector2(caretUnit, i, false);
                    if (caretVec2.x >= pos.x)
                    {
                        return caretUnit.SourceIndex;
                    }
                }
                else
                {
                    // 双方向言語の場合
                    var caretVec2 = GetCaretVector2(caretUnit, i, !glyphCaret.WrapMarker);
                    // 行頭であれば、右側を全てカバーする
                    if (caretUnit.IsBeginningOfLine())
                    {
                        if (caretVec2.x <= pos.x)
                        {
                            return caretUnit.SourceIndex;
                        }

                        continue;
                    }

                    // RTL文字の場合は、与えられた座標よりキャレットの位置が左にあれば対象の文字である
                    if (caretUnit.isRightToLeft && caretVec2.x <= pos.x)
                    {
                        return caretUnit.SourceIndex;
                    }

                    // 現在はRTLで次のキャレットがLTRの場合の時だけの対応
                    var nextCaretUnit = allCaretUnits[i + 1];
                    if (caretUnit.isRightToLeft && !nextCaretUnit.isRightToLeft)
                    {
                        var position = GetCaretVector2(caretUnit, i, false);
                        // RTLからLTRに変わる場合はキャレットの位置はLTRの左側になる為
                        // 1pxだけの座標判定を追加で行う
                        if ((int) pos.x == (int) position.x)
                        {
                            return caretUnit.SourceIndex;
                        }
                    }

                    // 双方向言語のLTR文字の場合
                    if (!caretUnit.isRightToLeft)
                    {
                        // 座標を取得する
                        caretVec2 = GetCaretVector2(caretUnit, i, true);

                        // 自動折返しの場合は左側を全てカバーする
                        if (glyphCaret.WrapMarker && pos.x <= caretVec2.x)
                        {
                            return caretUnit.SourceIndex;
                        }

                        // 1つ前の文字のキャレット位置との間に与えられた座標が入っているか確認する
                        var prevCaretUnit = allCaretUnits[i - 1];
                        var prevVec2 = GetCaretVector2(prevCaretUnit, i - 1, false);
                        if (prevVec2.x <= pos.x && pos.x <= caretVec2.x)
                        {
                            return caretUnit.SourceIndex;
                        }
                    }
                }
            }

            // 文末
            return allCaretUnits[allCaretUnits.Length - 1].SourceIndex;
        }

        /// <summary>
        /// キャレットを1行上に移動する操作
        /// </summary>
        /// <param name="caretPosition">InputFieldで管理しているキャレット番号</param>
        /// <returns></returns>
        public virtual int MoveUp(int caretPosition)
        {
            int caretUnitIndex;
            var caretUnit = FindCaretUnit(caretPosition, out caretUnitIndex);
            var glyphCaret = GetGlyphCaret(caretUnit);
            var caretX = GetCaretVector2(caretUnit, caretUnitIndex, false).x;

            // 後のキャレットが双方向LTRの場合、そちらのキャレットを行数と自動折返し行数の基準とする
            if (isBidirectionalText && (caretUnit.IsBeginningOfLine() || caretUnit.isRightToLeft))
            {
                if (caretUnitIndex < allCaretUnits.Length - 1)
                {
                    var nextCaretUnit = allCaretUnits[caretUnitIndex + 1];
                    if (!nextCaretUnit.isRightToLeft && !nextCaretUnit.IsBeginningOfLine())
                    {
                        caretUnit = allCaretUnits[caretUnitIndex + 1];
                        glyphCaret = GetGlyphCaret(caretUnit);
                    }
                }
            }
            else if (isBidirectionalText && !caretUnit.isRightToLeft)
            {
                // 双方向言語でLTRの場合、文末か次の文字がRTLの場合キャレットの配列を遡ってLTR始まりのキャレットを探す
                if (caretUnitIndex == allCaretUnits.Length - 1 || allCaretUnits[caretUnitIndex + 1].isRightToLeft)
                {
                    for (var i = caretUnitIndex; i >= 0; i--)
                    {
                        if (allCaretUnits[i].IsBeginningOfLine() || allCaretUnits[i].isRightToLeft)
                        {
                            caretUnit = allCaretUnits[i + 1];
                            glyphCaret = GetGlyphCaret(caretUnit);
                            break;
                        }
                    }
                }
            }

            // 1行目かつ自動折返しも1行目の場合は先頭
            if (caretUnit.lineIndex == 0 && glyphCaret.WrapIndex == 0)
            {
                return allCaretUnits[0].SourceIndex;
            }

            // 折返し行数
            int searchWrapIndex;
            // 改行の行数
            int targetLineIndex;

            // 自動折返しの場合の処理
            if (glyphCaret.WrapIndex > 0)
            {
                // 自動折返しマーカーの場合は同じ行の文字が対象なので折返し行のインデックスは変えない
                searchWrapIndex = glyphCaret.WrapIndex - (!(isBidirectionalText && !caretUnit.isRightToLeft) && glyphCaret.WrapMarker ? 0 : 1);
                targetLineIndex = caretUnit.lineIndex;
            }
            else
            {
                // 自動折返しマーカーの場合は同じ行の文字が対象なので折返し行のインデックスは変えない
                searchWrapIndex = lineCaretGlyphs[caretUnit.lineIndex - 1].WrapLineCount - 1;
                targetLineIndex = caretUnit.lineIndex - 1;
            }

            for (var i = 0; i < allCaretUnits.Length; i++)
            {
                var targetUnit = allCaretUnits[i];
                // 同じ行ではない場合は違う
                if (targetUnit.lineIndex != targetLineIndex)
                {
                    continue;
                }

                // 折返し行が違う場合は違う
                var targetGlyphCaret = GetGlyphCaret(targetUnit);
                if (targetGlyphCaret.WrapIndex != searchWrapIndex)
                {
                    continue;
                }

                // 双方向のLTRの場合 trueにする？
                return GetCaretPositionFromPosition(new Vector2(caretX, GetCaretVector2(targetUnit, i, isBidirectionalText && !targetUnit.isRightToLeft).y + 1));
            }

            // 行頭を返しておく
            return allCaretUnits[0].SourceIndex;
        }

        /// <summary>
        /// MoveDown操作
        /// </summary>
        /// <param name="caretPosition">キャレットポジション</param>
        /// <returns></returns>
        public virtual int MoveDown(int caretPosition)
        {
            int caretUnitIndex;
            var caretUnit = FindCaretUnit(caretPosition, out caretUnitIndex);

            // 文末の為終了
            if (caretUnitIndex == allCaretUnits.Length - 1)
            {
                return caretUnit.SourceIndex;
            }

            var lineCaret = lineCaretGlyphs[caretUnit.lineIndex];
            var glyphCaret = GetGlyphCaret(caretUnit);
            var caretX = GetCaretVector2(caretUnit, caretUnitIndex, false).x;

            // 自動折返しのキャレットをMoveDownする場合、自動折返しのキャレット表示位置は1つ下の行で表示されている為2行下の行に移動する必要がある
            // 双方向言語のLTRの自動折返しの場合は自動折返し文字と同じ行にキャレットが表示されている
            var diffLine = !(isBidirectionalText && !caretUnit.isRightToLeft) && glyphCaret.WrapMarker ? 2 : 1;
            if (glyphCaret.WrapIndex < lineCaret.WrapLineCount - diffLine)
            {
                var searchWrapIndex = glyphCaret.WrapIndex + diffLine;
                var searchLineIndex = caretUnit.lineIndex;
                // 探すのはCaretUnitから
                for (var i = 0; i < allCaretUnits.Length; i++)
                {
                    var targetCaretUnit = allCaretUnits[i];
                    // 改行indexが一致しない場合は何もしない
                    if (targetCaretUnit.lineIndex != searchLineIndex)
                    {
                        continue;
                    }

                    var targetGlyphCaret = GetGlyphCaret(targetCaretUnit);

                    // 折返し行が一致しない場合は特になにもしない
                    if (targetGlyphCaret.WrapIndex != searchWrapIndex)
                    {
                        continue;
                    }

                    // 高さを取得する
                    var targetVec2 = GetCaretVector2(targetCaretUnit, i, false);
                    // 双方向言語のltrの時に、行判定が下になってしまう為+1して行を正しく認識させている
                    return GetCaretPositionFromPosition(new Vector2(caretX, targetVec2.y + 1));
                }
            }
            else
            {
                // 次の改行箇所を探す
                // 同じ行内にLTRとRTLが混在している可能性がある為同じ行の改行がくるまで処理を行っている
                var searchWrapIndex = 0;
                var searchLineIndex = caretUnit.lineIndex + 1;
                // 探すのはキャレットから
                for (var i = caretUnitIndex + 1; i < allCaretUnits.Length; i++)
                {
                    var targetCaretUnit = allCaretUnits[i];
                    if (targetCaretUnit.lineIndex != searchLineIndex)
                    {
                        continue;
                    }

                    var targetGlyphCaret = GetGlyphCaret(targetCaretUnit);
                    if (targetGlyphCaret.WrapIndex != searchWrapIndex)
                    {
                        continue;
                    }

                    // 改行の場合
                    // 改行先が双方向言語のLTRで複数行の場合、行頭を見ると想定と違う位置のキャレット位置が返ってくる為、座標計算用のフラグをtrueで検索する
                    return GetCaretPositionFromPosition(new Vector2(caretX, GetCaretVector2(targetCaretUnit, i, true).y));
                }
            }

            // 文末
            return allCaretUnits[allCaretUnits.Length - 1].SourceIndex;
        }

        /// <summary>
        /// 行頭への移動
        /// </summary>
        /// <param name="caretPosition"></param>
        /// <returns>新しいCaretPosition</returns>
        public virtual int MoveToStartOfLine(int caretPosition)
        {
            int caretUnitIndex;
            var caretUnit = FindCaretUnit(caretPosition, out caretUnitIndex);
            var glyphCaret = GetGlyphCaret(caretUnit);

            // 自動折返しマーカーにいるので移動なし
            if (glyphCaret.WrapMarker)
            {
                return caretUnit.SourceIndex;
            }

            // 文頭を入れておく
            var result = allCaretUnits[0];

            // キャレットを後ろから順番に確認していき行が変わった箇所を調べる
            for (var i = caretUnitIndex - 1; i >= 0; i--)
            {
                result = allCaretUnits[i];

                // 改行により行が変わっているか確認
                if (caretUnit.lineIndex != result.lineIndex)
                {
                    result = allCaretUnits[i + 1];
                    break;
                }

                var resultGlyph = GetGlyphCaret(result);
                // 自動折返しマーカーがあるか
                if (resultGlyph.WrapMarker)
                {
                    break;
                }
            }

            return result.SourceIndex;
        }

        /// <summary>
        /// 行末へ移動する
        /// </summary>
        /// <param name="caretPosition"></param>
        /// <returns></returns>
        public virtual int MoveToEndOfLine(int caretPosition)
        {
            int caretUnitIndex;
            var caretUnit = FindCaretUnit(caretPosition, out caretUnitIndex);

            var result = allCaretUnits[allCaretUnits.Length - 1]; //文末を入れておく
            for (var i = caretUnitIndex + 1; i < allCaretUnits.Length; i++)
            {
                result = allCaretUnits[i];

                // 改行
                if (caretUnit.lineIndex != result.lineIndex)
                {
                    // 行が変わったので1つ前に戻る
                    result = allCaretUnits[i - 1];
                    break;
                }

                // 自動折返し
                var resultGlyph = GetGlyphCaret(result);
                // 自動折返しマーカーを見つけたので終了
                if (resultGlyph.WrapMarker)
                {
                    result = allCaretUnits[i - 1];
                    break;
                }
            }

            return result.SourceIndex;
        }

        /// <summary>
        /// 削除する文字サイズを返す
        /// </summary>
        /// <param name="caretPosition">開始位置</param>
        public virtual int DeleteLength(int caretPosition)
        {
            int caretUnitIndex;
            FindCaretUnit(caretPosition, out caretUnitIndex);
            var caretUnit = allCaretUnits[caretUnitIndex + 1];
            return caretUnit.source.length;
        }

        /// <summary>
        /// キャレット位置を1つ戻す
        /// </summary>
        /// <param name="caretPosition">InputFieldが管理しているキャレット位置</param>
        public virtual int MovePrevious(int caretPosition)
        {
            int caretUnitIndex;
            FindCaretUnit(caretPosition, out caretUnitIndex);
            caretUnitIndex--;
            caretUnitIndex = Mathf.Max(caretUnitIndex, 0);
            return allCaretUnits[caretUnitIndex].SourceIndex;
        }

        /// <summary>
        /// キャレット番号を1つ進める
        /// </summary>
        /// <param name="caretPosition">InputFieldが管理しているキャレット位置</param>
        public virtual int MoveNext(int caretPosition)
        {
            int caretUnitIndex;
            FindCaretUnit(caretPosition, out caretUnitIndex);
            caretUnitIndex++;
            caretUnitIndex = Mathf.Min(caretUnitIndex, allCaretUnits.Length - 1);

            return allCaretUnits[caretUnitIndex].SourceIndex;
        }
    }
}
