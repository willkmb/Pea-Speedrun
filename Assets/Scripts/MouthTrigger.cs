using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MouthTrigger : MonoBehaviour
{
    [SerializeField] string spoonTag = "Spoon";
    [SerializeField] string inSpoonTag = "PeaSpoon";
    [SerializeField] Camera cam;
    [SerializeField] float suckSpeed = 5f;
    [SerializeField] float holdTime = 2f;
    [SerializeField] float dist = 2f;
    [SerializeField] float scale = 1.2f;
    [SerializeField] Color newCol = Color.white;
    [SerializeField] Animation chew;
    public List<GameObject> peasAtCamera = new List<GameObject>();
    public int totalInMouth = 0;
    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(spoonTag)) return;
        foreach (var pea in GameObject.FindGameObjectsWithTag(inSpoonTag))
        {
            StartCoroutine(SuckAndHold(pea));
        }
        chew.Play("ChewIn");
        Invoke("playLoop", chew["ChewIn"].clip.length);
    }

    void playLoop()
    {
        chew.Play("ChewLoopAnim");
    }
    IEnumerator SuckAndHold(GameObject pea)
    {
        Rigidbody rb = pea.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        Renderer rend = pea.GetComponentInChildren<Renderer>();
        if (rend != null) rend.material.SetColor("_BaseCol", newCol);
        Vector3 target = cam.ViewportToWorldPoint(new Vector3(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), dist));
        Vector3 startPos = pea.transform.position;
        Vector3 targetScale = pea.transform.localScale * scale;
        Vector3 startScale = pea.transform.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * suckSpeed;
            pea.transform.position = Vector3.Lerp(startPos, target, t);
            pea.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        peasAtCamera.Add(pea);
        totalInMouth++;
    }
}
