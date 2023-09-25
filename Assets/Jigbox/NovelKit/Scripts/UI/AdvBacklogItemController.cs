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
    public class AdvBacklogItemController : MonoBehaviour
    {
#region properties
        
        /// <summary>テキスト</summary>
        [SerializeField]
        protected Components.TextView text;

        /// <summary>ラベル</summary>
        [SerializeField]
        protected Components.TextView label;

        /// <summary>音声リピートボタン</summary>
        [SerializeField]
        protected GameObject repeatButton;

        /// <summary>バックログの管理コンポーネント</summary>
        protected AdvBacklogManager backlogManager;

        /// <summary>サウンド情報</summary>
        protected string sound;

#endregion

#region public methods

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="manager">バックログの管理コンポーネント</param>
        public virtual void Init(AdvBacklogManager manager)
        {
            backlogManager = manager;
        }

        /// <summary>
        /// テキスト情報を設定します。
        /// </summary>
        /// <param name="info">テキスト情報</param>
        public virtual void SetText(AdvWindowTextController.TextInfo info)
        {
            label.Text = info.Label;
            text.Text = info.Text;
            sound = info.Sound;
            if (repeatButton != null)
            {
                repeatButton.SetActive(!string.IsNullOrEmpty(sound));
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// リピートボタンが押された際に呼び出されます。
        /// </summary>
        [AuthorizedAccess]
        protected virtual void OnClickRepeat()
        {
            backlogManager.RepeatSound(sound);
        }

#endregion
    }
}
