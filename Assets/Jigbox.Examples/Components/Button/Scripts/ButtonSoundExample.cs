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
using System;
using Jigbox.Components;

namespace Jigbox.Examples
{
    public sealed class ButtonSoundExample : ButtonSoundBase
    {
#region properties
        
        public override Type SoundDefinitionClass { get { return typeof(SoundEvents.Se); } }

#endregion

#region public methods

        protected override void PlaySound(string eventName)
        {
            Debug.Log("PlaySound\nEvent Name : " + eventName);
        }

        /// <summary>
        /// サウンド定義クラスのダミー
        /// 実際に作る場合は、きちんと専用のファイルに定義すべき
        /// </summary>
        public static class SoundEvents
        {
            public static class Se
            {
                public static class Common
                {
                    public static readonly string Yes = "Play_SE_YES";
                    public static readonly string No = "Play_SE_NO";
                }

                public static class Quest
                {
                    public static readonly string Attack = "Play_SE_ATTACK";
                    public static readonly string Guard = "Play_SE_GUARD";
                }
            }

            public static class Bgm
            {
                public static readonly string Bgm1 = "Play_BGM_01";
                public static readonly string Bgm2 = "Play_BGM_02";
                public static readonly string Bgm3 = "Play_BGM_03";
                public static readonly string Bgm4 = "Play_BGM_04";
            }
        }

#endregion
    }
}
