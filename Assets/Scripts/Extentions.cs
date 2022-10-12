using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public delegate void SimpleHandle();
    public delegate void SimpleHandle<T>(T arg);
    public delegate void SimpleHandle<T1, T2>(T1 arg1, T2 arg2);

    public class ReadOnlyAttribute : PropertyAttribute { }
    public class SQRFloatAttribute : PropertyAttribute { }


    public static class Constants
    {
        public static readonly string FloorTag = "Floor";
        public static readonly string ObstacleLayerName = "Obstacles";
        public static int ObstacleLayerInt;


        public static void Construct()
        {
            ObstacleLayerInt = LayerMask.NameToLayer(ObstacleLayerName);
        }
    }

    public static class MonoBehaviourExtentions
    {
        public static T FindComponent<T>(this MonoBehaviour source) where T : Component
        {
            var component = source.GetComponent<T>();
#if UNITY_EDITOR
            if(component == null)
            {
                PrintLog(typeof(T).Name, source.name);
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
                PrintLog(typeof(T).Name, source.name);
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
                PrintLog(typeof(T).Name, source.name);
            }
#endif
            return component;
        }

        private static void PrintLog(string componentName, string name)
        {
#if UNITY_EDITOR
            Editor.EditorExtentions.LogError($"Component : <b>{componentName}</b>" +
                    $"not found on GameObject : <i>{name}</i>", Editor.PriorityMessageType.Critical);
#endif
        }


    }


}
