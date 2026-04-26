using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Vehicle : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private Seat driverSeat;

    [field: Header("Visual")]
    [field: SerializeField] public Sprite InteractOverlay { get; private set; }
    [field: SerializeField] public string InteractTooltip { get; private set; }

    public GameObject Driver => driverSeat.entity;

    private readonly HashSet<GameObject> users = new HashSet<GameObject>();

    public bool HasUser(GameObject user)
    {
        return users.Contains(user);
    }

    public bool HasSpace()
    {
        return driverSeat.entity == null;
    }

    public bool CanSelect(GameObject source)
    {
        return !HasUser(source);
    }

    public bool CanInteract(GameObject source)
    {
        if (HasUser(source) || !HasSpace())
        {
            return false;
        }

        if (source.TryGetComponent(out VehicleHandler vehicleHandler))
        {
            return !vehicleHandler.HasVehicle();
        }

        return false;
    }

    public bool Interact(GameObject source)
    {
        if (CanInteract(source))
        {
            EnterVehicle(source);

            return true;
        }

        return false;
    }

    private void EnterVehicle(GameObject source)
    {
        if (TryGetEmptySeat(out Seat seat) && source.TryGetComponent(out VehicleHandler vehicleHandler))
        {
            users.Add(source);
            seat.entity = source;

            vehicleHandler.AssignVehicle(this);
            OnEnterVehicle(source);
        }
    }

    public void ExitVehicle(GameObject source)
    {
        if (HasUser(source) && source.TryGetComponent(out VehicleHandler vehicleHandler))
        {
            users.Remove(source);
            FreeSeat(source);

            vehicleHandler.UnassignVehicle(this);
            OnExitVehicle(source);
        }
    }

    protected virtual void OnEnterVehicle(GameObject source) { }

    protected virtual void OnExitVehicle(GameObject source) { }

    private bool TryGetEmptySeat(out Seat seat)
    {
        seat = null;

        if (!HasSpace()) return false;

        if (driverSeat.entity == null)
        {
            seat = driverSeat;
            return true;
        }

        return false;
    }

    private void FreeSeat(GameObject user)
    {
        if (driverSeat.entity == user)
        {
            driverSeat.entity = null;
            return;
        }
    }

    private void LateUpdate()
    {
        if (Driver == null) return;
        transform.SetPositionAndRotation(Driver.transform.position, Driver.transform.rotation);
    }

    private void OnDestroy()
    {
        if (driverSeat.entity != null)
        {
            ExitVehicle(Driver);
        }
    }

    [Serializable]
    protected class Seat
    {
        public Transform transform;
        [HideInInspector] public GameObject entity;
    }
}