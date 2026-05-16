using UnityEngine;

// An interface acts like a contract
// They are usually named with I at the beginning to indicate that it is an interface
public interface IScreenView
{
    // Whatever properties or method is defined in the interface must be implemented
    UIScreenType ScreenType { get; }
    bool IsVisible { get; }
    void ShowView();
    void HideView();
    void UpdateView();
}
