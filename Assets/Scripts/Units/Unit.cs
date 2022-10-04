using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units
{
    public class Unit : MonoBehaviour
    {
        private Animator _animator;
        private UnitInputComponent _inputs;
        private UnitStatsComponent _stats;
        private bool _inAnimation;

        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _inputs = GetComponent<UnitInputComponent>();
            _stats = GetComponent<UnitStatsComponent>();

            if (_inputs == null) return;

            _inputs.OnAttackEvent += OnAttack;
        }

        // Update is called once per frame
        void Update()
        {
            OnMove();
        }

        private void OnAttack(object sender, System.EventArgs args)
        {
            if (_inAnimation) return;

            _animator.SetTrigger("SwordAttack");
            _inAnimation = true;
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
    }
}
