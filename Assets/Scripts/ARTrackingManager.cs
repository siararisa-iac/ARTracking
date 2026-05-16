using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class ARTrackingManager : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager _trackedImageManager;

    [SerializeField]
    private ARPlaneManager _planeManager;

    [SerializeField]
    private ARRaycastManager _raycastManager;

    // This stores the data of what string-prefab pair to instantiate
    [SerializeField]
    private List<ARTrackedImageData> _trackedImageDatas;

    private List<ARRaycastHit> _raycastHits;

    // Tracks the trackables that were instantiated
    private readonly Dictionary<string, GameObject> _spawnedTrackables = new();

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        _trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        _trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    private void Update()
    {
        // Check for any active touches in the screen
        if (Touch.activeTouches.Count == 0)
        {
            // Do an early exit this frame because there's no interaction happening requiring touches
            return;
        }

        // Store the information of the first active touch in the screen
        var touch = Touch.activeTouches[0];

        if(touch.phase != TouchPhase.Began)
        {
            // We only need to detect the first point of contact
            return;
        }

        // We will check if the point in the screen that we touched actually has an ARPlane
        if(_raycastManager.Raycast(
            touch.screenPosition, // cast a ray from the position of the touch
            _raycastHits, // store the data of whatever ARRaycastHit information we got
            TrackableType.PlaneWithinPolygon)) // filter whatever trackable type we want
        {
            // If we hit a plane
            // Do something
        }
                                             
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            HandleTrackImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            HandleTrackImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            RemoveTrackImage(trackedImage);
        }
    }

    private void RemoveTrackImage(KeyValuePair<TrackableId, ARTrackedImage> trackedImage)
    {
        var trackedImageNameToRemove = trackedImage.Value.referenceImage.name;

        // Make sure to delete the registered object from the Dictionary if it exists
        if (_spawnedTrackables.TryGetValue(trackedImageNameToRemove, out var targetGameObjectToRemove))
        {
            Destroy(targetGameObjectToRemove);
            _spawnedTrackables.Remove(trackedImageNameToRemove);
        }
    }

    private void HandleTrackImage(ARTrackedImage trackedImage)
    {
        // The name that was defined in the reference image library
        var trackedImageName = trackedImage.referenceImage.name;

        // Store the state of the current tracked image
        var trackedState = trackedImage.trackingState;

        // Check the status of the tracked image

        // If the Image is NOT being tracked
        if (trackedState != TrackingState.Tracking)
        {
            // Check if the collection has the image with the name of the current ARTrackedImage existing
            if (_spawnedTrackables.TryGetValue(trackedImageName, out var existingGameObject))
            {
                // Disable the specific gameObject not being tracked
                existingGameObject.SetActive(false);
            }
            return;
        }

        // The Image is being tracked at this point

        // Check if the spawned object is/was tracked previously
        if (_spawnedTrackables.TryGetValue(trackedImageName, out var trackedGameObject))
        {
            // Enable the GameObject
            trackedGameObject.SetActive(true);
            // Make sure it follows the tracked image
            trackedGameObject.transform.SetPositionAndRotation(trackedImage.transform.position,
                trackedImage.transform.rotation);
            // This object is already updated so we simply return
            return;
        }

        // If this point is reached, that means we are tracking an object that has not yet been instantiated
        var newObject = Instantiate(
            GetSpawnPrefabFromData(trackedImageName),  // what prefab to spawn
            trackedImage.transform.position,    // where to spawn
            trackedImage.transform.rotation);   // rotation when spawned

        // Make sure to add this newObject to the dictionary

        _spawnedTrackables.Add(trackedImageName, newObject);
    }


    // Returns the spawn prefab based on the defined imageName
    private GameObject GetSpawnPrefabFromData(string imageName)
    {
        foreach (var data in _trackedImageDatas)
        {
            if (data.ImageReferenceName == imageName)
            {
                return data.PrefabToSpawn;
            }
        }

        return null;
    }
}
