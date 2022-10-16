using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class FocusUI : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Color _defaultColor;
        [SerializeField]
        private Color _friendColor;
        [SerializeField]
        private Color _enemyColor;
        [Space, SerializeField]
        private TextMeshProUGUI _nameText;
        [SerializeField]
        private TextMeshProUGUI _healthText;
        [Space, SerializeField, Range(10f, 500f)]
        private float _maxDistance = 500f;


        void LateUpdate()
        {
            var ray = Camera.main.ScreenPointToRay(transform.position);
            Units.UnitStateComponent stats;
            if(Physics.Raycast(ray, out var hit, _maxDistance))
            {
                stats = hit.transform.GetComponent<Units.UnitStateComponent>();
                if (stats == null)
                {
                    ClearFocus();
                    return;
                }

            }
            else
            {
                ClearFocus();
                return;
            }

            _nameText.text = string.Concat(stats.Name);
            _healthText.text = string.Concat(stats.CurrentHealth, '/', stats.MaxHealth);

            switch(stats.SideType)
            {
                case SideType.Friendly:
                    _image.color = _friendColor;
                    _nameText.color = _friendColor;
                    break;
                case SideType.Enemy:
                    _image.color = _enemyColor;
                    _nameText.color = _enemyColor;
                    break;
            }
        }


        private void ClearFocus()
        {
            _image.color = _nameText.color = _defaultColor;
            _nameText.text = _healthText.text = string.Empty;
        }



    }
}
