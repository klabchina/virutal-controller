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

namespace Jigbox.Examples
{
    /// <summary>
    /// Cellのサンプル用クラス
    /// </summary>
    public sealed class ExampleCell : MonoBehaviour, IPointerClickHandler
    {
#region properties
        
        /// <summary>
        /// テキスト
        /// </summary>
        [SerializeField]
        Text text = null;

#endregion

#region public methods

        /// <summary>
        /// 初期化用メソッド
        /// </summary>
        public void Initialize(string textValue)
        {
            text.text = textValue;
        }

        /// <summary>
        /// ボタンを押下し、ボタン上で指が離された際に呼び出されます
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log(text.text + " Click!");
        }

#endregion
    }
}
