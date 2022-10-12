using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units.Player
{
    public class WeaponSetComponent : MonoBehaviour
    {
        [SerializeField]
        private WeaponSet[] _weapons;
        [SerializeField]
        private WeaponType _type = WeaponType.None;

        public WeaponType WeaponType
        {
            get => _type;
            set
            {
                ChangeSet(_type, value);
                _type = value;
            }
        }

        private void ChangeSet(WeaponType disableType, WeaponType enableType)
        {
            foreach (var weapon in _weapons)
            {
                if (weapon.Type == disableType) weapon.Object.SetActive(false);
                else if (weapon.Type == enableType) weapon.Object.SetActive(true);
            }
        }

#if UNITY_EDITOR

        [ContextMenu("Disable All Sets")]
        private void DisableAllSets()
        {
            foreach (var weapon in _weapons)
                weapon.Object.SetActive(false);
        }
        [ContextMenu("Enable All Sets")]
        private void EnableAllSets()
        {
            foreach (var weapon in _weapons)
                weapon.Object.SetActive(true);
        }
#endif


        [System.Serializable]
        internal struct WeaponSet
        {
            public WeaponType Type;
            public GameObject Object;
        }


    }
}
