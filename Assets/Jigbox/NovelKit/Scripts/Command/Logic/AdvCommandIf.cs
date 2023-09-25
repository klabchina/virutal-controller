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
    public class AdvCommandIf : AdvCommandBase
    {
#region inner class, enum, and structs

        public enum ConditionType
        {
            None,
            /// <summary>同値</summary>
            Equal,
            /// <summary>同値でない</summary>
            NotEqual,
            /// <summary>左辺が右辺以上</summary>
            MoreEqual,
            /// <summary>左辺が右辺以下</summary>
            LessEqusl,
            /// <summary>左辺が右辺より大きい</summary>
            More,
            /// <summary>左辺が右辺より小さい</summary>
            Less,
        }

#endregion

#region constants

        /// <summary>条件</summary>
        public static string[] ConditionString = { "==", "!=", ">=", "<=", ">", "<" };

#endregion

#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 4; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 4; } }

        /// <summary>左辺のパラメータ</summary>
        protected string LeftFormulaParam { get { return Param[1]; } }

        /// <summary>条件のパラメータ</summary>
        protected string ConditionParam { get { return Param[2]; } }

        /// <summary>右辺のパラメータ</summary>
        protected string RightFormulaParam { get { return Param[3]; } }

        /// <summary>条件の種類</summary>
        public ConditionType Condition { get; protected set; }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandIf(AdvCommandBase command) : base(command)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            Type = CommandType.If;

            for (int i = (int) ConditionType.Equal; i <= (int) ConditionType.Less; ++i)
            {
                if (ConditionParam == ConditionString[i - 1])
                {
                    Condition = (ConditionType) i;
                    break;
                }
            }
            
            if (Condition == ConditionType.None)
            {
                Type = CommandType.ErrorCommand;
                ErrorMessage = "条件が不正です。";
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
            bool result = false;
            string leftFormula = LeftFormulaParam;
            string rightFormula = RightFormulaParam;

            string errorVariableName = AdvCommandVariable.ConvertVariables(ref leftFormula, VariablePrefix, engine.VariableManager);
            if (string.IsNullOrEmpty(errorVariableName))
            {
                errorVariableName = AdvCommandVariable.ConvertVariables(ref rightFormula, VariablePrefix, engine.VariableManager);
            }
            if (!string.IsNullOrEmpty(errorVariableName))
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                ErrorMessage = "定義されていない変数(" + errorVariableName + ")が指定されました。";
                AdvLog.Error(ErrorMessage);
#endif
                return result;
            }

            int leftValue = AdvFormulaUtil.GetValue(leftFormula);
            int rightValue = AdvFormulaUtil.GetValue(rightFormula);
            
            if (leftValue != AdvFormulaUtil.Failed && rightValue != AdvFormulaUtil.Failed)
            {
                switch (Condition)
                {
                    case ConditionType.Equal:
                        result = leftValue == rightValue;
                        break;
                    case ConditionType.NotEqual:
                        result = leftValue != rightValue;
                        break;
                    case ConditionType.MoreEqual:
                        result = leftValue >= rightValue;
                        break;
                    case ConditionType.LessEqusl:
                        result = leftValue <= rightValue;
                        break;
                    case ConditionType.More:
                        result = leftValue > rightValue;
                        break;
                    case ConditionType.Less:
                        result = leftValue < rightValue;
                        break;
                }
            }
#if UNITY_EDITOR || NOVELKIT_DEBUG
            else
            {
                ErrorMessage = "式を正しく処理できませんでした。";
                AdvLog.Error(ErrorMessage);
            }
#endif

            return result;
        }

#endregion
    }
}
