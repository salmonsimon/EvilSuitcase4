using UnityEngine;

[System.Serializable]
public struct RewardItem
{
    [Header("Main Configuration")]
    public Item Item;
    public ItemType ItemType;
    public float Probability;
    public int Amount;

    [Header("Specific Configuration")]
    [DrawIf("ItemType", ItemType.Ammo)]
    public Vector2Int AmmoMinMax;

    [DrawIf("ItemType", ItemType.Gun)]
    public Vector2Int LoadedAmmoMinMax;

    [DrawIf("ItemType", ItemType.Melee)]
    public Vector2 DurabilityMinMax;
}