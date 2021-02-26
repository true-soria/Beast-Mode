using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitMenu : SubMenu
{
    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
