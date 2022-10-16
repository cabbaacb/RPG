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
        [SerializeField, Range(0.1f, 2f)]
        private float _fallDelayStart = 0.5f;

        private LinkedList<GameObject> _obstacles = new LinkedList<GameObject>();
        private UnitStateComponent _state;
        private Coroutine _fallingCor;

        public event SimpleHandle<bool> OnFallingEvent;

        private void Start()
        {
            _state = this.FindComponent<UnitStateComponent>();
            _trigger.OnTriggerCollisionEventHandler += UpdateAir;
        }

        private void UpdateAir(Collider collider, bool enter)
        {
            if (!collider.CompareTag(Constants.FloorTag)) return;
            if (enter) _obstacles.AddLast(collider.gameObject);
            else _obstacles.Remove(collider.gameObject);

            _state.InAir = _obstacles.Count == 0;

            if (_state.InAir)
                _fallingCor = StartCoroutine(Falling());
            else
            {
                if (_fallingCor != null)
                {
                    StopCoroutine(_fallingCor);
                    _fallingCor = null;
                }
                OnFallingEvent?.Invoke(false);
            }
        }
        private IEnumerator Falling()
        {
            yield return new WaitForSeconds(_fallDelayStart);
            if (_state.InAir)
                OnFallingEvent?.Invoke(true);
            _fallingCor = null;
        }


        #region Editor
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
            GO.transform.localPosition= Vector3.zero;
            GO.transform.rotation = new Quaternion();
            GO.transform.localScale = Vector3.one;
            GO.layer = LayerMask.NameToLayer(Editor.EditorConstants.TriggerLayer);

            var box = GO.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = Editor.EditorConstants.AirColliderBound.size;
            box.center = Editor.EditorConstants.AirColliderBound.center;
            


            _trigger = GO.AddComponent<BaseTriggerComponent>();
            Editor.EditorExtentions.LogError($"Unit <b>{name}</b>: an air collider's been created", Editor.PriorityMessageType.Notification);
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
#endif
        #endregion

        private void OnDestroy()
        {
            _trigger.OnTriggerCollisionEventHandler -= UpdateAir;
        }

    }
}
