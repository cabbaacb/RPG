using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RPG.Units
{
    public class Unit : MonoBehaviour
    {
        [SerializeField]
        private Transform _targetPoint;

        private Animator _animator;
        private UnitInputComponent _inputs;
        private UnitStatsComponent _stats;
        private bool _inAnimation;
        private TriggerComponent[] _colliders;

        public SimpleHandle OnTargetLostHandler;
        
        public Unit Target { get; protected set; }
        public Transform TargetPoint => _targetPoint;

        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _inputs = GetComponent<UnitInputComponent>();
            _stats = GetComponent<UnitStatsComponent>();
            _colliders = GetComponentsInChildren<TriggerComponent>();
            foreach (var collider in _colliders)
                collider.Construct(this, _stats);
#if UNITY_EDITOR
            if (_inputs == null)
            {
                Editor.EditorExtentions.LogError($"unit {name} doesn't contain {nameof(UnitInputComponent)}", Editor.EditorMessageType.Game);
                return;
            }
#endif
            BindingEvents();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            OnMove();
        }

        protected virtual void BindingEvents(bool unbind = false)
        {
            if(unbind)
            {
                _inputs.OnAttackEvent -= OnSwordAttack;
                _inputs.OnShieldEvent -= OnShieldAttack;
                _inputs.OnTargetEvent -= OnTargetUpdate;
                return;
            }

            _inputs.OnAttackEvent += OnSwordAttack;
            _inputs.OnShieldEvent += OnShieldAttack;
            _inputs.OnTargetEvent += OnTargetUpdate;
        }

        private void OnSwordAttack()
        {
            if (_inAnimation) return;

            _animator.SetTrigger("SwordAttack");
            _inAnimation = true;
        }

        private void OnShieldAttack()
        {
            if (_inAnimation || _stats.CurrentCalldown > 0f) return;

            _animator.SetTrigger("ShieldAttack");
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
                transform.position += movement * _stats.MoveSpeed * Time.deltaTime;
            }
        }

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
                Editor.EditorExtentions.LogError($"unit {name} doesn't contain {nameof(UnitInputComponent)}", Editor.EditorMessageType.Editor);
                return;
            }
#endif

            BindingEvents(true);
        }
    }
}
