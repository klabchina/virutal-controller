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

namespace Jigbox.NovelKit
{
    using BlindType = AdvLetterBoxController.BlindType;

    public class AdvLetterBoxManager
    {
#region properties

        /// <summary>シナリオ制御統合コンポーネント</summary>
        protected AdvMainEngine engine;

        /// <summary>レターボックスの制御コンポーネント</summary>
        protected AdvLetterBoxController controller = null;

        /// <summary>レターボックスが表示されているかどうか</summary>
        public bool IsShow { get { return controller != null; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="engine">シナリオ制御統合コンポーネント</param>
        public AdvLetterBoxManager(AdvMainEngine engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// レターボックスを破棄します。
        /// </summary>
        public void Release()
        {
            if (controller != null)
            {
                Object.Destroy(controller.gameObject);
                controller = null;
            }
        }

        /// <summary>
        /// レターボックスを表示します。
        /// </summary>
        /// <param name="parent">親となるオブジェクト</param>
        /// <param name="type">ブラインドの種類</param>
        /// <param name="time">トランジションの時間</param>
        /// <param name="size">ブラインドの大きさ</param>
        /// <param name="isPixel">ピクセル指定かどうか</param>
        public virtual void Show(GameObject parent, BlindType type, float time, float size, bool isPixel)
        {
            if (isPixel)
            {
                controller = AdvLetterBoxController.CreateFromPixel(parent, OnHide, type, size);
            }
            else
            {
                controller = AdvLetterBoxController.CreateFromRatio(parent, OnHide, type, size);
            }
            controller.Show(time);
            engine.MovementManager.Register(controller.Tween);
        }

        /// <summary>
        /// ブラインドに画像を設定します。
        /// </summary>
        /// <param name="image">画像</param>
        public virtual void SetBlindImage(Sprite image)
        {
            if (controller == null)
            {
                return;
            }

            controller.SetImage(image);
        }

        /// <summary>
        /// レターボックスを非表示にします。
        /// </summary>
        /// <param name="time">トランジションの時間</param>
        public virtual void Hide(float time)
        {
            controller.Hide(time);
        }

#endregion

#region protected methods

        /// <summary>
        /// レターボックスが非表示になった際に呼び出されます。
        /// </summary>
        protected virtual void OnHide()
        {
            Object.Destroy(controller.gameObject);
            controller = null;
        }

#endregion
    }
}
