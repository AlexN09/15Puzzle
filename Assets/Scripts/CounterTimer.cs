using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class CounterTimer : MonoBehaviour
{
    public  Text time;
   public static int seconds = 0, minutes = 0,hours;
    
    void Start()
    {
     
        StartCoroutine(Timer());
      
    }
    
   
    IEnumerator Timer()
    {
        while (true)
        {
            time.text = (hours < 10 ? "0" + hours.ToString() : hours.ToString()) + ":" + (minutes < 10 ? "0" + minutes.ToString() : minutes.ToString()) + ":" + (seconds < 10 ? "0" + seconds.ToString() : seconds.ToString());
            seconds++;

           if (seconds == 60)
           {
                seconds = 0;
                minutes++;
                if (minutes == 60)
                {
                    minutes = 0;
                    hours++;
                }
           }
          
           


            yield return new WaitForSeconds(1.0f);
        }
    }
}
