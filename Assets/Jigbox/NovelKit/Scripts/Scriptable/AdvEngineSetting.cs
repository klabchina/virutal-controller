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

namespace Jigbox.NovelKit
{
    public class AdvEngineSetting : ScriptableObject
    {
#region inner classes, enum, and structs

        [System.Serializable]
        public class SavedEngineSetting
        {
            /// <summary>テキストの文字送り速度</summary>
            [SerializeField]
            public float TextCaptionMargin = 0.025f;

            /// <summary>自動再生時の次のテキストへの待ち時間</summary>
            [SerializeField]
            public float AutoPlayWait = 1.5f;

            /// <summary>自動再生時のテキストの文字送り速度</summary>
            [SerializeField]
            public float TextCaptionMarginAuto = 0.04f;

            /// <summary>クリックで音声を停止させるかどうか</summary>
            [SerializeField]
            public bool StopVoiceWithClick = false;

            /// <summary>自動再生時、ボイスの終了を待つかどうか</summary>
            [SerializeField]
            public bool WaitVoiceEndWhenAuto = false;

            /// <summary>選択肢で自動再生を解除するかどうか</summary>
            [SerializeField]
            public bool IsReleaseAutoWhenSelect = false;

            /// <summary>選択肢でスキップを解除するかどうか</summary>
            [SerializeField]
            public bool IsReleaseSkipWhenSelect = false;
        }

#endregion

#region constants

        /// <summary>設定の保存用キー</summary>
        static readonly string SaveKey = "ADV_ENGINE_SETTING";

#endregion

#region properties

        /// <summary>独自拡張したLoaderを使用するか</summary>
        [SerializeField]
        bool useCustomLoader = false;

        /// <summary>独自拡張したLoaderを使用するか</summary>
        public bool UseCustomLoader { get { return useCustomLoader; } }

        /// <summary>独自拡張したLocalizerを使用するか</summary>
        [SerializeField]
        bool useCustomLocalizer = false;

        /// <summary>独自拡張したLocalizerを使用するか</summary>
        public bool UseCustomLocalizer { get { return useCustomLocalizer; } }

        /// <summary>画面効果を使用するか</summary>
        [SerializeField]
        bool useScreenEffection = false;

        /// <summary>画面効果を使用するか</summary>
        public bool UseScreenEffection { get { return useScreenEffection; } }

        /// <summary>画面効果使用時に自動で表示を切り替えるか</summary>
        [SerializeField]
        bool autoSwitchScreenEffection = true;

        /// <summary>画面効果使用時に自動で表示を切り替えるか</summary>
        public bool AutoSwitchScreenEffection { get { return autoSwitchScreenEffection; } }

        /// <summary>コマンドの補完時にサウンドコマンドをスキップするかどうか</summary>
        [SerializeField]
        bool isSkipSoundCommandComplement = false;

        /// <summary>コマンドの補完時にサウンドコマンドをスキップするかどうか</summary>
        public bool IsSkipSoundCommandComplement { get { return isSkipSoundCommandComplement; } }

        /// <summary>バックログの長さ</summary>
        [SerializeField]
        int backlogLength = 20;

        /// <summary>バックログの長さ</summary>
        public int BacklogLength { get { return backlogLength; } }

        /// <summary>バックログのリソースのパス</summary>
        [SerializeField]
        string backlogResourcePath = string.Empty;

        /// <summary>バックログのリソースのパス</summary>
        public string BacklogResourcePath { get { return backlogResourcePath; } }

        /// <summary>選択肢のリソースのパス</summary>
        [SerializeField]
        string selectResourcePath = string.Empty;

        /// <summary>選択肢のリソースのパス</summary>
        public string SelectResourcePath { get { return selectResourcePath; } }

        /// <summary>テキストの表示モード</summary>
        [SerializeField]
        AdvWindowManager.TextMode defaultTextMode = AdvWindowManager.TextMode.Normal;

        /// <summary>テキストの表示モード</summary>
        public AdvWindowManager.TextMode DefaultTextMode { get { return defaultTextMode; } }

        /// <summary>TextModeをAdditiveNewLineに指定した際に自動挿入される改行タグ</summary>
        [SerializeField]
        string autoAdditiveNewLineTag = "<br/>";

        /// <summary>TextModeをAdditiveNewLineに指定した際に自動挿入される改行タグ</summary>
        public string AutoAdditiveNewLineTag { get { return autoAdditiveNewLineTag; } }

        /// <summary>スキップ状態での時間の短縮率</summary>
        [SerializeField]
        float skipTimeScale = 0.125f;

        /// <summary>スキップ状態での時間の短縮率</summary>
        public float SkipTimeScale { get { return skipTimeScale; } }

        /// <summary>ローカルでの保存が可能な設定情報</summary>
        [SerializeField]
        SavedEngineSetting savedSetting = null;

        /// <summary>ローカルでの保存が可能な設定情報</summary>
        public SavedEngineSetting SavedSetting { get { return savedSetting; } }

        /// <summary>テキストの文字送り速度</summary>
        public float TextCaptionMargin { get{ return savedSetting.TextCaptionMargin; } }

        /// <summary>自動再生時の次のテキストへの待ち時間</summary>
        public float AutoPlayWait { get { return savedSetting.AutoPlayWait; } }

        /// <summary>自動再生時のテキストの文字送り速度</summary>
        public float TextCaptionMarginAuto { get { return savedSetting.TextCaptionMarginAuto; } }

        /// <summary>クリックで音声を停止させるかどうか</summary>
        public bool StopVoiceWithClick { get { return savedSetting.StopVoiceWithClick; } }

        /// <summary>自動再生時、ボイスの終了を待つかどうか</summary>
        public bool WaitVoiceEndWhenAuto { get { return savedSetting.WaitVoiceEndWhenAuto; } }

        /// <summary>選択肢で自動再生を解除するかどうか</summary>
        public bool IsReleaseAutoWhenSelect { get { return savedSetting.IsReleaseAutoWhenSelect; } }

        /// <summary>選択肢でスキップを解除するかどうか</summary>
        public bool IsReleaseSkipWhenSelect { get { return savedSetting.IsReleaseSkipWhenSelect; } }

#endregion

#region public methods

        /// <summary>
        /// 保存可能な情報を保存します。
        /// </summary>
        public void Save()
        {
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(savedSetting));
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 保存されている情報を読み込みます。
        /// </summary>
        public void Load()
        {
            if (PlayerPrefs.HasKey(SaveKey))
            {
                string saveString = PlayerPrefs.GetString(SaveKey);
                JsonUtility.FromJsonOverwrite(saveString, savedSetting);
            }
        }

#endregion
    }
}
