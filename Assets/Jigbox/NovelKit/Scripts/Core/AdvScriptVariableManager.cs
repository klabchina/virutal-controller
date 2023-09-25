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
    public sealed class AdvScriptVariableManager
    {
#region properties

        /// <summary>変数</summary>
        Dictionary<string, int> values = new Dictionary<string, int>();

#endregion

#region public methods
        
        /// <summary>
        /// 変数を設定します。
        /// </summary>
        /// <param name="name">変数名</param>
        /// <param name="value">値</param>
        public void Set(string name, int value)
        {
            if (values.ContainsKey(name))
            {
                values[name] = value;
            }
            else
            {
                values.Add(name, value);
            }
        }

        /// <summary>
        /// 変数を取得します。
        /// </summary>
        /// <param name="name">変数名</param>
        /// <returns></returns>
        public int Get(string name)
        {
            return values[name];
        }

        /// <summary>
        /// 対象の変数が存在するかどうかを返します。
        /// </summary>
        /// <param name="name">変数名</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return values.ContainsKey(name);
        }

        /// <summary>
        /// 変数をクリアします。
        /// </summary>
        public void Clear()
        {
            values.Clear();
        }

#endregion
    }
}
