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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jigbox.TextView
{
    /// <summary>
    /// TextViewの縦方向アライメントの表示モードに関する列挙型です。
    /// </summary>
    public enum TextAlignMode
    {
        /// <summary>
        /// 配置される予定の文字情報を元に縦方向の配置座標を決定します。
        /// g,j,yなどの文字とABCなどの文字が混在するかしないかによって、配置座標が変わります。
        /// </summary>
        Placement,
        
        /// <summary>
        /// 使用されているフォントの情報を元に縦方向の配置座標を決定します。
        /// g,j,yなども文字とABCなどの文字が混在するしないに関わらず、配置座標が一定になります。
        /// </summary>
        Font,
    }
}
