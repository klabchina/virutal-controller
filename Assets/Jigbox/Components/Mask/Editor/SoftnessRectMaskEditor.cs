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
using UnityEditor;
using Jigbox.EditorUtils;
using Jigbox.UIControl;

namespace Jigbox.Components
{
    [CustomEditor(typeof(SoftnessRectMask), true)]
    public class SoftnessRectMaskEditor : BlendMaskEditor
    {
#region properties

        /// <summary>ソフトクリッピングによるクリッピング範囲</summary>
        protected SerializedVector2 softnessProperty;

#endregion

#region protected methods

        /// <summary>
        /// 各プロパティを表示します。
        /// </summary>
        protected override void DrawSerializedProperties()
        {
            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;
            softnessProperty.EditProperty("Softness", "S", 60.0f);
            needUpdateMaterial = Application.isPlaying && GUI.changed;

            base.DrawSerializedProperties();
        }

        /// <summary>
        /// トランジション対象の仮想位置を表示します。
        /// </summary>
        /// <param name="position">表示座標</param>
        /// <param name="size">大きさ</param>
        protected void DrawRect(Vector3 position, Vector2 size)
        {
            Rect rect = new Rect(position, size);

            // rectのままでもレンダリングできるがz軸方向の位置が飛ぶので頂点に変換する
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(rect.xMin, rect.yMax, position.z),
                new Vector3(rect.xMax, rect.yMax, position.z),
                new Vector3(rect.xMax, rect.yMin, position.z),
                new Vector3(rect.xMin, rect.yMin, position.z)
            };

            Color faceColor = Color.blue;
            faceColor.a = 0.1f;

            Handles.DrawSolidRectangleWithOutline(vertices, faceColor, Color.clear);
        }

#endregion

#region override unity methods

        protected override void OnEnable()
        {
            base.OnEnable();

            softnessProperty = new SerializedVector2(serializedObject.FindProperty("softness"));
        }

        protected virtual void OnSceneGUI()
        {
            if (!showMaskProperty.boolValue)
            {
                return;
            }

            RectTransform rectTransform = mask.rectTransform;
            Vector2 size = rectTransform.rect.size;
            size.x *= rectTransform.lossyScale.x;
            size.y *= rectTransform.lossyScale.y;

            // 左上
            Vector3 leftTop = rectTransform.position;
            leftTop.x -= size.x * rectTransform.pivot.x;
            leftTop.y += size.y * (1.0f - rectTransform.pivot.y);

            // 右上
            Vector3 rightTop = rectTransform.position;
            rightTop.x += size.x * (1.0f - rectTransform.pivot.x);
            rightTop.y += size.y * (1.0f - rectTransform.pivot.y);

            // 左下
            Vector3 rightBottom = rectTransform.position;
            rightBottom.x += size.x * (1.0f - rectTransform.pivot.x);
            rightBottom.y -= size.y * rectTransform.pivot.y;

            // 右下
            Vector3 leftBottom = rectTransform.position;
            leftBottom.x -= size.x * rectTransform.pivot.x;
            leftBottom.y -= size.y * rectTransform.pivot.y;
            
            // スケール済みのクリッピング範囲
            Vector2 softness = Vector2.zero;
            softness.x = softnessProperty.X.floatValue * rectTransform.lossyScale.x;
            softness.y = softnessProperty.Y.floatValue * rectTransform.lossyScale.y;

            // 上方のクリッピング範囲
            Vector2 drawSize = rightTop - leftTop;
            drawSize.y -= softness.y;
            DrawRect(leftTop, drawSize);

            // 下方のクリッピング範囲
            drawSize = rightBottom - leftBottom;
            drawSize.y += softness.y;
            DrawRect(leftBottom, drawSize);

            // 左方のクリッピング範囲
            drawSize = leftTop - leftBottom;
            drawSize.x += softness.x;
            DrawRect(leftBottom, drawSize);

            // 右方のクリッピング範囲
            drawSize = rightTop - rightBottom;
            drawSize.x -= softness.x;
            DrawRect(rightBottom, drawSize);
        }

#endregion
    }
}
