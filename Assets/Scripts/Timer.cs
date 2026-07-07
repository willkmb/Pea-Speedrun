using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    private float timeLeft = 60f;
    private bool done;

    void Update()
    {
        if (done) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            done = true;
            Debug.Log("Timer finished!");
        }

        int seconds = Mathf.CeilToInt(timeLeft);
        text.text = seconds.ToString();
    }
}
