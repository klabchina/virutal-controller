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

namespace Jigbox.Components
{
    [System.Serializable]
    public class GaugeModel
    {
#region inner classes, enum, and structs

        /// <summary>
        /// フィリングを行う際に対象となるコンポーネント
        /// </summary>
        public enum FillTargetComponent
        {
            /// <summary>RectTransform</summary>
            RectTransform = 0,
            /// <summary>Image</summary>
            Image = 1,
        }

#endregion

#region constants

        /// <summary>計算に失敗した場合に返される値</summary>
        public static readonly int FailedValue = -1;

#endregion

#region properties

        /// <summary>ゲージ全体の階調数(0で無階調)</summary>
        [HideInInspector]
        [SerializeField]
        protected int steps = 0;

        /// <summary>ゲージ全体の階調数(0で無階調)</summary>
        public int Steps { get { return steps; } set { steps = value > 0 ? value : 0; } }

        /// <summary>フィリング方法</summary>
        [HideInInspector]
        [SerializeField]
        protected int fillMethod = 0;

        /// <summary>フィリング方法</summary>
        public int FillMethod { get { return fillMethod; } }

        /// <summary>フィリングを行う際に対象となるコンポーネント</summary>
        [HideInInspector]
        [SerializeField]
        protected FillTargetComponent fillTarget = FillTargetComponent.RectTransform;

        /// <summary>フィリングを行う際に対象となるコンポーネント</summary>
        public FillTargetComponent FillTarget { get { return fillTarget; } }

        /// <summary>フィリングを行う際に基準となる点</summary>
        [HideInInspector]
        [SerializeField]
        protected int fillOrigin = 0;

        /// <summary>フィリングを行う際に基準となる点</summary>
        public int FillOrigin { get { return fillOrigin; } }

        /// <summary>1階調あたりの値</summary>
        protected float StepValue { get { return 1.0f / steps; } }

#endregion

#region public methods

        /// <summary>
        /// 階調数から値を取得します。
        /// </summary>
        /// <param name="step">階調数</param>
        /// <returns>ゲージの値、ゲージ全体の階調数が0の場合、FailedValueが返ります。</returns>
        public float GetValueFromStep(int step)
        {
            if (steps == 0)
            {
                return FailedValue;
            }

            step = Mathf.Clamp(step, 0, steps);
            // 誤差が出ないように最大値の場合は1.0を直接返す
            if (step == steps)
            {
                return 1.0f;
            }
            return StepValue * step;
        }

        /// <summary>
        /// 値から近似値を取る階調を取得します。
        /// </summary>
        /// <param name="value">値</param>
        /// <returns>階調、ゲージ全体の階調数が0の場合、FailedValueが返ります。</returns>
        public int GetStepFromValue(float value)
        {
            if (steps == 0)
            {
                return FailedValue;
            }

            value = Mathf.Clamp01(value);
            return GetStepFromValue(value, steps);
        }

        /// <summary>
        /// 階調数から値を取得します。
        /// </summary>
        /// <param name="step">取得する階調数</param>
        /// <param name="steps">ゲージの階調数</param>
        /// <returns>値</returns>
        public static float GetValueFromStep(int step, int steps)
        {
            step = Mathf.Clamp(step, 0, steps);
            if (step == steps)
            {
                return 1.0f;
            }
            return (1.0f / steps) * step;
        }

        /// <summary>
        /// 値から近似値を取る階調を取得します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="steps">ゲージの階調数</param>
        /// <returns>階調</returns>
        public static int GetStepFromValue(float value, int steps)
        {
            float stepValue = 1.0f / steps;
            int divisionValue = Mathf.RoundToInt(value / stepValue);

            int step = divisionValue;
            float diff = Mathf.Abs(value - (stepValue * step));

            int prevStep = divisionValue - 1;
            if (prevStep >= 0)
            {
                float prevDiff = Mathf.Abs(value - (stepValue * prevStep));
                if (prevDiff < diff)
                {
                    step = prevStep;
                    diff = prevDiff;
                }
            }

            int nextStep = divisionValue + 1;
            if (nextStep <= steps)
            {
                float nextDiff = Mathf.Abs(value - (stepValue * nextStep));
                if (nextDiff <= diff)
                {
                    step = nextStep;
                    diff = nextDiff;
                }
            }

            return step;
        }

#endregion
    }
}
