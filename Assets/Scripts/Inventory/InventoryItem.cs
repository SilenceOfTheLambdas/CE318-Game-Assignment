using UnityEngine;

/// <summary>
/// Original script based on the work by "GameDevChef" https://www.youtube.com/watch?v=aS7OqRuwzlk
/// </summary>
public abstract class InventoryItem : ScriptableObject
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private string itemName;
    [SerializeField] private Vector3 itemLocalPosition;
    [SerializeField] private Vector3 itemLocalRotation;

    public Sprite GetSprite() => itemSprite;
    public string GetName() => itemName;
    public GameObject GetPrefab() => itemPrefab;
    public Vector3 GetLocalPosition() => itemLocalPosition;
    public Quaternion GetLocalRotation() => Quaternion.Euler(itemLocalRotation);

    public abstract void AssignItemToPlayer(PlayerEquipmentController playerEquipment);
}
