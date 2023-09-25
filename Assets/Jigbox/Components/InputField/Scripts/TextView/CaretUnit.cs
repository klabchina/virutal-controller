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

namespace Jigbox.Components
{
    /// <summary>
    /// キャレット参照単位でのテキスト情報
    /// </summary>
    public sealed class CaretUnit
    {
        /// <summary>
        /// テキスト情報
        /// </summary>
        public sealed class TextInfo
        {
            /// <summary>インデックス</summary>
            public readonly int index;

            /// <summary>文字列の長さ</summary>
            public readonly int length;

            /// <summary>文頭からのオフセット</summary>
            public readonly int offset;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="index">インデックス</param>
            /// <param name="length">文字列の長さ</param>
            /// <param name="offset">文頭からのオフセット</param>
            public TextInfo(int index, int length, int offset)
            {
                this.index = index;
                this.length = length;
                this.offset = offset;
            }
        }

        /// <summary>入力文字列の情報</summary>
        public readonly TextInfo source;

        /// <summary>変換後の文字列の情報</summary>
        public readonly TextInfo destination;

        /// <summary>右から左にレイアウトするかどうか</summary>
        public readonly bool isRightToLeft = false;

        /// <summary>行数</summary>
        public readonly int lineIndex;

        /// <summary>入力文字列上のインデックス</summary>
        public int SourceIndex
        {
            get { return source.offset + source.index + source.length; }
        }

        /// <summary>TextLine上の参照index</summary>
        public int DestinationIndex
        {
            get { return destination.offset + destination.index + destination.length; }
        }

        /// <summary>
        /// <summary>行頭のキャレット参照単位の情報</summary>
        /// </summary>
        /// <param name="srcIndexOffset">文頭からのオフセット</param>
        /// <param name="dstIndexOffset">行頭からのオフセット</param>
        /// <param name="line">行数</param>
        /// <returns>行頭のCaretUnit</returns>
        public static CaretUnit FirstUnit(int srcIndexOffset, int dstIndexOffset, int line)
        {
            return new CaretUnit(-1, 1, srcIndexOffset, -1, 1, dstIndexOffset, false, line);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="srcIndex">入力文字列のインデックス</param>
        /// <param name="srcLength">入力文字列内での文字列の長さ</param>
        /// <param name="srcOffsetIndex">文頭からのオフセット</param>
        /// <param name="dstIndex">変換後の文字列のインデックス</param>
        /// <param name="dstLength">変換後の文字列内での文字列の長さ</param>
        /// <param name="dstIndexOffset">行頭からのオフセット</param>
        /// <param name="isRightToLeft">右から左にレイアウトするかどうか</param>
        /// <param name="lineIndex">行数</param>
        public CaretUnit(int srcIndex, int srcLength, int srcOffsetIndex, int dstIndex, int dstLength, int dstIndexOffset, bool isRightToLeft, int lineIndex)
        {
            source = new TextInfo(srcIndex, srcLength, srcOffsetIndex);
            destination = new TextInfo(dstIndex, dstLength, dstIndexOffset);
            this.isRightToLeft = isRightToLeft;
            this.lineIndex = lineIndex;
        }

        /// <summary>
        /// タイ語のCaretUnitからTextView.CaretUnitにするコンストラクタ
        /// </summary>
        public CaretUnit(ThaiUtils.CaretUnit caretUnit, int srcIndexOffset, int dstIndexOffset, int lineIndex)
        {
            source = new TextInfo(caretUnit.source.index, caretUnit.source.length, srcIndexOffset);
            destination = new TextInfo(caretUnit.destination.index, caretUnit.destination.length, dstIndexOffset);
            isRightToLeft = caretUnit.isRightToLeft;
            this.lineIndex = lineIndex;
        }

        /// <summary>
        /// アラビア語のCaretUnitからTextView.CaretUnitにするコンストラクタ
        /// </summary>
        public CaretUnit(ArabicUtils.CaretUnit caretUnit, int srcIndexOffset, int dstIndexOffset, int lineIndex)
        {
            source = new TextInfo(caretUnit.source.index, caretUnit.source.length, srcIndexOffset);
            destination = new TextInfo(caretUnit.destination.index, caretUnit.destination.length, dstIndexOffset);
            isRightToLeft = caretUnit.isRightToLeft;
            this.lineIndex = lineIndex;
        }

        /// <summary>
        /// 行頭かどうか
        /// </summary>
        public bool IsBeginningOfLine()
        {
            return destination.index == -1 && destination.length == 1 && destination.offset == 0;
        }

        /// <summary>
        /// 改行かどうか
        /// </summary>
        /// <returns></returns>
        public bool IsBreak()
        {
            return destination.index == -1 && destination.length == 1 && destination.offset == 0 && source.offset > 0;
        }

        /// <summary>ログ用文字列</summary>
        public override string ToString()
        {
            return string.Format("[CaretUnit] src(index={0},lenght={1},offset={2}), dest(index={3},length={4},offset={5}), isRTL={6}, line={7}, total={8}, dstTotal={9}, isBeginOfLine={10}",
                source.index, source.length, source.offset, destination.index, destination.length, destination.offset, isRightToLeft, lineIndex, SourceIndex, DestinationIndex, IsBeginningOfLine());
        }
    }
}
