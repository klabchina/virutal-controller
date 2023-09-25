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
using Jigbox.Components;
using Jigbox.Tween;

namespace Jigbox.Examples
{
    public sealed class ExamplePopupCustomTransition : PopupTransitionBase
    {
#region properties

        [SerializeField]
        GameObject[] viewObjects = null;

        [SerializeField]
        Transform barContainer = null;

        [SerializeField]
        CanvasGroup canvasGroup = null;

        [SerializeField]
        Transform headerBar = null;

        [SerializeField]
        Transform footerBar = null;

        [SerializeField]
        Transform bg = null;

        TweenVector3 barScaleTween = new TweenVector3();

        TweenSingle alphaTween = new TweenSingle();

        TweenVector3 headerMoveTween = new TweenVector3();

        TweenVector3 footerMoveTween = new TweenVector3();

        TweenVector3 bgScaleTween = new TweenVector3();

#endregion

#region public methods

        /// <summary>
        /// ポップアップを開きます。
        /// </summary>
        public override void Open()
        {
            SetOpenParameter();
            ClearCompleteCallback();

            barScaleTween.OnComplete(OnCompleteOpenBarScale);
            bgScaleTween.OnComplete(OnCompleteOpenBg);

            NotifyOnBeginOpen();

            barScaleTween.Start();
            alphaTween.Start();
        }

        /// <summary>
        /// ポップアップを閉じます。
        /// </summary>
        public override void Close()
        {
            SetCloseParameter();
            ClearCompleteCallback();

            bgScaleTween.OnComplete(OnCompleteCloseBg);
            barScaleTween.OnComplete(OnCompleteCloseBarScale);

            NotifyOnBeginClose();

            ShowViewObjects(false);

            headerMoveTween.Start();
            footerMoveTween.Start();
            bgScaleTween.Start();
        }

#endregion

#region private methods

        void SetOpenParameter()
        {
            barScaleTween.Begin = new Vector3(0.0f, 1.0f, 1.0f);
            barScaleTween.Final = Vector3.one;

            alphaTween.Begin = 0.0f;
            alphaTween.Final = 1.0f;

            headerMoveTween.Begin = new Vector3(0.0f, 14.0f, 0.0f);
            headerMoveTween.Final = new Vector3(0.0f, 120.0f, 0.0f);

            footerMoveTween.Begin = new Vector3(0.0f, -14.0f, 0.0f);
            footerMoveTween.Final = new Vector3(0.0f, -120.0f, 0.0f);

            bgScaleTween.Begin = new Vector3(1.0f, 0.0f, 1.0f);
            bgScaleTween.Final = Vector3.one;
        }

        void SetCloseParameter()
        {
            barScaleTween.Begin = Vector3.one;
            barScaleTween.Final = new Vector3(0.0f, 1.0f, 1.0f);

            alphaTween.Begin = 1.0f;
            alphaTween.Final = 0.0f;

            headerMoveTween.Begin = new Vector3(0.0f, 120.0f, 0.0f);
            headerMoveTween.Final = new Vector3(0.0f, 14.0f, 0.0f);

            footerMoveTween.Begin = new Vector3(0.0f, -120.0f, 0.0f);
            footerMoveTween.Final = new Vector3(0.0f, -14.0f, 0.0f);

            bgScaleTween.Begin = Vector3.one;
            bgScaleTween.Final = new Vector3(1.0f, 0.0f, 1.0f);
        }

        void ClearCompleteCallback()
        {
            barScaleTween.RemoveAllOnComplete();
            alphaTween.RemoveAllOnComplete();
            headerMoveTween.RemoveAllOnComplete();
            footerMoveTween.RemoveAllOnComplete();
            bgScaleTween.RemoveAllOnComplete();
        }

        void ForceCompleteAll()
        {
            barScaleTween.Complete();
            alphaTween.Complete();
            headerMoveTween.Complete();
            footerMoveTween.Complete();
            bgScaleTween.Complete();
        }

        void OnCompleteOpenBarScale(ITween<Vector3> tween)
        {
            headerMoveTween.Start();
            footerMoveTween.Start();
            bgScaleTween.Start();
        }

        void OnCompleteOpenBg(ITween<Vector3> tween)
        {
            ShowViewObjects(true);
            NotifyOnCompleteOpen();
        }

        void OnCompleteCloseBg(ITween<Vector3> tween)
        {
            barScaleTween.Start();
            alphaTween.Start();
        }

        void OnCompleteCloseBarScale(ITween<Vector3> tween)
        {
            // MissingReferenceException出てウザイので殺しておく
            barScaleTween.Kill();
            alphaTween.Kill();
            headerMoveTween.Kill();
            footerMoveTween.Kill();
            bgScaleTween.Kill();

            NotifyOnCompleteClose();
        }

        void ShowViewObjects(bool visible)
        {
            foreach (GameObject gameObject in viewObjects)
            {
                gameObject.SetActive(visible);
            }
        }

#endregion

#region override unity methods

        void Awake()
        {
            barScaleTween.Duration = 0.2f;
            barScaleTween.OnUpdate(t => barContainer.localScale = t.Value);
            alphaTween.Duration = 0.2f;
            alphaTween.OnUpdate(t => canvasGroup.alpha = t.Value);

            headerMoveTween.Duration = 0.1f;
            headerMoveTween.OnUpdate(t => headerBar.localPosition = t.Value);
            footerMoveTween.Duration = 0.1f;
            footerMoveTween.OnUpdate(t => footerBar.localPosition = t.Value);
            bgScaleTween.Duration = 0.1f;
            bgScaleTween.OnUpdate(t => bg.localScale = t.Value);

            ShowViewObjects(false);
            // 一旦全部閉じる
            ForceCompleteAll();            
        }

#endregion
    }
}
