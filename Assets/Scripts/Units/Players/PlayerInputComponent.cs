using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Units.Player
{
    public class PlayerInputComponent : UnitInputComponent
    {
        private PlayerControls _controls;
        

        // Start is called before the first frame update
        void Start()
        {
            
        }

        private void Awake()
        {
            _controls = new PlayerControls();
            _controls.Unit.SwordAttack.performed += OnSwordAttack;
        }

        private void OnSwordAttack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            CallOnAttackEvent();
        }

        // Update is called once per frame
        void Update()
        {
            _movement = SetMovement();
        }

        private void OnEnable()
        {
            _controls.Unit.Enable();
        }

        private void OnDisable()
        {
            _controls.Unit.Disable();
        }

        private void OnDestroy()
        {
            _controls.Dispose();
            _controls.Unit.SwordAttack.performed -= OnSwordAttack;
        }

        private Vector3 SetMovement()
        {
            Vector3 direction = new Vector3();
            direction.x = _controls.Unit.Horizontal.ReadValue<float>();
            direction.y = 0f;
            direction.z = _controls.Unit.Vertical.ReadValue<float>();

            return direction;
        }



    }
}
