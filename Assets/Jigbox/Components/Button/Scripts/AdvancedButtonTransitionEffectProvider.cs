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

namespace Jigbox.Components
{
    /// <summary>
    /// AdvancedButtonTransition のエフェクト生成の標準処理
    /// </summary>
    /// <remarks>押下毎にエフェクトを生成するだけのシンプルな実装です</remarks>
    public class AdvancedButtonTransitionEffectProvider : IAdvancedButtonTransitionEffectProvider
    {
        /// <summary> 生成されるエフェクトのテンプレート</summary>
        protected GameObject template;
        
        /// <summary> 複製したエフェクトのゲームオブジェクト</summary>
        protected GameObject instance;
        
        /// <summary> 複製したエフェクトのイベント実行インターフェース</summary>
        protected IAdvancedButtonTransitionEffect component;

        public AdvancedButtonTransitionEffectProvider(GameObject template)
        {
            this.template = template;
        }

        public virtual void OnAwake(AdvancedButtonTransition transition)
        {
        }

        public virtual void OnTransition(AdvancedButtonTransition transition, InputEventType type)
        {
            if (type == InputEventType.OnSelect)
            {
                OnPressTransition(transition);
            }
            if (component != null)
            {
                component.OnTransition(transition, type);
            }
        }

        protected virtual void OnPressTransition(AdvancedButtonTransition transition)
        {
            if (instance != null)
            {
                Object.Destroy(instance);
                component = null;
            }
            if (template != null)
            {
                instance = Object.Instantiate(template, transition.transform);
                component = instance.GetComponent(typeof(IAdvancedButtonTransitionEffect)) as IAdvancedButtonTransitionEffect;
            }
        }

        public virtual void OnNoticeAutoUnlock(AdvancedButtonTransition transition)
        {
            OnTransition(transition, InputEventType.OnDeselect);
        }

        public virtual void OnStopTransition(AdvancedButtonTransition transition)
        {
            if (component != null)
            {
                component.OnStopTransition(transition);
            }
        }

        public virtual void OnDestroy(AdvancedButtonTransition transition)
        {
            if (instance != null)
            {
                Object.Destroy(instance);
            }
        }
    }
}
