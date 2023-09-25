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

namespace Jigbox.TextView.ParserV2
{
    public abstract class ThreeProcessTokenizer : TextTokenizer
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 処理の状態
        /// </summary>
        protected enum State
        {
            /// <summary>トークン化する前の処理</summary>
            PreProcess,
            /// <summary>トークン化する処理</summary>
            TokenizeProcess,
            /// <summary>トークン化した後の処理</summary>
            PostProcess
        }

#endregion

#region properties

        /// <summary>処理の状態</summary>
        protected State state;

        /// <summary>自身のトークナイズ状態</summary>
        protected abstract TokenizeMode OwnMode { get; }

#endregion

#region protected methods

        /// <summary>
        /// 初期化します。
        /// </summary>
        protected override void Init()
        {
            fetchLength = 0;
            state = State.PreProcess;
        }

        /// <summary>
        /// 文字列からトークン化する文字列までを読み出します。
        /// </summary>
        /// <returns>読み出しが完了して、トークン化したら<c>true</c>を返します。</returns>
        protected override bool Read()
        {
            switch (state)
            {
                case State.PreProcess:
                    SeekTokenizePoint();
                    break;
                case State.TokenizeProcess:
                    ReadAndCreateToken();
                    break;
                case State.PostProcess:
                    SeekNextTokenizePoint();
                    break;

            }

            if (!tokenizeInfo.IsFailed)
            {
                ++tokenizeInfo.Index;
            }

            return Mode != OwnMode;
        }

        /// <summary>
        /// トークン化する文字列を読み出し始める位置まで読み進めます。
        /// </summary>
        protected abstract void SeekTokenizePoint();

        /// <summary>
        /// 文字列を読み出して、トークン化します。
        /// </summary>
        protected abstract void ReadAndCreateToken();

        /// <summary>
        /// 次にトークンを読み出し始める位置まで読み進めます。
        /// </summary>
        protected abstract void SeekNextTokenizePoint();

#endregion
    }
}
