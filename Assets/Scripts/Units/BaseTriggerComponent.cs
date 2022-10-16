using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units
{
    public class BaseTriggerComponent : MonoBehaviour
    {
        public SimpleHandle<Collider, bool> OnTriggerCollisionEventHandler;
        private Collider _collider;

        public bool Enable
        {
            get => _collider.enabled;
            set => _collider.enabled = value;
        }

        private void Start()
        {
            _collider = this.FindComponent<Collider>();

        }

        private void OnTriggerEnter(Collider other) => OnTriggerCollisionEventHandler?.Invoke(other, true);
        private void OnTriggerExit(Collider other) => OnTriggerCollisionEventHandler?.Invoke(other, false);
    }
}
