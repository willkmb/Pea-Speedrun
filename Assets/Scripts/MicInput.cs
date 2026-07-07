using UnityEngine;
using System.Collections;

public class MicInput : MonoBehaviour
{
    [SerializeField] float volThresh = 0.05f;
    [SerializeField] float silenceTimeout = 0.4f;
    [SerializeField] Animation[] teeth;
    [SerializeField] AnimationClip[] startClips;
    [SerializeField] AnimationClip[] midClips;
    [SerializeField] AnimationClip[] endClips;

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
    }

    void Update()
    {
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
        else if (isChewing && Time.time - lastAboveTime > silenceTimeout)
        {
            isChewing = false;
            if (chewRoutine != null) StopCoroutine(chewRoutine);
            chewRoutine = StartCoroutine(playEndAfterMid());
        }
    }

    IEnumerator StartThenMid()
    {
        float longest = 0f;
        for (int i = 0; i < teeth.Length; i++)
        {
            teeth[i].Play(startClips[i].name);
            longest = startClips[i].length;
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
