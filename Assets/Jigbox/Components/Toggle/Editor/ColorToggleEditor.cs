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
using UnityEditor;

namespace Jigbox.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ColorToggle), true)]
    public class ColorToggleEditor : BasicToggleEditor
    {
#region properties

        /// <summary>ボタン</summary>
        protected BasicButton basicButton;

#endregion

#region override unity methods

        public override void OnEnable()
        {
            base.OnEnable();

            if (targets.Length == 1)
            {
                var component = target as Component;
                basicButton = component.GetComponent<BasicButton>();
            }
            else
            {
                if (Selection.activeGameObject != null)
                {
                    var component = Selection.activeGameObject.GetComponent<Component>();
                    basicButton = component.GetComponent<BasicButton>();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            basicButton.IsSyncColor = false;
        }

#endregion
    }
}
