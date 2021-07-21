using Hexagon2D.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hexagon2D.HexagonSystem
{
    public class Bomb : Hexagon
    {
        private int _countDown;
        [SerializeField] private Text _countDownText;

        public override void Initialize(int colorID, Vector2Int index)
        {
            base.Initialize(colorID, index);
            _countDown = Random.Range(4, 6);
            _countDownText.text = _countDown.ToString();
            EventManager.Instance.OnCountDown += CountDown;
            transform.GetChild(0).GetComponent<Canvas>().worldCamera = Camera.main;
        }

        public void CountDown()
        {
            _countDown--;
            _countDownText.text = _countDown.ToString();
            if (_countDown == 0)
                EventManager.Instance.InvokeGameOver();
        }

        private void OnDestroy()
        {
            EventManager.Instance.OnCountDown -= CountDown;
        }

    }
}