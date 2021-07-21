using UnityEngine;
using Hexagon2D.IntersectionSystem;

namespace Hexagon2D.GridSystem
{
    public class GridIntersect : GridGeneric<Intersection>
    {


        public GridIntersect(Vector2Int gridSize) : base(gridSize)
        {

        }

    }
}