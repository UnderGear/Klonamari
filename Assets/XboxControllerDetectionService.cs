using UnityEngine;

namespace Klonamari
{
    public class XboxControllerDetectionService : ControllerDetectionService
    {
        private float timeSinceLastPoll = 0.0f;
        private float pollDelay;
        
        private bool usingXboxController = false;

        public void Init(float delay)
        {
            pollDelay = delay;
            Poll();
        }

        public void Tick(float deltaTime)
        {
            timeSinceLastPoll += deltaTime;
            if (timeSinceLastPoll >= pollDelay)
            {
                timeSinceLastPoll = 0.0f;
                Poll();
            }
        }

        private void Poll()
        {
            //this is seriously how you currently have to find out about controllers added/removed. I checked in ILSpy.
            //I had this code in Katamari.cs before, but it's so gross I pulled it out into this class instead and wrapped it in a service to be driven by KatamariMain.cs
            //Unity has been talking about fixing Input for a long time
            string[] names = Input.GetJoystickNames();
            int nameCount = names != null ? names.Length : 0;
            bool xboxFound = false;
            for (int i = 0; i < nameCount; ++i)
            {
                if (names[i].ToLower().Contains("xbox"))
                {
                    xboxFound = true;
                    break;
                }
            }

            //I'd prefer to change out a DI binding or Service in a Service Locator and then just send an empty event to have people respond.
            if (xboxFound && !usingXboxController)
            {
                KatamariEventManager.InputChanged(new KatamariDualThumbstickInput());
            } else if (!xboxFound && usingXboxController)
            {
                KatamariEventManager.InputChanged(new KatamariKeyboardInput());
            }
        }
    }
}
