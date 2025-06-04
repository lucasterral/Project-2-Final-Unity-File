using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Scene1Setup : MonoBehaviour
{
	public Transform returnAnchor;

	void Start()
	{
		// Find the persistent object
		PersistentOnGrab[] persistentObjects = FindObjectsOfType<PersistentOnGrab>();

		foreach (var persistentObject in persistentObjects)
		{
			// Parent it to the anchor
			persistentObject.transform.SetParent(returnAnchor);
			persistentObject.transform.localPosition = Vector3.zero;
			persistentObject.transform.localRotation = Quaternion.identity;

			// Remove XRGrabInteractable first
			XRGrabInteractable grabInteractable = persistentObject.GetComponent<XRGrabInteractable>();
			if (grabInteractable != null)
			{
				Destroy(grabInteractable);
			}

			// Remove Rigidbody second (after removing XRGrabInteractable dependency)
			Rigidbody rb = persistentObject.GetComponent<Rigidbody>();
			if (rb != null)
			{
				Destroy(rb);
			}
		}
	}
}
