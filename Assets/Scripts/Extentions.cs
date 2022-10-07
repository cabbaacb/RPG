using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public delegate void SimpleHandle();
    public delegate void SimpleHandle<T>(T arg);

    public static class MonoBehaviourExtentions
    {
        public static T FindComponent<T>(this MonoBehaviour source) where T : Component
        {
            var component = source.GetComponent<T>();
#if UNITY_EDITOR
            if(component == null)
            {
                Editor.EditorExtentions.LogError($"Component : <b>{typeof(T).Name}</b> not found on GameObject : <i>{source.name}</i>",
                    Editor.PriorityMessageType.Critical);
            }
#endif
            return component;
        }

        public static T FindComponentInChildren<T>(this MonoBehaviour source) where T : Component
        {
            var component = source.GetComponentInChildren<T>();
#if UNITY_EDITOR
            if (component == null)
            {
                Editor.EditorExtentions.LogError($"Component : <b>{typeof(T).Name}</b> not found on GameObject : <i>{source.name}</i>",
                    Editor.PriorityMessageType.Critical);
            }
#endif
            return component;
        }

        public static T[] FindComponentsInChildren<T>(this MonoBehaviour source) where T : Component
        {
            var component = source.GetComponentsInChildren<T>();
#if UNITY_EDITOR
            if(component.Length == 0 || component == null)
            {
                Editor.EditorExtentions.LogError($"Components : <b>{typeof(T).Name}</b>" +
                    $"not found on GameObject : <i>{source.name}</i>", Editor.PriorityMessageType.Critical);
            }
#endif
            return component;


        }




    }


}
