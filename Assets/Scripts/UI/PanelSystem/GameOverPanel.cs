using Hexagon2D.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hexagon2D.UI.PanelSystem
{
    public class GameOverPanel : PanelBase
    {

        public override void Initialize()
        {
            Deactivate();
            EventManager.Instance.OnGameOver += Activate;
        }

        public void OnDestroy()
        {
            EventManager.Instance.OnGameOver -= Activate;
        }
    }
}