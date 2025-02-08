using UnityEngine;

public class Configuration : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #if !UNITY_ANDROID && !UNITY_IOS
                // Nếu không phải Android hoặc iOS, đặt độ phân giải
                Screen.SetResolution(1280, 740, false);
#else
                Screen.fullScreen = true;
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
