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

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Jigbox.Components
{
    /// <summary>
    /// uGUIのInputFieldを継承したクラス
    /// uGUIのInputFieldでフックしたい内容などや処理をこのクラスで行っている
    /// </summary>
    public class InputFieldSub : InputField
    {
        protected readonly Event processingEvent = new Event();

        /// <summary>キーが入力されたコールバック</summary>
        public Func<Event, bool> KeyPressedCallback { get; set; }

        /// <summary>CanvasRebuildのコールバック</summary>
        public Action RebuildCallback { get; set; }

        /// <summary>キャレットの表示状態</summary>
        public bool CaretVisible
        {
            get { return m_CaretVisible; }
        }

        /// <summary>
        /// InputFieldのキー入力処理をフックしたい為、overrideしている
        /// Event.PopEventは一度行ってしまうとイベントがなくなってしまう為、ここで追加している
        /// Mac版ではSingleLineの時日本語入力できてない問題はUnity側でバグとして報告されている
        /// https://issuetracker.unity3d.com/issues/macos-input-field-clears-chinese-slash-japanese-slash-korean-input-when-pressing-enter
        /// </summary>
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (!isFocused)
            {
                return;
            }

            bool consumeEvent = false;

            while (Event.PopEvent(processingEvent))
            {
                if (processingEvent.rawType == EventType.KeyDown)
                {
                    consumeEvent = true;

                    // キー入力をフックする為追加
                    var isConsumeKeyPressed = false;
                    if (KeyPressedCallback != null)
                    {
                        isConsumeKeyPressed = KeyPressedCallback(processingEvent);
                    }

                    var isFinish = false;
                    if (!isConsumeKeyPressed)
                    {
                        isFinish = KeyPressed(processingEvent) == EditState.Finish;
                    }

                    if (isFinish)
                    {
                        DeactivateInputField();
                        break;
                    }
                }

                switch (processingEvent.type)
                {
                    case EventType.ValidateCommand:
                    case EventType.ExecuteCommand:
                        switch (processingEvent.commandName)
                        {
                            case "SelectAll":
                                SelectAll();
                                consumeEvent = true;
                                break;
                        }

                        break;
                }
            }

            if (consumeEvent)
            {
                UpdateLabel();
            }

            eventData.Use();
        }

        /// <summary>
        /// キャンパスのリビルドに合わせてキャレットの描画を更新している
        /// InputFieldの更新に合わせる為、overrideしてコールバックを追加している
        /// </summary>
        public override void Rebuild(CanvasUpdate update)
        {
            if (update != CanvasUpdate.LatePreRender)
            {
                return;
            }

            if (RebuildCallback != null)
            {
                RebuildCallback.Invoke();
            }
        }
    }
}
