using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexagon2D.GridSystem;
using Hexagon2D.HexagonSystem;

namespace Hexagon2D.IntersectionSystem
{
    public class IntersectManager
    {
        private GridIntersect _gridIntersect;
        private GridHex _gridHex;
        private Vector3 _hexSize;
        private PositionHandler _positionHandler;
        private Hexagon[] _hexNeighbors = new Hexagon[3];

        public IntersectManager(GridHex gridHex, PositionHandler positionHandler, Vector3 hexSize)
        {
            _gridHex = gridHex;
            _hexSize = hexSize;
            _positionHandler = positionHandler;
            _gridIntersect = new GridIntersect(new Vector2Int(gridHex.gridSize.x - 1, gridHex.gridSize.y * 2 - 2));
            Create();
        }

        private void Create()
        {
            for (int y = 0; y < _gridIntersect.gridSize.y; y++)
            {
                for (int x = 0; x < _gridIntersect.gridSize.x; x++)
                {
                    CreateIntersectionPoints(x, y);
                }
            }
        }

        // Creating Intersection Points according to position of Hexagons and with hexagon neighbours indexes. 
        // Neighbor hexagons itselfs can change during the game but grid indexes of neighbor hexagons will not change, thats what the meaning of hexagonNeighboursOfIntersection.
        private void CreateIntersectionPoints(int x, int y)
        {
            Vector2 position = Vector2.zero;
            bool isLeft = true;
            Vector2Int[] hexagonNeighboursOfIntersection = null;

            if (x % 2 == 0 && y % 2 == 0)
            {
                position = _positionHandler.GetPosition(new Vector2Int(x, y / 2));   //  (x,y/2) means: we are converting intersection point grid to hexagon grid.
                position.x += _hexSize.x / 2;
                isLeft = true;  // isleft defines Select image looking direction, will it be look left or right.
                hexagonNeighboursOfIntersection = new Vector2Int[3] { new Vector2Int(x + 1, (y / 2) + 1), new Vector2Int(x + 1, (y / 2)), new Vector2Int(x, y / 2) };
                // hexagonNeighboursOfIntersection: we are finding corresponding neighbor grid hexagons index according to grid intersection index.
            }
            else if (x % 2 == 1 && y % 2 == 0)
            {
                position = _positionHandler.GetPosition(new Vector2Int(x + 1, y / 2));
                position.x -= _hexSize.x / 2;
                isLeft = false;
                hexagonNeighboursOfIntersection = new Vector2Int[3] { new Vector2Int(x, (y / 2) + 1), new Vector2Int(x + 1, y / 2), new Vector2Int(x, (y / 2)) };
            }
            else if (x % 2 == 0 && y % 2 == 1)
            {
                position = _positionHandler.GetPosition(new Vector2Int(x + 1, (y + 1) / 2));
                position.x -= _hexSize.x / 2;
                isLeft = false;
                hexagonNeighboursOfIntersection = new Vector2Int[3] { new Vector2Int(x, (y + 1) / 2), new Vector2Int(x + 1, (y + 1) / 2), new Vector2Int(x, ((y + 1) / 2) - 1) };
            }
            else if (x % 2 == 1 && y % 2 == 1)
            {
                position = _positionHandler.GetPosition(new Vector2Int(x, (y + 1) / 2));
                position.x += _hexSize.x / 2;
                isLeft = true;
                hexagonNeighboursOfIntersection = new Vector2Int[3] { new Vector2Int(x + 1, (y + 1) / 2), new Vector2Int(x + 1, ((y + 1) / 2) - 1), new Vector2Int(x, (y + 1) / 2) };
            }

            Intersection inter = new Intersection(isLeft, new Vector2Int(x, y), hexagonNeighboursOfIntersection, position);
            _gridIntersect.Set(x, y, inter);
            //Debug.LogFormat("[IntersectManager] _gridIntersect({0},{1}) neighbours: _gridHex({2},{3}) , _gridHex({4},{5}) , _gridHex({6},{7})",
            //                    x, y, inter.myNeighbors[0].x, inter.myNeighbors[0].y,
            //                    inter.myNeighbors[1].x, inter.myNeighbors[1].y,
            //                    inter.myNeighbors[2].x, inter.myNeighbors[2].y);
        }

        // When we touch the screen during the game, Select image will go to nearest intersection point.
        public Intersection FindNearestIntersection(Vector2 touchPosition)
        {
            float maxDistance = Mathf.Infinity;
            Intersection nearestIntersection = null;
            for (int y = 0; y < _gridIntersect.gridSize.y; y++)
            {
                for (int x = 0; x < _gridIntersect.gridSize.x; x++)
                {
                    Intersection intersection = _gridIntersect.Get(x, y);
                    float distance = (touchPosition - intersection.position).sqrMagnitude;
                    if (distance < maxDistance)
                    {
                        nearestIntersection = intersection;
                        maxDistance = distance;
                    }
                }
            }
            return nearestIntersection;
        }

        // Looking for matching hexagons (same color) in local area, around the intersection point.
        public bool CheckExplosionLocal(int k, int m, ref List<Hexagon> hexList)
        {
            hexList.Clear();

            for (int x = Mathf.Max(0, k - 3); x < Mathf.Min(k + 3, _gridIntersect.gridSize.x); x++)
            {
                for (int y = Mathf.Max(0, m - 3); y < Mathf.Min(m + 3, _gridIntersect.gridSize.y); y++)
                {
                    if (CheckTriple(_gridIntersect.Get(x, y)))
                    {
                        for (int j = 0; j < _hexNeighbors.Length; j++)
                        {
                            if (!hexList.Contains(_hexNeighbors[j]))
                            {
                                hexList.Add(_hexNeighbors[j]);
                            }
                        }
                    }
                }
            }
            return hexList.Count > 0;
        }

        public void GetNeighbors(Intersection inter, ref Hexagon[] hexNeighbors)
        {
            for (int k = 0; k < inter.myNeighbors.Length; k++)
                hexNeighbors[k] = _gridHex.Get(inter.myNeighbors[k].x, inter.myNeighbors[k].y);
        }

        // Check if the neighbor hexagons of the intersection point have the same color.
        public bool CheckTriple(Intersection inter)
        {
            GetNeighbors(inter, ref _hexNeighbors);
            if (_hexNeighbors[0].ColorID == _hexNeighbors[1].ColorID && _hexNeighbors[1].ColorID == _hexNeighbors[2].ColorID)
                return true;
            return false;
        }

        // Looking for matching hexagons (same color) in all area, checking all through the intersection points.
        public bool CheckExplosionAll(ref List<Hexagon> hexList)
        {
            hexList.Clear();

            for (int x = 0; x < _gridIntersect.gridSize.x; x++)
            {
                for (int y = 0; y < _gridIntersect.gridSize.y; y++)
                {
                    if (CheckTriple(_gridIntersect.Get(x, y)))
                    {
                        for (int j = 0; j < _hexNeighbors.Length; j++)
                        {
                            if (!hexList.Contains(_hexNeighbors[j]))
                            {
                                hexList.Add(_hexNeighbors[j]);
                            }
                        }
                    }
                }
            }
            return hexList.Count > 0;
        }
    }
}