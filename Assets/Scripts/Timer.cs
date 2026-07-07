using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text curScore;
    [SerializeField] TMP_Text hsText;
    [SerializeField] MicInput mic;
    [SerializeField] SpoonMovement spoon;
    [SerializeField] MenuScript menu;
    [SerializeField] MouthTrigger mouth;
    [SerializeField] Animation tint;
    [SerializeField] Animation logo;
    [SerializeField] Animation[] texts;
    private float timeLeft = 60f;
    private bool done;

    void Update()
    {
        if (done)
        {
            if(Input.GetKey(KeyCode.Space)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            menu.enabled = false;
            timeLeft = 0f;
            done = true;
            EndGame();
            mic.enabled = false;
            spoon.enabled = false;
            mouth.enabled = false;
            tint.Play("TintIn");
            logo.Play("LogoIn");
            foreach (var text in texts) text.Play();
        }

        int seconds = Mathf.CeilToInt(timeLeft);
        text.text = seconds.ToString();
        curScore.text = "peas: " + mic.totalEaten.ToString("D3");
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
        hsText.text = "highscore: " + highScore.ToString("D3");
    }
}
