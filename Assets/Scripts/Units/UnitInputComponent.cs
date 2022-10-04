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
        public EventHandler OnAttackEvent;

        protected void CallOnAttackEvent() => OnAttackEvent?.Invoke(null, null);
        
    }
}
