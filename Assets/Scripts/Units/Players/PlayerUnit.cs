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


        //is the weapon taken
        private bool _isArms;

        protected override void OnRotate()
        {
            transform.rotation = new Quaternion(0f, _camera.PivotTransform.rotation.y, 0f, _camera.PivotTransform.rotation.w);
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
