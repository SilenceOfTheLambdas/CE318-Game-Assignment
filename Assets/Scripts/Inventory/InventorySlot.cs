using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    /// <summary>
    ///     Original script based on the work by "GameDevChef" https://www.youtube.com/watch?v=aS7OqRuwzlk
    ///     Adjusted by 1806094
    /// </summary>
    public class InventorySlot : MonoBehaviour
    {
        public InventoryItem SlotItem { get; set; }
        [SerializeField] private Image           itemImage;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemCountText;
        [SerializeField] private Button          slotButton;

        public void InitSlotVisualisation(Sprite itemSprite, string itemName, int itemCount)
        {
            itemImage.sprite = itemSprite;
            itemNameText.text = itemName;
            UpdateSlotCount(itemCount);
        }

        public void UpdateSlotCount(int itemCount)
        {
            itemCountText.text = itemCount.ToString();
        }

        public void AssignSlotButtonCallback(Action onClickCallback)
        {
            slotButton.onClick.AddListener(() => onClickCallback());
        }
    }
}