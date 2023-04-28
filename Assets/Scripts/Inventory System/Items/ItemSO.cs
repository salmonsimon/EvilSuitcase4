using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item Scriptable Object", order = 0)]
public class ItemSO : ScriptableObject
{
    public enum ItemType
    {
        Ammo,
        Gun,
        Melee,
        Consumable
    }

    public string itemName;
    public ItemType itemType;
    public Transform prefab;
    public Transform visual;
    [HideInInspector, SerializeField] public Transform fastSwapVisual;
    public Transform gridVisual;
    public int width;
    public int height;

    #region Custom Editor

#if UNITY_EDITOR
    [CustomEditor(typeof(ItemSO))]
    public class RandomScript_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ItemSO script = (ItemSO)target;

            if (script.itemType == ItemType.Gun || script.itemType == ItemType.Melee)
            {
                var gameObject = EditorGUILayout.ObjectField("Fast Swap Visual", script.fastSwapVisual, typeof(Transform), true) as Transform;
            }
        }
    }
#endif

    #endregion
}
