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
using Jigbox.SceneTransition;

namespace Jigbox.Examples
{
    public sealed class ExampleBackKeyTargetPopup : MonoBehaviour, IBackKeyNoticeTarget
    {
#region properties

        [SerializeField]
        int priority = 1;

        public int Priority { get { return priority; } }

        public bool Enable { get { return isActiveAndEnabled && isOpen; } }

        [SerializeField]
        TweenScale tween = null;

        [SerializeField]
        GameObject mask = null;

        bool isOpen = false;

        bool isTransition = false;

#endregion

#region public methods

        public void OnEscape()
        {
            Close();
        }

#endregion

#region private methods

        [AuthorizedAccess]
        void Open()
        {
            if (isOpen || isTransition)
            {
                return;
            }

            priority = BackKeyManager.Instance.GetPriorityMax() + 1;
            tween.OnComplete.Clear();
            tween.OnComplete.Add(OnCompleteOpen);
            tween.Tween.Start();
            mask.SetActive(true);
        }

        [AuthorizedAccess]
        void Close()
        {
            if (!isOpen || isTransition)
            {
                return;
            }

            tween.OnComplete.Clear();
            tween.OnComplete.Add(OnCompleteClose);
            tween.Tween.Start();
        }
        
        void OnCompleteOpen()
        {
            isOpen = true;

            Vector3 temp = tween.Tween.Begin;
            tween.Tween.Begin = tween.Tween.Final;
            tween.Tween.Final = temp;
        }

        void OnCompleteClose()
        {
            isOpen = false;
            mask.SetActive(false);

            Vector3 temp = tween.Tween.Begin;
            tween.Tween.Begin = tween.Tween.Final;
            tween.Tween.Final = temp;
        }

#endregion

#region override unity methods

        void Awake()
        {
            BackKeyManager.Instance.RegisterNoticeTarget(this);
            tween.Tween.OnStart(_ => isTransition = true);
            tween.Tween.OnComplete(_ => isTransition = false);
        }

        void OnDestroy()
        {
            BackKeyManager.Instance.UnregisterNoticeTarget(this);
        }

#endregion
    }
}
