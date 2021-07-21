using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon2D.GridSystem
{
    public class PositionHandler
    {
        private Vector2Int _gridSize;
        public Vector2Int gridSize => _gridSize;

        private Vector2 _hexSize;
        public Vector2 hexSize => _hexSize;

        private Vector2 _startPosition;
        public Vector2 startPosition => _startPosition;

        private Vector2 _endPosition;
        public Vector2 endPosition => _endPosition;


        private static readonly float _xMultiplier = 0.75f;

        public PositionHandler(Vector2Int gridSize, Vector2 hexSize)
        {
            _gridSize = gridSize;
            _hexSize = hexSize;
            Vector2 boardSizeInUnits = GetGridSize();
            _startPosition = new Vector2(-boardSizeInUnits.x / 2, -boardSizeInUnits.y / 2);
            Debug.Log("[PositionHandler] StartPosition: " + _startPosition);
            _endPosition = _startPosition + new Vector2(boardSizeInUnits.x, boardSizeInUnits.y);
        }

        public float GetDistanceBetweenColoumns()
        {
            return _hexSize.x * _xMultiplier;
        }

        public Vector3 GetPosition(Vector2Int indexes)
        {
            return new Vector2(_startPosition.x + GetDistanceBetweenColoumns() * indexes.x,
                _startPosition.y + _hexSize.y * indexes.y - (indexes.x % 2 == 1 ? _hexSize.y / 2 : 0));
        }

        public Vector2 GetGridSize()
        {
            return new Vector2(GetDistanceBetweenColoumns() * (_gridSize.x - 1) + _hexSize.x, _hexSize.y * (_gridSize.y + 0.5f));
        }

    }
}