using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexagon2D.ExplosionControl;
using Hexagon2D.GridSystem;
using Hexagon2D.EventSystem;

namespace Hexagon2D.InputSystem
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private Select _select;

        private Vector2 _startPosition;
        private Camera _camera;
        private Vector2Int _gridSize;
        private Vector3 _hexSize;
        private bool _active = false;

        public void Initialize(GridHex gridHex, Vector3 hexSize, PositionHandler positionHandler)
        {
            _select.Initialize(gridHex, positionHandler, hexSize);
            _camera = Camera.main;
            _gridSize = gridHex.gridSize;
            _hexSize = hexSize;
            EventManager.Instance.OnPlay += Activate;
            EventManager.Instance.OnPause += Deactivate;
        }

        public void Activate()
        {
            _active = true;
        }

        public void Deactivate()
        {
            _active = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_active)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                _startPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {

                Vector2 endPosition = _camera.ScreenToWorldPoint(Input.mousePosition);

                //Debug.Log("[InputManager] input: sqrMagnitude: " + (endPosition - _startPosition).sqrMagnitude);
                //Debug.Log("[InputManager] input: _hexSize.x * _gridSize.x / 40:  " + (_hexSize.x * _gridSize.x / 40));
                if ((endPosition - _startPosition).sqrMagnitude < _hexSize.x * _gridSize.x / 40)
                {
                    if (!_select.IsSelected)
                        _select.Activate();
                    _select.SelectNearestIntersectionPoint(endPosition);
                    return;
                }

                if (!_select.IsSelected)
                    return;

                Vector2 difference = endPosition - _startPosition;
                bool clockwise = true;
                if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
                {
                    if (difference.x > 0)
                        clockwise = true;
                    else
                        clockwise = false;
                }
                else
                {
                    if (difference.y > 0)
                        clockwise = true;
                    else
                        clockwise = false;
                }

                _select.Rotate(clockwise);
            }

        }

        private void OnDestroy()
        {
            EventManager.Instance.OnPlay -= Activate;
            EventManager.Instance.OnPause -= Deactivate;
        }

    }
}