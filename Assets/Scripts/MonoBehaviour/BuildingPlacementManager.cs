using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementManager : MonoBehaviour
{
    [SerializeField] private GameObject brothelGhost;

    [SerializeField] private bool _readyPlacement;

    private void Update()
    {
        if (!_readyPlacement)
        {
            if (brothelGhost.activeInHierarchy)
            {
                brothelGhost.SetActive(false);
            }
            return;
        }

        if (!brothelGhost.activeInHierarchy)
        {
            brothelGhost.SetActive(true);
        }
        
        Vector3 position = MouseWorldPosition.Instance.GetPosition();
        brothelGhost.transform.position = position;
    }
}
