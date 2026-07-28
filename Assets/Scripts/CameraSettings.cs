using UnityEngine;

// CameraSettings - Forces camera far clip plane and FOV on the main camera at startup.
// Uses RuntimeInitializeOnLoadMethod so it runs automatically without needing a GameObject.
public static class CameraSettings
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Apply()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.farClipPlane = 20000f;
            cam.fieldOfView = 80f;
            Debug.Log("[CameraSettings] Applied: farClipPlane=20000, FOV=80");
        }
        else
        {
            // If Camera.main isn't ready yet (e.g., loaded from prefab), subscribe to scene loaded
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, mode) =>
            {
                Camera c = Camera.main;
                if (c != null)
                {
                    c.farClipPlane = 20000f;
                    c.fieldOfView = 80f;
                    Debug.Log("[CameraSettings] Applied on scene load: farClipPlane=20000, FOV=80");
                }
            };
        }
    }
}
