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
using Jigbox.Tween;

namespace Jigbox.Examples
{
    public abstract class GestureExampleInfomationController : MonoBehaviour
    {
#region properties

        [SerializeField]
        protected Text infomation;

        [SerializeField]
        protected RectTransform labelParent;

        protected static Font font;

        protected static Font Font
        {
            get
            {
                if (font == null)
                {
                    font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                }
                return font;
            }
        }

#endregion

#region protected methods

        protected static void CreateLabel(string label, Color color, Transform parent, Vector3 position)
        {
            GameObject obj = new GameObject("GestureEventLable");
            Text text = obj.AddComponent<Text>();
            text.text = label;
            text.color = color;
            text.font = Font;
            text.fontSize = 18;
            text.alignment = TextAnchor.MiddleCenter;
            text.rectTransform.sizeDelta = new Vector2(160.0f, 30.0f);

            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = position;

            TweenVector3 tweenPosition = new TweenVector3();
            tweenPosition.Begin = position;
            Vector3 final = position;
            final.y += 20.0f;
            tweenPosition.Final = final;
            tweenPosition.Duration = 0.5f;
            tweenPosition.OnUpdate(t => obj.transform.localPosition = t.Value);

            TweenSingle tweenAlpha = new TweenSingle();
            tweenAlpha.Begin = 1.0f;
            tweenAlpha.Final = 0.0f;
            tweenAlpha.Duration = 0.5f;
            tweenAlpha.OnUpdate(t =>
            {
                Color c = text.color;
                c.a = t.Value;
                text.color = c;
            });
            tweenAlpha.OnComplete(_ => Destroy(obj));

            tweenPosition.Start();
            tweenAlpha.Start();
        }

#endregion
    }
}
