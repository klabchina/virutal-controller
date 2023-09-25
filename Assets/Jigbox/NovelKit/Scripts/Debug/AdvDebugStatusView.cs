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
using System.Collections.Generic;
using System.Text;
using Jigbox.UIControl;

namespace Jigbox.NovelKit
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public class AdvDebugStatusView : MonoBehaviour, IAdvDebugStatusHandler
    {
#region constants

        /// <summary>デバッグ表示枠の背景色</summary>
        protected static readonly Color BgColor = new Color(0.0f, 0.0f, 0.0f, 0.25f);

        /// <summary>デバッグ情報の表示文字の最大サイズ</summary>
        protected static readonly int FontSizeMax = 18;

        /// <summary>デバッグ表示文字の最小サイズ</summary>
        protected static readonly int FontSizeMin = 10;

        /// <summary>デバッグ用のテキスト表示領域のマージン</summary>
        protected static readonly float TextMargin = 4.0f;

#endregion

#region properties

        /// <summary>Textコンポーネント</summary>
        protected Text text = null;

        /// <summary>StringBuilder</summary>
        protected StringBuilder stringBuilder = new StringBuilder();

        /// <summary>デバッグ中の状態</summary>
        protected Dictionary<string, string> status = new Dictionary<string, string>();

        /// <summary>デバッグ中の状態に変更が入ったかどうか</summary>
        protected bool isDarty = false;

        /// <summary>FPSを表示するかどうか</summary>
        public bool IsShowFps { get; protected set; }

        /// <summary>タイムコードを表示するかどうか</summary>
        public bool IsShowTimecode { get; protected set; }

#endregion

#region public methods

        /// <summary>
        /// 標準のデバッグ情報の表示用構成を生成します。
        /// </summary>
        /// <param name="parent">親となるオブジェクトの参照</param>
        /// <param name="order">デバッグ情報の表示要求</param>
        /// <returns></returns>
        public static AdvDebugStatusView CreateDefaultView(Transform parent, AdvDebugStatusViewOrder order)
        {
            GameObject gameObject = new GameObject("DebugStatusWindow");
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.SetAsLastSibling();

            AdvDebugStatusView debugStatusWindow = gameObject.AddComponent<AdvDebugStatusView>();
            debugStatusWindow.SetAnchor(order);
            debugStatusWindow.CreateGraphic();
            debugStatusWindow.IsShowFps = order.IsShowFps;
            debugStatusWindow.IsShowTimecode = order.IsShowTimecode;

            // 入力判定は持たない
            CanvasGroup canvasGroup = debugStatusWindow.GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;

            return debugStatusWindow;
        }

        /// <summary>
        /// デバッグを開始します。
        /// </summary>
        public virtual void StartDebug()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// デバッグを終了します。
        /// </summary>
        public virtual void EndDebug()
        {
            status.Clear();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// デバッグ情報を設定します。
        /// </summary>
        /// <param name="tag">識別用のタグ名</param>
        /// <param name="data">デバッグ中の状態の表示内容</param>
        public void Set(string tag, string data)
        {
            if (status.ContainsKey(tag))
            {
                status[tag] = data;
            }
            else
            {
                status.Add(tag, data);
            }
            isDarty = true;
        }

        /// <summary>
        /// デバッグ情報を除外します。
        /// </summary>
        /// <param name="tag">識別用のタグ名</param>
        public void Remove(string tag)
        {
            if (status.ContainsKey(tag))
            {
                status.Remove(tag);
                isDarty = true;
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// Anchorを設定します。
        /// </summary>
        /// <param name="order">表示要求</param>
        protected virtual void SetAnchor(AdvDebugStatusViewOrder order)
        {
            RectTransform rectTransform = transform as RectTransform;
            RectTransformUtils.SetAnchor(rectTransform, order.AnchorPoint);
            rectTransform.offsetMin = order.OffsetMin;
            rectTransform.offsetMax = order.OffsetMax;
        }

        /// <summary>
        /// 表示を作成します。
        /// </summary>
        protected virtual void CreateGraphic()
        {
            // 背景
            GameObject bgObject = new GameObject("Bg");
            bgObject.transform.SetParent(transform, false);

            Image bg = bgObject.AddComponent<Image>();
            bg.color = BgColor;

            RectTransform bgTransform = bgObject.transform as RectTransform;
            bgTransform.anchorMin = Vector2.zero;
            bgTransform.anchorMax = Vector2.one;
            bgTransform.offsetMin = Vector2.zero;
            bgTransform.offsetMax = Vector2.zero;

            // テキスト
            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(transform, false);

            text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.supportRichText = false;
            text.color = Color.white;
            text.fontSize = FontSizeMax;
            text.resizeTextForBestFit = true;
            text.resizeTextMaxSize = FontSizeMax;
            text.resizeTextMinSize = FontSizeMin;

            RectTransform textTransform = textObject.transform as RectTransform;
            textTransform.anchorMin = Vector2.zero;
            textTransform.anchorMax = Vector2.one;
            textTransform.offsetMin = new Vector2(TextMargin, TextMargin);
            textTransform.offsetMax = new Vector2(-TextMargin, -TextMargin);
        }

        /// <summary>
        /// 表示するテキストを更新します。
        /// </summary>
        protected void UpdateText()
        {
            stringBuilder.Length = 0;

            foreach (KeyValuePair<string, string> data in status)
            {
                stringBuilder.AppendLine(data.Value);
            }

            text.text = stringBuilder.ToString();
        }

#endregion

#region override unity methods

        protected virtual void LateUpdate()
        {
            if (isDarty)
            {
                UpdateText();
                isDarty = false;
            }
        }

#endregion
    }
}
