using System.Collections.Generic;
using UnityEngine;

namespace Hexagon2D.IntersectionSystem
{
    public class Intersection
    {
        private bool _isLeft;
        public bool isleft => _isLeft;

        private Vector2Int _myIndex;
        public Vector2Int myIndex => _myIndex;

        private Vector2Int[] _myNeighbors;
        public Vector2Int[] myNeighbors => _myNeighbors;

        private Vector2 _position;
        public Vector2 position => _position;

        public Intersection(bool isLeft, Vector2Int myIndex, Vector2Int[] myNeighborIndexes, Vector2 position)
        {
            _isLeft = isLeft;
            _myIndex = myIndex;
            _myNeighbors = myNeighborIndexes;
            _position = position;
        }

    }
}