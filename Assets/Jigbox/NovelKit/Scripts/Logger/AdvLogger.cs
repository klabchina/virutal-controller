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
    public class AdvLogger
    {
#region public methods

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="message">出力する文字列、またはオブジェクトの参照</param>
        public virtual void Print(object message)
        {
            Debug.Log(message);
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="message">出力する文字列、またはオブジェクトの参照</param>
        /// <param name="content">messageが適用されるオブジェクトの参照</param>
        public virtual void Print(object message, Object content)
        {
            Debug.Log(message, content);
        }

        /// <summary>
        /// 警告を出力します。
        /// </summary>
        /// <param name="message">出力する文字列、またはオブジェクトの参照</param>
        public virtual void Warning(object message)
        {
            Debug.LogWarning(message);
        }

        /// <summary>
        /// 警告を出力します。
        /// </summary>
        /// <param name="message">出力する文字列、またはオブジェクトの参照</param>
        /// <param name="content">messageが適用されるオブジェクトの参照</param>
        public virtual void Warning(object message, Object content)
        {
            Debug.LogWarning(message, content);
        }

        /// <summary>
        /// エラーを出力します。
        /// </summary>
        /// <param name="message">出力する文字列、またはオブジェクトの参照</param>
        public virtual void Error(object message)
        {
            Debug.LogError(message);
        }

        /// <summary>
        /// エラーを出力します。
        /// </summary>
        /// <param name="message">出力する文字列、またはオブジェクトの参照</param>
        /// <param name="content">messageが適用されるオブジェクトの参照</param>
        public virtual void Error(object message, Object content)
        {
            Debug.LogError(message, content);
        }

#endregion
    }
}
