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

using Jigbox.Examples;
using UnityEngine;
using UnityEngine.UI;

public class ExampleWaveShakeController : ExampleSceneBase
{
    [SerializeField]
    ExampleWaveShakeComponent shakeComponent = null;

    [SerializeField]
    Text label = null;

    protected override void Awake()
    {
        base.Awake();
        UpdateLabel();
    }

    [AuthorizedAccess]
    void OnClickStart()
    {
        shakeComponent.shake.Start();
        UpdateLabel();
    }

    [AuthorizedAccess]
    void OnClickStop()
    {
        shakeComponent.shake.Complete();
        UpdateLabel();
    }

    [AuthorizedAccess]
    void OnClickPause()
    {
        shakeComponent.shake.Pause();
        UpdateLabel();
    }

    [AuthorizedAccess]
    void OnClickResume()
    {
        shakeComponent.shake.Resume();
        UpdateLabel();
    }

    void UpdateLabel()
    {
        label.text = shakeComponent.shake.State.ToString();
    }
}
