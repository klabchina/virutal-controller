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

using Jigbox.Components;
using UnityEngine;

namespace Jigbox.Examples
{
    public class MarqueeExampleController : ExampleSceneBase
    {
        [SerializeField]
        GameObject canvasRoot = null;

        [SerializeField]
        GameObject horizontalRoot = null;

        [SerializeField]
        GameObject verticalRoot = null;

        Marquee[] marquees;

        protected override void Awake()
        {
            base.Awake();
            marquees = canvasRoot.GetComponentsInChildren<Marquee>(true);
            OnActiveHorizontal();
        }

        [AuthorizedAccess]
        void StartMarquee()
        {
            foreach (var marquee in marquees)
            {
                marquee.StartTransition();
            }
        }

        [AuthorizedAccess]
        void KillMarquee()
        {
            foreach (var marquee in marquees)
            {
                marquee.KillTransition();
            }
        }

        [AuthorizedAccess]
        void PauseMarquee()
        {
            foreach (var marquee in marquees)
            {
                marquee.PauseTransition();
            }
        }

        [AuthorizedAccess]
        void ResumeMarquee()
        {
            foreach (var marquee in marquees)
            {
                marquee.ResumeTransition();
            }
        }

        [AuthorizedAccess]
        void OnStartTransition(Marquee marquee)
        {
            Debug.Log("OnStartTransition");
        }

        [AuthorizedAccess]
        void OnKillTransition(Marquee marquee)
        {
            Debug.Log("OnKillTransitoin");
        }

        [AuthorizedAccess]
        void OnCompleteDurationDelay(Marquee marquee)
        {
            Debug.Log("OnCompleteDurationDelay");
        }

        [AuthorizedAccess]
        void OnCompleteDuration(Marquee marquee)
        {
            Debug.Log("OnCompleteDuratoin");
        }
        
        [AuthorizedAccess]
        void OnCompleteEntranceDelay(Marquee marquee)
        {
            Debug.Log("OnCompleteEntranceDelay");
        }

        [AuthorizedAccess]
        void OnCompleteEntranceDuration(Marquee marquee)
        {
            Debug.Log("OnCompleteEntranceDuratoin");
        }
        
        [AuthorizedAccess]
        void OnCompleteExitDelay(Marquee marquee)
        {
            Debug.Log("OnCompleteExitDelay");
        }

        [AuthorizedAccess]
        void OnCompleteExitDuration(Marquee marquee)
        {
            Debug.Log("OnCompleteExitDuratoin");
        }

        [AuthorizedAccess]
        void OnCompleteInterval(Marquee marquee)
        {
            Debug.Log("OnCompleteInterval");
        }

        [AuthorizedAccess]
        void OnCompleteLoopDurationDelay(Marquee marquee)
        {
            Debug.Log("OnCompleteLoopDurationDelay");
        }

        [AuthorizedAccess]
        void OnCompleteLayoutContent(Marquee marquee, bool isScroll)
        {
            Debug.Log("OnCompleteLayoutContent IsScroll " + isScroll);
        }

        [AuthorizedAccess]
        void OnPause(Marquee marquee)
        {
            Debug.Log("OnPause");
        }

        [AuthorizedAccess]
        void OnResume(Marquee marquee)
        {
            Debug.Log("OnResume");
        }

        [AuthorizedAccess]
        void OnActiveHorizontal()
        {
            horizontalRoot.SetActive(true);
            verticalRoot.SetActive(false);
        }

        [AuthorizedAccess]
        void OnActiveVertical()
        {
            verticalRoot.SetActive(true);
            horizontalRoot.SetActive(false);
        }
        
        [AuthorizedAccess]
        void OnNormalDirection()
        {
            foreach (var marquee in marquees)
            {
                marquee.ScrollDirectionType = MarqueeScrollDirectionType.Normal;
                marquee.StartTransition();
            }
        }
        
        [AuthorizedAccess]
        void OnReverseDirection()
        {
            foreach (var marquee in marquees)
            {
                marquee.ScrollDirectionType = MarqueeScrollDirectionType.Reverse;
                marquee.StartTransition();
            }
        }
    }
}
