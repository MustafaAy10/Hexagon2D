using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hexagon2D.EventSystem;

namespace Hexagon2D.UI.PanelSystem
{
    public class ScorePanel : PanelBase
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _highestScoreText;

        private int _highestScore;
        private static int _score;
        public static int score => _score;

        public override void Initialize()
        {
            EventManager.Instance.OnScore += Score;
            _score = 0;
            UpdateScore();
            _highestScore = PlayerPrefs.GetInt("HighestScore");
            _highestScoreText.text = "Highest Score: " + _highestScore.ToString();
        }

        public void Score(int score)
        {
            _score += score;
            if (_score > _highestScore)
                PlayerPrefs.SetInt("HighestScore", _score);
            UpdateScore();
        }

        public void RestartClick()
        {

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void UpdateScore()
        {
            _scoreText.text = _score.ToString();
        }

        private void OnDestroy()
        {
            EventManager.Instance.OnScore -= Score;
        }

    }
}