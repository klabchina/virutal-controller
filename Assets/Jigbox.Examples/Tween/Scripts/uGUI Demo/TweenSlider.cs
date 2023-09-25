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

/// <summary>
/// uGUIのSliderにトゥイーンアニメーションを加えるサンプルです
/// </summary>
[RequireComponent(typeof(Slider))]
public class TweenSlider : MonoBehaviour
{
    [SerializeField]
    TweenSingle tween;

    /// <summary>
    /// 外部のスクリプトから操作する為のトゥイーンプロパティです
    /// </summary>
    /// <value>The tween.</value>
    public ITween<float> Tween { get { return tween ?? (tween = new TweenSingle()); } }

    Slider slider;

#region Unity Method

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Start()
    {
        // トゥイーンが更新される度に同期する処理を追加します
        Tween.OnUpdate(tween =>
        {
            if (slider)
            {
                slider.value = tween.Value;
            }
        });
    }

    void Update()
    {
        // Update でトゥイーンを制御する必要はありません
    }

    void OnEnable()
    {
        Tween.Resume();
    }

    void OnDisable()
    {
        Tween.Pause();
    }

#endregion

#region Tween Control

    public void StartTween()
    {
        Tween.Begin = slider.minValue;
        Tween.Final = slider.maxValue;
        Tween.Start();
    }

    public void StartTween(float finalValue)
    {
        Tween.Begin = slider.minValue;
        Tween.Final = Mathf.Clamp(finalValue, slider.minValue, slider.maxValue);
        Tween.Start();
    }

    public void StartTween(float startValue, float finalValue)
    {
        Tween.Begin = Mathf.Clamp(startValue, slider.minValue, slider.maxValue);
        Tween.Final = Mathf.Clamp(finalValue, slider.minValue, slider.maxValue);
        Tween.Start();
    }

#endregion
}
