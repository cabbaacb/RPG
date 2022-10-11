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
        private WeaponType _weaponType = WeaponType.SwordAndShield;
        [SerializeField, Range(0.1f, 10f)]
        private float _rotateSpeed = 5f;

        //is the weapon taken
        private bool _isArms;

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

        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();            
            if (_camera == null)
                _camera = this.FindComponentInChildren<CameraComponent>();
        }

        private void OnRearms(WeaponType type)
        {
            _weaponType = type;

            switch (_weaponType)
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

        protected override void BindingEvents(bool unbind = false)
        {
            base.BindingEvents(unbind);
            var inputs = (PlayerInputComponent)_inputs;

            if (unbind)
            {
                inputs.RangeSetEventHandler -= () => OnRearms(WeaponType.SwordAndShield);
                inputs.RangeSetEventHandler -= () => OnRearms(WeaponType.Bow);
                return;
            }

            inputs.RangeSetEventHandler += () => OnRearms(WeaponType.SwordAndShield);
            inputs.RangeSetEventHandler += () => OnRearms(WeaponType.Bow);
        }

    }
}
