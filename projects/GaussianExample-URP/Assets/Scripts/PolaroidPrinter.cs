using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
using System.Diagnostics;


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

		// Create new material and assign the snapshot texture
		Material newMat = new Material(photoMaterialTemplate);
		newMat.mainTexture = photoTexture;

		// Apply new material to the spawned photo
		MeshRenderer renderer = photo.GetComponent<MeshRenderer>();
		if (renderer != null)
		{
			renderer.material = newMat;
		}

		// Dynamically find the target object in any loaded scene (by name or tag)
		string targetObjectName = "InteractablePicture"; // Update this to the name of your target object
		GameObject targetObject = GameObject.Find(targetObjectName);

		if (targetObject != null)
		{
			MeshRenderer otherRenderer = targetObject.GetComponent<MeshRenderer>();
			if (otherRenderer != null)
			{
				otherRenderer.material = newMat; 
			}
			else
			{
				//Debug.LogWarning($"Found {targetObject.name}, but no MeshRenderer attached!");
			}
		}
		else
		{
			//Debug.LogWarning($"No object named '{targetObjectName}' found in loaded scenes.");
		}

		// Play shutter sound
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
