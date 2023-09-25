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
using Jigbox.UIControl;

namespace Jigbox.Examples
{
    [ExecuteInEditMode]
    public sealed class RectTransformUitlsExampleController : ExampleSceneBase
    {
#region properties

        /// <summary>RectTransformUtilsでの編集対象</summary>
        [SerializeField]
        RectTransform target1 = null;

        /// <summary>ベタにRectTransformを編集する対象</summary>
        [SerializeField]
        RectTransform target2 = null;

#endregion

#region private methods

        /// <summary>
        /// アンカーの設定ボタンが押された際に呼び出されます。
        /// </summary>
        [AuthorizedAccess]
        void OnClickSetAnchor()
        {
            RectTransformUtils.SetAnchor(target1, RectTransformUtils.AnchorPoint.StretchFull);
            target2.anchorMin = Vector2.zero;
            target2.anchorMax = Vector2.one;
        }

        /// <summary>
        /// ピボットの設定ボタンが押された際に呼び出されます。
        /// </summary>
        [AuthorizedAccess]
        void OnClickSetPivot()
        {
            RectTransformUtils.SetPivot(target1, Vector2.zero);
            target2.pivot = Vector2.zero;
        }

        /// <summary>
        /// リセットボタンが押された際に呼び出されます。
        /// </summary>
        [AuthorizedAccess]
        void OnClickReset()
        {
            RectTransformUtils.SetAnchor(target1, RectTransformUtils.AnchorPoint.Center);
            RectTransformUtils.SetPivot(target1, new Vector2(0.5f, 0.5f));

            RectTransformUtils.SetAnchor(target2, RectTransformUtils.AnchorPoint.Center);
            RectTransformUtils.SetPivot(target2, new Vector2(0.5f, 0.5f));
            RectTransformUtils.SetSize(target2, new Vector2(100.0f, 100.0f));
            target2.transform.localPosition = Vector3.zero;
        }

#endregion
    }
}
