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

using UnityEditor;
using System;

namespace Jigbox.EditorUtils
{
    public class EditorSerialProcessor
    {
#region properties

        /// <summary>
        /// Update処理内でExceptionが投げられた場合のコールバック処理
        /// </summary>
        public Action<Exception> UpdateExceptionCallback { get; set; }

        /// <summary>処理する内容</summary>
        protected Func<object, object>[] processes;

        /// <summary>現在の処理インデックス</summary>
        protected int current = 0;

        /// <summary>負荷軽減用のカウンタ</summary>
        protected int counter = 0;

        /// <summary>処理の際に受け渡す情報</summary>
        protected object argument = false;

        /// <summary>処理負荷軽減用の抵抗値</summary>
        protected virtual int RegistCount { get { return 10; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="firstArgment">最初の引数</param>
        /// <param name="processes">処理する内容</param>
        public EditorSerialProcessor(object firstArgment, params Func<object, object>[] processes)
        {
            if (processes.Length == 0)
            {
                UnityEngine.Assertions.Assert.IsTrue(true, "EditorSequentialProcessor : Can't use empty process!");
                return;
            }

            this.processes = processes;
            counter = RegistCount;
            argument = firstArgment;
            EditorApplication.update += Update;
        }

        /// <summary>
        /// 登録したプロセスをクリアしてUpdateの処理もとめます
        /// </summary>
        public void Clear()
        {
            processes = new Func<Object,Object>[] { };
            EditorApplication.update -= Update;
        }

#endregion

#region protected methods

        /// <summary>
        /// 状態を更新します。
        /// </summary>
        protected void Update()
        {
            ++counter;

            // EditorApplication.updateは超高速で発火するので
            // 毎回処理しているとエディタがフリーズしかねないので、
            // あえて処理を遅らせるために抵抗を挟む
            if (counter < RegistCount)
            {
                return;
            }

            counter = 0;

            // エディタがリフレッシュ中の間は処理しない
            if (!EditorApplication.isUpdating)
            {
                try
                {
                    argument = processes[current](argument);
                }
                catch (Exception e)
                {
                    if (UpdateExceptionCallback == null)
                    {
                        throw;
                    }

                    UpdateExceptionCallback(e);
                }
                 ++current;
                if (processes.Length <= current)
                {
                    EditorApplication.update -= Update;
                }
            }
        }

#endregion
    }
}
