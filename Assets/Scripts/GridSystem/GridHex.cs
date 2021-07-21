using Hexagon2D.HexagonSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon2D.GridSystem
{
    public class GridHex : GridGeneric<Hexagon>
    {

        public GridHex(Vector2Int gridSize) : base(gridSize)
        {

        }

        public override void Set(int x, int y, Hexagon gridElement)
        {
            base.Set(x, y, gridElement);
            gridElement?.ChangeIndex(x, y);
        }

        // When rotating the hexagons, we change grid index of these hexagons.
        public void SwapRotate(in Hexagon[] neighbors, bool clockwise)
        {
            Hexagon neighbor0 = neighbors[0];
            Vector2Int index0 = neighbors[0].Index;
            Hexagon neighbor1 = neighbors[1];
            Vector2Int index1 = neighbors[1].Index;
            Hexagon neighbor2 = neighbors[2];
            Vector2Int index2 = neighbors[2].Index;

            if (clockwise)
            {
                Set(index1.x, index1.y, neighbor0);
                Set(index2.x, index2.y, neighbor1);
                Set(index0.x, index0.y, neighbor2);
            }
            else
            {
                Set(index2.x, index2.y, neighbor0);
                Set(index1.x, index1.y, neighbor2);
                Set(index0.x, index0.y, neighbor1);
            }
        }

    }
}