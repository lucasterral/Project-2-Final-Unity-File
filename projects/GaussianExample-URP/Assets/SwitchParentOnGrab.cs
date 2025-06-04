using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabReparentHandler : MonoBehaviour
{
	public Transform newParentWhenGrabbed;
	public Transform originalParent;

	private XRGrabInteractable grabInteractable;

	void Awake()
	{
		grabInteractable = GetComponent<XRGrabInteractable>();
		grabInteractable.selectEntered.AddListener(OnGrab);
		grabInteractable.selectExited.AddListener(OnRelease);
	}

	void OnGrab(SelectEnterEventArgs args)
	{
		if (newParentWhenGrabbed != null)
		{
			transform.SetParent(newParentWhenGrabbed);
		}
	}

	void OnRelease(SelectExitEventArgs args)
	{
		if (originalParent != null)
		{
			transform.SetParent(originalParent);
		}
	}

	void OnDestroy()
	{
		// Clean up listeners to avoid memory leaks
		if (grabInteractable != null)
		{
			grabInteractable.selectEntered.RemoveListener(OnGrab);
			grabInteractable.selectExited.RemoveListener(OnRelease);
		}
	}
}
