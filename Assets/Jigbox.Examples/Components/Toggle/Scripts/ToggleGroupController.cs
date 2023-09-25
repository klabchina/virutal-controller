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
using Jigbox.Components;

namespace Jigbox.Examples
{
    public sealed class ToggleGroupController : ExampleSceneBase
    {
#region inner classes

        sealed class ToggleProvider : IInstanceProvider<BasicToggle>
        {
            public GameObject Prefab;

            public Components.ToggleGroup ToggleGroup;

            public BasicToggle Generate()
            {
                var obj = Instantiate(Prefab);
                var childrenNumber = ToggleGroup.Count + 1;
                obj.name = "Row " + childrenNumber;
                var text = obj.GetComponentInChildren<Text>();
                text.text = "Toggle " + childrenNumber;
                obj.transform.SetParent(ToggleGroup.transform, false);

                Debug.Log("ToggleProvider Generate");
                return obj.GetComponentInChildren<BasicToggle>();
            }
        }

        sealed class ToggleDisposer : ToggleInstanceDisposer
        {
            public override void Dispose(BasicToggle target)
            {
                var parentObj = target.transform.parent.gameObject;
                base.Dispose(target);
                Destroy(parentObj);
                Debug.Log("ToggleDisposer Dispose");
            }
        }

#endregion

#region fields

        [SerializeField]
        GameObject togglePrefab = null;

        [SerializeField]
        Components.ToggleGroup toggleGroup = null;

        ToggleProvider provider = new ToggleProvider();

        ToggleDisposer disposer = new ToggleDisposer();

#endregion

#region private methods

        GameObject CreateToggle()
        {
            var childrenNumber = toggleGroup.Count + 1;

            var newToggle = Instantiate(togglePrefab);
            newToggle.name = "Row " + childrenNumber;

            var text = newToggle.GetComponentInChildren<Text>();
            text.text = "Toggle " + childrenNumber;

            return newToggle;
        }

        [AuthorizedAccess]
        void OnAddButtonClick()
        {
            var newToggle = CreateToggle();

            toggleGroup.Add(newToggle.GetComponentInChildren<BasicToggle>(), 
                (toggle) =>
                {
                    toggle.transform.parent.SetParent(toggleGroup.transform, false);
                }
            );
        }

        [AuthorizedAccess]
        void OnAddButtonClickUseProvider()
        {
            toggleGroup.Add();
        }

        [AuthorizedAccess]
        void OnClearButtonClick()
        {
            toggleGroup.Clear(
                (toggle) =>
                {
                    Destroy(toggle.transform.parent.gameObject);
                }
            );
        }

        [AuthorizedAccess]
        void OnClearButtonClickUseDisposer()
        {
            toggleGroup.Clear();
        }

        [AuthorizedAccess]
        void OnActiveToggleChanged()
        {
            Debug.Log("New Active Toggle is " + toggleGroup.ActiveToggle.name);
        }

#endregion

#region unity methods

        protected override void Awake()
        {
            base.Awake();
            provider.Prefab = togglePrefab;
            provider.ToggleGroup = toggleGroup;

            toggleGroup.ToggleProvider = provider;
            toggleGroup.ToggleDisposer = disposer;
        }

#endregion
    }
}
