using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MicInput : MonoBehaviour
{
    [SerializeField] float volThresh = 0.05f;
    [SerializeField] float SilentFor = 0.4f;
    [SerializeField] Animation[] teeth;
    [SerializeField] AnimationClip[] startClip;
    [SerializeField] AnimationClip[] midClips;
    [SerializeField] AnimationClip[] endClips;
    [SerializeField] MouthTrigger mouth;
    [SerializeField] Image chewImg;
    [SerializeField] float totalTime = 2f;
    [SerializeField] float fillSmooth = 4f;
    [SerializeField] TMP_Text peaText;
    [SerializeField] Animation chew;
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject star;
    [SerializeField] Animation tint;
    [SerializeField] Spinner spinner;
    [SerializeField] GameObject spinHolder;

    public int totalEaten = 0;
    private int maxSeen = 0;
    private float targetFill = 0f;

    private AudioClip micClip;
    private string mic;
    private float lastAboveTime;
    private bool isChewing;
    private Coroutine chewRoutine;

    void Start()
    {
        if (Microphone.devices.Length == 0) return;
        mic = Microphone.devices[0];
        micClip = Microphone.Start(mic, true, 1, 44100);
        StartCoroutine(EatLoop());
    }

    void Update()
    {
        int count = mouth.peasAtCamera.Count;
        if (count == 0) maxSeen = 0;
        else if (count > maxSeen) maxSeen = count;
        targetFill = maxSeen > 0 ? (float)count / maxSeen : 0f;
        if (chewImg != null) chewImg.fillAmount = Mathf.Lerp(chewImg.fillAmount, targetFill, Time.deltaTime * fillSmooth);
        if (micClip == null) return;
        float volume = getVol();
        if (volume > volThresh)
        {
            lastAboveTime = Time.time;
            if (!isChewing)
            {
                isChewing = true;
                if (chewRoutine != null) StopCoroutine(chewRoutine);
                chewRoutine = StartCoroutine(StartThenMid());
            }
        }
        else if (isChewing && Time.time - lastAboveTime > SilentFor)
        {
            isChewing = false;
            if (chewRoutine != null) StopCoroutine(chewRoutine);
            chewRoutine = StartCoroutine(playEndAfterMid());
        }
    }

    IEnumerator EatLoop()
    {
        while (true)
        {
            var peas = mouth.peasAtCamera;
            if (isChewing && peas.Count > 0)
            {
                float wait = totalTime / Mathf.Max(maxSeen, 1);
                GameObject pea = peas[0];
                peas.RemoveAt(0);
                StartCoroutine(EatPea(pea));

                float t = 0f;
                while (t < wait)
                {
                    if (!isChewing) break;
                    t += Time.deltaTime;
                    yield return null;
                }

                if (peas.Count == 0)
                {
                    chew.Play("ChewOut");
                    tint.Play("EatTintOut");
                    mouth.FinishEating();
                }
            }
            else yield return null;
        }
    }

    IEnumerator EatPea(GameObject pea)
    {
        Animation anim = pea.GetComponent<Animation>();
        float wait = 0f;
        if (anim != null)
        {
            anim.Play();
            wait = anim.clip != null ? anim.clip.length : 0f;
        }

        bool isMystery = pea.GetComponent<IsMysteryPea>() != null;

        if (star != null && canvas != null)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(pea.transform.position);
            GameObject uiEffect = Instantiate(star, canvas.transform);
            uiEffect.transform.position = screenPos;

            Animation uiAnim = uiEffect.GetComponent<Animation>();
            float uiWait = 0f;
            if (uiAnim != null)
            {
                uiAnim.Play();
                uiWait = uiAnim.clip != null ? uiAnim.clip.length : 0f;
            }
            Destroy(uiEffect, uiWait);
        }

        yield return new WaitForSeconds(wait);
        Destroy(pea);
        totalEaten++;
        if (peaText != null) peaText.text = "peas: " + totalEaten.ToString("D3");

        if (isMystery)
        {
            StartCoroutine(Spinning());
        }
    }

    IEnumerator Spinning()
    {
        spinHolder.GetComponent<Animation>().Play("SpinnerIn");
        yield return new WaitForSeconds(spinHolder.GetComponent<Animation>()["SpinnerIn"].clip.length);
        spinner.Spin();
    }

    IEnumerator StartThenMid()
    {
        float longest = 0f;
        for (int i = 0; i < teeth.Length; i++)
        {
            teeth[i].Play(startClip[i].name);
            longest = startClip[i].length;
        }
        yield return new WaitForSeconds(longest);
        for (int i = 0; i < teeth.Length; i++)
        {
            teeth[i].Play(midClips[i].name);
        }
    }

    IEnumerator playEndAfterMid()
    {
        for (int i = 0; i < teeth.Length; i++)
        {
            StartCoroutine(playEnd(i));
        }
        yield return null;
    }

    IEnumerator playEnd(int i)
    {
        AnimationState state = teeth[i][midClips[i].name];
        float remaining = 0f;
        if (state != null && midClips[i].length > 0f)
        {
            float clipLength = midClips[i].length;
            remaining = clipLength - (state.time % clipLength);
        }
        yield return new WaitForSeconds(remaining);
        teeth[i].Play(endClips[i].name);
    }

    float getVol()
    {
        int sampleWindow = 128;
        int micPosition = Microphone.GetPosition(mic) - sampleWindow;
        if (micPosition < 0) return 0f;
        float[] samples = new float[sampleWindow];
        micClip.GetData(samples, micPosition);
        float sum = 0f;
        for (int i = 0; i < sampleWindow; i++)
        {
            sum += samples[i] * samples[i];
        }
        return Mathf.Sqrt(sum / sampleWindow);
    }

    void OnDestroy()
    {
        if (mic != null && Microphone.IsRecording(mic))
        {
            Microphone.End(mic);
        }
    }
}