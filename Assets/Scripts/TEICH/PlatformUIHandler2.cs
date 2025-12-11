using UnityEngine;
using System.Runtime.InteropServices;

public class PlatformUIHandler2 : MonoBehaviour
{
    public GameObject touchUI; // Reference to the on-screen touch UI


   
    [DllImport("__Internal")]
    private static extern bool IsMobile();

    

    void Start()
    {
       if(IsRunningOnMobile())
       {
        
        GameManager2.Instance.isMobile = true;
        
       

       }
       
       else
       {
       
        GameManager2.Instance.isMobile = false;
        
       }
    }

    bool IsRunningOnMobile()
    {
     #if UNITY_WEBGL && !UNITY_EDITOR
        return IsMobile();
      #else
        return false;
      #endif
    

    }
}
