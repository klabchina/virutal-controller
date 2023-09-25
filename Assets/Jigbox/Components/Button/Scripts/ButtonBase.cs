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
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using Jigbox.UIControl;
using Jigbox.Delegatable;

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RaycastArea))]
    public class ButtonBase : InputBehaviour
    {
#region inner classes, enum, and structs        

        /// <summary>
        /// イベントのエントリ情報
        /// </summary>
        [System.Serializable]
        public class EventEntry
        {
            /// <summary>イベントの種類</summary>
            [HideInInspector]
            [SerializeField]
            InputEventType type;

            /// <summary>イベントの種類</summary>
            public InputEventType Type { get { return type; } }

            /// <summary>デリゲート</summary>
            [HideInInspector]
            [SerializeField]
            DelegatableList delegates;

            /// <summary>デリゲート</summary>
            public DelegatableList Delegates { get { return delegates; } }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="type"></param>
            public EventEntry(InputEventType type)
            {
                this.type = type;
                delegates = new DelegatableList();
            }
        }

        /// <summary>
        /// ボタンの標準デリゲート型
        /// </summary>
        public class ButtonDelegate : EventDelegate<PointerEventData>
        {
            public ButtonDelegate(Callback eventDelegate) : base(eventDelegate)
            {
            }
        }

#endregion

#region properties

        /// <summary>RaycastArea</summary>
        [HideInInspector]
        [SerializeField]
        protected RaycastArea raycastArea;

        /// <summary>RaycastArea</summary>
        public RaycastArea RaycastArea { get { return raycastArea; } }

        /// <summary>ボタンの判定領域の大きさ</summary>
        public Vector2 Size { get { return RaycastArea.Size; } set { RaycastArea.Size = value; } }

        /// <summary>ボタンが有効かどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool clickable = true;

        /// <summary>ボタンが有効かどうか</summary>
        public virtual bool Clickable
        {
            get
            {
                return clickable;
            }
            set
            {
                if (clickable != value)
                {
                    clickable = value;

                    if (RaycastArea != null)
                    {
                        RaycastArea.raycastTarget = clickable;
                    }
                }
            }
        }
        
        /// <summary>入力の排他制御用クラス</summary>
        [HideInInspector]
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("collegue")]
        protected SubmitColleague colleague;

        /// <summary>イベント情報のリスト</summary>
        [HideInInspector]
        [SerializeField]
        protected List<EventEntry> entries = new List<EventEntry>();

        /// <summary>イベント情報のリスト</summary>
        public List<EventEntry> Entries { get { return entries; } }

        /// <summary>配下の画像情報管理クラス</summary>
        [HideInInspector]
        [SerializeField]
        protected GraphicGroup graphicGroup;

        /// <summary>ボタン配下の画像情報</summary>
        protected GraphicComponentInfo imageInfo = null;

        /// <summary>ボタン配下の画像情報</summary>
        public GraphicComponentInfo ImageInfo
        {
            get
            {
                if (graphicGroup != null)
                {
                    return graphicGroup.GraphicInfo;
                }

                if (imageInfo == null)
                {
                    imageInfo = new GraphicComponentInfo();
                    imageInfo.FindGraphicComponents(gameObject);
                }
                return imageInfo;
            }
        }

        /// <summary>ポインタが画面から離されたかどうか</summary>
        protected bool isRelease = false;

        /// <summary>入力ポインタによって選択されているかどうか</summary>
        protected bool isSelected = false;

        /// <summary>押下している状態のポインタのID</summary>
        protected int pointerId = int.MinValue;
        
#endregion

#region public methods

        /// <summary>
        /// <para>イベントを追加します。</para>
        /// <para>※実行時のみ有効で、System.Actionは登録できません。</para>
        /// </summary>
        /// <param name="type">イベントの種類</param>
        /// <param name="callback">void Func(void)の関数</param>
        /// <returns></returns>
        public bool AddEvent(InputEventType type, DelegatableObject.Callback callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("ButtonBase.AddEvent : Can't call! The method is enable when playing!");
                return false;
            }
#endif
            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(callback);
            AddEvent(type, delegatable);

            return true;
        }

        /// <summary>
        /// <para>イベントを追加します。</para>
        /// <para>※実行時のみ有効で、System.Actionは登録できません。</para>
        /// </summary>
        /// <param name="type">イベントの種類</param>
        /// <param name="target">イベントの発行対象</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="args">引数</param>
        /// <returns></returns>
        public bool AddEvent(InputEventType type, MonoBehaviour target, string methodName, params object[] args)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("ButtonBase.AddEvent : Can't call! The method is enable when playing!");
                return false;
            }
#endif

            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(target, methodName, args);
            AddEvent(type, delegatable);

            return true;
        }

        /// <summary>
        /// <para>イベントを追加します。</para>
        /// <para>※実行時のみ有効で、System.Actionは登録できません。</para>
        /// </summary>
        /// <param name="type">イベントの種類</param>
        /// <param name="callback">void Func(PointerEventData)の関数</param>
        /// <returns></returns>
        public bool AddEvent(InputEventType type, ButtonDelegate.Callback callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("ButtonBase.AddEvent : Can't call! The method is enable when playing!");
                return false;
            }

            switch (type)
            {
                case InputEventType.OnLongPress:
                case InputEventType.OnKeyRepeat:
                case InputEventType.OnSelect:
                case InputEventType.OnDeselect:
                case InputEventType.OnUpdateSelected:
                    Debug.LogWarning("ButtonBase.AddEvent : The event type can't get PointerEventData");
                    break;
            }
#endif
            DelegatableObject delegatable = new DelegatableObject();
            delegatable.SetEvent(new ButtonDelegate(callback));
            AddEvent(type, delegatable);

            return true;
        }

        /// <summary>
        /// ボタンが押下された際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            // isReleaseを条件に含めているのは同一フレームでOnPointerUpとOnPointerDownが発火した場合に
            // OnPointerDownをブロックするため
            if (!Clickable || colleague.IsEnable || isRelease)
            {
                return;
            }

            colleague.GetLock();

            if (!colleague.IsEnable)
            {
                return;
            }

            isRelease = false;
            pointerId = eventData.pointerId;

            Execute(InputEventType.OnPress, eventData);
            
            OnSelect();
            PressStart();
        }

        /// <summary>
        /// ボタン押下後に指が離された際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!IsValidEvent(eventData))
            {
                // isReleaseがtrueの時、Clickableがfalseになった場合などでも整合性が取れるように
                // Clickableがfalseの時にもボタンの状態は更新する。
                if (IsValidReflesh(eventData) && !Clickable)
                {
                    isRelease = true;
                    isSelected = false;
                }
                return;
            }

            isRelease = true;

            OnDeselect();

            Execute(InputEventType.OnRelease, eventData);
        }

        /// <summary>
        /// ボタンを押下し、ボタン上で指が離された際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!IsValidEvent(eventData))
            {
                return;
            }
            
            Execute(InputEventType.OnClick, eventData);
        }

        /// <summary>
        /// ボタンの判定領域内に指が入った際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerEnter(PointerEventData eventData)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            // マウスオーバーによりOnPointerUpの後にOnPointerEnterが呼ばれてしまい
            // Transition処理がおかしくなる問題があるのでisReleaseもみることで対処する
            if (!IsValidEvent(eventData) || isRelease)
#else
            if (!IsValidEvent(eventData))
#endif
            {
                return;
            }

            OnSelect();
        }

        /// <summary>
        /// ボタンの判定領域内から指が出た際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!IsValidEvent(eventData))
            {
                // Clickableがfalseの場合でも、整合性を取るためボタンの状態は更新する。
                if (IsValidReflesh(eventData) && !Clickable)
                {
                    isSelected = false;
                }
                return;
            }

            OnDeselect();
        }

        /// <summary>
        /// ドラッグが終了した際に、その位置に別なオブジェクトが存在していた場合、そのオブジェクトで呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDrop(PointerEventData eventData)
        {
            if (!Clickable)
            {
                return;
            }

            // PointerEventData.pointerPressの参照が入ってこない状態があるため対策
            if (eventData.pointerPress == null)
            {
                return;
            }

            // ドラッグされた対象がButtonBaseを持っていない、
            // もしくは、持っているがロックできていない場合は無効
            ButtonBase target = eventData.pointerPress.GetComponent<ButtonBase>() as ButtonBase;
            if (target == null)
            {
                return;
            }
            if (!target.colleague.IsEnable)
            {
                return;
            }

            Execute(InputEventType.OnDrop, eventData);
        }

        /// <summary>
        /// ドラッグ対象が見つかった際に呼び出されます。(実質押下と同タイミング)
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (!IsValidEvent(eventData))
            {
                return;
            }

            Execute(InputEventType.OnInitDrag, eventData);
        }

        /// <summary>
        /// ドラッグが開始された際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!IsValidEvent(eventData))
            {
                return;
            }

            Execute(InputEventType.OnBeginDrag, eventData);
        }

        /// <summary>
        /// ドラッグ中にポインタが移動した際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDrag(PointerEventData eventData)
        {
            if (!IsValidEvent(eventData))
            {
                return;
            }

            Execute(InputEventType.OnDrag, eventData);
        }

        /// <summary>
        /// ドラッグが終了した際に呼び出されます。
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (!IsValidEvent(eventData))
            {
                return;
            }

            Execute(InputEventType.OnEndDrag, eventData);
        }

#endregion

#region protected methods

        /// <summary>
        /// イベントが有効化どうかを返します。
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        protected bool IsValidEvent(PointerEventData eventData)
        {
            return Clickable && colleague.IsEnable && pointerId == eventData.pointerId;
        }

        /// <summary>
        /// ボタンが押されている際に状態を更新できるかどうかを返します。
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        protected bool IsValidReflesh(PointerEventData eventData)
        {
            return colleague.IsEnable && pointerId == eventData.pointerId;
        }

        /// <summary>
        /// コンポーネント配下に存在するオブジェクトのRaycastを無効化します。
        /// </summary>
        protected void DisableRaycastInChildren()
        {
            foreach (Graphic graphic in ImageInfo.GetGraphics())
            {
                // 自身以外は全て無効
                graphic.raycastTarget = graphic.gameObject == gameObject;
            }
        }
        
        /// <summary>
        /// イベント情報を取得します。
        /// </summary>
        /// <param name="type">イベントの種類</param>
        /// <returns>イベント情報が存在しない場合、nullを返します。</returns>
        protected EventEntry GetEntry(InputEventType type)
        {
            foreach (EventEntry entry in entries)
            {
                if (entry.Type == type)
                {
                    return entry;
                }
            }
            return null;
        }

        /// <summary>
        /// イベント情報が存在するかどうかを返します。
        /// </summary>
        /// <param name="type">イベントの種類</param>
        /// <returns>イベント情報が存在する場合、<c>true</c>を返します。</returns>
        protected bool IsExistEvent(InputEventType type)
        {
            EventEntry entry = GetEntry(type);
            return entry != null && entry.Delegates.Count > 0;
        }

        /// <summary>
        /// イベントを追加します。
        /// </summary>
        /// <param name="type">イベントの種類</param>
        /// <param name="delegatable">DelegatableObject</param>
        protected void AddEvent(InputEventType type, DelegatableObject delegatable)
        {
            EventEntry entry = null;
            foreach (EventEntry e in entries)
            {
                if (e.Type == type)
                {
                    entry = e;
                }
            }

            if (entry == null)
            {
                entry = new EventEntry(type);
                entries.Add(entry);
            }

            entry.Delegates.Add(delegatable);
        }

        /// <summary>
        /// イベントを実行します。
        /// </summary>
        /// <param name="type">イベントのタイプ</param>
        /// <returns></returns>
        protected virtual bool Execute(InputEventType type)
        {
            EventEntry entry = GetEntry(type);
            if (entry != null && entry.Delegates.Count > 0)
            {
                entry.Delegates.Execute();
                return true;
            }
            return false;
        }

        /// <summary>
        /// イベントを実行します。
        /// </summary>
        /// <param name="type">イベントのタイプ</param>
        /// <param name="data">イベント情報</param>
        /// <returns></returns>
        protected virtual bool Execute<T>(InputEventType type, T data)
        {
            EventEntry entry = GetEntry(type);
            if (entry != null && entry.Delegates.Count > 0)
            {
                entry.Delegates.Execute(data);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 押下され始めた際に1度だけ呼び出されます。
        /// </summary>
        protected virtual void PressStart()
        {
        }

        /// <summary>
        /// ポインタが画面から離された際に1度だけ呼び出されます。
        /// </summary>
        protected virtual void PressEnd()
        {
        }

        /// <summary>
        /// 自動アンロックされた際に呼び出されます。
        /// </summary>
        protected virtual void AutoUnlock()
        {
            isRelease = false;
            isSelected = false;
            pointerId = int.MinValue;
        }
    
        /// <summary>
        /// 選択状態で一定時間経過した際に呼び出されます。
        /// </summary>
        protected virtual void OnLongPress()
        {
        }

        /// <summary>
        /// 選択状態で一定間隔ごとに呼び出されます。
        /// </summary>
        protected virtual void OnKeyRepeat()
        {
        }

        /// <summary>
        /// 画面に触れている状態で、入力ポインタによって選択された際に呼び出されます。
        /// </summary>
        protected virtual void OnSelect()
        {
            if (isSelected)
            {
                return;
            }

            isSelected = true;
            Execute(InputEventType.OnSelect);
        }

        /// <summary>
        /// 画面に触れている状態で、入力ポインタによる選択状態が解除された際に呼び出されます。
        /// </summary>
        protected virtual void OnDeselect()
        {
            if (!isSelected)
            {
                return;
            }

            isSelected = false;
            Execute(InputEventType.OnDeselect);
        }

        /// <summary>
        /// 画面に触れている状態で、入力ポインタによって選択されている間、継続的に呼び出されます。
        /// </summary>
        protected virtual void OnUpdateSelected()
        {
        }

        /// <summary>
        /// ボタンの初期化を行います。
        /// </summary>
        protected virtual void Init()
        {
            // インスペクターでの操作なしでボタン作ることはほぼありえないため
            // 基本はシリアライズされている前提だが、念のため確認
            if (raycastArea == null)
            {
                raycastArea = GetComponent<RaycastArea>();
            }

            DisableRaycastInChildren();

            colleague.SetAutoUnlockCallback(AutoUnlock);
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            // GameObject自体を複製したり、PrefabをInstantiateした場合、
            // シリアライズされている情報は同じになるので、
            // Colleagueに設定されるID情報を新しいインスタンスのものに更新する
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                colleague = new SubmitColleague(this, colleague.Group, colleague.CoolTime);
            }
#else
            colleague = new SubmitColleague(this, colleague.Group, colleague.CoolTime);
#endif

            Init();
        }

        protected virtual void LateUpdate()
        {
            if (isRelease && colleague.IsEnable)
            {
                if (Clickable)
                {
                    PressEnd();
                }

                colleague.Unlock();
                isRelease = false;
                isSelected = false;
                pointerId = int.MinValue;
            }
        }

        protected virtual void OnDisable()
        {
            if (colleague.IsEnable)
            {
                colleague.Unlock();
                isRelease = false;
                isSelected = false;
                pointerId = int.MinValue;
            }
        }

        protected virtual void OnDestroy()
        {
            if (colleague.IsEnable)
            {
                colleague.Unlock();
                isRelease = false;
                isSelected = false;
                pointerId = int.MinValue;
            }
        }

#endregion
    }
}
