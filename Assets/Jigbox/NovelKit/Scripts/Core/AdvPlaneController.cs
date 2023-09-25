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

namespace Jigbox.NovelKit
{
    public class AdvPlaneController : MonoBehaviour
    {
#region properties

        /// <summary>Canvas Group</summary>
        [SerializeField]
        protected CanvasGroup group;

        /// <summary>プレーンのレベル</summary>
        public int Level { get; set; }
        
        /// <summary>Transform</summary>
        protected Transform localTransform;

        /// <summary>Transform</summary>
        public Transform LocalTransform
        {
            get
            {
                if (localTransform == null)
                {
                    localTransform = transform;
                }
                return localTransform;
            }
        }

        /// <summary>RectTransform</summary>
        protected RectTransform rectTransform;

        /// <summary>RectTransform</summary>
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = transform as RectTransform;
                }
                return rectTransform;
            }
        }

        /// <summary>プレーン内におけるDepth値の最大</summary>
        public int DepthMax
        {
            get{ return LocalTransform.childCount != 0 ? LocalTransform.childCount - 1 : 0; }
        }

#endregion

#region public methods

        /// <summary>
        /// オブジェクトをプレーン以下に配置します。
        /// </summary>
        /// <param name="obj">配置するオブジェクト</param>
        public virtual void SetObject(AdvObjectBase obj)
        {
            obj.LocalTransform.SetParent(LocalTransform, false);
            obj.Plane = this;
            if (obj is AdvObject2D)
            {
                (obj as AdvObject2D).UpdateDepth();
            }
        }
        
#endregion
    }
}
