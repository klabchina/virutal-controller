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
using Jigbox.TextView;

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    public class TextViewLayoutElement : UIBehaviour, ILayoutElement, ILayoutIgnorer
    {
#region properties

        /// <summary>レイアウトを無効化するかどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool isIgnore = false;

        /// <summary>TextView</summary>
        [HideInInspector]
        [SerializeField]
        protected TextView textView = null;

        /// <summary>必要な幅を求める方法</summary>
        [HideInInspector]
        [SerializeField]
        protected PreferredWidthType preferredWidthType = PreferredWidthType.FirstLine;

        /// <summary>必要な幅を求める方法</summary>
        public PreferredWidthType PreferredWidthType
        {
            get { return preferredWidthType; }
        }

        /// <summary>必要な高さを求める方法</summary>
        [HideInInspector]
        [SerializeField]
        protected PreferredHeightType preferredHeightType = PreferredHeightType.AllLine;

        /// <summary>必要な高さを求める方法</summary>
        public PreferredHeightType PreferredHeightType
        {
            get { return preferredHeightType; }
        }

        /// <summary>レイアウトを無効化するかどうか</summary>
        public bool ignoreLayout { get { return isIgnore; } set { isIgnore = value; } }

        /// <summary>利用可能なスペースが確保されている状態でレイアウトが割り当てられる相対的な幅</summary>
        public virtual float flexibleWidth { get { return 0.0f; } }

        /// <summary>利用可能なスペースが確保されている状態でレイアウトが割り当てられる相対的な高さ</summary>
        public virtual float flexibleHeight { get { return 0.0f; } }

        /// <summary>レイアウトに必要な最小の幅</summary>
        public virtual float minWidth { get { return 0.0f; } }

        /// <summary>レイアウトに必要な最小の高さ</summary>
        public virtual float minHeight { get { return 0.0f; } }

        /// <summary>テキストを表示するのに必要な幅</summary>
        protected float cachedPreferredWidth = 0.0f;

        /// <summary>テキストを表示するのに必要な幅</summary>
        public virtual float preferredWidth { get { return cachedPreferredWidth; } }

        /// <summary>テキストを表示するのに必要な高さ</summary>
        protected float cachedPreferredHeight = 0.0f;
        
        /// <summary>テキストを表示するのに必要な高さ</summary>
        public virtual float preferredHeight { get { return cachedPreferredHeight; } }

        /// <summary>レイアウトの優先度</summary>
        public virtual int layoutPriority { get { return 0; } }

#endregion

#region public methods

        /// <summary>
        /// レイアウトに必要な幅に関するパラメータを計算します。
        /// </summary>
        public virtual void CalculateLayoutInputHorizontal()
        {
            if (textView == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("TextViewLayoutElement.CalculateLayoutInputHorizontal : TextView not found!");
#endif
                return;
            }

            cachedPreferredWidth = textView.GetPreferredWidth(preferredWidthType);
        }

        /// <summary>
        /// レイアウトに必要な高さに関するパラメータを計算します。
        /// </summary>
        public virtual void CalculateLayoutInputVertical()
        {
            if (textView == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("TextViewLayoutElement.CalculateLayoutInputVertical : TextView not found!");
#endif
                return;
            }

            cachedPreferredHeight = textView.GetPreferredHeight(preferredHeightType);
        }

#endregion
    }
}
