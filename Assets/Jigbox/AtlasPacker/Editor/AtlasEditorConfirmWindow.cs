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
using System;

namespace Jigbox.AtlasPacker
{
    public class AtlasEditorConfirmWindow : EditorWindow
    {
#region properties

        /// <summary>表示するテキスト</summary>
        string message;

        /// <summary>OKを押した際のコールバック</summary>
        Action allowEvent;

        /// <summary>Cancelを押した際のコールバック</summary>
        Action denyEvent;

        /// <summary>コールバックに通知するオブジェクト</summary>
        object responseObject;

#endregion

#region public methods

        /// <summary>
        /// 確認ウィンドウを開きます。
        /// </summary>
        /// <param name="message">表示するテキスト</param>
        /// <param name="parentPosition">親ウィンドウの位置情報</param>
        /// <param name="allowEvent">OKを押した際のコールバック</param>
        /// <param name="denyEvent">Cancelを押した際のコールバック</param>
        public static void OpenWindow(string message, Rect parentPosition, Action allowEvent = null, Action denyEvent = null)
        {
            AtlasEditorConfirmWindow window = GetWindow(typeof(AtlasEditorConfirmWindow)) as AtlasEditorConfirmWindow;
            window.message = message;
            window.allowEvent = allowEvent;
            window.denyEvent = denyEvent;
            window.position = new Rect(parentPosition.x, parentPosition.y, 300.0f, 200.0f);
        }

#endregion

#region override unity methods

        void OnGUI()
        {
            TextAnchor anchor = GUI.skin.label.alignment;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0.0f, 0.0f, 300.0f, 150.0f), message);
            GUI.skin.label.alignment = anchor;
            GUI.color = Color.white;

            GUILayout.Space(150.0f);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(40);
                Color temp = GUI.backgroundColor;
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("OK", GUILayout.Width(90), GUILayout.MinWidth(90)))
                {
                    if (allowEvent != null)
                    {
                        allowEvent();
                    }
                    Close();
                }
                GUILayout.Space(40);
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Cancel", GUILayout.Width(90), GUILayout.MinWidth(90)))
                {
                    if (denyEvent != null)
                    {
                        denyEvent();
                    }
                    Close();
                }
                GUILayout.Space(40);
                GUI.backgroundColor = temp;

            }
            GUILayout.EndHorizontal();
        }

#endregion
    }
}
