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
using System.Collections.Generic;
using Jigbox.Delegatable;

namespace Jigbox.Components
{
    public class ToggleGroup : MonoBehaviour, IToggleValueChangeValidator
    {
#region inner classes, enum, and structs

        /// <summary>
        /// トグルグループの標準デリゲート型
        /// </summary>
        public class ToggleGroupDelegate : EventDelegate<int>
        {
            public ToggleGroupDelegate(Callback callback) : base(callback)
            {
            }
        }

#endregion

#region properties

        /// <summary>グループ対象のトグルコンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected List<BasicToggle> toggles = new List<BasicToggle>();

        /// <summary>現在アクティブとなっているトグルコンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected BasicToggle activeToggle = null;

        /// <summary>現在アクティブとなっているトグルコンポーネント</summary>
        public virtual BasicToggle ActiveToggle { get { return activeToggle; } }

        /// <summary>現在アクティブとなっているトグルのインデックス</summary>
        public virtual int ActiveToggleIndex
        {
            get
            {
                return toggles.IndexOf(activeToggle);
            }
        }

        /// <summary>デリゲート</summary>
        [HideInInspector]
        [SerializeField]
        protected DelegatableList onActiveToggleChangedDelegates = new DelegatableList();

        /// <summary>デリゲート</summary>
        public virtual DelegatableList OnActiveToggleChangedDelegates
        {
            get { return onActiveToggleChangedDelegates; }
        }

        /// <summary>
        /// 管理下のトグル数を返します。
        /// </summary>
        public virtual int Count
        {
            get
            {
                return toggles.Count;
            }
        }

        /// <summary>プロバイダーの参照</summary>
        protected IInstanceProvider<BasicToggle> toggleProvider;

        /// <summary>
        /// 任意の方法でBasicToggleをToggleGroupに渡すプロバイダーを参照/指定します
        /// </summary>
        /// <value>The toggle provider.</value>
        public virtual IInstanceProvider<BasicToggle> ToggleProvider
        {
            get { return toggleProvider; }
            set { toggleProvider = value; }
        }

        /// <summary>ディスポーザの参照</summary>
        protected IInstanceDisposer<BasicToggle> toggleDisposer = new ToggleInstanceDisposer();

        /// <summary>
        /// 任意の方法でBasicToggleを処分するディスポーザを参照/指定します
        /// </summary>
        public virtual IInstanceDisposer<BasicToggle> ToggleDisposer
        {
            get { return toggleDisposer; }
            set { toggleDisposer = value; }
        }

#endregion

#region public methods

        /// <summary>
        /// アクティブなトグルが変更された際のコールバックを追加します。
        /// </summary>
        /// <param name="callback">void Func(int)の関数</param>
        public virtual void AddEvent(ToggleGroupDelegate.Callback callback)
        {
            onActiveToggleChangedDelegates.Add(new ToggleGroupDelegate(callback));
        }

        /// <summary>
        /// 管理下のトグルリストの末尾にトグルを追加します。
        /// </summary>
        /// <param name="toggle">追加するトグル</param>
        /// <param name="callback">追加完了後に呼び出すコールバック。nullの場合はデフォルト処理(トグルを自身の子要素として設定)を行います。</param>
        public virtual void Add(BasicToggle toggle, Action<BasicToggle> callback = null)
        {
            toggle.IsOn = false;

            SetToggleUnderControl(toggle);

            toggles.Add(toggle);

            if (callback == null)
            {
                toggle.transform.SetParent(this.transform, false);
            }
            else
            {
                callback(toggle);
            }
        }

        /// <summary>
        /// 管理下のトグルリストの末尾にトグルを追加します。
        /// </summary>
        /// <remarks>
        /// BasicToggleをToggleProvider経由で生成します。
        /// </remarks>
        public virtual void Add()
        {
            if (toggleProvider == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Can not add because ToggleProvider is null.");
#endif
                return;
            }

            var toggle = toggleProvider.Generate();
            if (toggle == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Failed Generate.");
#endif
                return;
            }

            toggle.IsOn = false;

            SetToggleUnderControl(toggle);

            toggles.Add(toggle);
        }

        /// <summary>
        /// 管理下のトグルリストからすべてのトグルを削除します。
        /// </summary>
        /// <param name="callback">削除完了後に呼び出すコールバック。nullの場合はデフォルト処理(ToggleDisposerを使用してBasicToggleがついているGameObjectを削除)を行います。</param>
        public virtual void Clear(Action<BasicToggle> callback = null)
        {
            // あとでコールバックを呼び出す必要があるので、コピーを取っておく
            var togglesCopy = new List<BasicToggle>(toggles);

            for (int i = 0; i < toggles.Count; ++i)
            {
                UnsetToggleUnderControl(toggles[i]);
            }

            toggles.Clear();
            activeToggle = null;

            for (int i = 0; i < togglesCopy.Count; ++i)
            {
                if (callback == null)
                {
                    toggleDisposer.Dispose(togglesCopy[i]);
                }
                else
                {
                    callback(togglesCopy[i]);
                }
            }
        }

        /// <summary>
        /// トグルをアクティブにします。
        /// </summary>
        /// <param name="index">アクティブにするトグルのインデックス</param>
        public virtual void SetActive(int index)
        {
            if (index < 0 || index >= toggles.Count)
            {
#if UNITY_EDITOR
                Debug.LogError("ToggleGroup.SetActive : Invalid index!");
#endif
                return;
            }
            if (activeToggle == toggles[index])
            {
                return;
            }

            activeToggle = toggles[index];

            for (int i = 0; i < toggles.Count; ++i)
            {
                toggles[i].IsOn = i == index;
            }

            if (onActiveToggleChangedDelegates.Count > 0)
            {
                onActiveToggleChangedDelegates.Execute(index);
            }
        }

        /// <summary>
        /// トグルをアクティブにします。
        /// </summary>
        /// <param name="toggle"></param>
        public virtual void SetActive(BasicToggle toggle)
        {
            for (int i = 0; i < toggles.Count; ++i)
            {
                if (toggle == toggles[i])
                {
                    SetActive(i);
                    return;
                }
            }

#if UNITY_EDITOR
            Debug.LogError("ToggleGroup.SetActive : Target not found!");
#endif
        }

#endregion

#region protected methods

        /// <summary>
        /// グループ内のトグルの状態が変化した際に呼び出されます。
        /// </summary>
        /// <param name="toggle">状態が変化したトグル</param>
        protected virtual void OnChangeValue(BasicToggle toggle)
        {
            if (toggle.IsOn)
            {
                SetActive(toggle);
            }
        }

        /// <summary>
        /// トグルを管理下に置きます。
        /// </summary>
        /// <param name="toggle">対象のトグル</param>
        protected virtual void SetToggleUnderControl(BasicToggle toggle)
        {
            toggle.OnValueChangedDelegates.Add(this, "OnChangeValue", toggle);
            if (toggle.ValueChangeValidator == null)
            {
                toggle.ValueChangeValidator = CanChangeValue;
            }
        }

        /// <summary>
        /// トグルを管理下から外します。
        /// </summary>
        /// <param name="toggle">対象のトグル</param>
        protected virtual void UnsetToggleUnderControl(BasicToggle toggle)
        {
            toggle.OnValueChangedDelegates.Remove(this, "OnChangeValue");
            if (toggle.ValueChangeValidator == CanChangeValue)
            {
                toggle.ValueChangeValidator = null;
            }
        }

#endregion

#region override unity methods

        protected virtual void Start()
        {
            if (activeToggle != null)
            {
                for (int i = 0; i < toggles.Count; ++i)
                {
                    toggles[i].IsOn = activeToggle == toggles[i];
                }
            }

            for (int i = 0; i < toggles.Count; ++i)
            {
                SetToggleUnderControl(toggles[i]);
            }
        }

#endregion

#region IToggleValueChangeValidator

        /// <summary>
        /// BasicToggleの値が更新される前に呼び出されます。
        /// trueを返した場合、値が更新されますが、falseを返した場合は更新されません。
        /// </summary>
        public virtual bool CanChangeValue(BasicToggle toggle, bool newValue)
        {
            return toggle != activeToggle || newValue == true;
        }

#endregion
    }
}
