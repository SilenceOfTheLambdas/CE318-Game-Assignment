using UnityEngine;

/// <summary>
/// Original script based on the work by "GameDevChef" https://www.youtube.com/watch?v=aS7OqRuwzlk
/// Adjusted by 1806094
/// </summary>
[System.Serializable]
public class InventoryItemWrapper
{
    [SerializeField] private InventoryItem item;
    [SerializeField] private int count;

    public InventoryItem GetItem() => item;
    public int GetItemCount() => count;
}
