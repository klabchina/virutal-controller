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

namespace Jigbox.TextView.ParserV2
{
    public sealed class TokenizeInfo
    {
#region constants

        /// <summary>エラー時に抽出する文字列の長さ</summary>
        static readonly int ErrorTextPickupLength = 10;

#endregion

#region properties

        /// <summary>テキスト</summary>
        public string Text { get; private set; }

        /// <summary>テキストの長さ</summary>
        public int Length { get; private set; }

        /// <summary>現在参照している文字のインデックス</summary>
        public int Index { get; set; }

        /// <summary>現在参照している文字</summary>
        public char Character { get { return Text[Index]; } }

        /// <summary>次に参照する文字</summary>
        public char NextCharacter 
        {
            get
            {
#if UNITY_EDITOR
                if (IsFinalCharacter)
                {
                    UnityEngine.Assertions.Assert.IsTrue(false, "Can't read next character, ");
                    return '\0';
                }
#endif
                return Text[Index + 1];
            }
        }

        /// <summary>現在参照している文字が最後の文字かどうか</summary>
        public bool IsFinalCharacter { get { return Index + 1 >= Length; } }

        /// <summary>トークナイズに失敗したかどうか</summary>
        public bool IsFailed { get; private set; }

        /// <summary>トークナイズが終了したかどうか</summary>
        public bool IsEnd { get { return IsFailed || (Length <= Index); } }

        /// <summary>エラー時のメッセージ</summary>
        public string Message { get; private set; }

#endregion

#region public methods

        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="text">トークナイズするテキスト</param>
        public void Init(string text)
        {
            Text = text;
            Length = text.Length;
            Index = 0;
            IsFailed = false;
            Message = null;
        }

        /// <summary>
        /// エラー情報を設定します。
        /// </summary>
        /// <param name="errorMessage">エラー内容</param>
        public void SetError(string errorMessage)
        {
            Message = "テキスト" + "(" + Index + "/" + Length + ") : " + PickupError(Index)
                + "\nエラー内容 : " + errorMessage;
            IsFailed = true;
        }

        /// <summary>
        /// エラー情報を設定します。
        /// </summary>
        /// <param name="errorMessage">エラー内容</param>
        /// <param name="index">エラーした文字のインデックス</param>
        public void SetError(string errorMessage, int index)
        {
            Message = "テキスト" + "(" + Index + "/" + Length + ") : " + PickupError(index)
                + "\nエラー内容 : " + errorMessage;
            IsFailed = true;
        }

        /// <summary>
        /// <para>エラー情報を設定します。</para>
        /// <para>エラーしたテキストの抽出は、指定されたインデックス以前の特定文字から行います。</para>
        /// </summary>
        /// <param name="errorMessage">エラー内容</param>
        /// <param name="index">エラーした文字のインデックス</param>
        /// <param name="pickupCharacter">抽出する文字</param>
        public void SetError(string errorMessage, int index, char pickupCharacter)
        {
            int errorIndex = index;
            while (Text[errorIndex] != pickupCharacter)
            {
                --errorIndex;
                // 変な指定した場合の予防線
                if (errorIndex < 0)
                {
                    errorIndex = index;
                    break;
                }
            }

            Message = "テキスト" + "(" + Index + "/" + Length + ") : " + PickupError(errorIndex)
                + "\nエラー内容 : " + errorMessage;
            IsFailed = true;
        }

#endregion

#region private methods

        /// <summary>
        /// 元のテキストからエラーした文字列を抽出します。
        /// </summary>
        /// <param name="index">エラーした文字のインデックス</param>
        /// <returns></returns>
        string PickupError(int index)
        {
            int errorPoint = index < Length ? index : index - 1;

            int beforeLength = Mathf.Min(errorPoint, ErrorTextPickupLength);
            int beforePoint = errorPoint - beforeLength;
            string beforeText = Text.Substring(beforePoint, beforeLength);
            if (errorPoint > ErrorTextPickupLength)
            {
                beforeText = "..." + beforeText;
            }

            int afterPoint = errorPoint + 1;
            int afterLength = Mathf.Min(Length - afterPoint, ErrorTextPickupLength);
            string afterText = Text.Substring(afterPoint, afterLength);
            if (Length - afterPoint > ErrorTextPickupLength)
            {
                afterText += "...";
            }

            return beforeText
                + "<color=red>" + Text.Substring(errorPoint, 1) + "</color>" 
                + afterText;
        }

#endregion
    }
}
