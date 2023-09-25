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

namespace Jigbox.TextView
{
	/// <summary>
	/// Jigbox.TextViewにインライン画像を提供するクラスが実装すべきインターフェイスです。
	///
	/// Jigbox.TextViewを利用するプロジェクトで実装することを想定しています。
	/// 実装したクラスのインスタンスをTextView.InlineImageProviderに設定してください。
	/// </summary>
	public interface IInlineImageProvider
	{
		/// <summary>
		/// 画像の提供をリクエストします。
		///
		/// 画像の用意が整い次第、渡されたreceiverに対し、以下のいずれかのメソッドを呼び出してください
		///
		/// - Sprite形式の画像を用意した場合
		///     - Send(string identifier, Sprite sprite)
		/// - Texture形式の画像を用意した場合
		///     - Send(string identifier, Texture texture)
		/// - 画像を用意できない場合
		///     - SendError(string identifier, string errorMessage)
		///
		/// </summary>
		///
		/// <param name="identifier">画像の識別子</param>
		/// <param name="receiver">画像の提供先</param>
		///
		/// <remarks>
		/// SendErrorを呼びだした場合、receiverは自身の内部状態の更新のみを行います。
		/// エラーログ出力が必要な場合はプロジェクト側で出力してください。
		/// </remarks>
		void Request (string identifier, IInlineImageReceiver receiver);

		/// <summary>
		/// リクエスト中の画像の提供をキャンセルします。
		/// </summary>
		///
		/// <param name="identifier">画像の識別子</param>
		/// <param name="receiver">画像の提供先</param>
		void Cancel (string identifier, IInlineImageReceiver receiver);
	}
}
