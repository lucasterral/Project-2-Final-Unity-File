using System.Collections.Generic;
using UnityEngine;

public class ClothingManager : MonoBehaviour
{
    public Transform upperBodyAnchor;
    public Transform lowerBodyAnchor;
    public List<ClothingEntry> clothingPrefabs;

    private GameObject currentUpperBodyClothing;
    private GameObject currentLowerBodyClothing;

    private Stack<string> upperClothingStack = new Stack<string>();
    private Stack<string> lowerClothingStack = new Stack<string>();

    public void ChangeClothing(string itemId)
    {
        var match = clothingPrefabs.Find(entry => entry.itemId == itemId);
        if (match == null)
        {
            Debug.LogWarning("No matching clothing prefab found for ID: " + itemId);
            return;
        }

        if (match.category == ClothingCategory.Upper)
        {
            upperClothingStack.Push(itemId);
            UpdateClothingDisplay(ClothingCategory.Upper);
        }
        else if (match.category == ClothingCategory.Lower)
        {
            lowerClothingStack.Push(itemId);
            UpdateClothingDisplay(ClothingCategory.Lower);
        }
    }

    public void RemoveClothing(string itemId)
    {
        var match = clothingPrefabs.Find(entry => entry.itemId == itemId);
        if (match == null) return;

        var stack = match.category == ClothingCategory.Upper ? upperClothingStack : lowerClothingStack;

        // Create a new stack with itemId removed
        var tempList = new List<string>(stack);
        tempList.Remove(itemId);
        tempList.Reverse(); // restore original order after popping

        stack.Clear();
        foreach (var id in tempList)
        {
            stack.Push(id);
        }

        UpdateClothingDisplay(match.category);
    }

    private void UpdateClothingDisplay(ClothingCategory category)
    {
        if (category == ClothingCategory.Upper)
        {
            if (currentUpperBodyClothing != null)
                Destroy(currentUpperBodyClothing);

            if (upperClothingStack.Count > 0)
            {
                string topId = upperClothingStack.Peek();
                var prefab = clothingPrefabs.Find(entry => entry.itemId == topId)?.prefab;
                if (prefab != null)
                    currentUpperBodyClothing = Instantiate(prefab, upperBodyAnchor);
            }
        }
        else if (category == ClothingCategory.Lower)
        {
            if (currentLowerBodyClothing != null)
                Destroy(currentLowerBodyClothing);

            if (lowerClothingStack.Count > 0)
            {
                string topId = lowerClothingStack.Peek();
                var prefab = clothingPrefabs.Find(entry => entry.itemId == topId)?.prefab;
                if (prefab != null)
                    currentLowerBodyClothing = Instantiate(prefab, lowerBodyAnchor);
            }
        }
    }

    [System.Serializable]
    public class ClothingEntry
    {
        public string itemId;
        public GameObject prefab;
        public ClothingCategory category;
    }

    public enum ClothingCategory
    {
        Upper,
        Lower
    }
}
