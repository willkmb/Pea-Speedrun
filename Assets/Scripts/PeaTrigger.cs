using System.Collections.Generic;
using UnityEngine;

public class PeaTrigger : MonoBehaviour
{
    [SerializeField] string peaTag = "Pea";
    [SerializeField] string inSpoonTag = "PeaSpoon";
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(peaTag)) return;
        other.tag = inSpoonTag;
    }
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(inSpoonTag)) return;
        other.tag = peaTag;
    }
}
