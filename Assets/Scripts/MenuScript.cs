using System.Collections;
using TMPro;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public bool startGame = false;
    public bool started = false;
    [SerializeField] PeaSpawner peas;
    [SerializeField] SpoonMovement spoon;
    [SerializeField] MicInput mic;
    [SerializeField] Timer timer;
    [SerializeField] Animation menu;
    [SerializeField] Animation space;
    [SerializeField] TextMeshProUGUI number;
    [SerializeField] Animation tint;
    [SerializeField] Animation[] timerScore;

    private void Update()
    {
        spoon.enabled = started;
        timer.enabled = started;
        mic.enabled = started;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            startGame = true;
            menu.Play();
            space.Play();
            StartCoroutine(countdown());
        }
    }

    IEnumerator countdown()
    {
        Animation anim = number.gameObject.GetComponent<Animation>();
        yield return new WaitForSeconds(1f);
        peas.enabled = true;
        PlayFromStart(anim);
        yield return new WaitForSeconds(2f);
        number.text = "2";
        PlayFromStart(anim);
        yield return new WaitForSeconds(2f);
        number.text = "1";
        PlayFromStart(anim);
        yield return new WaitForSeconds(2f);
        tint.Play();
        started = true;
        foreach(var t in timerScore) t.Play();
    }

    void PlayFromStart(Animation anim)
    {
        anim.Stop();
        anim.Rewind();
        anim.Play();
    }

}
