using UnityEngine;

namespace Hexagon2D.Settings
{
    [CreateAssetMenu(fileName = "RotateSettings", menuName = "Asset/RotateSettings", order = 2)]
    public class RotateSettings : ScriptableObject
    {
        public float _rotateWaitSeconds;
        public float _rotateSpeed;
    }
}