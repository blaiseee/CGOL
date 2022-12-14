using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CGOL.Scripts
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public struct MinMax<T>
        {
            public T Min;
            public T Max;
        }

        public class SetupCameraEvent : UnityEvent<int, int> { }

        public static SetupCameraEvent SetupCamera;
        private Camera _camera;
        private Bounds _bounds;
        private Vector3 _initialMousePos;
        private Vector3 _originalPos;
        private Transform _transform;
        private MinMax<float> _orthographicSize;

        private const float ZoomSpeed = 100.0f;

        private float _zoomInputDelta;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _transform = GetComponent<Transform>();

            if (SetupCamera == null)
            {
                SetupCamera = new SetupCameraEvent();
            }
        }

        void OnEnable()
        {
            SetupCamera.AddListener(Setup);
        }

        void OnDisable()
        {
            SetupCamera.RemoveListener(Setup);
        }

        private void Setup(int gridWidth, int gridHeight)
        {
            var isWidthEven = (gridWidth & 0x1) == 0;
            var isHeightEven = (gridHeight & 0x1) == 0;

            _camera.orthographic = true;

            _originalPos = transform.position = new Vector3(isWidthEven ? -0.5f : 0.0f, isHeightEven ? -0.5f : 0.0f, -10.0f);

            var aspectRatioMultiplier = _camera.aspect >= 1.0f ? 1.0f : 1.0f / _camera.aspect;
            _orthographicSize.Max = _camera.orthographicSize = aspectRatioMultiplier * 0.5f * Mathf.Max(gridWidth, gridHeight);
            _orthographicSize.Min = aspectRatioMultiplier - 0.5f;
            _bounds = new Bounds(_camera, _orthographicSize.Max);
            _bounds.Update(_camera);
        }

        public class Bounds
        {
            private readonly float _hExtent;
            private readonly float _vExtent;
            private readonly float _maxOrthographicSize;

            private Vector3 _bottomLeft;
            private Vector3 _topRight;

            private float _top;
            private float _bottom;
            private float _left;
            private float _right;

            public Bounds(Camera camera, float max)
            {
                _vExtent = camera.orthographicSize;
                _hExtent = camera.aspect * _vExtent;

                _bottomLeft = camera.ViewportToWorldPoint(Vector2.zero);
                _topRight = camera.ViewportToWorldPoint(Vector2.one);

                _maxOrthographicSize = max;
            }

            public void Update(Camera camera)
            {
                var deltaOrthographicSize = _maxOrthographicSize - camera.orthographicSize;

                var vDeltaExtent = deltaOrthographicSize;
                var hDeltaExtent = camera.aspect * vDeltaExtent;

                _top = _topRight.y - _vExtent + vDeltaExtent;
                _bottom = _bottomLeft.y + _vExtent - vDeltaExtent;
                _left = _bottomLeft.x + _hExtent - hDeltaExtent;
                _right = _topRight.x - _hExtent + hDeltaExtent;
            }

            public Vector3 ClampCamera(Vector3 position)
            {
                position.x = Mathf.Clamp(position.x, _left, _right);
                position.y = Mathf.Clamp(position.y, _top, _bottom);
                return position;
            }
        }
    }
}
