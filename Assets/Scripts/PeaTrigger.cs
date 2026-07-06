using System.Collections.Generic;
using UnityEngine;

public class PeaTrigger : MonoBehaviour
{
    [SerializeField] string peaTag = "Pea";
    [SerializeField] string inSpoonLayerName = "InSpoon";
    [SerializeField] string defaultLayerName = "Default";

    private int inSpoonLayer;
    private int defaultLayer;

    void Awake()
    {
        inSpoonLayer = LayerMask.NameToLayer(inSpoonLayerName);
        defaultLayer = LayerMask.NameToLayer(defaultLayerName);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(peaTag)) return;
        other.gameObject.layer = inSpoonLayer;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(peaTag)) return;
        other.gameObject.layer = defaultLayer;
    }
}
