using UnityEngine;

public class VisualPlacementIndicator : MonoBehaviour
{
    [SerializeField] private Material[] _positiveMaterials;
    [SerializeField] private Material[] _negativeMaterials;
    [SerializeField] private MeshRenderer[] _renderers;
    [SerializeField] private LayerMask _buildingLayerMask;
    [SerializeField] private float _clearanceRange;
    
    private bool _previousCollided;
    private bool _collided;

    private void Start()
    {
        Debug.Log("Something");
    }

    private void FixedUpdate()
    {
        int size = Physics.OverlapSphereNonAlloc(transform.position, _clearanceRange, new Collider[5], _buildingLayerMask);

        if (size > 0)
        {
            _collided = true;

            if (_previousCollided != _collided)
            {
                _previousCollided = _collided;
                
                foreach (MeshRenderer renderer in _renderers)
                {
                    renderer.materials = _negativeMaterials;
                }
            }
        }
        else
        {
            _collided = false;
            
            if (_previousCollided != _collided)
            {
                _previousCollided = _collided;
                
                foreach (MeshRenderer renderer in _renderers)
                {
                    renderer.materials = _positiveMaterials;
                }
            }
        }
    }

    public bool CanPlace() => !_collided;
}
