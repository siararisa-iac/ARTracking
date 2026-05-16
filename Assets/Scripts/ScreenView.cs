using UnityEngine;

public class ScreenView : MonoBehaviour, IScreenView
{
    [SerializeField]
    private UIScreenType _screenType;

    private bool _isVisible = false;

    public UIScreenType ScreenType => _screenType;

    public bool IsVisible => _isVisible;

    public void HideView()
    {
        _isVisible = false;
        gameObject.SetActive(false);
    }

    public void ShowView()
    {
        _isVisible = true;
        gameObject.SetActive(true);
    }

    public virtual void UpdateView() { }
}
