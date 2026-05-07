using UnityEngine;


    public class SetBorderless : MonoBehaviour
    {
        private static WindowScript WindowScript => FindAnyObjectByType<WindowScript>();
        
    
        private bool isEnableResetWindowSizeAndNoBorder = true;
    
        private void Awake()
        {
            #if UNITY_EDITOR
                isEnableResetWindowSizeAndNoBorder = false;
            #endif

            if (isEnableResetWindowSizeAndNoBorder)
            {
                WindowScript.OnNoBorderBtnClick();
                WindowScript.ResetWindowSize();
            }
        
        }
    
    }

