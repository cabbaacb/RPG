using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RPG.Units
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField]
        private Transform _targetPoint;

        protected UnitInputComponent _inputs;
        protected Animator _animator;

        private UnitStatsComponent _stats;
        private bool _inAnimation;
        private TriggerComponent[] _colliders;
        public SimpleHandle OnTargetLostHandler;
        
        public Unit Target { get; protected set; }
        public Transform TargetPoint => _targetPoint;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            _animator = this.FindComponent<Animator>();
            _inputs = this.FindComponent<UnitInputComponent>();
            _stats = this.FindComponent<UnitStatsComponent>();

            _colliders = this.FindComponentsInChildren<TriggerComponent>();
            foreach (var collider in _colliders)
                collider.Construct(this, _stats);
#if UNITY_EDITOR
            if (_inputs == null)
            {
                Editor.EditorExtentions.LogError($"unit {name} doesn't contain {nameof(UnitInputComponent)}", Editor.PriorityMessageType.Critical);
                return;
            }
#endif
            BindingEvents();
        }

#if UNITY_EDITOR
        [ContextMenu("UpdateInternalStates")]
        private void UpdateInternalStates()
        {
            if (_targetPoint != null)
                return;
            else
                _targetPoint = this.FindComponentsInChildren<Transform>().First(t => t.name == Editor.EditorConstants.FocusTargetPointName);
        }
#endif


        // Update is called once per frame
        protected virtual void Update()
        {
            OnMove();
            OnRotate();
        }

        protected virtual void BindingEvents(bool unbind = false)
        {
            if(unbind)
            {
                _inputs.MainAttackEventHandler -= OnMainAction;
                _inputs.AdditionalAttackEventHandler -= OnAdditionalAction;
                _inputs.TargetEventHandler -= OnTargetUpdate;
                return;
            }

            _inputs.MainAttackEventHandler += OnMainAction;
            _inputs.AdditionalAttackEventHandler += OnAdditionalAction;
            _inputs.TargetEventHandler += OnTargetUpdate;
        }

        private void OnMainAction()
        {
            if (_inAnimation) return;

            _animator.SetTrigger("MainAction");
            _inAnimation = true;
        }

        private void OnAdditionalAction()
        {
            if (_inAnimation || _stats.CurrentCalldown > 0f) return;

            _animator.SetTrigger("AdditionalAction");
            _inAnimation = true;

            _stats.CurrentCalldown = _stats.CalldownShieldAttack;
        }

        private void OnTargetUpdate()
        {          

            if(Target != null)
            {
                Target = null;
                OnTargetLostHandler?.Invoke(); //todo
                return;
            }

            var distance = float.MaxValue;
            UnitStatsComponent target = null;

            var units = FindObjectsOfType<UnitStatsComponent>();
            foreach(var unit in units)
            {
                if (unit.SideType == _stats.SideType) continue;

                var currentDistance = (unit.transform.position - transform.position).sqrMagnitude;
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    target = unit;
                }
            }

            if (target == null) Debug.LogError("No bots in scene");
            Target = target.GetComponent<Unit>();

        }

        private void OnMove()
        {
            if (_inAnimation) return;

            ref var movement = ref _inputs.MoveDirection();
            _animator.SetFloat("ForwardMove", movement.z);
            _animator.SetFloat("SideMove", movement.x);

            if (movement.x == 0f && movement.z == 0f)
            {
                _animator.SetBool("Moving", false);
            }
            else
            {
                _animator.SetBool("Moving", true);
                transform.position += transform.TransformVector(movement) * _stats.MoveSpeed * Time.deltaTime;
            }
        }

        protected abstract void OnRotate();

        private void OnAnimationEnd_UnityEvent(AnimationEvent data)
        {
            _inAnimation = false;
        }

        private void OnCollider_UnityEvent(AnimationEvent data)
        {
            var collider = _colliders.FirstOrDefault(t => t.GetID == data.intParameter);

#if UNITY_EDITOR
            if(collider == null)
            {
                Debug.LogError($"collider with ID {data.intParameter} not found!");
                UnityEditor.EditorApplication.isPaused = true;
            }
#endif

            collider.Enable = data.floatParameter == 1f;


        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (_inputs == null)
            {
                Editor.EditorExtentions.LogError($"unit {name} doesn't contain {nameof(UnitInputComponent)}", Editor.PriorityMessageType.Editor);
                return;
            }
#endif

            BindingEvents(true);
        }
    }
}
