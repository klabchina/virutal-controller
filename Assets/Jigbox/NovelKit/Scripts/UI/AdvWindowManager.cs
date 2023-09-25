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
using UnityEngine.UI;
using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    public class AdvWindowManager : MonoBehaviour, IAdvManagementComponent
    {
#region inner classes, enum, and structs

        /// <summary>
        /// ウィンドウ情報
        /// </summary>
        [System.Serializable]
        public class Window
        {
            /// <summary>Windowのルートとなるオブジェクト</summary>
            [SerializeField]
            GameObject rootObject = null;

            /// <summary>Windowのルートとなるオブジェクト</summary>
            public GameObject RootObject { get { return rootObject; } }

            /// <summary>背景</summary>
            [SerializeField]
            Image mainBg = null;

            /// <summary>背景</summary>
            public Image MainBg { get { return mainBg; } }

            /// <summary>ラベル背景</summary>
            [SerializeField]
            Image labelBg = null;

            /// <summary>ラベル背景</summary>
            public Image LabelBg { get { return labelBg; } }

            /// <summary>テキスト表示制御コンポーネント</summary>
            [SerializeField]
            AdvWindowTextController textController = null;

            /// <summary>テキスト表示制御コンポーネント</summary>
            public AdvWindowTextController TextController { get { return textController; } }

            /// <summary>サムネイル表示制御コンポーネント</summary>
            [SerializeField]
            AdvThumbnailViewController thumbnailViewController = null;

            /// <summary>サムネイル表示制御コンポーネント</summary>
            public AdvThumbnailViewController ThumbnailViewController { get { return thumbnailViewController; } }
        }

        /// <summary>
        /// テキストの表示モード
        /// </summary>
        public enum TextMode
        {
            /// <summary>通常モード(クリックでクリア)</summary>
            Normal,
            /// <summary>追加モード(自動でクリアされない)</summary>
            Additive,
            /// <summary>自動改行付き追加モード(自動でクリアされない)</summary>
            AdditiveNewLine,
        }

#endregion

#region properties

        /// <summary>デフォルトで使用するウィンドウインデックス(1~N)</summary>
        [SerializeField]
        protected int mainWindowIndex = 1;

        /// <summary>ウィンドウの参照</summary>
        [SerializeField]
        protected List<Window> windows = new List<Window>();
        
        /// <summary>現在アクティブなウィンドウ</summary>
        protected Window activeWindow = null;

        /// <summary>現在アクティブなウィンドウ</summary>
        public Window ActiveWindow { get { return activeWindow; } }

        /// <summary>テキストの表示モード</summary>
        protected TextMode textMode = TextMode.Normal;

        /// <summary>ウィンドウが表示されているかどうか</summary>
        public bool IsShow { get { return activeWindow.RootObject.activeSelf; } }

        /// <summary>テキストの表示が終了しているかどうか</summary>
        public bool IsEndShowText { get { return activeWindow.TextController.IsEndShow; } }

#endregion

#region public methods

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="engine">シナリオの統合管理コンポーネント</param>
        public void Init(AdvMainEngine engine)
        {
            textMode = engine.Settings.EngineSetting.DefaultTextMode;
            string newLineTag = engine.Settings.EngineSetting.AutoAdditiveNewLineTag;

            int activeIndex = mainWindowIndex - 1;
            
            for (int i = 0; i < windows.Count; ++i)
            {
                windows[i].TextController.Init(engine.UIManager.OnEndShowText, newLineTag);
                windows[i].RootObject.SetActive(i == activeIndex);
            }

            if (mainWindowIndex < 1 || mainWindowIndex > windows.Count)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("AdvWindowManager.Awake : MainWindowIndex is invalid!", gameObject);
#endif
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPaused = true;
#endif
                return;
            }

            activeWindow = windows[activeIndex];
        }

        /// <summary>
        /// 初期化前の状態に戻します。
        /// </summary>
        public virtual void Uninit()
        {
            activeWindow = windows[mainWindowIndex - 1];
            textMode = TextMode.Normal;

            for (int i = 0; i < windows.Count; ++i)
            {
                windows[i].TextController.UnInit();
            }
        }

        /// <summary>
        /// アクティブなウィンドウを切り替えます。
        /// </summary>
        /// <param name="index">ウィンドウのインデックス(1~N)</param>
        public void ChangeActive(int index)
        {
            if (index < 1 || index > windows.Count)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("AdvWindowManager.ChangeActive : The index is Invalid!");
#endif
                return;
            }

            activeWindow.RootObject.SetActive(false);
            activeWindow = windows[index - 1];
            activeWindow.RootObject.SetActive(true);
        }

        /// <summary>
        /// テキスト、ラベルを設定します。
        /// </summary>
        /// <param name="テキスト情報">テキスト情報</param>
        public void SetText(AdvWindowTextController.TextInfo info)
        {
            switch (textMode)
            {
                case TextMode.Normal:
                    activeWindow.TextController.SetText(info);
                    break;
                case TextMode.Additive:
                    activeWindow.TextController.AddText(info, false);
                    break;
                case TextMode.AdditiveNewLine:
                    activeWindow.TextController.AddText(info, true);
                    break;
            }
        }

        /// <summary>
        /// 表示間隔(表示速度)を設定します。
        /// </summary>
        /// <param name="speed">表示間隔(表示速度)(0に近づくほど高速)</param>
        public void SetTextCaptionSpeed(float speed)
        {
            foreach (Window window in windows)
            {
                window.TextController.SetCaptionSpeed(speed);
            }
        }

        /// <summary>
        /// テキストの表示モードを設定します。
        /// </summary>
        /// <param name="textMode"></param>
        public void SetTextMode(TextMode textMode)
        {
            this.textMode = textMode;
        }

#endregion
    }
}
