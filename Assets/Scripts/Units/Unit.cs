using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public SimpleHandle OnTargetLostHandler;
        
        public Unit Target { get; protected set; }
        public Transform TargetPoint => _targetPoint;

        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _inputs = GetComponent<UnitInputComponent>();
            _stats = GetComponent<UnitStatsComponent>();

            if (_inputs == null) return;

            _inputs.OnAttackEvent += OnAttack;
            _inputs.OnTargetEvent += OnTargetUpdate;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            OnMove();
        }

        private void OnAttack()
        {
            if (_inAnimation) return;

            _animator.SetTrigger("SwordAttack");
            _inAnimation = true;
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




        private void OnDisable()
        {
            _inputs.OnAttackEvent -= OnAttack;
            _inputs.OnTargetEvent -= OnTargetUpdate;
        }
    }
}
