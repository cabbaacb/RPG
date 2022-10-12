using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Units
{
    public class UnitSpaceComponent : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private BaseTriggerComponent _trigger;

        private void Start()
        {
            
        }

#if UNITY_EDITOR
        [ContextMenu("Create or Find collider")]
        private void Construct()
        {
            var triggers = GetComponentsInChildren<BaseTriggerComponent>().Where(t => t.name == Editor.EditorConstants.AirColliderName).ToArray();
            if(triggers.Length == 1)
            {
                _trigger = triggers[0];
                return;
            }
            else if(triggers.Length > 1)
            {
                Editor.EditorExtentions.LogError($"Unit <b>{name}</b> contains more than one air collider. Current count: <b>{triggers.Length}</b>",
                    Editor.PriorityMessageType.Critical);
                return;
            }

            var GO = new GameObject(Editor.EditorConstants.AirColliderName);
            GO.transform.parent = transform;
            GO.transform.position = Vector3.zero;
            GO.transform.rotation = new Quaternion();
            GO.transform.localScale = Vector3.one;

            var box = GO.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = Editor.EditorConstants.AirColliderBound.size;
            box.center = Editor.EditorConstants.AirColliderBound.center;


            _trigger = GO.AddComponent<BaseTriggerComponent>();
            Editor.EditorExtentions.LogError($"Unit <b>{name}</b>: an air collider's been created", Editor.PriorityMessageType.Notification);
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
#endif

    }
}
