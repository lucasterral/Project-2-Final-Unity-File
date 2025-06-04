using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PersistentOnGrab : MonoBehaviour
{
	private XRGrabInteractable grabInteractable;

	private void Awake()
	{
		grabInteractable = GetComponent<XRGrabInteractable>();
		grabInteractable.selectEntered.AddListener(OnGrabbed);
	}

	private void OnGrabbed(SelectEnterEventArgs args)
	{
		// Make this object persist across scenes
		DontDestroyOnLoad(gameObject);
	}

	private void OnDestroy()
	{
		// Prevent memory leaks by unsubscribing
		if (grabInteractable != null)
		{
			grabInteractable.selectEntered.RemoveListener(OnGrabbed);
		}
	}
}
