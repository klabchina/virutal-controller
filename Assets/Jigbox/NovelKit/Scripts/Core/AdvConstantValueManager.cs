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
    public sealed class AdvConstantValueManager
    {
#region properties

        /// <summary>定数</summary>
        Dictionary<string, string[]> values = new Dictionary<string, string[]>();

#endregion

#region public methods

        /// <summary>
        /// 定数を記録します。
        /// </summary>
        /// <param name="name">定数名</param>
        /// <param name="param">定数のパラメータ</param>
        public void Add(string name, string[] param)
        {
            if (values.ContainsKey(name))
            {
                values[name] = param;
            }
            else
            {
                values.Add(name, param);
            }
        }

        /// <summary>
        /// 定数を記録します。
        /// </summary>
        /// <param name="values">定数情報</param>
        public void Add(Dictionary<string, string[]> values)
        {
            foreach (KeyValuePair<string, string[]> value in values)
            {
                Add(value.Key, value.Value);
            }
        }

        /// <summary>
        /// 記録されている定数を破棄します。
        /// </summary>
        /// <param name="name">定数名</param>
        public void Remove(string name)
        {
            if (values.ContainsKey(name))
            {
                values.Remove(name);
            }
        }

        /// <summary>
        /// 記録されている定数を破棄します。
        /// </summary>
        /// <param name="names">定数名</param>
        public void Remove(List<string> names)
        {
            foreach (string name in names)
            {
                Remove(name);
            }
        }

        /// <summary>
        /// 定数を取得します。
        /// </summary>
        /// <param name="name">定数名</param>
        /// <returns>定数が存在しなかった場合、nullが返ります。</returns>
        public string[] Get(string name)
        {
            if (values.ContainsKey(name))
            {
                return values[name];
            }
            return null;
        }

        /// <summary>
        /// 定数をクリアします。
        /// </summary>
        public void Clear()
        {
            values.Clear();
        }

#endregion
    }
}
