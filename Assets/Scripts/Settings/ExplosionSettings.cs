using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon2D.Settings
{
    [CreateAssetMenu(fileName = "ExplosionSettings", menuName = "Asset/ExplosionSettings", order = 3)]
    public class ExplosionSettings : ScriptableObject
    {
        public float _explosionWaitSeconds;
        public int _pointsPerExplodedHexagon;
        public int _pointsToCreateBomb;
        public float _moveSpeed;
    }
}