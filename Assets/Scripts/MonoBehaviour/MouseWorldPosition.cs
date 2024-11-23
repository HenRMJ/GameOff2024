using UnityEngine;

public class MouseWorldPosition : MonoBehaviour
{
    public static MouseWorldPosition Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetPosition()
    {
        Vector3 position = Vector3.zero;

        Ray mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new(Vector3.up, Vector3.zero);

        if (plane.Raycast(mouseCameraRay, out float distance))
        {
            position = mouseCameraRay.GetPoint(distance);
        }

        return position;
    }
}
