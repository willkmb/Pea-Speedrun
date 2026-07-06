using System.Collections.Generic;
using UnityEngine;

public class PeaTrigger : MonoBehaviour
{
    [SerializeField] string peaTag = "Pea";
    [SerializeField] string childName = "Highlight";

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(peaTag)) return;
        SetHighlightChild(other, true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(peaTag)) return;
        SetHighlightChild(other, false);
    }

    void SetHighlightChild(Collider col, bool state)
    {
        Transform child = col.transform.Find(childName);
        if (child != null) child.gameObject.SetActive(state);
    }
}
