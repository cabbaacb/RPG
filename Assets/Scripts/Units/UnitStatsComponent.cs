using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Units
{
    public class UnitStatsComponent : MonoBehaviour
    {
        [Range(0f, 10f)]
        public float MoveSpeed = 3f;
    }
}
