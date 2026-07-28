using UnityEngine;

// CameraSettings - Forces far clip plane and FOV on the main camera at startup.
// Runs after scene load so Camera.main is guaranteed to exist.
public static class CameraSettings
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Apply()
    {
        // Try to apply immediately
        if (!ApplyToCamera(Camera.main))
        {
            // If camera not found yet, retry after a short delay
            GameObject go = new GameObject("CameraSettingsRunner");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<CameraSettingsRunner>();
        }
    }

    static bool ApplyToCamera(Camera cam)
    {
        if (cam != null)
        {
            cam.farClipPlane = 20000f;
            cam.fieldOfView = 80f;
            Debug.Log("[CameraSettings] Applied to " + cam.name + ": farClipPlane=20000, FOV=80");
            return true;
        }
        return false;
    }

    // Small helper MonoBehaviour that applies settings once then destroys itself
    private class CameraSettingsRunner : MonoBehaviour
    {
        void Update()
        {
            if (ApplyToCamera(Camera.main))
            {
                Destroy(gameObject);
            }
        }
    }
}
