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
    public class AdvCommandVariable : AdvCommandBase
    {
#region constants

        /// <summary>式内の区切り文字</summary>
        public static readonly char[] FormulaDelimiter = { '+', '-', '*', '/' };

#endregion

#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 1; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 2; } }

        /// <summary>変数名</summary>
        public string Name { get { return Param[0].Substring(1); } }
        
        /// <summary>式</summary>
        public string Formula { get { return Param.Length == MinParameterCount ? "0" : Param[1]; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandVariable(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns>次のコマンドを連続で実行する場合、trueを返します。</returns>
        public override bool Execute(AdvMainEngine engine)
        {
            SetVariable(VariablePrefix, engine.VariableManager);
            return true;
        }

        /// <summary>
        /// 式に含まれる変数を展開します。
        /// </summary>
        /// <param name="formula">式</param>
        /// <param name="prefix">識別用のプレフィックス</param>
        /// <param name="variableManager">変数管理クラス</param>
        /// <returns>展開に失敗した場合、失敗した変数名を返します。</returns>
        public static string ConvertVariables(ref string formula, char prefix, AdvScriptVariableManager variableManager)
        {
            string[] values = formula.Split(FormulaDelimiter);

            for (int i = 0; i < values.Length; ++i)
            {
                string value = values[i];
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                if (value[0] == prefix)
                {
                    string name = value.Substring(1);
                    if (variableManager.Contains(name))
                    {
                        formula = formula.Replace(value, variableManager.Get(name).ToString());
                    }
                    else
                    {
                        return name;
                    }
                }
            }

            return string.Empty;
        }

#endregion

#region protected methods

        /// <summary>
        /// 変数の内容を設定します。
        /// </summary>
        /// <param name="prefix">識別用のプレフィックス</param>
        /// <param name="variableManager">変数管理クラス</param>
        protected virtual void SetVariable(char prefix, AdvScriptVariableManager variableManager)
        {
            // 定義のみの場合、値の更新は行わない
            if (Param.Length == MinParameterCount)
            {
                if (!variableManager.Contains(Name))
                {
                    variableManager.Set(Name, 0);
                }
            }
            else
            {
                string formula = Formula;

                string errorVariableName = ConvertVariables(ref formula, prefix, variableManager);
                if (!string.IsNullOrEmpty(errorVariableName))
                {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                    ErrorMessage = "定義されていない変数(" + errorVariableName + ")が指定されました。";
                    AdvLog.Error(ErrorMessage);
#endif
                    return;
                }

                int variableValue = AdvFormulaUtil.GetValue(formula);
                if (variableValue != AdvFormulaUtil.Failed)
                {
                    variableManager.Set(Name, variableValue);
                }
#if UNITY_EDITOR || NOVELKIT_DEBUG
                else
                {
                    ErrorMessage = "式を正しく処理できませんでした。";
                    AdvLog.Error(ErrorMessage);
                }
#endif
            }
            return;
        }

#endregion
    }
}
