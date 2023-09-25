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
using UnityEngine.UI;

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    public class TextView_InputFieldCaret : MaskableGraphic
    {
        public override void Cull(Rect clipRect, bool validRect)
        {
            // キャレットや文字選択はカリングされて消えると困る為カリング処理を削除している
            // キャレットや文字選択が描画されているということはInputFieldを使っている場合なので、カリングを削除している事の不都合はない
        }
    }
}
