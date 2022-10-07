using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units.Player
{
    public class PlayerUnit : Unit
    {
        [SerializeField]
        private CameraComponent _camera;

        protected override void OnRotate()
        {
            transform.rotation = new Quaternion(0f, _camera.PivotTransform.rotation.y, 0f, _camera.PivotTransform.rotation.w);
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _camera = this.FindComponentInChildren<CameraComponent>();
        }



    }
}
