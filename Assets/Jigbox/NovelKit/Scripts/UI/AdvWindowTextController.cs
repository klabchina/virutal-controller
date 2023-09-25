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
using System;

namespace Jigbox.NovelKit
{
    public class AdvWindowTextController : MonoBehaviour
    {
#region inner classes, enum, and structs

        /// <summary>
        /// テキスト情報
        /// </summary>
        public class TextInfo
        {
            /// <summary>ラベル</summary>
            public string Label { get; protected set; }

            /// <summary>テキスト</summary>
            public string Text { get; protected set; }

            /// <summary>サウンド情報</summary>
            public string Sound { get; protected set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="label">ラベル</param>
            /// <param name="text">テキスト</param>
            /// <param name="sound">サウンド情報</param>
            public TextInfo(string label, string text, string sound)
            {
                Label = label;
                Text = text;
                Sound = sound;
            }
        }

#endregion

#region properties

        /// <summary>
        /// <para>メインのテキスト</para>
        /// <para>この参照は必ず付いている前提</para>
        /// </summary>
        [SerializeField]
        protected Components.TextView mainText;

        /// <summary>メインのテキスト</summary>
        public Components.TextView MainText { get { return mainText; } }

        /// <summary>
        /// <para>ラベルテキスト</para>
        /// <para>この参照は場合によってはnullとなる可能性がある</para>
        /// </summary>
        [SerializeField]
        protected Components.TextView labelText;

        /// <summary>ラベルテキスト</summary>
        public Components.TextView LabelText { get { return labelText; } }

        /// <summary>クリック待ち用のマーカー</summary>
        [SerializeField]
        protected GameObject clickMarker;

        /// <summary>ラベルが空の場合、自動で非表示するかどうか</summary>
        [SerializeField]
        protected bool isAutoHideLabel;

        /// <summary>ラベルを非表示にする際のラベルのルートとなるオブジェクト</summary>
        [SerializeField]
        protected GameObject labelRoot;

        /// <summary>テキストの表示終了時のコールバック</summary>
        protected Func<bool> onEndShowText = null;
        
        /// <summary>テキストの文字送り速度</summary>
        protected float captionSpeed = 0.0f;

        /// <summary>現在の表示インデックス</summary>
        protected int currentIndex = 0;

        /// <summary>文字列の長さ</summary>
        protected int textLength = 0;

        /// <summary>表示時間の合計</summary>
        protected float totalViewTime = 0.0f;

        /// <summary>現在のインデックスを表示してからの経過時間</summary>
        protected float deltaTime = 0.0f;

        /// <summary>改行タグ</summary>
        protected string newLineTag;

        /// <summary>テキストの表示が終了しているかどうか</summary>
        public bool IsEndShow { get { return currentIndex == textLength; } }
        
#endregion

#region public methods

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="onEndShowText">テキストの表示終了時のコールバック</param>
        /// <param name="newLineTag">改行タグ</param>
        public virtual void Init(Func<bool> onEndShowText, string newLineTag)
        {
            this.onEndShowText = onEndShowText;
            this.newLineTag = newLineTag;

            mainText.Text = string.Empty;
            mainText.VisibleLength = 0;
            mainText.raycastTarget = false;
            if (labelText != null)
            {
                labelText.Text = string.Empty;
                labelText.VisibleLength = -1;
                labelText.raycastTarget = false;
                if (isAutoHideLabel)
                {
                    labelRoot.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 初期化前の状態に戻します。
        /// </summary>
        public virtual void UnInit()
        {
            onEndShowText = null;
            captionSpeed = 0.0f;
            currentIndex = 0;
            textLength = 0;
            totalViewTime = 0.0f;
            deltaTime = 0.0f;
            newLineTag = string.Empty;
            
            mainText.Text = string.Empty;
            mainText.VisibleLength = 0;
            mainText.raycastTarget = false;

            if (labelText != null)
            {
                labelText.Text = string.Empty;
                labelText.VisibleLength = -1;
                labelText.raycastTarget = false;
            }
        }

        /// <summary>
        /// テキスト、ラベルを設定します。
        /// </summary>
        /// <param name="info">テキスト情報</param>
        public virtual void SetText(TextInfo info)
        {
            mainText.Text = info.Text;
            textLength = mainText.MaxIndex + 1;
            totalViewTime = captionSpeed * textLength;
            currentIndex = 0;
            deltaTime = 0.0f;

            if (!SetTextA(info))
            {
                mainText.VisibleLength = 0;
            }
        }

        /// <summary>
        /// テキストを追加します。
        /// </summary>
        /// <param name="info">テキスト情報</param>
        /// <param name="isNewLine">改行するかどうか</param>
        public virtual void AddText(TextInfo info, bool isNewLine)
        {
            int lastTextLength;
            if (isNewLine && !string.IsNullOrEmpty(mainText.Text))
            {
                lastTextLength = textLength + 1;
                mainText.Text = mainText.Text + newLineTag + info.Text;
            }
            else
            {
                lastTextLength = textLength;
                mainText.Text += info.Text;
            }
            textLength = mainText.MaxIndex + 1;
            deltaTime = lastTextLength * captionSpeed;
            totalViewTime = captionSpeed * textLength;

            if (!SetTextA(info))
            {
                if (currentIndex == 0)
                {
                    mainText.VisibleLength = 0;
                }
            }
        }

        /// <summary>
        /// テキストをクリアします。
        /// </summary>
        public virtual void ClearText()
        {
            mainText.VisibleLength = 0;
            mainText.Text = string.Empty;
            if (labelText != null)
            {
                labelText.Text = string.Empty;
            }
            if (isAutoHideLabel)
            {
                labelRoot.SetActive(false);
            }
            if (clickMarker != null)
            {
                clickMarker.SetActive(false);
            }
            textLength = 0;
            currentIndex = 0;
            totalViewTime = 0.0f;
        }

        /// <summary>
        /// 全ての文言を表示します。
        /// </summary>
        public void ShowTextAll()
        {
            if (IsEndShow)
            {
                return;
            }

            currentIndex = textLength;
            deltaTime = 0.0f;
            EndShowText();
        }

        /// <summary>
        /// テキストの文字送り速度を設定します。
        /// </summary>
        /// <param name="speed">テキストの文字送り速度(0に近づくほど高速)</param>
        public void SetCaptionSpeed(float speed)
        {
            captionSpeed = speed > 0.0f ? speed : 0.0f;
        }

#endregion

#region protected methods

        /// <summary>
        /// テキストの共通部を設定します。
        /// </summary>
        /// <param name="info"></param>
        /// <returns>即座に全表示するかどうか</returns>
        protected virtual bool SetTextA(TextInfo info)
        {
            if (labelText != null && labelText.Text != info.Label)
            {
                labelText.Text = info.Label;
                if (isAutoHideLabel && string.IsNullOrEmpty(info.Label))
                {
                    labelRoot.SetActive(false);
                }
                else if (!labelRoot.activeSelf)
                {
                    labelRoot.SetActive(true);
                }
            }

            bool isAllView = captionSpeed <= 0.0f;
            
            // 表示間隔が0の場合は、即座に全ての文言を表示
            if (isAllView)
            {
                currentIndex = textLength;
                EndShowText();
            }
            else
            {
                if (clickMarker != null)
                {
                    clickMarker.SetActive(false);
                }
            }

            return isAllView;
        }

        /// <summary>
        /// テキストの表示を全て終了した状態にします。
        /// </summary>
        protected virtual void EndShowText()
        {
            mainText.VisibleLength = textLength;
            bool showMarker = true;
            if (onEndShowText != null)
            {
                showMarker = onEndShowText();
            }
            if (clickMarker != null && showMarker)
            {
                clickMarker.SetActive(true);
            }
        }

#endregion

#region override unity methods

        protected virtual void Update()
        {
            if (currentIndex == textLength)
            {
                return;
            }

            deltaTime += Time.deltaTime;
            float percentage = deltaTime / (totalViewTime > 0 ? totalViewTime : 1);
            percentage = percentage > 1.0f ? 1.0f : percentage;
            int index = (int) (textLength * percentage);
            if (currentIndex != index)
            {
                currentIndex = index;
                mainText.VisibleLength = currentIndex;

                if (currentIndex == textLength)
                {
                    EndShowText();
                }
            }
        }

#endregion
    }
}
