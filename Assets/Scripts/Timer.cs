using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text curScore;
    [SerializeField] TMP_Text hsText;
    [SerializeField] MicInput mic;
    [SerializeField] SpoonMovement spoon;
    [SerializeField] MenuScript menu;
    private float timeLeft = 60f;
    private bool done;

    void Update()
    {
        if (done) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            menu.enabled = false;
            timeLeft = 0f;
            done = true;
            EndGame();
            mic.enabled = false;
            spoon.enabled = false;
        }

        int seconds = Mathf.CeilToInt(timeLeft);
        text.text = seconds.ToString();
        curScore.text = "Score: " + mic.totalEaten.ToString("D3");
    }

    void EndGame()
    {
        int score = mic.totalEaten;
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
        hsText.text = "High Score: " + highScore.ToString("D3");
    }
}
