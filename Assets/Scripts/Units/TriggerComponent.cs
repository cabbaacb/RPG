using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units
{
    public class TriggerComponent : MonoBehaviour
    {
        [SerializeField]
        private int _id = 0;

        private Collider _collider;
        private Unit _unit;
        private UnitStatsComponent _stats;
        

        public int GetID => _id;
        public bool Enable
        {
            get => _collider.enabled;
            set => _collider.enabled = value;
        }

        public void Construct(Unit unit, UnitStatsComponent stats)
        {
            _unit = unit;
            _stats = stats;

        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Enable ? Color.green : Color.red;

            Gizmos.DrawCube(_collider.bounds.center, _collider.bounds.size);
        }


        // Start is called before the first frame update
        void Start()
        {
            if(_collider == null)
                _collider = GetComponent<Collider>();
            _collider.enabled = false;
            _collider.isTrigger = true;
        }

        private void OnValidate()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var unit = other.GetComponent<UnitStatsComponent>();
            if (unit == null) return;

            unit.CurrentHealth -= 5f;

            if(_id == 115)
            {
                var body = other.GetComponent<Rigidbody>();
                if (body == null) return;
                body.constraints = RigidbodyConstraints.None;
                other.transform.LookAt(_unit.transform);
                body.AddForce(-other.transform.forward * 1000f, ForceMode.Impulse);
            }


            if (unit.CurrentHealth <= 0f) Destroy(unit.gameObject);
        }




    }
}
