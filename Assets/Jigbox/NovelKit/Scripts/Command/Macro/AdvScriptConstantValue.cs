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

namespace Jigbox.NovelKit
{
    public class AdvScriptConstantValue : AdvScriptParams
    {
#region properties
        
        /// <summary>定数名</summary>
        public string Name { get; protected set; }
        
#endregion

#region public methods
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="str">元となる文字列</param>
        public AdvScriptConstantValue(string str) : base(str)
        {
            // パラメータが1以下の場合、無効
            if (Param.Length <= 1)
            {
                 ErrorMessage = "定数の実体が存在しない定義が含まれています。";
                return;
            }
            // 第1パラメータが1文字以下の場合、無効
            if (Param[0].Length <= 1)
            {
                ErrorMessage = "定数名は、1文字以上でなければなりません。";
                return;
            }
            // 1文字目が"@"ではない場合、無効
            if (Param[0][0] != AdvCommandBase.ConstantValuePrefix)
            {
                ErrorMessage = "定数定義の先頭文字は、\"@\"でなければなりません。";
                return;
            }

            for (int i = 1; i < Param.Length; ++i)
            {
                if (Param[i][0] == AdvCommandBase.ConstantValuePrefix)
                {
                    ErrorMessage = "定数の定義に別な定数を使用することはできません";
                    return;
                }
            }

            Name = Param[0].Substring(1);

            // パラメータから定数名を除外
            string[] temp = new string[Param.Length - 1];
            for (int i = 0; i < temp.Length; ++i)
            {
                temp[i] = Param[i + 1];
            }
            Param = temp;
        }

#endregion
    }
}
