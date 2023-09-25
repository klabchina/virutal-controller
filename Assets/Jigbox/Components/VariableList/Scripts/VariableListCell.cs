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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// 可変長リストのセルクラス
    /// </summary>
    public class VariableListCell : MonoBehaviour
    {
#region Serialize Fields

        /// <summary>
        /// サイズが可変するかどうか
        /// </summary>
        [SerializeField]
        bool isVariable = true;

        /// <summary>
        /// サイズが可変するかどうかのフラグへの参照
        /// </summary>
        public virtual bool IsVariable
        {
            get { return isVariable; }
        }

        /// <summary>
        /// セルのサイズ
        /// </summary>
        [SerializeField]
        float cellSize = 0.0f;

        /// <summary>
        /// セルのサイズへの参照
        /// </summary>
        public virtual float CellSize
        {
            get { return cellSize; }
            set { cellSize = value; }
        }

        /// <summary>
        /// セルの前方間隔
        /// </summary>
        [SerializeField]
        float spacingFront = 0.0f;

        /// <summary>
        /// セルの前方間隔への参照
        /// </summary>
        public virtual float SpacingFront
        {
            get { return spacingFront; }
            set { spacingFront = value; }
        }

        /// <summary>
        /// セルの後方間隔
        /// </summary>
        [SerializeField]
        float spacingBack = 0.0f;

        /// <summary>
        /// セルの後方間隔への参照
        /// </summary>
        public virtual float SpacingBack
        {
            get { return spacingBack; }
            set { spacingBack = value; }
        }

#endregion

#region fields & properties

        /// <summary>
        /// セルの番号
        /// </summary>
        int index = int.MaxValue;

        /// <summary>
        /// セルの番号への参照
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// セルのRectTransform
        /// </summary>
        RectTransform rectTransform = null;

        /// <summary>
        /// セルのRectTransformへの参照
        /// </summary>
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = GetComponent<RectTransform>();
                }

                return rectTransform;
            }
        }

        /// <summary>
        /// セルの生成プロパティ
        /// セルの生成方法を変える場合はこのプロパティをoverrideすること
        /// </summary>
        public virtual Func<Transform,VariableListCell> Generator
        {
            get { return (parent) => Instantiate(this, parent, false); }
        }

#endregion
    }
}
