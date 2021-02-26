using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubMenu : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    private GameObject _previousButton;
    
    private void OnEnable()
    {
        _previousButton = EventSystem.current.currentSelectedGameObject;
        closeButton.Select();
        closeButton.OnSelect(null);
    }

    public void CloseMenu()
    {
        EventSystem.current.SetSelectedGameObject(_previousButton);
        gameObject.SetActive(false);
    }
}
