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

#if UNITY_EDITOR || NOVELKIT_EDITOR
using System;
using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    public class AdvAnnotationProcessor : IAdvAnnotationProcessor
    {
#region constants

        /// <summary>デフォルトで定義されているアノテーション</summary>
        protected static readonly List<string> DefaultAnnotations = new List<string> { "TODO", "FIXME", "OTHER" };

        /// <summary>その他に分類されるアノテーション</summary>
        protected static readonly string AnnotationOther = "OTHER";

        /// <summary>コメント判別用の文字の長さ</summary>
        protected static readonly int CommentPrefixLength = 2;

#endregion

#region properties

        /// <summary>アノテーションの</summary>
        protected static List<string> annotations = DefaultAnnotations;

        /// <summary>利用するアノテーションを返します。</summary>
        public virtual List<string> Annotations { get { return null; } }

#endregion

#region public methods

        /// <summary>
        /// アノテーションの一覧を更新します。
        /// </summary>
        /// <param name="annotation">デフォルト定義以外で追加するアノテーション</param>
        public static void UpdateAnotation(List<string> annotation)
        {
            annotations = DefaultAnnotations;
            if (annotation != null && annotation.Count > 0)
            {
                annotations.AddRange(annotation);
            }
        }

        /// <summary>
        /// アノテーションかどうかを判別して返します。
        /// </summary>
        /// <param name="command">コマンド</param>
        /// <returns></returns>
        public static bool IsAnnotation(AdvCommandBase command)
        {
            string annotation = null;
            string[] commandParam = command.Param;
            // 「//TODO message」、「// TODO message」のフォーマットで書かれたものを処理する
            if (commandParam[0].Length > CommentPrefixLength)
            {
                annotation = GetAnnotation(commandParam[0].Substring(CommentPrefixLength));
            }
            if (annotation == null && commandParam.Length > 1)
            {
                annotation = GetAnnotation(commandParam[1]);
            }

            return !string.IsNullOrEmpty(annotation);
        }

        /// <summary>
        /// アノテーションのタグを抽出して返します。
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns></returns>
        public static string GetAnnotation(string str)
        {
            foreach (string annotation in annotations)
            {
                if (str.CompareTo(annotation) == 0)
                {
                    return annotation;
                }
                if (str.Contains(annotation))
                {
                    return AnnotationOther;
                }
            }
            return null;
        }

        /// <summary>
        /// アノテーションとして出力する文字列を取得します。
        /// </summary>
        /// <param name="lineIndex">記述されている行数</param>
        /// <param name="command">対象となるコマンド</param>
        /// <param name="targetPath">対象ファイルのパス</param>
        /// <returns></returns>
        public virtual string GetMessage(int lineIndex, AdvCommandBase command, string targetPath)
        {
            string annotation = null;
            string[] commandParam = command.BaseParam;
            string[] messageParam = null;
            if (commandParam[0].Length > CommentPrefixLength)
            {
                annotation = GetAnnotation(commandParam[0].Substring(CommentPrefixLength));
            }
            // 先頭がコメント用識別子のみの場合
            if (annotation == null)
            {
                annotation = GetAnnotation(commandParam[1]);

                int messageLength = commandParam.Length - 2;
                messageParam = new string[messageLength];
                // 先頭はコメント用の識別子、次がアノテーションの識別子なので2つ除外
                Array.Copy(command.BaseParam, 2, messageParam, 0, messageLength);
            }
            // 先頭にアノテーションの識別子も含まれる場合
            else
            {
                int messageLength = commandParam.Length - 1;
                messageParam = new string[messageLength];
                // 先頭のみ除外
                Array.Copy(command.BaseParam, 1, messageParam, 0, messageLength);
            }

            // 先頭2行分までしかコンソールのリストには出ないため、2行目以降に閉じタグがあると
            // 見た目上白いログが出てしまうので、途中で閉じタグを挟んでいる
            return string.Format(
                "<color=yellow>{0} : {1}\n対象ファイル : {2}</color>\n<color=yellow>行番号 : {3}</color>",
                annotation,
                AdvScriptParams.Join(messageParam),
                targetPath,
                lineIndex);
        }

#endregion
    }
}
#endif
