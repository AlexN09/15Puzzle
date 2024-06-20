
using UnityEngine;


public class TouchInput : MonoBehaviour {

    public bool wasTouch = false;
  
    public void OnMouseDown()
    {       
      wasTouch = true;
    }
}
