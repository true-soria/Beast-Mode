using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectMenu : SubMenu
{
    public void TrainingRoom()
    {
        SceneManager.LoadScene("HelloWorld");
    }
    
    public void Mission1()
    {
        SceneManager.LoadScene("SSH");
    }
}
