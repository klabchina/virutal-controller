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

using UnityEditor;
using System.Collections.Generic;
using Jigbox.Tween;

namespace Jigbox.TweenEditor
{
    [CustomEditor(typeof(TweenWorker))]
    public class TweenWorkerEditor : Editor
    {
        Dictionary<IMovement, TweenEditor.Foldout> foldoutCache = new Dictionary<IMovement, TweenEditor.Foldout>();

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            var worker = (TweenWorker) target;
            var foldouts = new Dictionary<IMovement, TweenEditor.Foldout>();

            if (worker.Count == 0)
            {
                EditorGUILayout.LabelField("Tweens", "[EMPTY]");
                return;
            }

            EditorGUILayout.LabelField("Tweens", worker.Count.ToString());
            EditorGUI.indentLevel++;

            foreach (var tween in worker.Movements)
            {
                DrawTween(tween, foldouts);
            }

            EditorGUI.indentLevel--;

            foldoutCache.Clear();
            foldoutCache = foldouts;
        }

        void DrawTween(IMovement tween, IDictionary<IMovement, TweenEditor.Foldout> foldouts)
        {
            var foldout = foldoutCache.ContainsKey(tween) ? foldoutCache[tween] : TweenEditor.Foldout.closeAll;
            foldout = TweenEditor.DrawITweenFoldable(tween, foldout);

            foldouts.Add(tween, foldout);
        }
    }
}

