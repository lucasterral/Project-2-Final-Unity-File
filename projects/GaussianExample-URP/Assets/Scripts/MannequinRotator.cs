using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class MannequinRotator : MonoBehaviour
{
    [Header("XR Input")]
    public XRRayInteractor rightRayInteractor; // Assign the right-hand ray interactor
    public InputActionReference rightJoystickInput; // Assign: XRI RightHand Locomotion/Move
    public InputActionReference rightTriggerInput;  // Assign: XRI RightHand Interaction/Activate

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;

    void Update()
    {
        if (rightRayInteractor == null || rightJoystickInput == null || rightTriggerInput == null)
            return;

        // Only allow rotation if controller is pointing at the mannequin
        if (IsPointingAtMannequin())
        {
            // Read the trigger value
            float triggerValue = rightTriggerInput.action.ReadValue<float>();

            // Trigger must be pressed beyond a threshold (0.1)
            if (triggerValue > 0.1f)
            {
                Vector2 input = rightJoystickInput.action.ReadValue<Vector2>();
                float horizontal = input.x;

                if (Mathf.Abs(horizontal) > 0.1f)
                {
                    transform.Rotate(Vector3.up, horizontal * rotationSpeed * Time.deltaTime);
                }
            }
        }
    }

    bool IsPointingAtMannequin()
    {
        return rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit) &&
               hit.collider != null &&
               hit.collider.gameObject == gameObject;
    }
}
