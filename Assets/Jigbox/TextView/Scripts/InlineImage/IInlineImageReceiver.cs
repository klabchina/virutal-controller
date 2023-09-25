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

namespace Jigbox.TextView
{
	/// <summary>
	/// IInlineImageProviderからインライン画像を受け取るクラスが実装すべきインターフェイスです。
	///
	/// Jigbox内のクラスが実装することを想定しており、Jigboxを利用するプロジェクトが実装することは想定していません。
	/// 具体的には以下のクラスが実装しています。
	/// - Jigbox.TextView.InlineImageLoader
	/// - Jigbox.TextView.InlineImageRepository
	/// </summary>
	public interface IInlineImageReceiver
	{
		/// <summary>
		/// 画像を送信します。
		/// </summary>
		///
		/// <param name="identifier">画像の識別子</param>
		/// <param name="sprite">Sprite形式の画像</param>
		void Send (string identifier, Sprite sprite);

		/// <summary>
		/// 画像を送信します。
		/// </summary>
		///
		/// <param name="identifier">画像の識別子</param>
		/// <param name="texture">Texture形式の画像</param>
		void Send (string identifier, Texture texture);

		/// <summary>
		/// エラーを通知します。
		/// </summary>
		///
		/// <param name="identifier">画像の識別子</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		void SendError (string identifier, string errorMessage);
	}
}
