using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Units
{
    public class UnitInputComponent : MonoBehaviour
    {
        protected Vector3 _movement;

        public ref Vector3 MoveDirection() => ref _movement;
        public SimpleHandle OnAttackEvent;
        public SimpleHandle OnTargetEvent;
        public SimpleHandle OnShieldEvent;

        protected void CallOnAttackEvent() => OnAttackEvent?.Invoke();
        protected void CallOnTargetEvent() => OnTargetEvent?.Invoke();
        protected void CallOnShieldEvent() => OnShieldEvent?.Invoke();
        
    }
}
