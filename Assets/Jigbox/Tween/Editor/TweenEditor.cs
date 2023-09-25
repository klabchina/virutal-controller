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
using Jigbox.Tween;
using System;

namespace Jigbox.TweenEditor
{
    public static class TweenEditor
    {
        public struct Foldout
        {
            public bool viewInstance;
            public bool viewSettings;
            public bool viewStatus;

            public Foldout(bool viewInstance, bool viewSettings, bool viewStatus)
            {
                this.viewInstance = viewInstance;
                this.viewSettings = viewSettings;
                this.viewStatus = viewStatus;
            }

            public static readonly Foldout closeAll = new Foldout(false, false, false);
        }

        static string TweenSummary(IMovement tween)
        {
            var tweenType = "IMovement";
            if (tween is ITween<float>)
            {
                tweenType = "Tween<float>";
            }
            else if (tween is ITween<Vector2>)
            {
                tweenType = "Tween<Vector2>";
            }
            else if (tween is ITween<Vector3>)
            {
                tweenType = "Tween<Vector3>";
            }
            else if (tween is ITween<Vector4>)
            {
                tweenType = "Tween<Vector4>";
            }
            else if (tween is ITween<Quaternion>)
            {
                tweenType = "Tween<Quaternion>";
            }
            return string.Format("{0} - {1}", tweenType, StatusSummary(tween));
        }

        static string ConfigSummary(IMovement movement)
        {
            if (movement is ITween)
            {
                var tween = (ITween) movement;
                var motion = tween.MotionType == MotionType.Linear
                    ? "Linear"
                    : string.Format("{0}-{1}", tween.MotionType, tween.EasingType);

                return string.Format("{0} ({1})", motion, tween.LoopMode);
            }
            return "config";
        }

        static string StatusSummary(IMovement tween)
        {
            return string.Format("{0}", tween.State);
        }

        public static Foldout DrawITweenFoldable(IMovement tween, Foldout foldout, bool editable = true)
        {
            foldout.viewInstance = EditorGUILayout.Foldout(foldout.viewInstance, TweenSummary(tween));
            if (foldout.viewInstance)
            {
                EditorGUI.indentLevel++;

                foldout.viewSettings = EditorGUILayout.Foldout(foldout.viewSettings, ConfigSummary(tween));
                if (foldout.viewSettings)
                {
                    EditorGUI.indentLevel++;
                    GUI.enabled = editable;

                    if (tween is ITween)
                    {
                        DrawTweenSettings((ITween) tween);
                    }
                    else
                    {
                        DrawMovementSettings(tween);
                    }

                    GUI.enabled = true;
                    EditorGUI.indentLevel--;
                }

                foldout.viewStatus = EditorGUILayout.Foldout(foldout.viewStatus, StatusSummary(tween));
                if (foldout.viewStatus)
                {
                    EditorGUI.indentLevel++;
                    GUI.enabled = false;

                    DrawStatus(tween);

                    GUI.enabled = true;
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }
            return foldout;
        }

        public static void DrawITween(ITween tween)
        {
            DrawTweenSettings(tween);
        }

        static void DrawTweenSettings(ITween tween)
        {
            tween.MotionType = (MotionType) EditorGUILayout.EnumPopup("Motion Type", tween.MotionType);

            if (tween.MotionType == MotionType.Custom)
            {
                tween.CustomMotion = EditorGUILayout.CurveField("Custom", tween.CustomMotion, Color.green, new Rect(0, 0, 0, 0), GUILayout.Height(100f));
            }
            else if (tween.MotionType != MotionType.Linear)
            {
                tween.EasingType = (EasingType) EditorGUILayout.EnumPopup("Easing Type", tween.EasingType);
            }

            DrawBeginFinal(tween);

            tween.Duration = EditorGUILayout.FloatField("Duration", tween.Duration);

            tween.Delay = EditorGUILayout.FloatField("Delay", tween.Delay);

            tween.Interval = EditorGUILayout.FloatField("Interval", tween.Interval);

            tween.LoopMode = (LoopMode) EditorGUILayout.EnumPopup("Loop", tween.LoopMode);

            tween.FollowTimeScale = EditorGUILayout.Toggle("Follow Time Scale", tween.FollowTimeScale);
        }

        static void DrawMovementSettings(IMovement tween)
        {
            tween.Delay = EditorGUILayout.FloatField("Delay", tween.Delay);

            tween.FollowTimeScale = EditorGUILayout.Toggle("Follow Time Scale", tween.FollowTimeScale);
        }

        static void DrawStatus(IMovement tween)
        {
            EditorGUILayout.EnumPopup("State", tween.State);
            EditorGUILayout.FloatField("DeltaTime", tween.DeltaTime);
            if (tween is ITween)
            {
                DrawValue((ITween) tween);
            }
        }

        static void DrawBeginFinal(ITween itween)
        {
            if (itween is ITween<float>)
            {
                var tween = itween as ITween<float>;
                tween.Begin = EditorGUILayout.FloatField("Begin", tween.Begin);
                tween.Final = EditorGUILayout.FloatField("Final", tween.Final);
            }
            else if (itween is ITween<Vector2>)
            {
                var tween = itween as ITween<Vector2>;
                tween.Begin = EditorGUILayout.Vector2Field("Begin", tween.Begin);
                tween.Final = EditorGUILayout.Vector2Field("Final", tween.Final);
            }
            else if (itween is ITween<Vector3>)
            {
                var tween = itween as ITween<Vector3>;
                tween.Begin = EditorGUILayout.Vector3Field("Begin", tween.Begin);
                tween.Final = EditorGUILayout.Vector3Field("Final", tween.Final);
            }
            else if (itween is ITween<Vector4>)
            {
                var tween = itween as ITween<Vector4>;
                tween.Begin = EditorGUILayout.Vector4Field("Begin", tween.Begin);
                tween.Final = EditorGUILayout.Vector4Field("Final", tween.Final);
            }
            else if (itween is ITween<Color>)
            {
                var tween = itween as ITween<Color>;
                tween.Begin = EditorGUILayout.ColorField("Begin", tween.Begin);
                tween.Final = EditorGUILayout.ColorField("Final", tween.Final);
            }
        }

        static void DrawValue(ITween itween)
        {
            if (itween is ITween<float>)
            {
                var tween = itween as ITween<float>;
                EditorGUILayout.FloatField("Value", tween.Value);
            }
            else if (itween is ITween<Vector2>)
            {
                var tween = itween as ITween<Vector2>;
                EditorGUILayout.Vector2Field("Value", tween.Value);
            }
            else if (itween is ITween<Vector3>)
            {
                var tween = itween as ITween<Vector3>;
                EditorGUILayout.Vector3Field("Value", tween.Value);
            }
            else if (itween is ITween<Vector4>)
            {
                var tween = itween as ITween<Vector4>;
                EditorGUILayout.Vector4Field("Value", tween.Value);
            }
        }
    }
}
