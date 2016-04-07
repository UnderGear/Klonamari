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
            //this is seriously how you currently have to find out about controllers added/removed in Unity. I checked dug through with ILSpy a fair amount.
            //I had this code in Katamari.cs before, but it's so gross I pulled it out into this class instead and wrapped it in a service to be driven by KatamariMain.cs
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
            
            if (xboxFound && !usingXboxController)
            {
                EventManager.InputChanged(new DualThumbstickInput());
            } else if (!xboxFound && usingXboxController)
            {
                EventManager.InputChanged(new KeyboardInput());
            }
        }
    }
}
