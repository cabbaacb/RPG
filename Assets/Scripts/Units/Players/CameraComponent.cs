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
        [Space, SerializeField, Range(0.5f, 5f)]
        private float _moveSpeed = 3f;
        [SerializeField, Range(0.1f, 1f)]
        private float _rotateSpeed = 0.5f;
        [SerializeField, Range(10f, 0f)]
        private float _smooting = 1f;
        [SerializeField, Range(0.5f, 10f)]
        private float _lockCameraSpeed = 3f;


        private PlayerControls _controls;
        private Unit _target;

        
        private Transform _camera;

        //current rotation
        private float _angleX;
        private float _angleY;

        //camera rotation
        private Quaternion _transformTargetRotation;
        private Quaternion _pivotTargetRotation;
        private Vector3 _pivotEulers;
        private Quaternion _defaultCameraRotation;


        public Transform PivotTransform;
        //public Transform PivotTransform { get => _pivot; }

        private void Start()
        {
            _target = transform.parent.GetComponent<Unit>();
            PivotTransform = transform.GetChild(0);
            _pivotEulers = PivotTransform.eulerAngles;

            
            _camera = GetComponentInChildren<Camera>().transform;
            _defaultCameraRotation = _camera.localRotation;
            transform.parent = null;

            _target.OnTargetLostHandler += () => _camera.localRotation = _defaultCameraRotation;
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _target.transform.position, Time.deltaTime * _moveSpeed);
            if(_target.Target == null) FreeCamera();
            else LockCamera();
            
        }

        private void Awake()
        {
            _controls = new PlayerControls();
        }

        private void FreeCamera()
        {
            var delta = _controls.Camera.Delta.ReadValue<Vector2>();

            _angleX += delta.x * _rotateSpeed;
            _angleY -= delta.y * _rotateSpeed;
            _angleY = Mathf.Clamp(_angleY, _minY, _maxY);

            _pivotTargetRotation = Quaternion.Euler(_angleY, _pivotEulers.y, _pivotEulers.z);
            _transformTargetRotation = Quaternion.Euler(0f, _angleX, 0f);

            PivotTransform.localRotation = Quaternion.Slerp(PivotTransform.localRotation, _pivotTargetRotation, _smooting * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, _transformTargetRotation, _smooting * Time.deltaTime);


        }

        private void LockCamera()
        {
            var rotation = Quaternion.LookRotation(_target.Target.TargetPoint.position - _camera.position);
            _camera.rotation = Quaternion.Slerp(_camera.rotation, rotation, _lockCameraSpeed * Time.deltaTime);

            rotation = Quaternion.LookRotation(_target.Target.TargetPoint.position - PivotTransform.position);
            PivotTransform.rotation = Quaternion.Slerp(PivotTransform.rotation, rotation, _lockCameraSpeed * Time.deltaTime);
            //_camera.LookAt(_target.Target.transform.position, Vector3.up);
            //transform.LookAt(_target.Target.transform.position, Vector3.up);
        }

        private void OnDrawGizmos()
        {
            if (_target == null || PivotTransform == null || _camera == null) return;            

            Gizmos.DrawSphere(_target.transform.position, 0.15f);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.15f);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(PivotTransform.position, PivotTransform.forward);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_camera.position, _camera.forward);
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
