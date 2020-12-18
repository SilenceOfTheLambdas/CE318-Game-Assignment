using System.Collections.Generic;
using Inventory;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private List<PickableItem> itemsToSpawn;
    [Range(0f, 100f)]
    [SerializeField] private float[]        spawnChance;

    private bool _itemSpawned = false;

    private void Start()
    {
        while (!_itemSpawned)
        {
            for (var i = 0; i < itemsToSpawn.Count; i++)
            {
                if (spawnChance[i] >= Random.value * 100f)
                {
                    Instantiate(itemsToSpawn[i], transform.position, itemsToSpawn[i].transform.rotation);
                    itemsToSpawn.Remove(itemsToSpawn[i]);
                    _itemSpawned = true;
                }
            }
        }
    }
}
