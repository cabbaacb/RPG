using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Units.Player
{
    public class CameraComponent : MonoBehaviour
    {
        [SerializeField, Range(-90f, 0f), Tooltip("Min camera angle")]
        private float _minY = -45f;
        [SerializeField, Range(0f, 90f), Tooltip("Max camera angle")]
        private float _maxY = 30f;
        [Space, SerializeField, Range(0.5f, 10f)]
        private float _moveSpeed = 5f;
        [SerializeField, Range(0.5f, 10f)]
        private float _rotateSpeed = 3f;

        private PlayerControls _controls;
        private Unit _target;

        private Transform _pivot;
        private Transform _camera;

        //current rotation
        private float _angleX;
        private float _angleY;

        private Quaternion _pivotTargetRotation;
        private Vector3 _pivotEulers;

        private void Start()
        {
            _target = transform.parent.GetComponent<Unit>();
            _pivot = transform.GetChild(0);

            _camera = GetComponentInChildren<Camera>().transform;
            transform.parent = null;
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _target.transform.position, Time.deltaTime * _moveSpeed);
            FreeCamera();
        }

        private void Awake()
        {
            _controls = new PlayerControls();
        }

        private void FreeCamera()
        {
            var delta = _controls.Camera.Delta.ReadValue<Vector2>();

            _angleX += delta.x * _rotateSpeed;
            _angleY += delta.y * _rotateSpeed;
            _angleY = Mathf.Clamp(_angleY, _minY, _maxY);

            _pivotTargetRotation = Quaternion.Euler(_angleY, _pivotEulers.y, _pivotEulers.z);
        }

        private void OnEnable()
        {
            _controls.Camera.Enable();
        }

        private void OnDisable()
        {
            _controls.Camera.Disable();
        }

        private void OnDestroy()
        {
            _controls.Dispose();
        }
    }
}
