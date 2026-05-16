using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlaneTrackingMode : MonoBehaviour, IARTrackingMode
{
    [SerializeField]
    private ARPlaneManager _planeManager;

    [SerializeField]
    private GameObject _prefabToSpawnFromPlane;

    [SerializeField]
    private ARRaycastManager _raycastManager;

    // This stores the data of what string-prefab pair to instantiate

    private readonly List<ARRaycastHit> _raycastHits = new();

    public ARTrackingMode Mode => ARTrackingMode.PlaneTracking;

    public void Initialize()
    {
        throw new System.NotImplementedException();
    }

    public void EnableMode()
    {
        EnhancedTouchSupport.Enable();
    }

    public void DisableMode()
    {
        EnhancedTouchSupport.Disable();
    }

    public void UpdateMode()
    {
        // Check for any active touches in the screen
        if (Touch.activeTouches.Count == 0)
        {
            // Do an early exit this frame because there's no interaction happening requiring touches
            return;
        }

        // Store the information of the first active touch in the screen
        var touch = Touch.activeTouches[0];

        if (touch.phase != TouchPhase.Began)
        {
            // We only need to detect the first point of contact
            return;
        }
        Debug.Log("[AR] Touch detected");

        // We will check if the point in the screen that we touched actually has an ARPlane
        if (_raycastManager.Raycast(
            touch.screenPosition, // cast a ray from the position of the touch
            _raycastHits, // store the data of whatever ARRaycastHit information we got
            TrackableType.PlaneWithinPolygon)) // filter whatever trackable type we want
        {
            Debug.Log("[AR] Trying to spawn object");
            // If we hit a plane, spawn it where we clicked
            var spawnedObject = Instantiate(
                _prefabToSpawnFromPlane,
                _raycastHits[0].pose.position,
                _raycastHits[0].pose.rotation);
        }
    }
}
