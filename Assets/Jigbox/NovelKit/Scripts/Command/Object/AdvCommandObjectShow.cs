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
    using ObjectType = AdvObjectBase.ObjectType;

    public class AdvCommandObjectShow : AdvObjectCommandBase
    {
#region properties

        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return isSub ? 4 : 3; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return isSub ? 5 : 4; } }

        /// <summary>トランジションの時間(第4パラメータ)</summary>
        protected string TransitionTimeParam { get { return isSub ? Param[4] : Param[3]; } }

        /// <summary>トランジションの設定が存在するか</summary>
        protected bool IsExistTransition { get { return Param.Length > MinParameterCount; } }

        /// <summary>トランジションの時間</summary>
        protected float transitionTime = 0.0f;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        /// <param name="objectType">オブジェクトの種類</param>
        /// <param name="isSub">差分オブジェクトかどうか</param>
        public AdvCommandObjectShow(AdvCommandBase command, ObjectType objectType, bool isSub = false)
            : base(command, objectType, isSub)
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            if (IsExistTransition)
            {
                if (!float.TryParse(TransitionTimeParam, out transitionTime))
                {
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "トランジションの時間設定が不正です。";
                    return;
                }
                else if (transitionTime < 0.0f)
                {
                    Type = CommandType.ErrorCommand;
                    ErrorMessage = "トランジションの時間に負数は設定できません。";
                    return;
                }
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// コマンドによって発生する処理を行います。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <param name="obj">対象となるオブジェクト</param>
        protected override void CommandProcess(AdvMainEngine engine, AdvObjectBase obj)
        {
            if (obj is IAdvObjectShowTransition)
            {
                IAdvObjectShowTransition transitionObject = obj as IAdvObjectShowTransition;
                if (IsExistTransition)
                {
                    transitionObject.Show(GetScaledTime(engine, transitionTime));
                }
                else
                {
                    transitionObject.Show(GetScaledTime(engine, transitionObject.ShowTransitionTime));
                }
                engine.MovementManager.Register(transitionObject.AlphaTween);
            }
            else
            {
                obj.Show();
            }
        }

#endregion
    }
}
