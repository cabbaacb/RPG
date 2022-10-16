using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Zenject;
using RPG.Managers;

namespace RPG.Units
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField]
        private Transform _targetPoint;
        [SerializeField, SQRFloat, Tooltip("max distance^2")]
        protected float _sqrtFindTargetDistance = 500f;

        
        private TriggerComponent[] _colliders;

        [Inject]
        protected UnitManager _unitManager;
        protected UnitStateComponent _state;
        protected UnitInputComponent _inputs;
        protected Animator _animator;
        protected bool _inAnimation;
        protected UnitSpaceComponent _space;

        public SimpleHandle OnTargetLostHandler;
        
        public Unit Target { get; protected set; }
        public Transform GetTargetPoint => _targetPoint;
        public UnitStateComponent GetStats => _state;


        // Start is called before the first frame update
        protected virtual void Start()
        {
            _animator = this.FindComponent<Animator>();
            _inputs = this.FindComponent<UnitInputComponent>();
            _state = this.FindComponent<UnitStateComponent>();
            _space = this.FindComponent<UnitSpaceComponent>();

            _colliders = this.FindComponentsInChildren<TriggerComponent>();
            foreach (var collider in _colliders)
                collider.Construct(this, _state);
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
            //OnRotate();
        }

        protected void OnFalling(bool isFalling)
        {
            Debug.Log("Fall: " + isFalling);
        }

        protected abstract void FindNewTarget();

        protected virtual void BindingEvents(bool unbind = false)
        {
            if(unbind)
            {
                _inputs.MainAttackEventHandler -= OnMainAction;
                _inputs.AdditionalAttackEventHandler -= OnAdditionalAction;
                _inputs.TargetEventHandler -= OnTargetUpdate;
                _space.OnFallingEvent -= OnFalling;
                return;
            }

            _inputs.MainAttackEventHandler += OnMainAction;
            _inputs.AdditionalAttackEventHandler += OnAdditionalAction;
            _inputs.TargetEventHandler += OnTargetUpdate;
            _space.OnFallingEvent += OnFalling;
        }

        private void OnMainAction()
        {
            if (_inAnimation) return;

            _animator.SetTrigger("MainAction");
            _inAnimation = true;
        }

        private void OnAdditionalAction()
        {
            if (_inAnimation || _state.CurrentCalldown > 0f) return;

            _animator.SetTrigger("AdditionalAction");
            _inAnimation = true;

            _state.CurrentCalldown = _state.CalldownShieldAttack;
        }

        private void OnTargetUpdate()
        {          

            if(Target != null)
            {
                Target = null;
                OnTargetLostHandler?.Invoke(); //todo
                return;
            }

            FindNewTarget();

        }

        protected virtual void OnMove()
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
                transform.position += transform.TransformVector(movement) * _state.GetMoveSpeed * Time.deltaTime;
            }

        }

        //protected abstract void OnRotate();

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
                Editor.EditorExtentions.LogError($"unit {name} doesn't contain {nameof(UnitInputComponent)}", Editor.PriorityMessageType.Low);
                return;
            }
#endif

            BindingEvents(true);
        }
    }
}
