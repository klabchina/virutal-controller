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

using Jigbox.Delegatable;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    public class Marquee : MonoBehaviour, IMarqueeTransitionHandler
    {
#region Inner Class

        public class MarqueeDelegate : EventDelegate<Marquee>
        {
            public MarqueeDelegate(Callback callback) : base(callback)
            {
            }
        }

        public class MarqueeLayoutDelegate : EventDelegate<Marquee, bool>
        {
            public MarqueeLayoutDelegate(Callback callback) : base(callback)
            {
            }
        }

#endregion

#region fields

        /// <summary>
        /// LayoutGroupへの参照
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected HorizontalOrVerticalLayoutGroup layoutGroup;

        [SerializeField]
        [HideInInspector]
        protected MarqueeTransitionBase transition;

        /// <summary>
        /// トランジションに必要な情報を持たせるためのモデルクラス
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected MarqueeTransitionProperty transitionProperty;

        [SerializeField]
        [HideInInspector]
        protected CanvasGroup canvasGroup;

        /// <summary>
        /// Start実行時に自動でトランジションの開始を行うかどうか
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected bool playOnStart;

        [SerializeField]
        [HideInInspector]
        protected DelegatableList onUpdateContent = new DelegatableList();

        [SerializeField]
        [HideInInspector]
        protected DelegatableList onStartTransition = new DelegatableList();

        [FormerlySerializedAs("onCompleteStartDelay")]
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onCompleteDurationDelay = new DelegatableList();

        [SerializeField]
        [HideInInspector]
        protected DelegatableList onCompleteDuration = new DelegatableList();
        
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onCompleteEntranceDelay = new DelegatableList();

        [SerializeField]
        [HideInInspector]
        protected DelegatableList onCompleteEntranceDuration = new DelegatableList();
        
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onCompleteExitDelay = new DelegatableList();

        [SerializeField]
        [HideInInspector]
        protected DelegatableList onCompleteExitDuration = new DelegatableList();

        [SerializeField]
        [HideInInspector]
        protected DelegatableList onCompleteInterval = new DelegatableList();

        [FormerlySerializedAs("onCompleteLoopStartDelay")]
        [SerializeField]
        [HideInInspector]
        protected DelegatableList onCompleteLoopDurationDelay = new DelegatableList();

        [SerializeField]
        [HideInInspector]
        protected DelegatableList onCompleteLayoutContent = new DelegatableList();

        [SerializeField]
        [HideInInspector]
        protected DelegatableList onPause = new DelegatableList();

        [SerializeField]
        [HideInInspector]
        protected DelegatableList onResume = new DelegatableList();

        [SerializeField]
        [HideInInspector]
        protected DelegatableList onKillTransition = new DelegatableList();

        IMarqueeView cachedMarqueeView;

#endregion

#region properties

        protected virtual IMarqueeView MarqueeView
        {
            get
            {
                if (layoutGroup == null)
                {
#if UNITY_EDITOR
                    Debug.LogErrorFormat("{0} : layoutGroup is null.", gameObject.name);
#endif
                    return null;
                }

                if (cachedMarqueeView == null)
                {
                    cachedMarqueeView = layoutGroup.GetComponent<IMarqueeView>();
                }

                return cachedMarqueeView;
            }
        }

        public virtual MarqueeScrollType ScrollType { get { return transitionProperty.ScrollType; } set { transitionProperty.ScrollType = value; } }

        public virtual MarqueeScrollDirectionType ScrollDirectionType { get { return transitionProperty.ScrollDirectionType; } set { transitionProperty.ScrollDirectionType = value; } }
        
        public virtual float Speed { get { return transitionProperty.Speed; } set { transitionProperty.Speed = value; } }

        public virtual float DurationDelay { get { return transitionProperty.DurationDelay; } set { transitionProperty.DurationDelay = value; } }

        public virtual float Interval { get { return transitionProperty.Interval; } set { transitionProperty.Interval = value; } }

        public virtual float LoopDurationDelay { get { return transitionProperty.LoopDurationDelay; } set { transitionProperty.LoopDurationDelay = value; } }

        public virtual float StartPositionRate { get { return transitionProperty.StartPositionRate; } set { transitionProperty.StartPositionRate = value; } }

        public virtual float EndPositionRate { get { return transitionProperty.EndPositionRate; } set { transitionProperty.EndPositionRate = value; } }

        public virtual bool IsLoop { get { return transitionProperty.IsLoop; } set { transitionProperty.IsLoop = value; } }

        public virtual bool PlayOnStart { get { return playOnStart; } set { playOnStart = value; } }

        public virtual RectTransform ViewPort { get { return transitionProperty.Viewport; } set { transitionProperty.Viewport = value; } }

        public virtual Vector2 ViewportSize { get { return transitionProperty.ViewportSize; } }

        public virtual MarqueeAnimationProperty EntranceAnimationProperty { get { return transitionProperty.EntranceAnimationProperty; } set { transitionProperty.EntranceAnimationProperty = value; } }
        
        public virtual MarqueeAnimationProperty ExitAnimationProperty { get { return transitionProperty.ExitAnimationProperty; } set { transitionProperty.ExitAnimationProperty = value; } }

        public DelegatableList OnUpdateContentDelegates { get { return onUpdateContent; } }

        public DelegatableList OnStartTransitionDelegates { get { return onStartTransition; } }

        public DelegatableList OnCompleteDurationDelayDelegates { get { return onCompleteDurationDelay; } }

        public DelegatableList OnCompleteDurationDelegates { get { return onCompleteDuration; } }

        public DelegatableList OnCompleteEntranceDelayDelegates { get { return onCompleteEntranceDelay; } }

        public DelegatableList OnCompleteEntranceDurationDelegates { get { return onCompleteEntranceDuration; } }

        public DelegatableList OnCompleteExitDelayDelegates { get { return onCompleteExitDelay; } }

        public DelegatableList OnCompleteExitDurationDelegates { get { return onCompleteExitDuration; } }
        
        public DelegatableList OnCompleteIntervalDelegates { get { return onCompleteInterval; } }

        public DelegatableList OnCompleteLoopDurationDelayDelegates { get { return onCompleteLoopDurationDelay; } }

        public DelegatableList OnCompleteLayoutContentDelegates { get { return onCompleteLayoutContent; } }

        public DelegatableList OnPauseDelegates { get { return onPause; } }

        public DelegatableList OnResumeDelegates { get { return onResume; } }

        public DelegatableList OnKillTransitionDelegates { get { return onKillTransition; } }

#endregion

#region public methods

        public virtual void OnStartTransition()
        {
            if (OnStartTransitionDelegates.Count > 0)
            {
                OnStartTransitionDelegates.Execute(this);
            }

            if (OnUpdateContentDelegates.Count > 0)
            {
                OnUpdateContentDelegates.Execute(this);
            }

            if (MarqueeView != null)
            {
                // Layoutの計算が一度は実行されるようにし、Transition側のOnCompleteLayoutが呼び出されるようにします
                MarqueeView.MarkLayoutForRebuild();
            }

            HideView();
        }

        public virtual void OnCompleteLayout()
        {
            ShowView();
        }

        public virtual void OnCompleteDurationDelay()
        {
            if (OnCompleteDurationDelayDelegates.Count > 0)
            {
                OnCompleteDurationDelayDelegates.Execute(this);
            }
        }

        public virtual void OnCompleteDuration()
        {
            if (OnCompleteDurationDelegates.Count > 0)
            {
                OnCompleteDurationDelegates.Execute(this);
            }
        }
        
        public virtual void OnCompleteEntranceDelay()
        {
            if (OnCompleteEntranceDelayDelegates.Count > 0)
            {
                OnCompleteEntranceDelayDelegates.Execute(this);
            }
        }

        public virtual void OnCompleteEntranceDuration()
        {
            if (OnCompleteEntranceDurationDelegates.Count > 0)
            {
                OnCompleteEntranceDurationDelegates.Execute(this);
            }
        }
        
        public virtual void OnCompleteExitDelay()
        {
            if (OnCompleteExitDelayDelegates.Count > 0)
            {
                OnCompleteExitDelayDelegates.Execute(this);
            }
        }

        public virtual void OnCompleteExitDuration()
        {
            if (OnCompleteExitDurationDelegates.Count > 0)
            {
                OnCompleteExitDurationDelegates.Execute(this);
            }
        }

        public virtual void OnCompleteInterval()
        {
            if (OnCompleteIntervalDelegates.Count > 0)
            {
                OnCompleteIntervalDelegates.Execute(this);
            }
        }

        public virtual void OnCompleteLoopDurationDelay()
        {
            if (OnCompleteLoopDurationDelayDelegates.Count > 0)
            {
                OnCompleteLoopDurationDelayDelegates.Execute(this);
            }
        }

        public virtual void OnCompleteLayoutContent(bool isScroll)
        {
            if (OnCompleteLayoutContentDelegates.Count > 0)
            {
                OnCompleteLayoutContentDelegates.Execute(this, isScroll);
            }
        }

        public virtual void OnPause()
        {
            if (OnPauseDelegates.Count > 0)
            {
                OnPauseDelegates.Execute(this);
            }
        }

        public virtual void OnResume()
        {
            if (OnResumeDelegates.Count > 0)
            {
                OnResumeDelegates.Execute(this);
            }
        }

        public virtual void OnKillTransition()
        {
            if (OnKillTransitionDelegates.Count > 0)
            {
                OnKillTransitionDelegates.Execute(this);
            }
        }

        public virtual void OnLoopStart()
        {
            if (OnUpdateContentDelegates.Count > 0)
            {
                OnUpdateContentDelegates.Execute(this);
            }

            if (MarqueeView != null)
            {
                // Layoutの計算が一度は実行されるようにし、Transition側のOnCompleteLayoutが呼び出されるようにします
                MarqueeView.MarkLayoutForRebuild();
            }

            HideView();
        }

        public virtual void OnCompleteLoopLayout()
        {
            ShowView();
        }

        /// <summary>
        /// Contentの更新タイミングのコールバックを設定する
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddUpdateContentEvent(MarqueeDelegate.Callback callback)
        {
            onUpdateContent.Add(new MarqueeDelegate(callback));
        }

        /// <summary>
        /// トランジション開始時のコールバックを設定する
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddStartTransitionEvent(MarqueeDelegate.Callback callback)
        {
            onStartTransition.Add(new MarqueeDelegate(callback));
        }

        /// <summary>
        /// トランジションを開始して、動き出すまでの最初の遅延終了時のコールバックを設定する
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddCompleteDurationDelayEvent(MarqueeDelegate.Callback callback)
        {
            onCompleteDurationDelay.Add(new MarqueeDelegate(callback));
        }

        /// <summary>
        /// トランジションによる移動終了時のコールバックを設定する
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddCompleteDurationEvent(MarqueeDelegate.Callback callback)
        {
            onCompleteDuration.Add(new MarqueeDelegate(callback));
        }
        
        /// <summary>
        /// トランジションを開始して、動き出すまでの最初の遅延終了時のコールバックを設定する
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddCompleteEntranceDelayEvent(MarqueeDelegate.Callback callback)
        {
            this.onCompleteEntranceDelay.Add(new MarqueeDelegate(callback));
        }

        /// <summary>
        /// トランジションによる移動終了時のコールバックを設定する
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddCompleteEntranceDurationEvent(MarqueeDelegate.Callback callback)
        {
            onCompleteEntranceDuration.Add(new MarqueeDelegate(callback));
        }
        
        /// <summary>
        /// トランジションを開始して、動き出すまでの最初の遅延終了時のコールバックを設定する
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddCompleteExitDelayEvent(MarqueeDelegate.Callback callback)
        {
            onCompleteExitDelay.Add(new MarqueeDelegate(callback));
        }

        /// <summary>
        /// トランジションによる移動終了時のコールバックを設定する
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddCompleteExitDurationEvent(MarqueeDelegate.Callback callback)
        {
            onCompleteExitDuration.Add(new MarqueeDelegate(callback));
        }

        /// <summary>
        /// トランジションによる移動後、指定された待機時間終了時のコールバックを設定する
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddCompleteIntervaEvent(MarqueeDelegate.Callback callback)
        {
            onCompleteInterval.Add(new MarqueeDelegate(callback));
        }

        /// <summary>
        /// トランジションが二回目以降の移動を開始する前の遅延終了時のコールバックを設定する
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddCompleteLoopDurationDelayEvent(MarqueeDelegate.Callback callback)
        {
            onCompleteLoopDurationDelay.Add(new MarqueeDelegate(callback));
        }

        /// <summary>
        /// スクロールするかどうかが確定したタイミングで呼ばれるコールバックを設定する
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddCompleteLayoutContent(MarqueeLayoutDelegate.Callback callback)
        {
            onCompleteLayoutContent.Add(new MarqueeLayoutDelegate(callback));
        }

        /// <summary>
        /// 一時停止用コールバックを設定する
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddPauseEvent(MarqueeDelegate.Callback callback)
        {
            onPause.Add(new MarqueeDelegate(callback));
        }

        /// <summary>
        /// 再開用コールバックを設定する
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddResumeEvent(MarqueeDelegate.Callback callback)
        {
            onResume.Add(new MarqueeDelegate(callback));
        }

        /// <summary>
        /// Kill用コールバックを設定する
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddKillTransitionEvent(MarqueeDelegate.Callback callback)
        {
            onKillTransition.Add(new MarqueeDelegate(callback));
        }

        /// <summary>
        /// 移動開始位置にContainerを移動させます
        /// 注意：Layoutの計算のタイミング次第では結果が変わる場合があります
        /// </summary>
        public virtual void InitPosition()
        {
            if (transition != null)
            {
                transition.InitPosition();
            }
        }

        /// <summary>
        /// トランジションを開始させます
        /// </summary>
        public virtual void StartTransition()
        {
            if (transition != null)
            {
                transition.StartMovement();
            }
        }

        /// <summary>
        /// トランジションを強制停止します
        /// </summary>
        public virtual void KillTransition()
        {
            if (transition != null)
            {
                transition.Kill();
            }
        }

        /// <summary>
        /// トランジションを一時停止します　
        /// </summary>
        public virtual void PauseTransition()
        {
            if (transition != null)
            {
                transition.PauseMovement();
            }
        }

        /// <summary>
        /// トランジションの再開をします
        /// </summary>
        public virtual void ResumeTransition()
        {
            if (transition != null)
            {
                transition.ResumeMovement();
            }
        }

        /// <summary>
        /// マーキーのContentを追加します
        /// </summary>
        /// <param name="content"></param>
        public virtual void AddContent(GameObject content)
        {
            if (MarqueeView != null)
            {
                MarqueeView.AddContent(content);
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// 初期化処理を行う
        /// </summary>
        protected virtual void Init()
        {
            if (Application.isPlaying)
            {
                // 初期状態では表示は消しておく
                HideView();
            }

            transitionProperty.SetMarqueeViewProperty(MarqueeView);
            if (transition != null && MarqueeView != null)
            {
                transition.Init(this, transitionProperty);
                MarqueeView.CompleteLayoutCallback += transition.OnCompleteLayout;
            }
            else
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    if (transition == null)
                    {
                        Debug.LogErrorFormat("{0} : transition is null.", name);
                    }

                    // layoutGroup の null は MarqueeView プロパティにアクセスした時点でエラーがでているのでここでは出さない
                }
#endif
            }
        }

        /// <summary>
        /// Contentを表示します
        /// </summary>
        protected virtual void ShowView()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
            }
        }

        /// <summary>
        /// Contentを表示しないようにします
        /// </summary>
        protected virtual void HideView()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }
        }

#endregion

#region unity methods

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void Start()
        {
            if (PlayOnStart)
            {
                StartTransition();
            }
        }

        protected virtual void OnEnable()
        {
            if (transition != null)
            {
                transition.EnableTransition();
            }
        }

        protected virtual void OnDisable()
        {
            if (transition != null)
            {
                transition.DisableTransition();
            }
        }

#endregion
    }
}
