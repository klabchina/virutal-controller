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

namespace Jigbox.Components
{
	public interface IGaugeLimiter
	{
	}

	public interface IGaugeValueLimiter : IGaugeLimiter
	{
		/// <summary>
		/// GaugeのValue値が更新される時に呼び出されます。
		/// 設定したいValue値に制約をつけたい場合に使用します。
		/// </summary>
		/// <param name="currentValue">現在の値</param>
		/// <param name="newValue">設定しようとしている値</param>
		/// <returns>設定する値</returns>
		float LimitValue(float currentValue, float newValue);
	}

	public interface IGaugeStepLimiter : IGaugeLimiter
	{
		/// <summary>
		/// GaugeのCurrentStep値が更新される時に呼び出されます。
		/// 設定したいCurrentStep値に制約をつけたい場合に使用します。 
		/// </summary>
		/// <param name="steps">ゲージ全体の階調数</param>
		/// <param name="currentStep">現在の階調</param>
		/// <param name="nextStep">次の階調</param>
		/// <returns>設定する値</returns>
		int LimitCurrentStep(int steps, int currentStep, int nextStep);
	}
}
