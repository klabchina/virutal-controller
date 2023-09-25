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
using System.Collections;
using System.Collections.Generic;
using Jigbox.TextView;

namespace Jigbox.Examples
{
	/// <summary>
	/// Jigbox.Exampleシーン用のInlineImageProviderの実装です。
	/// スプライトとテクスチャの非同期読み込みをサポートしています。
	/// マルチプルスプライトには対応していません。(最初のスプライトが読み込まれます)
	/// マルチプルスプライトの実装方法はJigbox.TextView.DefaultImageProviderを参照してください。
	/// </summary>
	public class ExampleInlineImageProvider : MonoBehaviour, IInlineImageProvider
	{
		private RequestInfo requestInfo;

		public ExampleInlineImageProvider ()
		{
			this.requestInfo = new RequestInfo ();
		}

		public void Request (string identifier, IInlineImageReceiver receiver)
		{
			if (this.requestInfo.Exists (identifier, receiver) == false) {
				var coroutine = StartCoroutine (Load (identifier, receiver));
				this.requestInfo.Add (identifier, receiver, coroutine);
			}
		}

		public void Cancel (string identifier, IInlineImageReceiver receiver)
		{
			var coroutine = this.requestInfo.Get (identifier, receiver);
			if (coroutine != null) {
				StopCoroutine (coroutine);
				this.requestInfo.Remove (identifier, receiver);
			}
		}

		private IEnumerator Load (string identifier, IInlineImageReceiver receiver)
		{
			yield return new WaitForFixedUpdate ();

			var sprite = Resources.Load (identifier, typeof(Sprite)) as Sprite;
			if (sprite != null) {
				receiver.Send (identifier, sprite);
			} else {
				var texture = Resources.Load (identifier, typeof(Texture)) as Texture;
				if (texture != null) {
					receiver.Send (identifier, texture);
				} else {
					var errorMessage = "Sprite or Texture " + identifier + " is not found in Resources.";
					receiver.SendError (identifier, errorMessage);
				}
			}

			this.requestInfo.Remove (identifier, receiver);
		}
	}

	internal class RequestInfo
	{
		private Dictionary<string, Dictionary<IInlineImageReceiver, Coroutine>> requests;

		public RequestInfo ()
		{
			this.requests = new Dictionary<string, Dictionary<IInlineImageReceiver, Coroutine>> ();
		}

		public bool Exists (string identifier, IInlineImageReceiver receiver)
		{
			if (this.requests.ContainsKey (identifier) == false) {
				return false;
			}

			return this.requests [identifier].ContainsKey (receiver);
		}

		public Coroutine Get (string identifier, IInlineImageReceiver receiver)
		{
			if (this.Exists (identifier, receiver) == false) {
				return null;
			}

			return this.requests [identifier] [receiver];
		}

		public void Add (string identifier, IInlineImageReceiver receiver, Coroutine coroutine)
		{
			if (this.requests.ContainsKey (identifier)) {
				var d = this.requests [identifier];
				if (d.ContainsKey (receiver) == false) {
					d.Add (receiver, coroutine);
				}
			} else {
				var d = new Dictionary<IInlineImageReceiver, Coroutine> ();
				d.Add (receiver, coroutine);
				this.requests.Add (identifier, d);
			}
		}

		public void Remove (string identifier, IInlineImageReceiver receiver)
		{
			if (this.Exists (identifier, receiver)) {
				this.requests [identifier].Remove (receiver);

				if (this.requests [identifier].Count == 0) {
					this.requests.Remove (identifier);
				}
			}
		}
	}
}
