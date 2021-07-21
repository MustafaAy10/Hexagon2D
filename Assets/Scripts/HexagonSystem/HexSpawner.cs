using Hexagon2D.EventSystem;
using Hexagon2D.GridSystem;
using Hexagon2D.InputSystem;
using Hexagon2D.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon2D.HexagonSystem
{
    public class HexSpawner : MonoBehaviour
    {
        [SerializeField] private Hexagon _hexPrefab;

        [SerializeField] private InputManager _inputManager;
        [SerializeField] private CreateSettings _createSettings;

        private Vector2 _hexSize;
        private PositionHandler _positionHandler;
        private GridHex _gridHex;
        private int _counter;

        private void Awake()
        {
            EventManager.Instance.ResetAllEvents();
            Vector3 size = _hexPrefab.GetComponent<SpriteRenderer>().bounds.size;
            _hexSize = size.x < 0 ? Vector3.one : size * 1.05f;
            Debug.Log("Hexagon Sprite Size: " + _hexSize);
            _positionHandler = new PositionHandler(_createSettings._gridSize, _hexSize);
            _gridHex = new GridHex(_createSettings._gridSize);

            for (int x = 0; x < _createSettings._gridSize.x; x++)
            {
                for (int y = 0; y < _createSettings._gridSize.y; y++)
                {
                    CreateHexagon(new Vector2Int(x, y));
                }
            }
            _inputManager.Initialize(_gridHex, _hexSize, _positionHandler);
            EventManager.Instance.InvokePause();
        }

        public Hexagon CreateHexagon(Vector2Int index, bool createSingleHexagon = false)
        {
            Vector2 position = _positionHandler.GetPosition(index);
            Hexagon hex = Instantiate(_hexPrefab, position + Vector2.up * _hexSize.y * _createSettings._gridSize.y, Quaternion.identity, transform);

            // Check neighbor colorId to avoid being same color with neihgbor hexagons.
            // By this way there will be no hexagons ready to explode at the beginning.
            int colorID = Random.Range(0, _createSettings._colors.Length);
            // If we are creating single hexagon to replace exploded Bomb, color can be same with neighbors. (createSingleHexagon == true)
            // If we are creating many hexagons on the start of the game, neighbors color should be different.
            if (0 < index.x && !createSingleHexagon)
            {
                int neighborColorId = _gridHex.Get(index.x - 1, index.y).ColorID;
                while (neighborColorId == colorID)
                    colorID = Random.Range(0, _createSettings._colors.Length);
            }

            hex.GetComponent<SpriteRenderer>().color = _createSettings._colors[colorID];
            hex.Initialize(colorID, index);
            _gridHex.Set(index.x, index.y, hex);
            if (!createSingleHexagon)
                StartCoroutine(MoveDown(hex, position));
            return hex;
        }

        private IEnumerator MoveDown(Hexagon hex, Vector2 targetPosition)
        {
            Vector2 startPosition = hex.transform.position;
            float t = 0;
            while (t <= 1)
            {
                t += Time.deltaTime * _createSettings._moveSpeed;
                hex.transform.position = Vector2.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            hex.transform.position = targetPosition;

            _counter++;
            if (_counter >= _createSettings._gridSize.x * _createSettings._gridSize.y)
                EventManager.Instance.InvokePlay();
        }
    }
}