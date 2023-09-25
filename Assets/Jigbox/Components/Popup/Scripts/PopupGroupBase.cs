﻿/**
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

namespace Jigbox.Components
{
    public class PopupGroupBase : PopupBase
    {
#region properties

        /// <summary>
        /// グループ名を返します
        /// </summary>
        public string GroupName { get; protected set; }

        /// <summary>
        /// 所属するグループのシーン名を返します
        /// </summary>
        public string SceneName { get; protected set; }

#endregion

#region public methods

        public override void Init(PopupView view, PopupOrder order, int priority)
        {
            // Viewがグルーピング対応のViewだった場合Group名とKeyをキャッシュする
            if (view is PopupGroupView)
            {
                var groupView = view as PopupGroupView;
                GroupName = groupView.GroupName;
                SceneName = groupView.BelongSceneName;
            }

            base.Init(view, order, priority);
        }

#endregion
    }
}
