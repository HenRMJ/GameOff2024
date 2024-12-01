using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualPlacementIndicator : MonoBehaviour
{
    [SerializeField] private Material[] _positiveMaterials;
    [SerializeField] private Material[] _negativeMaterials;
    [SerializeField] private MeshRenderer[] _renderers;

    private int collidingWithCounter = 0;
    
    private void OnTriggerEnter(Collider other)
    {
        collidingWithCounter++;
        Debug.Log("Test");

        foreach (MeshRenderer render in _renderers)
        {
            render.materials = _negativeMaterials;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        collidingWithCounter--;

        Debug.Log("Test2");

        if (collidingWithCounter > 0) return;
        
        foreach (MeshRenderer renderer in _renderers)
        {
            renderer.materials = _positiveMaterials;
        }
    }
}
