using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class ItemRecommender : MonoBehaviour
{
    [SerializeField]
    public string itemId = ""; // Unique ID for this sketch

    [Header("Frame")]
    public GameObject frameObject; // Assign the frame child here

    private static List<ItemRecommender> allSketches = new List<ItemRecommender>();
    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        allSketches.Add(this);
        if (frameObject != null)
            frameObject.SetActive(false); // Hide the frame initially

        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
        }
    }

    void OnDestroy()
    {
        allSketches.Remove(this);

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("Sketch grabbed: " + itemId);
        StartCoroutine(SendRecommendationRequest(itemId));
    }

    IEnumerator SendRecommendationRequest(string itemId)
    {
        string json = JsonUtility.ToJson(new ItemRequest { item_id = itemId });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest("http://localhost:8000/recommend", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);

            var response = JsonUtility.FromJson<ItemResponse>(request.downloadHandler.text);

 

            HighlightRecommendedSketch(response.recommended_id);
        }
        else
        {
            Debug.LogError("Request failed: " + request.error);
        }
    }

    void HighlightRecommendedSketch(string recommendedId)
    {
        Debug.Log("Highlighting sketch with ID: " + recommendedId);

        foreach (var sketch in allSketches)
        {
            if (sketch.frameObject == null)
            {
                Debug.LogWarning($"Sketch {sketch.itemId} has no frameObject assigned.");
                continue;
            }

            bool isSnappedToSocket = sketch.grabInteractable != null &&
                                     sketch.grabInteractable.selectingInteractor is XRSocketInteractor;

            bool shouldHighlight = sketch.itemId == recommendedId && !isSnappedToSocket;

            var glow = sketch.frameObject.GetComponent<GlowAnimator>();

            if (shouldHighlight)
            {
                if (glow != null)
                    glow.StartGlow();
                else
                    sketch.frameObject.SetActive(true); // fallback
            }
            else
            {
                if (glow != null)
                {
                    glow.StopGlow();
                    sketch.frameObject.SetActive(false); // ensure frame is hidden if not glowing
                }
                else
                {
                    sketch.frameObject.SetActive(false);
                }
            }

          //  Debug.Log($"Sketch {sketch.itemId}: Snapped = {isSnappedToSocket}, Highlight = {shouldHighlight}");
        }
    }

    [System.Serializable]
    public class ItemRequest
    {
        public string item_id;
    }

    [System.Serializable]
    public class ItemResponse
    {
        public string recommended_id;
    }
}
