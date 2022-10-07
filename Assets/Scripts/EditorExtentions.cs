#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
                case PriorityMessageType.Editor:
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
                {PriorityMessageType.Editor, "<color=#000077>[Editor]</color>" },
                {PriorityMessageType.Critical, "<color=#880000>[Game]</color>" }
            };
        }

    }

    public static class EditorConstants
    {
        public static readonly string FocusTargetPointName = "B-neck";
    }


    public enum PriorityMessageType : byte
    {
        None,
        Editor,
        Critical
    }
}
#endif
