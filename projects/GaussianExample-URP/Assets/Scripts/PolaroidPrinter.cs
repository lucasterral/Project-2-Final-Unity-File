using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PolaroidPrinter : MonoBehaviour
{
    [Header("Camera & Rendering")]
    public Camera snapshotCamera;
    public RenderTexture snapshotRenderTexture;

    [Header("Photo Settings")]
    public GameObject photoPrefab;
    public Material photoMaterialTemplate;
    public Transform photoSpawnPoint;

    [Header("XR Interaction")]
    public XRSocketInteractor photoSocket;

    [Header("Audio")]
    public AudioSource shutterSound;

    public AudioSource instructionAudio;

    [Header("Input Actions")]
    public InputActionReference takePhotoAction; // This should point to Camera/TakePhoto in XRI Default Input Actions

    private XRGrabInteractable grabInteractable;
    private bool hasPlayedInstruction = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (takePhotoAction != null)
        {
            takePhotoAction.action.Enable();
            takePhotoAction.action.performed += OnTakePhoto;
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (takePhotoAction != null)
        {
            takePhotoAction.action.performed -= OnTakePhoto;
            takePhotoAction.action.Disable();
        }
    }

    private void OnTakePhoto(InputAction.CallbackContext context)
    {
        TakePhoto();
        Debug.Log("Photo trigger pressed.");

        if (!hasPlayedInstruction)
        {
            hasPlayedInstruction = true;
            StartCoroutine(PlayInstructionAfterDelay(3f)); // 3 seconds
        }
    }

    private IEnumerator PlayInstructionAfterDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        if (instructionAudio != null)
        {
            instructionAudio.Play();
            Debug.Log("Instruction sound played.");
        }
    }

    public void TakePhoto()
    {
        // Render snapshot
        snapshotCamera.Render();

        // Read pixels into Texture2D
        RenderTexture.active = snapshotRenderTexture;
        Texture2D photoTexture = new Texture2D(snapshotRenderTexture.width, snapshotRenderTexture.height, TextureFormat.RGB24, false);
        photoTexture.ReadPixels(new Rect(0, 0, snapshotRenderTexture.width, snapshotRenderTexture.height), 0, 0);
        photoTexture.Apply();
        RenderTexture.active = null;

        // Create photo object
        GameObject photo = Instantiate(photoPrefab, photoSpawnPoint.position, photoSpawnPoint.rotation);

        // Assign snapshot texture to material
        Material newMat = new Material(photoMaterialTemplate);
        newMat.mainTexture = photoTexture;

        MeshRenderer renderer = photo.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = newMat;
        }

        // Play sound
        if (shutterSound != null)
        {
            shutterSound.Play();
        }

        // Place the photo into the socket
        XRGrabInteractable grab = photo.GetComponent<XRGrabInteractable>();
        if (grab != null && photoSocket != null)
        {
            photoSocket.StartManualInteraction(grab);
        }
    }
}
