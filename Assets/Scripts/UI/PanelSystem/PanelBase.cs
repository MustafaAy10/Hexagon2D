using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon2D.UI.PanelSystem
{
    public abstract class PanelBase : MonoBehaviour
    {
        protected bool _active;

        public abstract void Initialize();

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
    }
}