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
using UnityEngine.EventSystems;

namespace Jigbox.Examples
{
    public class TestButtonEventController : ExampleSceneBase
    {
#region properties

        [SerializeField]
        Text text = null;

        int clickCount = 0;

#pragma warning disable 414    
        [AuthorizedAccess]
        [SerializeField]
        int value = 0;

        [AuthorizedAccess]
        [SerializeField]
        string str = "";
#pragma warning restore 414
        
#endregion

#region private methods

        [AuthorizedAccess]
        void OnClick()
        {
            ++clickCount;
            text.text = "ClickCount : " + clickCount;
        }

        [AuthorizedAccess]
        void Func1()
        {
            Debug.Log("Called Event\nObject Name : " + name);
        }

        [AuthorizedAccess]
        void Func1(int v)
        {
            Debug.Log("value : " + v);
        }

        [AuthorizedAccess]
        void Func1(string s)
        {
            Debug.Log("string : " + s);
        }

        [AuthorizedAccess]
        void Func2(int v)
        {
            Debug.Log("value : " + v);
        }

        [AuthorizedAccess]
        void Func3(string s)
        {
            Debug.Log("string : " + s);
        }

        [AuthorizedAccess]
        void GetPointerData(PointerEventData p)
        {
            Debug.Log("x : " + p.position.x + "\ny : " + p.position.y);
        }

        [AuthorizedAccess]
        void GetEventData(BaseEventData e)
        {
            Debug.Log(e.ToString());
        }

#endregion

#region override unity methods

        protected override void Awake()
        {
            base.Awake();
            text.text = "ClickCount : " + clickCount;
        }

#endregion
    }
}
