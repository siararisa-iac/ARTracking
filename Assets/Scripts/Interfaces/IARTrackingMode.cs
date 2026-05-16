using UnityEngine;

public interface IARTrackingMode 
{
    public ARTrackingMode Mode { get; }
    void Initialize();
    void EnableMode();
    void DisableMode();
    void UpdateMode();
}
