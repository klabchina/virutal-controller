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
    public class AdvSelectItemController : MonoBehaviour
    {
#region properties

        /// <summary>テキストコンポーネント</summary>
        [SerializeField]
        protected Components.TextView text;

        /// <summary>選択肢管理コンポーネント</summary>
        protected AdvSelectManager manager;

        /// <summary>選択肢コマンド</summary>
        protected AdvCommandSelect command;

#endregion

#region public methods

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="text">選択肢に表示するテキスト</param>
        /// <param name="command">選択肢コマンド</param>
        public virtual void Init(string text, AdvCommandSelect command)
        {
            this.text.Text = text;
            this.command = command;
        }

        /// <summary>
        /// 管理コンポーネントを設定します。
        /// </summary>
        /// <param name="manager">選択肢管理コンポーネント</param>
        public void SetManager(AdvSelectManager manager)
        {
            this.manager = manager;
        }

#endregion

#region protected methods

        /// <summary>
        /// 選択肢が選択された際に呼び出されます。
        /// </summary>
        [AuthorizedAccess]
        protected virtual void OnClick()
        {
            manager.OnSelect();
            command.ChangeScene();
        }

#endregion
    }
}
