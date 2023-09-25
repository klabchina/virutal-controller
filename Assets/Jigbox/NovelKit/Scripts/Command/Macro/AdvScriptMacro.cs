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
    public class AdvScriptMacro : AdvScriptParams
    {
#region constants

        /// <summary>引数とデフォルト値の区切り文字</summary>
        protected static readonly char ArgmentDelimiter = '=';

#endregion

#region properties

        /// <summary>マクロ名</summary>
        public string Name { get; protected set; }

        /// <summary>引数情報</summary>
        protected Dictionary<string, string> args = new Dictionary<string, string>();

        /// <summary>引数情報</summary>
        public Dictionary<string, string> Args { get { return new Dictionary<string, string>(args); } }
        
#endregion

#region public methods
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="str">元となる文字列</param>
        public AdvScriptMacro(string str) : base(str)
        {
            // 第1パラメータが1文字以下の場合、無効
            if (Param[0].Length <= 1)
            {
                ErrorMessage = "マクロ名は、1文字以上でなければなりません。";
                return;
            }
            // 1文字目が"%"ではない場合、無効
            if (Param[0][0] != AdvCommandBase.MacroPrefix)
            {
                ErrorMessage = "マクロ定義の先頭文字は、\"%\"でなければなりません。";
                return;
            }

            for (int i = 1; i < Param.Length; ++i)
            {
                if (Param[i][0] != AdvCommandBase.VariablePrefix)
                {
                    ErrorMessage = "引数の先頭文字は、\"$\"でなければなりません。";
                    return;
                }
                string[] arg = Param[i].Split(ArgmentDelimiter);
                if (arg.Length == 2)
                {
                    if (args.ContainsKey(arg[0]))
                    {
                        ErrorMessage = "引数名が重複しています。";
                        return;
                    }
                    else
                    {
                        args.Add(arg[0], arg[1]);
                    }
                }
                else
                {
                    ErrorMessage = "引数にデフォルト値が正しく設定されていません。";
                    return;
                }
            }

            Name = Param[0].Substring(1);            
        }

#endregion
    }
}
