using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PegSocketHandler : MonoBehaviour
{
    public ClothingManager clothingManager;

    private XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnSketchSnapped);
        socket.selectExited.AddListener(OnSketchRemoved);
    }

    private void OnSketchSnapped(SelectEnterEventArgs args)
    {
        var sketch = args.interactableObject.transform.GetComponent<ItemRecommender>();
        if (sketch != null && clothingManager != null)
        {
            clothingManager.ChangeClothing(sketch.itemId);
            Debug.Log("Clothing updated for: " + sketch.itemId);
        }
    }

    private void OnSketchRemoved(SelectExitEventArgs args)
    {
        var sketch = args.interactableObject.transform.GetComponent<ItemRecommender>();
        if (sketch != null && clothingManager != null)
        {
            clothingManager.RemoveClothing(sketch.itemId);
            Debug.Log("Clothing removed for: " + sketch.itemId);
        }
    }

    void OnDestroy()
    {
        if (socket != null)
        {
            socket.selectEntered.RemoveListener(OnSketchSnapped);
            socket.selectExited.RemoveListener(OnSketchRemoved);
        }
    }
}
