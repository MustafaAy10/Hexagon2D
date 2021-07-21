using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hexagon2D.Settings;
using Hexagon2D.IntersectionSystem;
using Hexagon2D.HexagonSystem;
using Hexagon2D.GridSystem;
using Hexagon2D.EventSystem;

namespace Hexagon2D.ExplosionControl
{
    public class Select : MonoBehaviour
    {

        [SerializeField] RotateSettings _rotateSettings;
        [SerializeField] ExplosionController _explosionController;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private GridHex _gridHex;
        private IntersectManager _intersectManager;
        private Intersection _selectedIntersection;
        private WaitForSeconds _rotateWait;
        private Hexagon[] _hexNeighbors = new Hexagon[3];
        private List<Hexagon> _explosionList;

        public bool IsSelected => _selectedIntersection != null;

        public void Initialize(GridHex gridHex, PositionHandler positionHandler, Vector3 hexSize)
        {
            _rotateWait = new WaitForSeconds(_rotateSettings._rotateWaitSeconds);
            _explosionList = new List<Hexagon>();
            _gridHex = gridHex;
            _intersectManager = new IntersectManager(gridHex, positionHandler, hexSize);
            _explosionController.Initialize(gridHex, positionHandler, _intersectManager, hexSize);
            EventManager.Instance.OnExplosionStart += Deactivate;
            EventManager.Instance.OnExplosionEnd += Activate;
        }

        public void Activate()
        {
            _spriteRenderer.enabled = true;
        }

        public void Deactivate()
        {
            _spriteRenderer.enabled = false;
        }

        public void SelectNearestIntersectionPoint(Vector2 touchPosition)
        {
            _selectedIntersection = _intersectManager.FindNearestIntersection(touchPosition);
            transform.position = _selectedIntersection.position;
            transform.rotation = Quaternion.Euler(0, (_selectedIntersection.isleft ? 0 : 180), 0);
        }

        public void Rotate(bool clockwise)
        {
            EventManager.Instance.InvokePause();
            StartCoroutine(RotateAround(clockwise));
        }

        private IEnumerator RotateAround(bool clockwise)
        {
            float time;
            Vector3 direction = clockwise ? Vector3.back : Vector3.forward;

            // We get neighbor Hexagons of selected intersection points.
            _intersectManager.GetNeighbors(_selectedIntersection, ref _hexNeighbors);

            for (int i = 0; i < 3; i++)
            {
                time = 0;
                while (time * _rotateSettings._rotateSpeed <= 120f)
                {
                    time += Time.deltaTime;
                    transform.RotateAround(_selectedIntersection.position, direction, _rotateSettings._rotateSpeed * Time.deltaTime);
                    for (int n = 0; n < _selectedIntersection.myNeighbors.Length; n++)
                        _hexNeighbors[n].transform.RotateAround(_selectedIntersection.position, direction, _rotateSettings._rotateSpeed * Time.deltaTime);
                    yield return null;
                }

                // Changing the indexes of hexagons every time they rotate.
                _gridHex.SwapRotate(in _hexNeighbors, clockwise);

                for (int n = 0; n < _selectedIntersection.myNeighbors.Length; n++)
                    _hexNeighbors[n].transform.rotation = Quaternion.identity;

                // Checking for explosion locally (around the _selectedIntersection point), there is no need to check all intersection points.  
                if (_intersectManager.CheckExplosionLocal(_selectedIntersection.myIndex.x, _selectedIntersection.myIndex.y, ref _explosionList))
                {
                    if(_explosionList.Count(x=> x is Bomb) == 0)
                        EventManager.Instance.InvokeCountDown();    // if there is no bomb in the explosionList, we can count down.

                    _explosionController.Explode(ref _explosionList);

                    yield break;
                }
                yield return _rotateWait;
            }
            EventManager.Instance.InvokePlay();
        }

        private void OnDestroy()
        {
            EventManager.Instance.OnExplosionStart -= Deactivate;
            EventManager.Instance.OnExplosionEnd -= Activate;
        }
    }
}