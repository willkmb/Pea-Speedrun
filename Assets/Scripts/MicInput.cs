using UnityEngine;

public class MicInput : MonoBehaviour
{
    [SerializeField] float volThresh = 0.05f;
    [SerializeField] float cooldown = 0.3f;
    [SerializeField] Animation[] teeth;

    private AudioClip micClip;
    private string mic;
    private float lastClip;
    private float aboveThreshTimer;
    private bool wasAbove;

    void Start()
    {
        if (Microphone.devices.Length == 0) return;
        mic = Microphone.devices[0];
        micClip = Microphone.Start(mic, true, 1, 44100);
    }

    void Update()
    {
        if (micClip == null) return;
        float volume = GetMicVolume();

        if (volume > volThresh && Time.time - lastClip > cooldown)
        {
            lastClip = Time.time;
            onClipHeard(volume);
        }
    }

    float GetMicVolume()
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

    void onClipHeard(float volume)
    {
        foreach(var anim in teeth)
        {
            anim.Play();
        }
    }

    void OnDestroy()
    {
        if (mic != null && Microphone.IsRecording(mic))
        {
            Microphone.End(mic);
        }
    }
}
