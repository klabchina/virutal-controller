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

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Jigbox.EditorUtils
{
    public static class EditorUtilsTools
    {
#region constants

        /// <summary>ヘッダーが開いている状態の色</summary>
        static readonly Color HeaderOpenedColor = new Color(0.5f, 0.5f, 0.5f);

        /// <summary>ヘッダーが閉じている状態の色</summary>
        static readonly Color HeaderClosedColor = new Color(0.65f, 0.65f, 0.65f);

#endregion

#region public methods

        /// <summary>
        /// ラベルの表示領域幅を設定します。
        /// </summary>
        /// <param name="value"></param>
        public static void SetLabelWidth(float value)
        {
            EditorGUIUtility.labelWidth = value;
        }

        /// <summary>
        /// 値に差があるかどうかを返します。
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool Difference(float v1, float v2)
        {
            return Mathf.Abs(v1 - v2) > 0.0001f;
        }

        /// <summary>
        /// 回転角に差があるかどうかを返します。
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static bool DifferenceAngle(float a1, float a2)
        {
            return Difference(ValidateDegreeAngle(a1), ValidateDegreeAngle(a2));
        }

        /// <summary>
        /// -180°～180°のdegree角を返します。
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns></returns>
        public static float ValidateDegreeAngle(float angle)
        {
            if (angle > 180.0f)
            {
                while (angle > 180.0f)
                {
                    angle -= 360.0f;
                }
            }
            else
            {
                while (angle < -180.0f)
                {
                    angle += 360.0f;
                }
            }

            return angle;
        }

        /// <summary>
        /// 複数選択可能なFloat編集用エディタ表示を行います。
        /// </summary>
        /// <param name="name">ラベル名</param>
        /// <param name="value">値</param>
        /// <param name="isMultiSelected">複数選択されているかどうか</param>
        /// <returns>値が変更されたかどうか</returns>
        public static bool FloatField(string name, ref float value, bool isMultiSelected)
        {
            GUI.changed = false;
            float editValue = value;
            if (!isMultiSelected)
            {
                editValue = EditorGUILayout.FloatField(name, editValue);
            }
            else
            {
                float.TryParse(EditorGUILayout.TextField(name, "--"), out editValue);
            }

            if (GUI.changed && Difference(editValue, value))
            {
                value = editValue;
                return true;
            }

            return false;
        }

        /// <summary>Vector2をインスペクタ上で描画する</summary>
        public static bool Vector2Field(string label, float labelWidth, float propertyLabelWidth, bool xDisable, bool yDisable, Vector2[] screenPositions, out Vector2 result)
        {
            EditorGUI.BeginChangeCheck();
            // 元のラベルWidthを保持しておく
            var tmpLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUILayout.BeginHorizontal();
            {
                if (!string.IsNullOrEmpty(label))
                {
                    SetLabelWidth(labelWidth);
                    EditorGUILayout.LabelField(label);
                }

                SetLabelWidth(propertyLabelWidth);

                float x;
                float y;
                EditorGUI.showMixedValue = screenPositions.Select(sp => sp.x).Distinct().Count() >= 2;
                using (new EditorGUI.DisabledScope(xDisable))
                {
                    x = EditorGUILayout.FloatField("X", screenPositions[0].x);
                }

                EditorGUI.showMixedValue = screenPositions.Select(sp => sp.y).Distinct().Count() >= 2;
                using (new EditorGUI.DisabledScope(yDisable))
                {
                    y = EditorGUILayout.FloatField("Y", screenPositions[0].y);
                }

                result = new Vector2(x, y);
            }
            EditorGUILayout.EndHorizontal();

            SetLabelWidth(tmpLabelWidth);

            return EditorGUI.EndChangeCheck();
        }

        /// <summary>
        /// 開閉記録付きのグループ用ヘッダーを表示します。
        /// </summary>
        /// <param name="label">ヘッダーに表示するラベル</param>
        /// <param name="key">開閉を保存するためのキー情報</param>
        /// <param name="defaultState">デフォルトの開閉状態</param>
        /// <returns></returns>
        public static bool DrawGroupHeader(string label, string key = "", bool defaultState = true)
        {
            bool isShow = true;
            if (!string.IsNullOrEmpty(key))
            {
                isShow = EditorPrefs.GetBool(key, defaultState);
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(-2.0f);

                GUI.backgroundColor = isShow ? HeaderOpenedColor : HeaderClosedColor;

                if (!GUILayout.Toggle(true, label, "dragtab") && !string.IsNullOrEmpty(key))
                {
                    isShow = !isShow;
                    EditorPrefs.SetBool(key, isShow);
                }

                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();

            return isShow;
        }

        /// <summary>
        /// ヘッダーに続くコンテンツ領域をヘッダーにフィットさせます。
        /// </summary>
        public static void FitContentToHeader()
        {
            GUILayout.Space(-3.5f);
        }

        /// <summary>
        /// Undoのための情報を記録します。
        /// </summary>
        /// <param name="description"></param>
        /// <param name="objects"></param>
        public static void RegisterUndo(string description, params Object[] objects)
        {
            RegisterUndo(description, true, objects);
        }

        /// <summary>
        /// Undoのための情報を記録します。
        /// </summary>
        /// <param name="description"></param>
        /// <param name="dirty"></param>
        /// <param name="objects"></param>
        public static void RegisterUndo(string description, bool dirty, params Object[] objects)
        {
            if (objects == null)
            {
                return;
            }

            if (objects.Length == 0)
            {
                return;
            }

            Undo.RecordObjects(objects, description);

            if (dirty)
            {
                foreach (Object obj in objects)
                {
                    if (obj != null)
                    {
                        EditorUtility.SetDirty(obj);
                    }
                }
            }
        }

#endregion
    }
}
