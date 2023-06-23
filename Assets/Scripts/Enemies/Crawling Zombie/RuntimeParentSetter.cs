using System;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeParentSetter : MonoBehaviour
{
    [Serializable]
    public struct ChildParentCouple
    {
        [SerializeField] private Transform child;
        public Transform Child { get { return child; } }

        [SerializeField] private Transform parent;
        public Transform Parent { get { return parent; } }
    }

    [SerializeField] private List<ChildParentCouple> childParentCoupleList;

    private void Start()
    {
        foreach (ChildParentCouple childParentCouple in childParentCoupleList)
        {
            childParentCouple.Child.parent = childParentCouple.Parent;
        }
    }

    /*
    private void Start()
    {
        StartCoroutine(WaitAndSetChildParentRelationships());
    }

    private IEnumerator WaitAndSetChildParentRelationships()
    {
        yield return new WaitForSeconds(Config.SMALL_DELAY);

        foreach (ChildParentCouple childParentCouple in childParentCoupleList)
        {
            childParentCouple.Child.parent = childParentCouple.Parent;
        }
    }
    */
}
