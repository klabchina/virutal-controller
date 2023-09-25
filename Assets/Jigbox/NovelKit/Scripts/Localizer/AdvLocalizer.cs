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

namespace Jigbox.NovelKit
{
    public class AdvLocalizer : IAdvLocalizer
    {
#region properties

        /// <summary>ユーザー名のタグ</summary>
        protected virtual string UserNameTag { get { return "[player]"; } }

        /// <summary>辞書</summary>
        protected Dictionary<string, string> dictionary = new Dictionary<string, string>();

        /// <summary>ユーザー名</summary>
        protected string userName;

#endregion

#region public methods

        /// <summary>
        /// 指定されたkeyから該当する文字列を返します。
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns></returns>
        public string Resolve(string key)
        {
            string text = key;
            if (dictionary.ContainsKey(key))
            {
                text = dictionary[key];
            }

            if (text.Contains(UserNameTag))
            {
                return text.Replace(UserNameTag, userName);
            }
            return text;
        }

        /// <summary>
        /// 文言を設定します。
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="value">文言</param>
        public void Set(string key, string value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        /// <summary>
        /// 文言を削除します。
        /// </summary>
        /// <param name="key">キー</param>
        public void Remove(string key)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);
            }
        }

        /// <summary>
        /// ユーザー名を設定します。
        /// </summary>
        /// <param name="userName">ユーザー名</param>
        public void SetUserName(string userName)
        {
            this.userName = userName;
        }

#endregion
    }
}
