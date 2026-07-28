using UnityEngine;

// Replaces the FirstPersonController with freeform swimming when the player enters water.
// Controls movement (WASD + Space/Shift for up/down), camera look, and underwater fog.
public class UnderwaterController : MonoBehaviour
{
    public StarterAssets.FirstPersonController fpsController;

    [Header("Swimming")]
    public float swimSpeed = 7f;          // horizontal swim speed (WASD)
    public float swimSprintSpeed = 10f;   // swim speed when holding Shift (sprint)
    public float swimVerticalSpeed = 7f;  // speed for Space (up) and Shift (down) while swimming

    [Header("Camera")]
    public float swimRotationSpeed = 2f;  // mouse look sensitivity while underwater

    [Header("Underwater Fog")]
    public Color fogColor = new Color(0.2f, 0.4f, 0.55f); // bluish underwater fog color
    public float fogDensity = 0.002f;                       // thickness of the fog (lower = clearer)

    private CharacterController controller;
    private StarterAssets.StarterAssetsInputs input;
    private Transform cameraTarget;       // Cinemachine camera target (the object we pitch on)
    private float cameraPitch;            // current up/down look angle
    private bool isUnderwater;
    private float lastWaterTouch;         // Time.time when we last touched water (for exit debounce)

    // Store original FPC settings so we can restore them when leaving the water
    private float originalMoveSpeed;
    private float originalSprintSpeed;
    private float originalGravity;
    private float originalJumpHeight;

    void Start()
    {
        // Grab references from this GameObject
        if (fpsController == null) fpsController = GetComponent<StarterAssets.FirstPersonController>();
        controller = GetComponent<CharacterController>();
        input = GetComponent<StarterAssets.StarterAssetsInputs>();

        // Auto-fix CinemachineCameraTarget if it was lost during prefab unpacking
        if (fpsController.CinemachineCameraTarget == null)
        {
            // Search children for a GameObject tagged "CinemachineTarget" or named "PlayerCameraRoot"
            Transform root = transform.Find("PlayerCameraRoot");
            if (root == null)
            {
                // Fallback: search all children recursively
                foreach (Transform child in GetComponentsInChildren<Transform>())
                {
                    if (child.CompareTag("CinemachineTarget") || child.name == "PlayerCameraRoot")
                    {
                        root = child;
                        break;
                    }
                }
            }
            if (root != null)
                fpsController.CinemachineCameraTarget = root.gameObject;
        }

        cameraTarget = fpsController.CinemachineCameraTarget.transform;

        // Save original FPC values so exiting water feels the same as before entering
        originalMoveSpeed = fpsController.MoveSpeed;
        originalSprintSpeed = fpsController.SprintSpeed;
        originalGravity = fpsController.Gravity;
        originalJumpHeight = fpsController.JumpHeight;
    }

    void Update()
    {
        if (isUnderwater)
        {
            // Debounce: stay underwater a short while after leaving the trigger,
            // to avoid flickering at the water surface
            if (Time.time - lastWaterTouch > 0.3f)
                ExitWater();
            else
                HandleSwimming();
        }
    }

    void LateUpdate()
    {
        if (isUnderwater)
            HandleCamera();
    }

    // Move the player using WASD (horizontal) and Space/Shift (vertical)
    void HandleSwimming()
    {
        float targetSpeed = input.sprint ? swimSprintSpeed : swimSpeed;

        Vector3 move = Vector3.zero;

        // Horizontal movement based on camera-relative forward/right
        if (input.move != Vector2.zero)
            move = (transform.forward * input.move.y + transform.right * input.move.x).normalized * targetSpeed;

        // Vertical movement: Space = rise, Shift (while swimming) = sink
        if (Input.GetKey(KeyCode.Space))
            move += Vector3.up * swimVerticalSpeed;
        else if (Input.GetKey(KeyCode.LeftShift))
            move += Vector3.down * swimVerticalSpeed;

        controller.Move(move * Time.deltaTime);
    }

    // Look around: pitch on the camera target, yaw on the whole player object
    void HandleCamera()
    {
        if (input.look.sqrMagnitude >= 0.01f) // ignore tiny mouse movements
        {
            cameraPitch += input.look.y * swimRotationSpeed * Time.deltaTime;
            cameraPitch = Mathf.Clamp(cameraPitch, fpsController.BottomClamp, fpsController.TopClamp);

            cameraTarget.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
            transform.Rotate(Vector3.up * input.look.x * swimRotationSpeed * Time.deltaTime);
        }
    }

    // While touching the water trigger, keep updating the timer
    void OnTriggerStay(Collider other)
    {
        if (other.name == "WaterBlock_50m")
        {
            lastWaterTouch = Time.time;
            if (!isUnderwater)
                EnterWater();
        }
    }

    // When leaving the trigger, note the time (so debounce can count from here)
    void OnTriggerExit(Collider other)
    {
        if (other.name == "WaterBlock_50m")
            lastWaterTouch = Time.time;
    }

    // Switch from FPC to swimming mode, enable fog
    void EnterWater()
    {
        isUnderwater = true;
        fpsController.enabled = false; // we take over all movement and camera

        // Turn on exponential fog for an underwater feel
        RenderSettings.fog = true;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = fogDensity;
    }

    // Switch back to FPC, restore original settings, disable fog
    void ExitWater()
    {
        isUnderwater = false;
        fpsController.enabled = true;

        // Restore original values (FPC's own settings may have been changed)
        fpsController.MoveSpeed = originalMoveSpeed;
        fpsController.SprintSpeed = originalSprintSpeed;
        fpsController.Gravity = originalGravity;
        fpsController.JumpHeight = originalJumpHeight;

        RenderSettings.fog = false;
    }
}
