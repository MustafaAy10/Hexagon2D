using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon2D.GridSystem
{
    public class GridGeneric<T> where T : class
    {
        protected T[,] _grid;
        protected Vector2Int _gridSize;
        public Vector2Int gridSize => _gridSize;

        public GridGeneric(Vector2Int gridSize)
        {
            _grid = new T[gridSize.x, gridSize.y];
            _gridSize = gridSize;
        }

        public virtual void Set(int x, int y, T gridElement)
        {
            if (CheckLimits(x, y))
            {
                _grid[x, y] = gridElement;
            }
            else
                Debug.LogError("[Grid] _grid Set limits are out of scope: " + new Vector2Int(x, y));
        }

        public T Get(int x, int y)
        {
            if (CheckLimits(x, y))
            {
                return _grid[x, y];
            }
            else
            {
                Debug.LogError("[Grid] _grid Get limits are out of scope: " + new Vector2Int(x, y));
                return null;
            }
        }

        private bool CheckLimits(int x, int y)
        {
            if (0 <= x && x < _gridSize.x
                && 0 <= y && y < _gridSize.y)
                return true;
            return false;
        }
    }
}