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
using UnityEngine.UI;
using System.Collections.Generic;
using Jigbox.Components;
using Jigbox.Mock;
using Jigbox.TextView;

namespace Jigbox.Examples
{
    public class TextViewSample : ExampleSceneBase
    {
#region properties

        [SerializeField]
        TextAsset[] textAssets = null;

        [SerializeField]
        Text languageText = null;

        [SerializeField]
        Text sampleTextName = null;

        [SerializeField]
        Components.TextView textView = null;

        [SerializeField]
        TextViewPager pager = null;

        [SerializeField]
        bool isCaptureScreenShot = false;

        [SerializeField]
        BasicButton captureButton = null;

        [SerializeField]
        Image textWindowImage = null;

        public Image TextWindowImage
        {
            get { return this.textWindowImage; }
        }

        [SerializeField]
        UnityEngine.UI.Outline textWindowOutline = null;

        public UnityEngine.UI.Outline TextWindowOutline
        {
            get { return this.textWindowOutline; }
        }

        [SerializeField]
        Font defaultFont = null;
        
        [SerializeField]
        Font thaiFont = null;

        [SerializeField] 
        Text alignModeText;

        int ruleMaxNum;
        int rule;

        int sampleTextIndex;

        List<string> sampleTextNames = new List<string>();
        string[] sampleTexts;

        /// <summary>タイ語のテキストが入っているIndex</summary>
        static readonly int ThaiTextIndex = 17;

        /// <summary>タイ語のテキストが入っているIndex</summary>
        static readonly int ArabicTextIndex = 18;

        /// <summary>FPSが60無いとスクショ撮影が正しく行われない為</summary>
        protected override int TargetFrameRate
        {
            get { return 60; }
        }

#endregion

#region public methods

        public void SetText(int index)
        {
            OnSelectedSampleTextIndex(index);
        }

        public void ModifyTextViewOptions(Action<Components.TextView> modifier)
        {
            textView.BeginUpdateProperties();
            {
                modifier(textView);
            }
            textView.EndUpdateProperties();

        }

        public void SetTextViewOverflow(Components.TextView.TextOverflowDelegate.Callback callback)
        {
            textView.AddOverflowEvent(callback);
        }

#endregion

#region private methods

        void LoadSampleTexts()
        {
            if (this.textAssets == null)
            {
                return;
            }

            this.sampleTextNames.Clear();
            var texts = new List<string>();
            foreach (var asset in this.textAssets)
            {
                this.sampleTextNames.Add(asset.name.Replace("\\.txt", ""));
                texts.Add(asset.text.Replace("^\\s*|((\\r|\\n)*\\s*$)", ""));
            }

            this.sampleTexts = texts.ToArray();
        }

        void OnSelectedSampleTextIndex(int index)
        {
            // テキストを差し替える前にページャーの【戻る】【進む】ボタンを無効化する
            if (this.pager != null)
            {
                pager.ChangeInteractableNextButton(false);
                pager.ChangeInteractablePrevButton(false);
            }

            // テキストを差し替える
            if (this.textView != null)
            {
                var nextIndex = index;
                if (nextIndex < 0)
                {
                    nextIndex = this.sampleTexts.Length - 1;
                }
                else if (nextIndex >= this.sampleTexts.Length)
                {
                    nextIndex = 0;
                }
                this.sampleTextIndex = nextIndex;

                this.textView.Text = this.sampleTexts[this.sampleTextIndex];
                this.sampleTextName.text = this.sampleTextNames[this.sampleTextIndex];
                this.textView.LanguageType = this.sampleTextIndex == ThaiTextIndex ? TextLanguageType.Thai : TextLanguageType.Default;
                this.textView.LanguageType = this.sampleTextIndex == ArabicTextIndex
                    ? TextLanguageType.Arabic
                    : this.textView.LanguageType;
                this.textView.Font = this.sampleTextIndex == ThaiTextIndex ? thaiFont :defaultFont;
            }

            // ページャーが文字送り有効であれば、はじめからやりなおす
            if (this.pager != null && this.pager.IsIncremental)
            {
                this.pager.RestartIncrementalDisplay();
            }
        }

        void OnLanguageIndex(int index)
        {
            this.textView.LineBreakRule = (TextLineBreakRule) Enum.ToObject(typeof(TextLineBreakRule), index);
        }

        [AuthorizedAccess]
        void OnNextLineBreakRule()
        {
            ChangeLineBreakRule(rule + 1);
        }

        void ChangeLineBreakRule(int index)
        {
            rule = index >= ruleMaxNum ? 0 : index;
            this.textView.LineBreakRule = (TextLineBreakRule) Enum.ToObject(typeof(TextLineBreakRule), rule);
            this.languageText.text = this.textView.LineBreakRule.ToString();
        }

        [AuthorizedAccess]
        void OnNextSampleText()
        {
            OnSelectedSampleTextIndex(this.sampleTextIndex + 1);
        }

        [AuthorizedAccess]
        void OnPrevSampleText()
        {
            OnSelectedSampleTextIndex(this.sampleTextIndex - 1);
        }

        [AuthorizedAccess]
        void OnAlignModeChanged(bool isFontMode)
        {
            this.textView.AlignMode = isFontMode ? TextAlignMode.Font : TextAlignMode.Placement;
            this.alignModeText.text = "AlignMode : " + string.Format(isFontMode ? "Font" : "Placement");

        }

#endregion

#region override unity methods

        // Use this for initialization
        void Start()
        {
            this.LoadSampleTexts();

            this.ruleMaxNum = Enum.GetValues(typeof(TextLineBreakRule)).Length;

            ChangeLineBreakRule((int)TextLineBreakRule.Japanese);

            OnSelectedSampleTextIndex(0);

#if UNITY_EDITOR
            if (isCaptureScreenShot)
            {
                Camera camera = textView.canvas.worldCamera;
                if (camera == null)
                {
                    captureButton.gameObject.SetActive(false);
                    Debug.LogError("Can't Capture, because camera doesn't set in canvas!");
                    return;
                }
                TextViewScreenShotCapture capture = camera.gameObject.AddComponent<TextViewScreenShotCapture>();
                capture.Init(this, captureButton, textAssets.Length);
            }
            else
            {
                captureButton.gameObject.SetActive(false);
            }
#else
            captureButton.gameObject.SetActive(false);
#endif
        }

#endregion
    }
}
