using System;
using UnityEngine;
using UnityEngine.UI;

namespace Script.UI
{
    
    public enum ENUM_Device_Type
    {
        Tablet,
        Phone
    }
    
    public class ChangeTabletCanvasScaler : MonoBehaviour
    {
        [SerializeField] private CanvasScaler _scaler;
        
        private void Awake()
        {
            if (GetDeviceType() == ENUM_Device_Type.Tablet)
            {
                _scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_scaler.GetComponent<RectTransform>());
            }
        }

        private float DeviceDiagonalSizeInInches()
        {
            float screenWidth = Screen.width / Screen.dpi;
            float screenHeight = Screen.height / Screen.dpi;
            float diagonalInches =Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
 
            return diagonalInches;
        }
 
        public ENUM_Device_Type GetDeviceType()
        {
#if UNITY_IOS
            bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
            if (deviceIsIpad)
            {
                return ENUMHERE.Tablet;
            }
 
            bool deviceIsIphone = UnityEngine.iOS.Device.generation.ToString().Contains("iPhone");
            if (deviceIsIphone)
            {
                return ENUMHERE.Phone;
            }
#endif
            float aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
            bool isTablet = (DeviceDiagonalSizeInInches() > 6.5f && aspectRatio < 2f);
 
            if (isTablet)
            {
                return ENUM_Device_Type.Tablet;
            }
            else
            {
                return ENUM_Device_Type.Phone;
            }
        }
    }
}