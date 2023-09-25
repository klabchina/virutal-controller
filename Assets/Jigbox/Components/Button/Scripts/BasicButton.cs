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

namespace Jigbox.Components
{
    public class BasicButton : ButtonBase
    {
#region constants

        /// <summary>無効状態でのボタンのデフォルト色</summary>
        protected static readonly Color DisableColorDefault = new Color(0.5f, 0.5f, 0.5f);

        /// <summary>長押し判定の判定時間</summary>
        protected static readonly float LongPressJudgedTime = 0.35f;

        /// <summary>初期状態のKeyRepeat間隔(2回/s)</summary>
        protected static readonly float RepeatIntervalDefault = 0.5f;

        /// <summary>最速状態のKeyRepeat間隔のデフォルト値</summary>
        protected static readonly float RepeatIntervalFastestDefault = 0.05f;

        /// <summary>KeyRepeat間隔の加速度のデフォルト値</summary>
        protected static readonly float RepeatIntervalAccelerationDefault = 0.07f;

#endregion

#region properties

        /// <summary>ボタンが有効かどうか</summary>
        public override bool Clickable
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
#if UNITY_EDITOR
                    // 実行中以外は色の変更はしない
                    if (!Application.isPlaying)
                    {
                        if (isSyncColor)
                        {
                            isEnableColor = clickable;
                        }
                        return;
                    }
#endif
                    if (isSyncColor)
                    {
                        ChangeColor();
                    }
                }
            }
        }

        /// <summary>色の変更を有効にするかどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool isEnableColorChange = true;

        /// <summary>色の変更を有効にするかどうか</summary>
        public bool IsEnableColorChange
        {
            get
            {
                return isEnableColorChange;
            }
            set
            {
                if (isEnableColorChange != value)
                {
                    isEnableColorChange = value;
                    
#if UNITY_EDITOR
                    // 実行中以外は色の変更はしない
                    if (!Application.isPlaying)
                    {
                        return;
                    }
#endif
                    // 色の変更が無効の場合はデフォルト色に設定
                    if (!isEnableColorChange)
                    {
                        ImageInfo.ResetColor();
                    }
                    else
                    {
                        ChangeColor();
                    }
                }
            }
        }

        /// <summary>無効状態でのボタンの色</summary>
        [HideInInspector]
        [SerializeField]
        protected Color disableColor = DisableColorDefault;

        /// <summary>無効状態でのボタンの色</summary>
        public Color DisableColor
        {
            get
            {
                return disableColor;
            }
            set
            {
                if (disableColor != value)
                {
                    disableColor = value;

#if UNITY_EDITOR
                    // 実行中以外は色の変更はしない
                    if (!Application.isPlaying)
                    {
                        return;
                    }
#endif
                    if (!isEnableColor)
                    {
                        SetColorMultiply(disableColor);
                    }
                }
            }
        }

        /// <summary>ボタンの色が有効状態のものかどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool isEnableColor = true;

        /// <summary>ボタンの色が有効状態のものかどうか</summary>
        public bool IsEnableColor
        {
            get
            {
                return isEnableColor;
            }
            set
            {
                // 色をボタン状態と同期している間は外部から変更不可
                if (isSyncColor)
                {
                    return;
                }
                if (isEnableColor != value)
                {
                    isEnableColor = value;
#if UNITY_EDITOR
                    // 実行中以外は色の変更はしない
                    if (!Application.isPlaying)
                    {
                        return;
                    }
#endif
                    ChangeColor();
                }
            }
        }

        /// <summary>ボタンの状態と色を同期させるかどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool isSyncColor = true;

        /// <summary>ボタンの状態と色を同期させるかどうか</summary>
        public bool IsSyncColor
        {
            get
            {
                return isSyncColor;
            }
            set
            {
                if (isSyncColor != value)
                {
                    isSyncColor = value;                    
#if UNITY_EDITOR
                    // 実行中以外は色の変更はしない
                    if (!Application.isPlaying)
                    {
                        return;
                    }
#endif
                    if (isSyncColor)
                    {
                        isEnableColor = clickable;
                        ChangeColor();
                    }
                }
            }
        }

        /// <summary>ボタンの動作を制御するコンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected ButtonTransitionBase transitionComponent;

        /// <summary>イベント毎にサウンドを再生させるコンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected ButtonSoundBase soundComponent;

        /// <summary>デフォルトのトランジション用のコンポーネントの型</summary>
        public virtual System.Type DefaultTransitionClass { get { return typeof(AdvancedButtonTransition); } }

        /// <summaryデフォルトのサウンド再生用のコンポーネントの型</summary>
        public virtual System.Type DefaultSoundClass { get { return null; } }

        /// <summary>選択状態での経過時間</summary>
        protected float selectedTime = 0.0f;

        /// <summary>KeyRepeatの時間</summary>
        protected float repeatIntervalTime = RepeatIntervalDefault;

        /// <summary>前回のKeyRepeatの時間</summary>
        protected float lastRepeatIntervalTime = RepeatIntervalDefault;

        /// <summary>
        /// <para>KeyRepeatの時間を初期化する際の時間</para>
        /// <para>OnKeyRepeatが最初に発火するまでの時間を調整したい場合に上書きして下さい。</para>
        /// </summary>
        protected virtual float RepeatIntervalInitializeTime { get { return RepeatIntervalDefault; } }

        /// <summary>
        /// <para>最速状態のKeyRepeat間隔</para>
        /// <para>OnKeyRepeatの最速状態での発火間隔を調整したい場合に上書きして下さい。</para>
        /// </summary>
        protected virtual float RepeatIntervalFastest { get { return RepeatIntervalFastestDefault; } }

        /// <summary>
        /// <para>KeyRepeat間隔の加速度</para>
        /// <para>OnKeyRepeatの発火間隔の短縮を調整したい場合に上書きして下さい。</para>
        /// </summary>
        protected virtual float RepeatIntervalAcceleration { get { return RepeatIntervalAccelerationDefault; } }

#endregion
        
#region public methods

        /// <summary>
        /// ボタン押下後に指が離された際に呼び出されます。
        /// ボタンのClickableがfalseの際にOnDeselect()が実行されないため
        /// 拡張コンポーネントにOnDeselect通知を発行します。
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            // 呼び出し順番をbase -> NoticeExtendにするためにフラグを一時キャッシュ
            bool isNoticeExtendComponents = isSelected;

            base.OnPointerUp(eventData);

            if (isNoticeExtendComponents && !isSelected && !Clickable)
            {
                NoticeExtendComponents(InputEventType.OnDeselect);
            }
        }

        /// <summary>
        /// ボタンの判定領域内から指が出た際に呼び出されます。
        /// ボタンのClickableがfalseの際にOnDeselect()が実行されないため
        /// 拡張コンポーネントにOnDeseslect通知を発行します。
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerExit(PointerEventData eventData)
        {
            // 呼び出し順番をbase -> NoticeExtendにするためにフラグを一時キャッシュ
            bool isNoticeExtendComponents = isSelected;

            base.OnPointerExit(eventData);

            if (isNoticeExtendComponents && !isSelected && !Clickable)
            {
                NoticeExtendComponents(InputEventType.OnDeselect);
            }
        }

        /// <summary>
        /// 色情報を更新します。
        /// </summary>
        public void RefreshRegisteredColor()
        {
            if ((isSyncColor && !clickable) || !isEnableColor)
            {
#if UNITY_EDITOR
                Debug.LogWarning("BasicButton.RefreshRegisteredColor : Can't use when color disable!");
#endif
                return;
            }
            ImageInfo.RegisterColor();
        }

#endregion

#region protected methods

        /// <summary>
        /// イベントを実行します。
        /// </summary>
        /// <param name="type">イベントのタイプ</param>
        /// <returns></returns>
        protected override bool Execute(InputEventType type)
        {
            bool result = base.Execute(type);
            NoticeExtendComponents(type);
            return result;
        }

        /// <summary>
        /// イベントを実行します。
        /// </summary>
        /// <param name="type">イベントのタイプ</param>
        /// <param name="data">イベント情報</param>
        protected override bool Execute<T>(InputEventType type, T data)
        {
            bool result = base.Execute<T>(type, data);
            NoticeExtendComponents(type);
            return result;
        }
        
        /// <summary>
        /// 自動アンロックされた際に呼び出されます。
        /// </summary>
        protected override void AutoUnlock()
        {
            base.AutoUnlock();
            if (transitionComponent != null)
            {
                transitionComponent.NoticeAutoUnlock();
            }
        }

        /// <summary>
        /// 選択状態で一定時間経過した際に呼び出されます。
        /// </summary>
        protected override void OnLongPress()
        {
            if (!Clickable || !colleague.IsEnable)
            {
                return;
            }

            // 長押しは、イベントが登録されている状態で実行された場合、その時点でロックを解除する
            if (IsExistEvent(InputEventType.OnLongPress))
            {
                isRelease = true;
                Execute(InputEventType.OnLongPress);
                OnDeselect();
            }
        }

        /// <summary>
        /// 選択状態で一定間隔ごとに呼び出されます。
        /// </summary>
        protected override void OnKeyRepeat()
        {
            if (!Clickable || !colleague.IsEnable)
            {
                return;
            }

            float next = lastRepeatIntervalTime - RepeatIntervalAcceleration;
            next = Mathf.Max(next, RepeatIntervalFastest);
            repeatIntervalTime = next;
            lastRepeatIntervalTime = repeatIntervalTime;

            Execute(InputEventType.OnKeyRepeat);
        }

        /// <summary>
        /// 画面に触れている状態で、入力ポインタによって選択された際に呼び出されます。
        /// </summary>
        protected override void OnSelect()
        {
            if (isSelected)
            {
                return;
            }

            isSelected = true;            
            selectedTime = 0.0f;
            repeatIntervalTime = RepeatIntervalInitializeTime;
            lastRepeatIntervalTime = repeatIntervalTime;

            Execute(InputEventType.OnSelect);
        }

        /// <summary>
        /// 画面に触れている状態で、入力ポインタによる選択状態が解除された際に呼び出されます。
        /// </summary>
        protected override void OnDeselect()
        {
            if (!isSelected)
            {
                return;
            }

            isSelected = false;
            selectedTime = 0.0f;
            repeatIntervalTime = RepeatIntervalInitializeTime;
            lastRepeatIntervalTime = repeatIntervalTime;

            Execute(InputEventType.OnDeselect);
        }

        /// <summary>
        /// 画面に触れている状態で、入力ポインタによって選択されている間、継続的に呼び出されます。
        /// </summary>
        protected override void OnUpdateSelected()
        {
            if (!Clickable || !colleague.IsEnable)
            {
                return;
            }

            Execute(InputEventType.OnUpdateSelected);
        }

        /// <summary>
        /// 拡張用コンポーネントにイベントを通知します。
        /// </summary>
        /// <param name="type">イベントの種類</param>
        protected virtual void NoticeExtendComponents(InputEventType type)
        {
            if (transitionComponent != null)
            {
                transitionComponent.NoticeEvent(type);
            }
            if (soundComponent != null)
            {
                soundComponent.NoticeEvent(type);
            }
        }

        /// <summary>
        /// 色状態を変更します。
        /// </summary>
        protected virtual void ChangeColor()
        {
            // ボタン状態と同期
            if (isSyncColor)
            {
                if (clickable)
                {
                    ResetColor();
                    isEnableColor = true;
                }
                else
                {
                    SetColorMultiply(disableColor);
                    isEnableColor = false;
                }
            }
            // 色は個別
            else
            {
                if (isEnableColor)
                {
                    ResetColor();
                }
                else
                {
                    SetColorMultiply(disableColor);
                }
            }
        }

        /// <summary>
        /// 色を元に戻します。
        /// </summary>
        protected virtual void ResetColor()
        {
            if (!isEnableColorChange)
            {
                return;
            }

            if (graphicGroup != null)
            {
                graphicGroup.ResetColor();
            }
            else
            {
                ImageInfo.ResetColor();
            }
        }

        /// <summary>
        /// 色を乗算設定します。
        /// </summary>
        /// <param name="color">設定する色</param>
        public virtual void SetColorMultiply(Color color)
        {
            if (!isEnableColorChange)
            {
                return;
            }

            if (graphicGroup != null)
            {
                graphicGroup.SetColorMultiply(color);
            }
            else
            {
                ImageInfo.SetColorMultiply(color);
            }
        }

        /// <summary>
        /// ボタンの初期化を行います。
        /// </summary>
        protected override void Init()
        {
            base.Init();

            // 複数同時編集で参照がついていない可能性があるので、
            // nullの場合は一応確認する
            if (transitionComponent == null)
            {
                transitionComponent = GetComponent<ButtonTransitionBase>();
            }
            if (soundComponent == null)
            {
                soundComponent = GetComponent<ButtonSoundBase>();
            }
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            ChangeColor();
        }

        protected virtual void Update()
        {
            if (!colleague.IsEnable)
            {
                return;
            }

            if (isSelected)
            {
                selectedTime += Time.deltaTime;
                repeatIntervalTime -= Time.deltaTime;
                if (repeatIntervalTime <= 0.0f)
                {
                    OnKeyRepeat();
                }
                if (selectedTime >= LongPressJudgedTime)
                {
                    OnLongPress();
                }
                OnUpdateSelected();
            }
        }

#endregion
    }
}
