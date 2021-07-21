using UnityEngine;

namespace Hexagon2D.Settings
{
    [CreateAssetMenu(fileName = "CreateSettings", menuName = "Asset/CreateSettings", order = 1)]
    public class CreateSettings : ScriptableObject
    {
        public Vector2Int _gridSize;
        public Color[] _colors;
        public float _moveSpeed = 0.1f;
    }
}