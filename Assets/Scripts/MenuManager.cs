using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
   public void setFieldSize(int size)
    {
        Game.fieldSize = size;
    }
   public void ChangeScene(int sceneId)
   {
       
        SceneManager.LoadScene(sceneId);
   }
   public void Quit()
    {
        Application.Quit();
    }
}
