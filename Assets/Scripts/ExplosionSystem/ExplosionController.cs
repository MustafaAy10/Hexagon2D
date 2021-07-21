using Hexagon2D.EventSystem;
using Hexagon2D.GridSystem;
using Hexagon2D.HexagonSystem;
using Hexagon2D.IntersectionSystem;
using Hexagon2D.Settings;
using Hexagon2D.UI.ControlSystem;
using Hexagon2D.UI.PanelSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hexagon2D.ExplosionControl
{
    public class ExplosionController : MonoBehaviour
    {
        private int _lastBombScore;
        private GridHex _gridHex;
        private Vector3 _hexSize;
        private PositionHandler _positionHandler;
        private IntersectManager _intersectManager;
        private WaitForSeconds _explosionWait;
        public List<Hexagon> _explosionList;
        private int _moveCountStart;
        private int _moveCountEnd;
        private bool _active = true;

        [SerializeField] private ExplosionSettings _explosionSettings;
        [SerializeField] private CreateSettings _createSettings;
        [SerializeField] private CanvasController _canvasController;
        [SerializeField] private HexSpawner _hexSpawner;
        [SerializeField] private Bomb _bombPrefab;

        public void Initialize(GridHex gridHex, PositionHandler positionHandler, IntersectManager intersectManager, Vector3 hexSize)
        {

            _lastBombScore = _explosionSettings._pointsToCreateBomb;
            _gridHex = gridHex;
            _hexSize = hexSize;
            _positionHandler = positionHandler;
            _intersectManager = intersectManager;
            _explosionWait = new WaitForSeconds(_explosionSettings._explosionWaitSeconds);
            _canvasController.Initialize();
            EventManager.Instance.OnGameOver += Deactivate;
        }

        public void Activate()
        {
            _active = true;
        }

        public void Deactivate()
        {
            _active = false;
        }

        public void Explode(ref List<Hexagon> explosionList)
        {
            EventManager.Instance.InvokeExplosionStart();
            EventManager.Instance.InvokeScore(explosionList.Count * _explosionSettings._pointsPerExplodedHexagon);
            _explosionList = explosionList;
            _moveCountEnd = 0;
            _moveCountStart = 0;

            CheckForBomb();  // If there are bombs in the explosion list, first we should eleminate them, we dont want use bombs again, right after the explosion.

            if (ScorePanel.score >= _lastBombScore)
            {
                _lastBombScore += _explosionSettings._pointsToCreateBomb;
                _explosionList.Add(CreateBomb());
                Debug.Log("[ExplosionController] Destroying object: " + explosionList[0].gameObject.name);
                Destroy(explosionList[0].gameObject);
                _explosionList.RemoveAt(0);
            }

            for (int k = 0; k < _explosionList.Count; k++)
            {
                _explosionList[k].Deactivate();
                _gridHex.Set(_explosionList[k].Index.x, _explosionList[k].Index.y, null);
            }

            StartCoroutine(SetExplosion());
        }

        public IEnumerator SetExplosion()
        {
            yield return _explosionWait;

            ChangeColoumns();
            FillGrid();

            yield return new WaitUntil(() => _moveCountStart == _moveCountEnd);  // we are waiting for all MoveDown coroutines finished

            if (_active && _intersectManager.CheckExplosionAll(ref _explosionList))  // If it is _active (not gameover yet) and if there are new explosions then Explode()
            {
                Explode(ref _explosionList);
                yield break;
            }

            if (_active)
                EventManager.Instance.InvokeExplosionEnd();
        }

        // After explosion we are moving existing hexagons at coloumns down to fill the space.
        public void ChangeColoumns()
        {
            int inActiveCount = 0;
            for (int x = 0; x < _gridHex.gridSize.x; x++)
            {
                for (int y = 0; y < _gridHex.gridSize.y; y++)
                {
                    Hexagon hex = _gridHex.Get(x, y);
                    if (hex == null)
                        inActiveCount++;
                    else if (inActiveCount > 0)
                    {
                        _gridHex.Set(x, y - inActiveCount, hex); // we are moving down.
                        _gridHex.Set(x, y, null);  // we are moving down, so the current index should be null, a hexagon can only be at one place on grid, cant be two place at the same time.
                        _moveCountStart++;
                        StartCoroutine(MoveDown(hex));
                    }
                }
                inActiveCount = 0;
            }
        }

        // After explosion we didnt destroy exploded hexagons, just deactive them, now we use them to fill the grid,
        // by this way we dont need to use destroy and instantiate hexagons again and again, performance optimization :)
        public void FillGrid()
        {
            float counter = 0;
            Hexagon bomb = null;

            for (int x = 0; x < _gridHex.gridSize.x; x++)
            {
                for (int y = 0; y < _gridHex.gridSize.y; y++)
                {
                    Hexagon hex = _gridHex.Get(x, y);
                    if (hex == null)
                    {
                        hex = _explosionList[0];
                        _gridHex.Set(x, y, hex);

                        if (!(hex is Bomb))  // If the hexagon is not bomb, we need to change its color.
                        {
                            int colorID = Random.Range(0, _createSettings._colors.Length);
                            hex.ChangeColor(_createSettings._colors[colorID], colorID);
                        }
                        else
                        {
                            bomb = hex;  // we will check color of bomb at the end of this method.
                        }

                        counter++;
                        hex.transform.position = (Vector2)_positionHandler.GetPosition(new Vector2Int(x, _gridHex.gridSize.y)) + Vector2.up * (counter + 1f) * _hexSize.y;
                        hex.Activate();

                        _explosionList.RemoveAt(0);
                        _moveCountStart++;
                        StartCoroutine(MoveDown(hex));
                    }
                }
                counter = 0;
            }

            if (bomb)
            {
                CheckBombColor(bomb);
            }
        }

        // we assign a color to bomb different then neighbors of it, by this way bomb will not explode when bomb take place in the grid after FillGrid method.
        private void CheckBombColor(Hexagon bomb)
        {
            int previousColorID = _gridHex.Get(Mathf.Max(0, bomb.Index.x - 1), bomb.Index.y).ColorID;
            int nextColorID = _gridHex.Get(Mathf.Min(bomb.Index.x + 1, _gridHex.gridSize.x - 1), bomb.Index.y).ColorID;

            int currentColorID = Random.Range(0, _createSettings._colors.Length);
            while ((previousColorID == currentColorID) || (nextColorID == currentColorID))
                currentColorID = Random.Range(0, _createSettings._colors.Length);
            Color color = _createSettings._colors[currentColorID];
            color.a = 0.6f;
            bomb.ChangeColor(color, currentColorID);
        }

        public IEnumerator MoveDown(Hexagon hex)
        {
            Vector2 startPosition = hex.transform.position;
            Vector2 targetPosition = _positionHandler.GetPosition(hex.Index);
            float t = 0;
            while (t <= 1)
            {
                t += Time.deltaTime * _explosionSettings._moveSpeed;
                hex.transform.position = Vector2.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
            hex.transform.position = targetPosition;
            _moveCountEnd++;
        }

        // We create a bomb instead of one of hexagon in the _explosionList if we reach _explosionSettings._pointsToCreateBomb
        public Hexagon CreateBomb()
        {
            Debug.Log("[ExplosionController] Creating Bomb...");
            Vector2Int index = new Vector2Int(_explosionList[0].Index.x, _explosionList[0].Index.y);
            Hexagon bomb = Instantiate(_bombPrefab, _positionHandler.GetPosition(index), Quaternion.identity);
            bomb.Initialize(0, index);
            return bomb;
        }

        private void OnDestroy()
        {
            EventManager.Instance.OnGameOver -= Deactivate;
        }

        private void CheckForBomb()
        {
            // if there is bomb in the list we should destroy it, we can't use it again in the next FillGrid method.
            for (int i = 0; i < _explosionList.Count; i++)
            {
                if (_explosionList[i] is Bomb)
                {
                    Hexagon hex = _explosionList[i];
                    _explosionList.Remove(hex);
                    _explosionList.Add(_hexSpawner.CreateHexagon(hex.Index, true));   // Creating new Hexagon instead of Bomb, we will use this hexagon on FillGrid method after explosion.
                    Destroy(hex.gameObject);
                }
            }
        }
    }
}