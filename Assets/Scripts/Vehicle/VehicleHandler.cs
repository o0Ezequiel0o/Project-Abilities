using UnityEngine;

public class VehicleHandler : MonoBehaviour
{
    public Vehicle Vehicle { get; private set; }

    public bool HasVehicle()
    {
        return Vehicle != null;
    }

    public void AssignVehicle(Vehicle vehicle)
    {
        Vehicle = vehicle;
    }

    public void UnassignVehicle(Vehicle vehicle)
    {
        if (Vehicle != vehicle) return;
        Vehicle = null;
    }

    public void ExitVehicle()
    {
        if (HasVehicle())
        {
            Vehicle.ExitVehicle(gameObject);
        }
    }

    private void OnDestroy()
    {
        ExitVehicle();
    }
}