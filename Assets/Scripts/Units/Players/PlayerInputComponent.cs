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

        protected override void Awake()
        {
            base.Awake();
            _controls = new PlayerControls();
            _controls.Unit.SwordAttack.performed += OnMainAttack;
            _controls.Unit.LockTarget.performed += OnTargetLock;
            _controls.Unit.ShieldAttack.performed += OnAdditionalAttack;
        }

        private void OnMainAttack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            CallSimplerHandle(nameof(MainAttackEventHandler));
        }

        private void OnAdditionalAttack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            CallSimplerHandle(nameof(AdditionalAttackEventHandler));
        }
        private void OnTargetLock(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            CallSimplerHandle(nameof(TargetEventHandler));
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
            _controls.Unit.SwordAttack.performed -= OnMainAttack;
            _controls.Unit.LockTarget.performed -= OnTargetLock;
            _controls.Unit.ShieldAttack.performed -= OnAdditionalAttack;
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
