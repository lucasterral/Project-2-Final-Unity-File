// Scripts/PhotoApplier.cs
using UnityEngine;

public class PhotoApplier : MonoBehaviour
{
    public MeshRenderer targetRenderer;

    void Start()
    {
        if (GlobalPhotoData.CapturedPhotoTexture != null)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.mainTexture = GlobalPhotoData.CapturedPhotoTexture;
            targetRenderer.material = mat;
        }
        else
        {
            Debug.LogWarning("No captured photo texture found.");
        }
    }
}
