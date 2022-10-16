using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units.Player
{
    public class PlayerUnit : Unit
    {
        [SerializeField]
        private CameraComponent _camera;
        [SerializeField]
        private WeaponSetComponent _sets;
        //private WeaponType _weaponType = WeaponType.SwordAndShield;
        [SerializeField, Range(0.1f, 10f)]
        private float _rotateSpeed = 5f;

        //is the weapon taken
        private bool _isArms;

        protected override void FindNewTarget()
        {
            var units = _unitManager.GetNPCs;
            var distance = _sqrtFindTargetDistance;
            Target = null;

            foreach(var unit in units)
            {
                if (unit.GetStats.SideType == GetStats.SideType) continue;
                var currentDistance = (unit.transform.position - transform.position).sqrMagnitude;
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    Target = unit;
                }
            }
        }
        protected override void OnMove()
        {
            base.OnMove();
            ref var movement = ref _inputs.MoveDirection();

            if(Target != null)
            {
                transform.rotation = Quaternion.Euler(0f, _camera.PivotTransform.eulerAngles.y, 0f);
            }
            else if(movement.z > 0f && movement.x == 0f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.Euler(0f, _camera.transform.eulerAngles.y, 0f), _rotateSpeed * Time.deltaTime);
            }
            else if(movement.z < 0f && movement.x == 0f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.Euler(0f, _camera.transform.eulerAngles.y, 0f), _rotateSpeed / 2f * Time.deltaTime);
            }


            if ((movement.z <= 0f || movement.x != 0f) && _stats.InSprint)
                OnSprint();

        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();            
            if (_camera == null)
                _camera = this.FindComponentInChildren<CameraComponent>();
            _sets = this.FindComponent<WeaponSetComponent>();
        }

        private void OnRearms(WeaponType type)
        {
            _sets.WeaponType = type;

            switch (type)
            {
                case WeaponType.None:
                    break;
                case WeaponType.SwordAndShield:
                    _animator.SetLayerWeight(_animator.GetLayerIndex("SwordAndShield"), 1f);
                    _animator.SetLayerWeight(_animator.GetLayerIndex("Bow"), 0f);
                    break;
                case WeaponType.Bow:
                    _animator.SetLayerWeight(_animator.GetLayerIndex("SwordAndShield"), 0f);
                    _animator.SetLayerWeight(_animator.GetLayerIndex("Bow"), 1f);
                    break;
                case WeaponType.TwoHandedSword:
                    break;
                case WeaponType.Mage:
                    break;
            }
        }

        private void OnCrouch()
        {
            _stats.InCrouch = !_stats.InCrouch;
            _animator.SetBool("Crouch", _stats.InCrouch);

            if (_stats.InCrouch && _stats.InSprint) OnSprint();
        }
        
        private void OnSprint()
        {
            _stats.InSprint = !_stats.InSprint;
            _animator.SetBool("Sprint", _stats.InSprint);

            if (_stats.InSprint && _stats.InCrouch) OnCrouch();
        }

        private void OnJump()
        {

        }

        protected override void BindingEvents(bool unbind = false)
        {
            base.BindingEvents(unbind);
            var inputs = (PlayerInputComponent)_inputs;

            if (unbind)
            {
                inputs.RangeSetEventHandler -= () => OnRearms(WeaponType.SwordAndShield);
                inputs.RangeSetEventHandler -= () => OnRearms(WeaponType.Bow);
                inputs.SprintEventHandler -= OnSprint;
                inputs.CrouchEventHandler -= OnCrouch;
                //inputs.JumpEventHandler -= OnJump;
                return;
            }

            inputs.RangeSetEventHandler += () => OnRearms(WeaponType.SwordAndShield);
            inputs.RangeSetEventHandler += () => OnRearms(WeaponType.Bow);
            inputs.SprintEventHandler += OnSprint;
            inputs.CrouchEventHandler += OnCrouch;
            //inputs.JumpEventHandler += OnJump;
        }

    }
}
