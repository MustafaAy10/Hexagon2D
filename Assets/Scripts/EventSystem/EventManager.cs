using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Hexagon2D.EventSystem
{
    public class EventManager
    {
        public event Action<int> OnScore;
        public event Action OnCountDown;
        public event Action OnGameOver;
        public event Action OnPlay;
        public event Action OnPause;
        public event Action OnExplosionStart;
        public event Action OnExplosionEnd;

        private static readonly EventManager instance = new EventManager();

        public static EventManager Instance
        {
            get
            {
                return instance;
            }
        }

        public void InvokeScore(int score)
        {
            if (OnScore != null)
                OnScore(score);
            //  Debug.Log("[EventManager] EventManager.Instance.OnScore is null? " + (OnScore == null));

        }

        public void InvokeGameOver()
        {
            if (OnGameOver != null)
                OnGameOver();
            if (OnPause != null)
                OnPause();
        }

        public void InvokePlay()
        {
            if (OnPlay != null)
                OnPlay();
            //Debug.Log("[EventManager] EventManager.Instance.OnPlay is null? " + (OnPlay == null));
        }

        public void InvokePause()
        {
            if (OnPause != null)
                OnPause();
            // Debug.Log("[EventManager] EventManager.Instance.OnPause is null? " + (OnPause == null));

        }

        public void InvokeExplosionStart()
        {
            if (OnExplosionStart != null)
                OnExplosionStart();
            if (OnPause != null)
                OnPause();
        }

        public void InvokeExplosionEnd()
        {
            if (OnExplosionEnd != null)
                OnExplosionEnd();
            if (OnPlay != null)
                OnPlay();
        }

        public void InvokeCountDown()
        {
            if (OnCountDown != null)
                OnCountDown();
        }

        public void ResetAllEvents()
        {
            OnScore = null;
            OnCountDown = null;
            OnGameOver = null;
            OnPlay = null;
            OnPause = null;
            OnExplosionStart = null;
            OnExplosionEnd = null;
        }

    }
}