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
using Jigbox.Delegatable;

namespace Jigbox.Components
{
    public class BasicToggle : MonoBehaviour
    {
#region inner classes, enum, and structs

        /// <summary>
        /// トグルの標準デリゲート型
        /// </summary>
        public class ToggleDelegate : EventDelegate<bool>
        {
            public ToggleDelegate(Callback callback) : base(callback)
            {
            }
        }

#endregion

#region properties

        /// <summary>トグル状態</summary>
        [HideInInspector]
        [SerializeField]
        protected bool isOn = true;

        /// <summary>トグル状態</summary>
        [AuthorizedAccessAttribute]
        public virtual bool IsOn
        {
            get
            {
                return isOn;
            }
            set
            {
                if (isOn == value)
                {
                    return;
                }

                if (ValueChangeValidator != null && !ValueChangeValidator(this, value))
                {
                    return;
                }

                isOn = value;

                OnUpdateIsOn();

                if (OnValueChangedDelegates.Count > 0)
                {
                    OnValueChangedDelegates.Execute(isOn);
                }
            }
        }

        /// <summary>デリゲート</summary>
        [HideInInspector]
        [SerializeField]
        protected DelegatableList onValueChangedDelegates = new DelegatableList();

        /// <summary>デリゲート</summary>
        public virtual DelegatableList OnValueChangedDelegates
        {
            get { return onValueChangedDelegates; }
        }

        /// <summary>値更新バリデータ</summary>
        [HideInInspector]
        [SerializeField]
        protected MonoBehaviour valueChangeValidationTarget = null;

        /// <summary>値更新バリデータ</summary>
        public virtual System.Func<BasicToggle, bool, bool> ValueChangeValidator { get; set; }

#endregion

#region public methods

        /// <summary>
        /// トグルの状態が更新された際のコールバックを追加します。
        /// </summary>
        /// <param name="callback">void Func(bool)の関数</param>
        public virtual void AddEvent(ToggleDelegate.Callback callback)
        {
            onValueChangedDelegates.Add(new ToggleDelegate(callback));
        }

#endregion
        
#region protected methods

        /// <summary>
        /// BasicButton.OnClickデリゲートに登録するコールバック
        /// </summary>
        [AuthorizedAccess]
        protected virtual void OnClick()
        {
            IsOn = !IsOn;
        }

        /// <summary>
        /// IsOnの値が更新され、OnValueChangedデリゲート呼び出し前に呼び出されます。
        /// </summary>
        protected virtual void OnUpdateIsOn()
        {
        }

        /// <summary>
        /// 内部状態に合わせて表示を更新します
        /// </summary>
        public virtual void UpdateDisplay()
        {
        }

#endregion

#region unity method

        protected virtual void Awake()
        {
            if (valueChangeValidationTarget != null)
            {
                var target = valueChangeValidationTarget as IToggleValueChangeValidator;
                if (target != null)
                {
                    ValueChangeValidator = target.CanChangeValue;
                }
            }
        }

#endregion
    }
}
