using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContollerMenu : SubMenu
{
    [SerializeField] private GameObject kbmControlsImage;
    [SerializeField] private GameObject controllerControlsImage;
    [SerializeField] private Text controlsButtonText;

    public void ChangeControlsPreview()
    {
        if (controllerControlsImage.activeSelf)
        {
            kbmControlsImage.SetActive(true);
            controllerControlsImage.SetActive(false);
            controlsButtonText.text = "Keyboard Layout";
        }
        else
        {
            kbmControlsImage.SetActive(false);
            controllerControlsImage.SetActive(true);
            controlsButtonText.text = "Controller Layout";
        }
    }
}
