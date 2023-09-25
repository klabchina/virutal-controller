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
using System.Collections;
using System.Collections.Generic;
using Jigbox.Components;
using System;

namespace Jigbox.Mock
{
    /// <summary>
    /// TextView を使って、自動のページング処理や、一文字づつ表示する処理を実装する場合のサンプルです
    /// </summary>
    public class TextViewPager : MonoBehaviour
    {
        #region Properties

        /// <summary>
        /// 対象となるTextViewを指定します
        /// </summary>
        [SerializeField]
        Components.TextView textView;

        /// <summary>
        /// 対象となるTextViewを指定します
        /// </summary>
        public Components.TextView TextView
        {
            get { return textView; }
            set { textView = value; }
        }

        /// <summary>
        /// ページングされた場合、前のページに『戻る』ボタンを指定します
        /// </summary>
        [SerializeField]
        Button prevButton = null;

        /// <summary>
        /// ページングされた場合、次のページに『進む』ボタンを指定します
        /// </summary>
        [SerializeField]
        Button nextButton = null;

        /// <summary>
        /// 文字を1つづつ表示していくかどうかを取得、設定します
        /// </summary>
        [SerializeField]
        bool isIncremental;

        /// <summary>
        /// 文字を1つづつ表示していくかどうかを取得、設定します
        /// </summary>
        /// <value><c>true</c> if this instance is incremental; otherwise, <c>false</c>.</value>
        public bool IsIncremental
        {
            get { return isIncremental; }
            set { isIncremental = value; }
        }

        /// <summary>
        /// <c>true</c> の時 Debug.Log() に、コルーチンや文字の表示領域溢れの情報を出力します
        /// </summary>
        [SerializeField]
        bool enableLogOutput;

        /// <summary>
        /// <c>true</c> の時 Debug.Log() に、コルーチンや文字の表示領域溢れの情報を出力します
        /// </summary>
        public bool IsEnableLogOutput
        {
            get { return enableLogOutput; }
            set { enableLogOutput = value; }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 表示するテキストの行の履歴をスタックで記憶します
        /// </summary>
        Stack<int> historyLineIndex = new Stack<int>();

        /// <summary>
        /// テキストが表示領域からはみ出てしまう場合の、次の行番号を0始まりで表現します
        /// </summary>
        int nextLineIndex;

        /// <summary>
        /// 一文字づつ表示する処理のコルーチンです
        /// </summary>
        IEnumerator incrementalDisplay;

        #region public method

        public void RestartIncrementalDisplay()
        {
            if (incrementalDisplay != null)
            {
                StopCoroutine(incrementalDisplay);
            }
            incrementalDisplay = IncrementalDisplay();
            StartCoroutine(incrementalDisplay);
        }

        void ChangeInteractiveButton(Button button, bool interactable)
        {
            if (button == null)
            {
                return;
            }
            button.interactable = interactable;
        }

        public void ChangeInteractableNextButton(bool interactable)
        {
            ChangeInteractiveButton(nextButton, interactable);
        }

        public void ChangeInteractablePrevButton(bool interactable)
        {
            ChangeInteractiveButton(prevButton, interactable);
        }

        #endregion

        #endregion

        IEnumerator IncrementalDisplay()
        {
            TextView.VisibleLength = 0;
            yield return null;

            var min = TextView.VisibleMinIndex;
            var max = TextView.VisibleMaxIndex;

            if (enableLogOutput)
            {
                Debug.Log(string.Format("Incremental Display : {0} to {1}", min, max));
            }

            for (TextView.VisibleLength = min; TextView.VisibleLength <= max; ++TextView.VisibleLength)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        void PrevTextLine()
        {
            if (TextView == null)
            {
                return;
            }
            if (historyLineIndex.Count <= 0)
            {
                return;
            }
            var prev = historyLineIndex.Pop();
            if (prev == 0)
            {
                ChangeInteractablePrevButton(false);
            }
            TextView.VisibleLineStart = prev;
            if (IsIncremental)
            {
                RestartIncrementalDisplay();
            }
        }

        void NextTextLine()
        {
            if (TextView == null)
            {
                return;
            }
            historyLineIndex.Push(TextView.VisibleLineStart);

            ChangeInteractablePrevButton(true);
            ChangeInteractableNextButton(false);

            TextView.VisibleLineStart = nextLineIndex;
            if (IsIncremental)
            {
                RestartIncrementalDisplay();
            }
        }

        void OnOverflowTextLine(TextViewOverflow e)
        {
            if (enableLogOutput)
            {
                Debug.Log(string.Format(
                    "[TextView] Over flow TextView line: {0}\nEnable view glyphs {1} to {2}",
                    e.OverflowTextLineIndex,
                    e.VisibleGlyphMinIndex,
                    e.VisibleGlyphMaxIndex
                ));
            }

            nextLineIndex = e.OverflowTextLineIndex;
            ChangeInteractableNextButton(true);
        }

        #region Unity Method

        void OnEnable()
        {
            if (prevButton != null)
            {
                prevButton.onClick.AddListener(PrevTextLine);
            }

            if (nextButton != null)
            {
                nextButton.onClick.AddListener(NextTextLine);
            }

            if (TextView != null)
            {
                textView.OnOverflowTextLines.Add(this, "OnOverflowTextLine", (TextViewOverflow) null);
            }
        }

        void OnDisable()
        {
            if (prevButton != null)
            {
                prevButton.onClick.RemoveListener(PrevTextLine);
            }
            if (nextButton != null)
            {
                nextButton.onClick.RemoveListener(NextTextLine);
            }
            if (TextView != null)
            {
                TextView.OnOverflowTextLines.Remove(this, "OnOverflowTextLine");
            }
        }

        // Use this for initialization
        void Start()
        {
            ChangeInteractableNextButton(false);
            if (TextView != null)
            {
                if (TextView.VisibleLineStart == 0)
                {
                    ChangeInteractablePrevButton(false);
                }
                if (IsIncremental)
                {
                    RestartIncrementalDisplay();
                }
            }
        }

        #endregion
    }
}
