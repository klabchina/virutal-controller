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

namespace Jigbox.NovelKit
{
    public static class AdvLog
    {
#region properties

        /// <summary>インスタンス</summary>
        static AdvLogger instance;

        /// <summary>インスタンス</summary>
        public static AdvLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AdvLogger();
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }

#endregion

#region public methods

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="message">出力する文字列、またはオブジェクトの参照</param>
        public static void Print(object message)
        {
            Instance.Print(message);
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="message">出力する文字列、またはオブジェクトの参照</param>
        /// <param name="content">messageが適用されるオブジェクトの参照</param>
        public static void Print(object message, Object content)
        {
            Instance.Print(message, content);
        }

        /// <summary>
        /// 警告を出力します。
        /// </summary>
        /// <param name="message">出力する文字列、またはオブジェクトの参照</param>
        public static void Warning(object message)
        {
            Instance.Warning(message);
        }

        /// <summary>
        /// 警告を出力します。
        /// </summary>
        /// <param name="message">出力する文字列、またはオブジェクトの参照</param>
        /// <param name="content">messageが適用されるオブジェクトの参照</param>
        public static void Warning(object message, Object content)
        {
            Instance.Warning(message, content);
        }

        /// <summary>
        /// エラーを出力します。
        /// </summary>
        /// <param name="message">出力する文字列、またはオブジェクトの参照</param>
        public static void Error(object message)
        {
            Instance.Error(message);
        }

        /// <summary>
        /// エラーを出力します。
        /// </summary>
        /// <param name="message">出力する文字列、またはオブジェクトの参照</param>
        /// <param name="content">messageが適用されるオブジェクトの参照</param>
        public static void Error(object message, Object content)
        {
            Instance.Error(message, content);
        }

#endregion
    }
}
