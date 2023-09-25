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

[RequireComponent(typeof(Text))]
public class TweenSliderLabel : MonoBehaviour
{
    public string TextToDisplay
    {
        get
        { 
            return selfText.text;
        }
        set
        {
            if (selfText.text != value)
            {
                selfText.text = value;
            }
        }
    }

    [SerializeField]
    TweenSlider slider = null;

    Text selfText;

    // Use this for initialization
    void Start()
    {
        selfText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        var mt = slider.Tween.MotionType;
        var et = slider.Tween.EasingType;
        TextToDisplay = mt == MotionType.Linear
            ? string.Format("{0}", mt)
            : string.Format("{0} {1}", mt, et);
    }
}
