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

using System.Linq;
using Jigbox.EditorUtils;
using UnityEditor;
using UnityEngine;

namespace Jigbox.Components
{
    /// <summary>
    /// 拡張コンポーネントのEditor拡張
    /// </summary>
    [CustomEditor(typeof(VirtualPadActivateExtensionBase), true)]
    public class VirtualPadActivateExtensionBaseEditor : Editor
    {
#region fields

        /// <summary> 拡張コンポーネントの参照 </summary>
        protected VirtualPadActivateExtensionBase virtualPadActivateExtensionBase;

        /// <summary>バーチャルパッドの参照</summary>
        protected VirtualPadController virtualPadController;

#endregion

#region protected methods

        /// <summary>
        /// 同じExtensionが複数のオブジェクトに紐づいていないか検索し、紐づいている場合はErrorを出します
        /// </summary>
        protected virtual void ValidAttachToAnyComponent()
        {
            var controllers = virtualPadActivateExtensionBase.GetComponents<VirtualPadController>();

            if (controllers.Count() <= 1)
            {
                return;
            }

            var attachCount = controllers.Where(c => c.VirtualPadActivateExtension != null).Count(c =>
                c.VirtualPadActivateExtension.GetHashCode() == virtualPadActivateExtensionBase.GetHashCode());

            if (attachCount >= 2)
            {
                Debug.LogError("VirtualPadActivateExtension can't register with more than one component");

                foreach (var controller in controllers)
                {
                    if (controller.GetHashCode() != controllers[0].GetHashCode())
                    {
                        controller.VirtualPadActivateExtension = null;
                    }
                }
            }
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            virtualPadActivateExtensionBase = target as VirtualPadActivateExtensionBase;

            virtualPadController = virtualPadActivateExtensionBase.GetComponent<VirtualPadController>();

            if (virtualPadController.VirtualPadActivateExtension == null)
            {
                virtualPadController.VirtualPadActivateExtension = virtualPadActivateExtensionBase;
            }

            EditorUtilsTools.RegisterUndo("Edit VirtualPad", GUI.changed, target);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            ValidAttachToAnyComponent();

            EditorUtilsTools.RegisterUndo("Edit VirtualPad", GUI.changed, target);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
