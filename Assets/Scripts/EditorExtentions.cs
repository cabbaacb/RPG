#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.Editor
{
    public static class EditorExtentions
    {
        private static Dictionary<EditorMessageType, string> _dic;

        public static void LogError(object message, EditorMessageType type = EditorMessageType.None)
        {
            switch (type)
            {
                case EditorMessageType.None:
                    Debug.LogError(message);
                    break;
                case EditorMessageType.Editor:
                    Debug.LogError(string.Concat(_dic[type], message));
                    break;
                case EditorMessageType.Game:
                    Debug.LogError(string.Concat(_dic[type], message));
                    EditorApplication.isPaused = true;
                    break;
            }           
        }


        static EditorExtentions()
        {
            _dic = new Dictionary<EditorMessageType, string>()
            {
                { EditorMessageType.None, string.Empty},
                {EditorMessageType.Editor, "<color=#000077>[Editor]</color>" },
                {EditorMessageType.Game, "<color=#880000>[Game]</color>" }
            };
        }

    }

    public enum EditorMessageType : byte
    {
        None,
        Editor,
        Game
    }
}
#endif
