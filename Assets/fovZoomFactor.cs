using UnityEngine;
using UnityEngine.XR; // Include this for XRDevice

public class VRFOVAdjuster : MonoBehaviour
{
    public float fovZoomFactor = 0.5f; // Default zoom factor

    void Start()
    {
        // Set the initial zoom factor
        XRDevice.fovZoomFactor = fovZoomFactor;
    }

    void Update()
    {
        // You can update the zoom factor dynamically if needed
        XRDevice.fovZoomFactor = fovZoomFactor;
    }
}
