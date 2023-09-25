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
using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    public static class AdvFormulaUtil
    {
#region constants
        
        /// <summary>計算に失敗した際の返り値</summary>
        public static readonly int Failed = -int.MaxValue;

#endregion

#region public methods

        /// <summary>
        /// 計算式から値を取得します。
        /// </summary>
        /// <param name="formula">計算式</param>
        /// <returns></returns>
        public static int GetValue(string formula)
        {
            List<int> values = new List<int>();
            List<char> operators = new List<char>();

            int beginIndex = 0;
            int subLength = 0;
            bool isParse = false;

            // 要素分解
            for (int i = 0; i < formula.Length; ++i)
            {
                // 符号
                if (subLength == 0 && 
                    (formula[i] == '+' || formula[i] == '-'))
                {
                    ++subLength;
                    continue;
                }

                if (formula[i] >= '0' && formula[i] <= '9')
                {
                    ++subLength;
                    if (i == formula.Length - 1)
                    {
                        int value;
                        if (int.TryParse(formula.Substring(beginIndex, subLength), out value))
                        {
                            values.Add(value);
                        }
                        else
                        {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                            AdvLog.Error("計算式に値として不適切な要素が含まれています。");
#endif
                            return Failed;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                
                switch (formula[i])
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                        isParse = true;
                        break;
                }

                if (isParse)
                {
                    int value;
                    if (int.TryParse(formula.Substring(beginIndex, subLength), out value))
                    {
                        values.Add(value);
                        operators.Add(formula[i]);
                        beginIndex = i + 1;
                        subLength = 0;
                    }
                    else
                    {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                        AdvLog.Error("計算式に値として不適切な要素が含まれています。");
#endif
                        return Failed;
                    }
                    isParse = false;
                }
            }

            // 値が展開できていない
            if (values.Count == 0)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("計算式を展開できませんでした。");
#endif
                return Failed;
            }
            if (values.Count != operators.Count + 1)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("値と演算子の数が合っていません");
#endif
                return Failed;
            }

            // 乗算、除算処理
            for (int i = 0; i < operators.Count;)
            {
                switch (operators[i])
                {
                    case '*':
                        {
                            int value = values[i] * values[i + 1];
                            values[i] = value;
                            values.RemoveAt(i + 1);
                            operators.RemoveAt(i);
                        }
                        break;
                    case '/':
                        {
                            // 0除算
                            if (values[i + 1] == 0)
                            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                                AdvLog.Error("0除算です。");
#endif
                                return Failed;
                            }
                            int value = values[i] / values[i + 1];
                            values[i] = value;
                            values.RemoveAt(i + 1);
                            operators.RemoveAt(i);
                        }
                        break;
                    default:
                        ++i;
                        break;
                }
            }

            // 加算、減算処理
            for (int i = 0; i < operators.Count;)
            {
                switch (operators[i])
                {
                    case '+':
                        {
                            int value = values[i] + values[i + 1];
                            values[i] = value;
                            values.RemoveAt(i + 1);
                            operators.RemoveAt(i);
                        }
                        break;
                    case '-':
                        {
                            int value = values[i] - values[i + 1];
                            values[i] = value;
                            values.RemoveAt(i + 1);
                            operators.RemoveAt(i);
                        }
                        break;
                    default:
                        ++i;
                        break;
                }
            }

            // 値が全て計算できていない
            if (values.Count != 1)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("計算式を正しく計算できませんでした。");
#endif
                return Failed;
            }

            return values[0];
        }

#endregion
    }
}
