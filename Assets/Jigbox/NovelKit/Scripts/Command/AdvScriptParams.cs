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

using System.Text.RegularExpressions;

namespace Jigbox.NovelKit
{
    public class AdvScriptParams
    {
#region constants

        /// <summary>コマンドをパースする際の区切り文字</summary>
        public static readonly string[] Delimiter = new string[] { " ", "\t" };

        /// <summary>コマンドをパースする際の区切り文字</summary>
        public static readonly string DelimiterString = " ";

        /// <summary>文字列をまとめて1パラメータ扱いにする際の正規表現パターン</summary>
        public static readonly string PackedParameterRegexPattern = @"\|\|((?!\|\|).)*\|\|";

        /// <summary>
        /// パラメータ展開に使うRegexは同じインスタンスで良いのでキャッシュさせるためにstaticで保持する
        /// .NET4.x向けにstaticにしている。.NET3.5ではRegexクラスが内部でキャッシュしていた。
        /// </summary>
        protected static readonly Regex regex = new Regex(PackedParameterRegexPattern, RegexOptions.IgnoreCase);

#endregion

#region properties

        /// <summary>分割したパラメータ</summary>
        public string[] Param 
        {
            get
            {
                return referencedParam;
            }
            protected set
            {
                BaseParam = value;
                CheckVariableAndApplyDefault();
            }
        }

        /// <summary>分割したパラメータの元の状態</summary>
        public string[] BaseParam { get; private set; }

        /// <summary>参照用の分割したパラメータ</summary>
        string[] referencedParam;

        /// <summary>引数内に変数があるかどうか</summary>
        public bool HasVariables { get; private set; }

        /// <summary>変数が利用可能となるパラメータのインデックス</summary>
        protected virtual int VariableEnableIndex { get { return int.MaxValue; } }

#if UNITY_EDITOR || NOVELKIT_DEBUG
        /// <summary>エラーメッセージ</summary>
        public string ErrorMessage { get; set; }
#else
        /// <summary>エラーメッセージ</summary>
        public string ErrorMessage { get { return string.Empty; } set { } }
#endif

#endregion

#region public methods
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="str">元となる文字列</param>
        public AdvScriptParams(string str)
        {
            Parse(str);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="param">パラメータ</param>
        public AdvScriptParams(string[] param)
        {
            Param = param;
        }

        /// <summary>
        /// 現在の変数の値をパラメータに反映させます。
        /// </summary>
        /// <param name="variableParamManager">引数に設定可能な変数管理クラス</param>
        public bool ApplyVariables(AdvScriptVariableManager variableParamManager)
        {
            for (int i = 0; i < referencedParam.Length; ++i)
            {
                if (BaseParam[i][0] == AdvCommandBase.VariableParamPrefix)
                {
                    string name = BaseParam[i].Substring(1);
                    if (variableParamManager.Contains(name))
                    {
                        referencedParam[i] = variableParamManager.Get(name).ToString();
                    }
                    else
                    {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                        ErrorMessage = "記述内容 : " + Join(BaseParam) 
                            + "\n定義されていない変数(" + name + ")が指定されました。";
                        AdvLog.Error(ErrorMessage);
#endif
                        return false;
                    }
                }
            }

            return UpdateByVariable();
        }

        /// <summary>
        /// 文字列をタブ、スペースで分割して取得します。
        /// </summary>
        /// <param name="str">元となる文字列</param>
        /// <returns></returns>
        public static string[] Split(string str)
        {
            return str.Split(Delimiter, System.StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 文字列をスベース区切りで結合します。
        /// </summary>
        /// <param name="str">元となる文字列</param>
        /// <returns></returns>
        public static string Join(string[] str)
        {
            return string.Join(DelimiterString, str);
        }

#endregion

#region protected methods

        /// <summary>
        /// 文字列からパラメータを展開します。
        /// </summary>
        /// <param name="str"></param>
        protected virtual void Parse(string str)
        {
            Param = str.Split(Delimiter, System.StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 文字列を正規表現に従ってまとめながら、パラメータを展開します。
        /// </summary>
        /// <param name="str"></param>
        protected void ParseWithPacking(string str)
        {
            MatchCollection match = regex.Matches(str);
            if (match.Count == 0)
            {
                Param = str.Split(Delimiter, System.StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                for (int i = 0; i < match.Count; ++i)
                {
                    str = str.Replace(match[i].Value, "match-marker-" + i);
                }

                string[] param = str.Split(Delimiter, System.StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < match.Count; ++i)
                {
                    string replaceString = "match-marker-" + i;
                    for (int j = 0; j < param.Length; ++j)
                    {
                        if (param[j] == replaceString)
                        {
                            // 先頭、末尾の識別用文字列分を飛ばして設定
                            param[j] = match[i].Value.Substring(2, match[i].Length - 4);
                        }
                    }
                }

                Param = param;
            }
        }

        /// <summary>
        /// コマンドの引数内に変数が含まれているかどうかを確認します。
        /// </summary>
        protected void CheckVariableAndApplyDefault()
        {
            for (int i = VariableEnableIndex; i < BaseParam.Length; ++i)
            {
                string param = BaseParam[i];
                if (param[0] == AdvCommandBase.VariableParamPrefix && param.Length > 1)
                {
                    // 引数に設定可能な変数は、性質上完全なバリデーションができないため、
                    // パース時には、引数に指定される値にはデフォルトで1を入れておき、
                    // パース自体が出来ないような状態を避ける
                    if (referencedParam == null)
                    {
                        referencedParam = BaseParam.Clone() as string[];
                    }
                    referencedParam[i] = "1";
                    HasVariables = true;
                }
            }
            
            if (!HasVariables)
            {
                referencedParam = BaseParam;
            }
        }

        /// <summary>
        /// 現在の変数の値に合わせて、自身の状態を更新します。
        /// </summary>
        /// <returns></returns>
        protected virtual bool UpdateByVariable()
        {
            return true;
        }

#endregion
    }
}
