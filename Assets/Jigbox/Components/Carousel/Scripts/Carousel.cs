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
using System;
using Jigbox.UIControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Jigbox.Components
{
    /// <summary>
    /// Carouselクラス
    /// </summary>
    [RequireComponent(typeof(RaycastArea))]
    [DisallowMultipleComponent]
    public class Carousel : DragBehaviour
    {
#region inner classes, enum, and structs

        /// <summary>
        /// カルーセルのループタイプ
        /// </summary>
        public enum CarouselLoopType
        {
            None = 0,   // ループさせない
            Loop = 1,   // ループさせる
        }

        /// <summary>
        /// カルーセルのOnChangeIndex時のデリゲート型
        /// </summary>
        public class CarouselChangeIndexDelegate : EventDelegate<int>
        {
            public CarouselChangeIndexDelegate(Callback eventDelegate) : base(eventDelegate)
            {
            }
        }

#endregion

#region constants

        /// <summary>
        /// オートスクロールの間隔のデフォルト値
        /// </summary>
        protected static readonly float DefaultAutoScrollIntervalValue = 5.0f;

        /// <summary>
        /// showIndex用の初期値
        /// </summary>
        protected static readonly int InitShowIndexValue = int.MaxValue;

        /// <summary>
        /// Loopさせるために必要なセルの最少数
        /// </summary>
        protected static readonly int MinimumUnitOfLoopCellCount = 3;

        /// <summary>
        /// 処理を行っていない時にキャッシュするpointerID
        /// </summary>
        protected static readonly int InvalidPointerId = int.MinValue;
        
        /// <summary>
        /// フリック操作の加速度を求める際のフレームレート
        /// </summary>
        public static int FlickAccelerationFramerate = 60;
        
        /// <summary>
        /// フリックが有効となるピクセル単位の距離の閾値のデフォルト
        /// </summary>
        public static float DefaultFlickMovementThreshold = 1.0f;
        
        /// <summary>
        /// フリックが有効となる加速度の閾値のデフォルト
        /// </summary>
        public static float DefaultFlickAccelerationThreshold = 0.5f;
        
        /// <summary>
        /// フリックが有効となる時間の閾値のデフォルト
        /// </summary>
        public static float DefaultFlickTimeThreshold = 0.3f;

#endregion

#region properties

        /// <summary>
        /// バナー用のGridLayoutGroup
        /// </summary>
        [SerializeField]
        protected GridLayoutGroup cellLayoutGroup;

        /// <summary>
        /// バナー用のGridLayoutGroupを返します
        /// </summary>
        public GridLayoutGroup CellLayoutGroup { get { return cellLayoutGroup; } }

        /// <summary>
        /// ContentのTransformを返します
        /// </summary>
        public Transform ContentTransform { get { return cellLayoutGroup.transform; } }

        /// <summary>
        /// Transition用のコンポーネント
        /// </summary>
        [SerializeField]
        protected CarouselTransitionBase transitionComponent;

        /// <summary>
        /// Transition用のコンポーネント
        /// </summary>
        public CarouselTransitionBase TransitionBase { get { return transitionComponent; } }

        /// <summary>
        /// Bulletを管理するコンポーネント
        /// </summary>
        [SerializeField]
        protected BulletControllerBase bulletController;

        /// <summary>
        /// Bulletを管理するコンポーネント
        /// </summary>
        public BulletControllerBase BulletController { get { return bulletController; } }

        /// <summary>
        /// CarouselのCellのループ指定
        /// </summary>
        [SerializeField]
        protected CarouselLoopType loopType = CarouselLoopType.Loop;

        /// <summary>
        /// CarouselのCellのループ指定を取得します
        /// </summary>
        public CarouselLoopType LoopType { get { return loopType; } }

        /// <summary>
        /// loopTypeがループ設定になっているかどうかを返します
        /// </summary>
        public virtual bool IsLoop { get { return loopType == CarouselLoopType.Loop; } }

        /// <summary>
        /// ループ可能になっているかどうかを返します
        /// </summary>
        public virtual bool CanLoop
        {
            get
            {
                return CellLayoutGroup.transform.childCount >= MinimumUnitOfLoopCellCount;
            }
        }

        /// <summary>
        /// 時間制御によるオートスクロールを行うか
        /// </summary>
        public virtual bool IsAutoScroll
        {
            get
            {
                return autoScrollInterval > 0f;
            }
        }

        /// <summary>
        /// オートスクロールの間隔
        /// </summary>
        [SerializeField]
        protected float autoScrollInterval = DefaultAutoScrollIntervalValue;

        /// <summary>
        /// Cellがタッチされているかどうか
        /// </summary>
        protected bool isSelectCell;

        /// <summary>
        /// Transition中の一時保存用index
        /// </summary>
        protected int showIndex = InitShowIndexValue;

        /// <summary>
        /// 現在表示中のCellのIndexを返します
        /// </summary>
        public virtual int ShowIndex { get { return IsTransition ? showIndex : CurrentIndex; } }

        /// <summary>
        /// 現在選択されているCellのIndex
        /// </summary>
        protected int currentIndex;

        /// <summary>
        /// 現在選択されているCellのIndexを返します
        /// </summary>
        public virtual int CurrentIndex
        {
            get
            {
                return currentIndex;
            }
            protected set
            {
                if (CellCount <= 0)
                {
                    currentIndex = 0;
                    return;
                }

                int nextIndex = model.ValidateIndex(value);
                if (currentIndex != nextIndex)
                {
                    currentIndex = nextIndex;
                    OnChangeIndex();
                }
            }
        }

        /// <summary>
        /// Initialize実行時のCell個数を返します
        /// </summary>
        protected int cellCount;

        /// <summary>
        /// Initialize実行時のCell個数を返します
        /// </summary>
        public virtual int CellCount { get { return cellCount; } }

        /// <summary>
        /// レイアウトに対応したViewを保持します
        /// </summary>
        protected CarouselViewBase view;

        /// <summary>
        /// CarouselのModel
        /// </summary>
        protected CarouselModel model;

        /// <summary>
        /// オートスクロールの待機中の時間を保持します
        /// </summary>
        protected float autoScrollWaitTime;

        /// <summary>
        /// 前フレームのUpdateイベント時のInputCount数を保持します
        /// </summary>
        protected int prevInputCount = 0;

        /// <summary>
        /// OnChangeIndex用のDelegatableList
        /// </summary>
        [SerializeField]
        DelegatableList onChangeIndexDelegates = new DelegatableList();

        /// <summary>
        /// OnChangeIndex用のDelegatableList
        /// </summary>
        public DelegatableList OnChangeIndexDelegates { get { return onChangeIndexDelegates; } }

        /// <summary>
        /// OnCompleteTransition用のDelegatableList
        /// </summary>
        [SerializeField]
        DelegatableList onCompleteTransitionDelegates = new DelegatableList();

        /// <summary>
        /// OnCompleteTransition用のDelegatableList
        /// </summary>
        public DelegatableList OnCompleteTransitionDelegates { get { return onCompleteTransitionDelegates; } }

        /// <summary>
        /// Cell生成用のInstanceProvider
        /// </summary>
        protected IInstanceProvider<GameObject> cellProvider;

        /// <summary>
        /// 任意の方法でセルとなるGameObjectをCellのContentに渡すプロバイダーを参照/指定します
        /// </summary>
        /// <value>The cell provider.</value>
        public virtual IInstanceProvider<GameObject> CellProvider { get { return cellProvider; } set { cellProvider = value; } }

        /// <summary>
        /// Cell処分用のInstanceDisposer
        /// </summary>
        protected IInstanceDisposer<GameObject> cellDisposer = new CarouselInstanceDisposer();

        /// <summary>
        /// 任意の方法でセルを処分するディスポーザを参照/指定します
        /// </summary>
        public virtual IInstanceDisposer<GameObject> CellDisposer { get { return cellDisposer; } set { cellDisposer = value; } }

        /// <summary>
        /// スクロール処理を止めるかどうか(この値がtrueの場合は自動スクロール停止/ドラッグ不可となります)
        /// </summary>
        public virtual bool DisableScroll { get; set; }

        /// <summary>
        /// トランジション実行中かを返します
        /// </summary>
        public virtual bool IsTransition { get { return showIndex != InitShowIndexValue; } }
        
        /// <summary>
        /// フリック操作を有効にするかどうか
        /// </summary>
        public bool IsControllableFlick = false;

        /// <summary>
        /// フリックが有効となるピクセル単位の距離の閾値
        /// </summary>
        public float FlickMovementThreshold = DefaultFlickMovementThreshold;
        
        /// <summary>
        /// フリックが有効となる加速度の閾値
        /// </summary>
        public float FlickAccelerationThreshold = DefaultFlickAccelerationThreshold;
        
        /// <summary>
        /// フリックが有効となる時間の閾値
        /// </summary>
        public float FlickTimeThreshold = DefaultFlickTimeThreshold;

        /// <summary>
        /// フリックの開始フレーム
        /// </summary>
        protected float FlickBeginTime = 0.0f;
        
        /// <summary>
        /// フリックの開始座標
        /// </summary>
        protected Vector2 FlickBeginPosition = new Vector2();

        /// <summary>
        /// フリック開始時のセル
        /// </summary>
        protected int FlickStartCellIndex = 0;

        /// <summary>
        /// 現在処理中のpointerID
        /// </summary>
        protected int CurrentPointerId = InvalidPointerId;

#endregion

#region override methods

        /// <summary>
        /// ドラッグ対象が見つかった際に呼び出されます。(実質押下と同タイミング)
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            // DisableScrollが有効な場合は何もしない
            if (DisableScroll)
            {
                return;
            }

            // 現在処理中なので何もしない
            if (CurrentPointerId != InvalidPointerId)
            {
                return;
            }

            isSelectCell = true;

            StopTransition();
        }

        /// <summary>
        /// ドラッグが開始された際に呼び出されます
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnBeginDrag(PointerEventData eventData)
        {
            // DisableScrollが有効な場合は何もしない
            if (DisableScroll)
            {
                return;
            }
            
            // 現在処理中なので何もしない
            if (CurrentPointerId != InvalidPointerId)
            {
                return;
            }

            CurrentPointerId = eventData.pointerId;
            
            // GestureのFlickはタッチ開始時点でフリック判定用の情報を取得しているが
            // Carouselはタッチ終了時の判定をとれないのでドラッグ開始時に取得するようにしている
            if (IsControllableFlick)
            {
                // フリック検知に必要な情報を取得
                FlickBeginTime = Time.realtimeSinceStartup;
                FlickBeginPosition = eventData.position;
                FlickStartCellIndex = CurrentIndex;
            }
        }

        /// <summary>
        /// ドラッグ中にポインタが移動した際に呼び出されます
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDrag(PointerEventData eventData)
        {
            // DisableScrollが有効な場合は何もしない
            if (DisableScroll)
            {
                return;
            }
            
            // 現在処理中のpointerIDと異なる場合は何もしない
            if (CurrentPointerId != eventData.pointerId)
            {
                return;
            }

            // OnInitializePotentialDragでもtrueにしているが、
            // タップ後にドラッグせずにShowCellAtとOnCompleteTransitionが実行されるとfalseになるため、再度trueを設定し直す
            isSelectCell = true;

            var delta = eventData.delta;
            if (!IsLoop)
            {
                // ループしない時は一番端のカルーセルより先にドラッグさせないのでチェックする
                if (view.CheckDeltaPrevVector(delta))
                {
                    delta = view.GetValidDeltaPrev(delta);
                }
                else if (view.CheckDeltaNextVector(delta))
                {
                    delta = view.GetValidDeltaNext(delta);
                }
            }
            // viewからDelta値を基にして求めたoffsetIndex値を返してもらって反映させる
            CurrentIndex += view.MoveContentByDelta(delta);
        }

        /// <summary>
        /// ドラッグが終了した際に呼び出されます
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnEndDrag(PointerEventData eventData)
        {
            // DisableScrollが有効な場合は何もしない
            if (DisableScroll)
            {
                return;
            }
            
            // 現在処理中のpointerIDと異なる場合は何もしない
            if (CurrentPointerId != eventData.pointerId)
            {
                return;
            }
            
            ResetAutoScrollWaitTime();
            
            if (IsControllableFlick)
            {
                FlickDetection(eventData);
            }

            // pointerIDを初期化
            CurrentPointerId = InvalidPointerId;
        }

#endregion

#region public methods

        /// <summary>
        /// レイアウトを初期化します(カルーセルに表示させるアイテムが全て揃ってから叩く必要があります)
        /// セルの順序は、Hierarchy上に配置されているGameObjectの順序に依存します。
        /// ランタイム中にLoopTypeを変更したい場合は引数ありのメソッドを使用してください。
        /// </summary>
        public virtual void Relayout()
        {
            // 途中でCellがAddされてしまうとCell数依存による計算に食い違いが発生する可能性があるのでInitialize実行時のCell数を保持しておく
            // cellCountはCarouselViewPropertyで参照されるので、参照される前に値を入れておく
            cellCount = CellLayoutGroup.transform.childCount;
            currentIndex = 0;
            ContentTransform.localPosition = Vector3.zero;

            // Loopを構成できる最小単位に満たない場合はLoopType.Noneに変更する
            if (cellCount < MinimumUnitOfLoopCellCount && IsLoop)
            {
                Debug.LogError("Can't to set 'LoopType.Loop' because not enough cell count!");
                loopType = CarouselLoopType.None;
            }

            model.CreateView(ref view);

            var viewProperty = new CarouselViewProperty(this);

            view.Initialize(viewProperty);

            if (transitionComponent != null)
            {
                transitionComponent.StopTransition();
                transitionComponent.Initialize(this, view);
            }

            // CurrentIndexの更新をする前にBulletControllerのRelayoutが終わっていないと、
            // Bulletの参照がnullとなって、Exceptionを吐く
            if (bulletController != null)
            {
                bulletController.Relayout();
            }

            // 最初のCellがContentの中央位置に来るように位置補正を行った際currentIndexがずれてしまうので補正を行う
            // プロパティを使用するとその時点でOnChangeIndexが呼ばれてしまうので変数へ直接代入
            currentIndex = model.GetStartCellIndex();
            var to = view.GetAmountOfMovement(currentIndex);
            // ここを呼び出すと中でCurrentIndexが更新され補正分含めて中央に表示されるCellのindexが0になる
            CurrentIndex += view.MoveContentByDelta(to);
            // Loopしない場合は初期化処理を走らせる
            if (!IsLoop)
            {
                view.InitNoLoopViewData();
            }
            ResetAutoScrollWaitTime();
        }

        /// <summary>
        /// レイアウトを初期化します(カルーセルに表示させるアイテムが全て揃ってから叩く必要があります)
        /// セルの順序は、Hierarchy上に配置されているGameObjectの順序に依存します。
        /// ランタイム中にLoopTypeを変更したい場合はこちらを使用してください。
        /// </summary>
        /// <param name="isLoop"></param>
        public virtual void Relayout(bool isLoop)
        {
            // ランタイム中にLoopTypeが変更された場合、Relayoutを叩いたタイミングでSiblingIndexがずれている可能性があるので開始位置のIndexに戻す
            // これを行わないと最初に表示されるオブジェクトが変わってしまう
            if (view != null && IsLoop)
            {
                ShowCellAt(model.GetStartCellIndex(), false);
            }

            view = null;
            loopType = isLoop ? CarouselLoopType.Loop : CarouselLoopType.None;
            Relayout();
        }

        /// <summary>
        /// 指定されたIndexのCellが見えるように移動します
        /// </summary>
        /// <param name="index"></param>
        /// <param name="isAnimation"></param>
        public virtual void ShowCellAt(int index, bool isAnimation = true)
        {
            // DisableScrollが有効な場合は何もしない
            if (isAnimation && DisableScroll)
            {
                return;
            }

            // アニメーションよりはタップの処理を優先させる
            if (isAnimation && isSelectCell)
            {
                return;
            }

            if (CellCount <= 0)
            {
                // 何もしない
                return;
            }

            var tempIndex = IsLoop ? index : model.ValidateIndex(index);

            if (tempIndex == CurrentIndex)
            {
                // 同じIndexなら何もしない
                return;
            }

            ResetAutoScrollWaitTime();
            var offsetIndex = GetNearOffsetIndex(tempIndex);
            // 移動量はViewから取得
            var to = view.GetContentMoveAmountByOffsetIndex(offsetIndex);
            if (isAnimation)
            {
                if (!IsTransition)
                {
                    showIndex = CurrentIndex;
                }
                CurrentIndex = tempIndex;
                transitionComponent.MoveContent(to);
            }
            else
            {
                CurrentIndex += view.MoveContentByDelta(to);
            }
        }

        /// <summary>
        /// 次のIndexのCellを表示させます
        /// </summary>
        [AuthorizedAccess]
        public virtual void ChangeNextCell()
        {
            ShowCellAt(CurrentIndex + 1);
        }

        /// <summary>
        /// 前のIndexのCellを表示させます
        /// </summary>
        [AuthorizedAccess]
        public virtual void ChangePrevCell()
        {
            ShowCellAt(CurrentIndex - 1);
        }

        /// <summary>
        /// TransitionのTweenがCompleteされた時に呼び出されます
        /// </summary>
        public virtual void OnCompleteTransition()
        {
            // Transitionが完了したのでshowIndexを初期値に戻す
            showIndex = InitShowIndexValue;
            // AutoScrollの待ち時間をリセットする
            ResetAutoScrollWaitTime();
            // ここでisSelectCellをfalseにするのはClick(Dragされずにタップを離された)時の対策用
            isSelectCell = false;
            // OnCenterを発火
            if (OnCompleteTransitionDelegates.Count > 0)
            {
                OnCompleteTransitionDelegates.Execute();
            }
        }

        /// <summary>
        /// Cellを追加します
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>追加に成功したらtrueを返します</returns>
        public virtual bool AddCell(GameObject obj)
        {
            if (obj == null)
            {
#if UNITY_EDITOR
                Debug.LogError("obj is null.");
#endif
                return false;
            }

            if (view != null)
            {
#if UNITY_EDITOR
                Debug.LogError("Can't add a new cell after calling 'Relayout'.");
#endif
                return false;
            }

            obj.transform.SetParent(ContentTransform, false);
            return true;
        }

        /// <summary>
        /// Cellを追加します
        /// </summary>
        /// <param name="generator"></param>
        /// <returns>追加に成功したらtrueを返します</returns>
        public virtual bool AddCell(Func<GameObject> generator)
        {
            if (generator == null)
            {
#if UNITY_EDITOR
                Debug.LogError("generator is null.");
#endif
                return false;
            }

            if (view != null)
            {
#if UNITY_EDITOR
                Debug.LogError("Can't add a new cell after calling 'Relayout'.");
#endif
                return false;
            }

            var obj = generator();
            obj.transform.SetParent(ContentTransform, false);
            return true;
        }

        /// <summary>
        /// Cellを追加します(InstanceProvider使用)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>追加に成功したらtrueを返します</returns>
        public virtual bool AddCell()
        {
            if (cellProvider == null)
            {
#if UNITY_EDITOR
                Debug.LogError("cellProvider is null.");
#endif
                return false;
            }

            if (view != null)
            {
#if UNITY_EDITOR
                Debug.LogError("Can't add a new cell after calling 'Relayout'.");
#endif
                return false;
            }

            var obj = cellProvider.Generate();
            obj.transform.SetParent(ContentTransform, false);
            return true;
        }

        /// <summary>
        /// Cellを全て削除します
        /// </summary>
        public virtual void RemoveAllCell()
        {
            for (int i = CellLayoutGroup.transform.childCount - 1; i >= 0; i--)
            {
                cellDisposer.Dispose(ContentTransform.GetChild(i).gameObject);
            }
            cellCount = 0;
            view = null;
        }

        /// <summary>
        /// 現在のindexから指定したindexに移動するにあたり、移動距離が近いindexの差分を返します(Loop指定されている場合はLoopも考慮されます)
        /// </summary>
        /// <param name="index">移動したいindex</param>
        /// <returns>移動距離が近いindexの差分</returns>
        public virtual int GetNearOffsetIndex(int index)
        {
            return model.GetNearOffsetIndex(index);
        }

        /// <summary>
        /// Transitionを停止させます
        /// </summary>
        public virtual void StopTransition()
        {
            if (transitionComponent != null)
            {
                transitionComponent.StopTransition();
            }

            if (showIndex != InitShowIndexValue)
            {
                CurrentIndex = showIndex;
                showIndex = InitShowIndexValue;
            }
        }

        /// <summary>
        /// TransitionのOnUpdateTweenが叩かれた時に呼ばれます
        /// viewから受け取ったoffsetIndexでCurrentIndexを更新します
        /// </summary>
        /// <param name="to"></param>
        public virtual void OnMoveFromTransition(Vector3 to)
        {
            showIndex += view.MoveContentByDelta(to);
        }

        /// <summary>
        /// <para>イベントを追加します。</para>
        /// <para>※実行時のみ有効で、System.Actionは登録できません。</para>
        /// </summary>
        /// <param name="callback">void Func(void)の関数</param>
        /// <returns></returns>
        public bool AddCompleteTransitionEvent(DelegatableObject.Callback callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("Carousel.AddCompleteTransitionEvent : Can't call! The method is enable when playing!");
                return false;
            }
#endif
            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(callback);
            OnCompleteTransitionDelegates.Add(delegatable);

            return true;
        }

        /// <summary>
        /// <para>イベントを追加します。</para>
        /// <para>※実行時のみ有効で、System.Actionは登録できません。</para>
        /// </summary>
        /// <param name="callback">void Func(int)の関数</param>
        /// <returns></returns>
        public bool AddChangeIndexEvent(CarouselChangeIndexDelegate.Callback callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("Carousel.AddChangeIndexEvent : Can't call! The method is enable when playing!");
                return false;
            }
#endif
            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(new CarouselChangeIndexDelegate(callback));
            OnChangeIndexDelegates.Add(delegatable);

            return true;
        }

#endregion

#region protected methods

        /// <summary>
        /// 初期化
        /// </summary>
        protected virtual void Initialize()
        {
            model = new CarouselModel(new CarouselModelProperty(this));

            // SerializeFieldになければDefaultであるBasicCarouselTransitionをAddComponentする
            transitionComponent = transitionComponent ?? gameObject.AddComponent<BasicCarouselTransition>();

            if (bulletController != null)
            {
                bulletController.Initialize(this);
            }
        }

        /// <summary>
        /// Contentをセンタリングしたい時に呼び出します
        /// </summary>
        protected virtual void CenteringContent()
        {
            if (CellCount > 0)
            {
                var to = view.DistanceToCenteringPosition();
                transitionComponent.MoveContent(to);
            }
        }

        /// <summary>
        /// Indexが更新された時に呼び出されます
        /// </summary>
        protected virtual void OnChangeIndex()
        {
            // BulletControllerへ通知
            if (bulletController != null)
            {
                bulletController.ChangeBulletAt(CurrentIndex);
            }

            // OnChangeIndexを発火
            if (OnChangeIndexDelegates.Count > 0)
            {
                OnChangeIndexDelegates.Execute(CurrentIndex);
            }
        }

        /// <summary>
        /// オートスクロールする待ち時間をリセットします
        /// </summary>
        protected virtual void ResetAutoScrollWaitTime()
        {
            autoScrollWaitTime = 0f;
        }

        /// <summary>
        /// フリックの操作を検知してセルを移動させます
        /// </summary>
        protected virtual void FlickDetection(PointerEventData eventData)
        {
            // 長くタップしていたのでフリック操作ではない
            if (Time.realtimeSinceStartup - FlickBeginTime > FlickTimeThreshold)
            {
                return;
            }
            
            // ドラッグ開始位置とドラッグ終了位置が狭いためフリック操作ではない
            if (FlickMovementThreshold > Vector2.Distance(eventData.position, FlickBeginPosition))
            {
                return;
            }
            
            // 操作の加速度をフレームレートを元に計算する
            Vector2 acceleration = (eventData.position - FlickBeginPosition) / FlickAccelerationFramerate;
            
            // 加速度が閾値より高い場合はフリック判定
            if (acceleration.sqrMagnitude >= FlickAccelerationThreshold * FlickAccelerationThreshold)
            {
                VectorDirectionUtils.Direction direction = VectorDirectionUtils.GetDirection(acceleration);

                // フリック方向とセルのレイアウト方向が異なる場合は処理を行わない
                // フリック方向が左右でセルレイアウトが縦の場合
                if ((direction == VectorDirectionUtils.Direction.Left ||
                     direction == VectorDirectionUtils.Direction.Right) &&
                    CellLayoutGroup.startAxis == GridLayoutGroup.Axis.Vertical)
                {
                    return;
                }
                
                // フリック方向が上下でセルレイアウトが横の場合
                if ((direction == VectorDirectionUtils.Direction.Up ||
                     direction == VectorDirectionUtils.Direction.Down) &&
                    CellLayoutGroup.startAxis == GridLayoutGroup.Axis.Horizontal)
                {
                    return;
                }
                
                int showCellIndex = 0;
                
                // フリック方向に応じて処理を変更
                switch (direction)
                {
                    case VectorDirectionUtils.Direction.Left:
                        showCellIndex = FlickStartCellIndex + 1;
                        break;
                    case VectorDirectionUtils.Direction.Right:
                        showCellIndex = FlickStartCellIndex - 1;
                        break;
                    case VectorDirectionUtils.Direction.Up:
                        showCellIndex = FlickStartCellIndex + 1;
                        break;
                    case VectorDirectionUtils.Direction.Down:
                        showCellIndex = FlickStartCellIndex - 1;
                        break;
                }

                if (CurrentIndex != showCellIndex)
                {
                    // ループしない際にレイアウトの端にいる状態では存在しないセルに向けてのフリック処理を行わない
                    if (LoopType == CarouselLoopType.None && 
                        (0 > showCellIndex || showCellIndex > CellCount - 1))
                    {
                        return;
                    }
                    
                    isSelectCell = false;
                    ShowCellAt(showCellIndex);
                }
            }
        }

#endregion

#region override unity methods

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        protected virtual void Update()
        {
            if (view == null)
            {
                return;
            }

            int inputCount = 0;

#if UNITY_EDITOR || UNITY_STANDALONE
            inputCount = InputWrapper.GetMouseButton(0) ? 1 : 0;
#else
            inputCount = InputWrapper.GetTouchCount();
#endif

            // 無入力状態になっている場合
            if (inputCount == 0)
            {
                // 選択状態で前フレームのInputCount数と比較して異なればタップもしくはドラッグ扱いとなるはず
                if (prevInputCount != inputCount && isSelectCell)
                {
                    // トランジション実行中でない場合は正しい位置にセンタリングするトランジションを開始
                    if (!IsTransition)
                    {
                        CenteringContent();
                        ResetAutoScrollWaitTime();
                    }
                }
            }

            if (view != null && IsAutoScroll && !isSelectCell && !DisableScroll)
            {
                autoScrollWaitTime += Time.deltaTime;
                if (autoScrollWaitTime >= autoScrollInterval)
                {
                    ChangeNextCell();
                    ResetAutoScrollWaitTime();
                }
            }
        }

        protected virtual void LateUpdate()
        {
            if (view == null)
            {
                return;
            }
            
            int inputCount = 0;
            
#if UNITY_EDITOR || UNITY_STANDALONE
            inputCount = InputWrapper.GetMouseButton(0) ? 1 : 0;
#else
            inputCount = InputWrapper.GetTouchCount();
#endif

            // 無入力状態になっている場合
            if (inputCount == 0)
            {
                // 選択状態で前フレームのInputCount数と比較して異なればタップもしくはドラッグ扱いとなるはず
                if (prevInputCount != inputCount && isSelectCell)
                {
                    CurrentPointerId = InvalidPointerId;
                    isSelectCell = false;
                }
            }
            
            prevInputCount = inputCount;
        }

#endregion
    }
}
