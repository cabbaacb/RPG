#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.ComponentModel;

namespace RPG.Editor
{
    public static class EditorExtentions
    {
        private static Dictionary<PriorityMessageType, string> _dic;

        public static void LogError(object message, PriorityMessageType type = PriorityMessageType.None)
        {
            switch (type)
            {
                case PriorityMessageType.None:
                    Debug.LogError(message);
                    break;
                case PriorityMessageType.Notification:
                    Debug.LogError(string.Concat(_dic[type], message));
                    break;
                case PriorityMessageType.Low:
                    Debug.LogError(string.Concat(_dic[type], message));
                    break;
                case PriorityMessageType.Critical:
                    Debug.LogError(string.Concat(_dic[type], message));
                    EditorApplication.isPaused = true;
                    break;
            }           
        }


        static EditorExtentions()
        {
            _dic = new Dictionary<PriorityMessageType, string>()
            {
                { PriorityMessageType.None, string.Empty},
                { PriorityMessageType.Notification, "<color=#002244[Notification]</color>" },
                { PriorityMessageType.Low, "<color=#000077>[Editor]</color>" },
                { PriorityMessageType.Critical, "<color=#880000>[Game]</color>" }
            };
        }

    }

    public static class EditorConstants
    {
        public static readonly string FocusTargetPointName = "Neck";
        public static readonly string AirColliderName = "AirCollider";
        public static Bounds AirColliderBound = new Bounds(new Vector3(0f, 0.037f, 0f), new Vector3(0.5f, 0.08f, 0.5f));
    }


    public enum PriorityMessageType : byte
    {
        None,
        Notification,
        Low,
        Critical
    }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, true);

            GUI.enabled = true;
        }
    }

    [CustomPropertyDrawer(typeof(SQRFloatAttribute))]
    public class SQRFloatDrawer : PropertyDrawer
    {
        private float _labelWidthPercent = 0.4f;
        private float space = 35f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelRect = new Rect(position.x, position.y, position.width * _labelWidthPercent,
                EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, property.displayName);

            var rectangle = new Rect(position.x + position.width * _labelWidthPercent, position.y, position.width * (1 - _labelWidthPercent) / 2f,
                EditorGUIUtility.singleLineHeight);
            var printRect = new Rect(rectangle.x + space, rectangle.y, rectangle.width - space, rectangle.height);
            GUI.enabled = false;
            var value = EditorGUI.FloatField(printRect, GUIContent.none, Mathf.Sqrt(property.floatValue));
            GUI.enabled = true;
            rectangle.x += rectangle.width;

            printRect = new Rect(rectangle.x, rectangle.y, rectangle.width - space, rectangle.height);
            EditorGUI.LabelField(printRect, "Sqrt");

            printRect = new Rect(rectangle.x + space, rectangle.y, rectangle.width - space, rectangle.height);
            EditorGUI.PropertyField(printRect, property, GUIContent.none);


        }
    }


}
#endif
