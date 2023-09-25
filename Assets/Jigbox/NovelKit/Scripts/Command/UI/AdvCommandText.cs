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
    public class AdvCommandText : AdvCommandBase, IAdvCommandSoundResource
    {
#region constants
        
        /// <summary>キャラクター出ない場合のラベル</summary>
        protected static readonly string NoneCharacterLabel = "-";

#endregion

#region properties
        
        /// <summary>コマンド実行に最低限必要なパラメータ数</summary>
        protected override int MinParameterCount { get { return 2; } }

        /// <summary>コマンド実行に使用可能なパラメータ数の上限</summary>
        protected override int MaxParameterCount { get { return 4; } }

        /// <summary>ラベル(キャラクター名)</summary>
        protected string Label { get { return Param[0] != NoneCharacterLabel ? Param[0] : string.Empty; } }

        /// <summary>テキスト(本文、セリフ)</summary>
        protected string Text { get { return Param[1]; } }
        
        /// <summary>サウンド</summary>
        protected string sound = string.Empty;

        /// <summary>コマンドを連続処理するかどうか</summary>
        protected bool isContinual = false;
        
        /// <summary>コマンドで使用するサウンドのリソース</summary>
        public virtual string SoundResource { get { return sound; } }

#endregion

#region public methods
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となるコマンドの基礎データ</param>
        public AdvCommandText(AdvCommandBase command) : base(command)
        {
            ValidateParmeters();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command">元となる文字列</param>
        public AdvCommandText(string str) : base(str)
        {
            // こちらのコンストラクタでは自動で呼び出されないので、明示的に呼び出す
            IsValidParameterCount();
            ValidateParmeters();
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        /// <returns>次のコマンドを連続で実行する場合、trueを返します。</returns>
        public override bool Execute(AdvMainEngine engine)
        {
            string label = engine.Localizer.Resolve(Label);
            string text = engine.Localizer.Resolve(Text);

            AdvScenarioStatusManager statusManager = engine.StatusManager;

            if (!isContinual)
            {
                statusManager.State.Wait(AdvPlayStateManager.PlayState.WaitClick);
            }
            engine.UIManager.SetText(new AdvWindowTextController.TextInfo(label, text, sound));

            if (!string.IsNullOrEmpty(sound))
            {
                if (statusManager.Mode.IsAuto && engine.Settings.EngineSetting.WaitVoiceEndWhenAuto)
                {
                    statusManager.State.Wait(AdvPlayStateManager.PlayState.WaitSound, engine.UIManager.IsSingleTextMode);
                }
                engine.SoundManager.StopVoice();
                engine.SoundManager.Post(sound, statusManager.State.OnEndSound, engine.Player.DelayManager.OnMarkedSound);
            }
            return isContinual;
        }

#endregion

#region protected methods

        /// <summary>
        /// 文字列からパラメータを展開します。
        /// </summary>
        /// <param name="str"></param>
        protected override void Parse(string str)
        {
            ParseWithPacking(str);
        }

        /// <summary>
        /// パラメータのバリデーションを行います。
        /// </summary>
        protected virtual void ValidateParmeters()
        {
            if (Type == CommandType.ErrorCommand)
            {
                return;
            }

            Type = CommandType.Text;

            if (Param.Length > MinParameterCount)
            {
                if (Param.Length == MaxParameterCount)
                {
                    sound = Param[2];
                    if (Param[3] == ContinualCommandParameter)
                    {
                        isContinual = true;
                    }
                    else
                    {
                        Type = CommandType.ErrorCommand;
                        ErrorMessage = "第4パラメータの指定が不正です。";
                        return;
                    }
                }
                // サウンドが未指定で連続処理指定の場合
                else if (Param[2] == ContinualCommandParameter)
                {
                    isContinual = true;
                }
                else
                {
                    sound = Param[2];
                }
            }
        }

#endregion
    }
}

