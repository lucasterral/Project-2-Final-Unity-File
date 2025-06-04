using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class VRSceneLoader : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad = "MainVRScene";

    [Header("Ray Interactors")]
    [SerializeField] private XRRayInteractor leftRayInteractor;
    [SerializeField] private XRRayInteractor rightRayInteractor;

    [Header("Teleport / Action Input")]
    [SerializeField] private InputActionReference teleportAction; // The button you want to trigger the scene load

    private void OnEnable()
    {
        if (teleportAction != null)
            teleportAction.action.performed += OnTeleport;
    }

    private void OnDisable()
    {
        if (teleportAction != null)
            teleportAction.action.performed -= OnTeleport;
    }

    private void Update()
    {
        // No scene loading here — just enabling the action
        if (teleportAction != null && !teleportAction.action.enabled)
        {
            teleportAction.action.Enable();
        }
    }

    private void OnTeleport(InputAction.CallbackContext context)
    {
        // Scene load only if the ray hits this object when the button is pressed
        if (IsRayHittingThis(leftRayInteractor) || IsRayHittingThis(rightRayInteractor))
        {
            Debug.Log("Trigger button pressed while pointing at " + gameObject.name);
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private bool IsRayHittingThis(XRRayInteractor rayInteractor)
    {
        if (rayInteractor == null)
            return false;

        return rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit) && hit.collider.gameObject == gameObject;
    }
}
