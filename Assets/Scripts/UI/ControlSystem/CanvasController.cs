using Hexagon2D.UI.PanelSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon2D.UI.ControlSystem
{
    public class CanvasController : MonoBehaviour
    {
        private PanelBase[] _panels;

        public void Initialize()
        {
            _panels = GetComponentsInChildren<PanelBase>(true);   // Getting PanelBase from children, including inactive PanelBase 
            if (_panels != null)
                for (int i = 0; i < _panels.Length; i++)
                {
                    _panels[i].Initialize();
                    Debug.Log("[CanvasController] initialized _panel type: " + _panels[i].GetType());
                }
        }
    }
}