using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon2D.HexagonSystem
{
    public class Hexagon : MonoBehaviour
    {
        private int _colorID;
        public int ColorID => _colorID;

        private Vector2Int _index;
        public Vector2Int Index => _index;

        private bool _active;
        public bool IsActive => _active;

        private SpriteRenderer _spriteRenderer;

        public virtual void Initialize(int colorID, Vector2Int index)
        {
            _colorID = colorID;
            _index = index;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Activate()
        {
            _active = true;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            _active = false;
            gameObject.SetActive(false);
        }

        public void ChangeIndex(int x, int y)
        {
            _index = new Vector2Int(x, y);
        }

        public void ChangeColor(Color color, int colorID)
        {
            _spriteRenderer.color = color;
            _colorID = colorID;
        }
    }
}