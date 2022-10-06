using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Units
{
    public class UnitStatsComponent : MonoBehaviour
    {
        [Range(0f, 10f)]
        public float MoveSpeed = 3f;
        [Range(3f, 100f)]
        public float Health = 10f;
        public SideType SideType;
        public float CalldownShieldAttack = 6f;
        public float CurrentCalldown = 6f;

        //private List<(string, float)> _calldown = new List<(string, float)>();

        private void Update()
        {
            if (CurrentCalldown > 0f)
                CurrentCalldown -= Time.deltaTime;
        }
    }
}
