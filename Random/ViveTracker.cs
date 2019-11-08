using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Nfynt.Tracking
{
    public class ViveTracker : MonoBehaviour
    {
        public Transform trackedObj1;
        public Transform trackedObj2;

        InputDevice tracker;
        InputDevice tracker2;

        void Start()
        {
            if (trackedObj1 == null)
                return;

            var allDevices = new List<InputDevice>();
            InputDevices.GetDevices(allDevices);
            tracker = allDevices.Find(d => d.role == InputDeviceRole.HardwareTracker);
            tracker2 = allDevices.Find(d => d.role == InputDeviceRole.HardwareTracker && d != tracker);
        }

        void Update()
        {
            if (tracker != null)
            {
                tracker.TryGetFeatureValue(CommonUsages.devicePosition, out var pos);
                trackedObj1.position = pos;
                tracker.TryGetFeatureValue(CommonUsages.deviceRotation, out var rot);
                trackedObj1.rotation = rot;
            }
            if (tracker2 != null)
            {
                tracker.TryGetFeatureValue(CommonUsages.devicePosition, out var pos);
                trackedObj2.position = pos;
                tracker.TryGetFeatureValue(CommonUsages.deviceRotation, out var rot);
                trackedObj2.rotation = rot;
            }
        }
    }
}




/*
 __  _ _____   ____  _ _____  
|  \| | __\ `v' /  \| |_   _| 
| | ' | _| `. .'| | ' | | |   
|_|\__|_|   !_! |_|\__| |_|
 
*/
