using UnityEngine;

public class OverlayManager : MonoBehaviour
{
    private void Start()
    {
        SelectionManager.Instance.OnBuildingSelected += SelectionManager_OnBuildingSelected;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        SelectionManager.Instance.OnBuildingSelected -= SelectionManager_OnBuildingSelected;
    }

    private void SelectionManager_OnBuildingSelected(object sender, BuildingTypes buildingType)
    {
        gameObject.SetActive(true);
        
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        transform.GetChild((int)buildingType).gameObject.SetActive(true);
    }
}
