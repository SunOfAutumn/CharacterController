using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Reflection;
using UnityStandardAssets.CrossPlatformInput;

namespace SunOfAutumn.CharacterController
{
    public enum InputDevice
    {
        MouseKeyboard,
        Joystick,
        Mobile
    };
    public class SOAInput : MonoBehaviour
    {
        public delegate void OnChangeInputType(InputDevice type);
        public event OnChangeInputType onChangeInputType;
        private static SOAInput _instance;
        public static SOAInput instance
        {
            get
            {
                if(_instance)
                {
                    _instance = GameObject.FindObjectOfType<SOAInput>();
                    if(_instance == null)
                    {
                        new GameObject("SOAInputType", typeof(SOAInput));
                        return SOAInput.instance;
                    }
                }
                return _instance;
            }
        }

        private InputDevice _inputType = InputDevice.MouseKeyboard;
        public InputDevice inputDevice
        {
            get { return _inputType; }
            set
            {
                _inputType = value;
                OnChangeInput();
            }
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// GAMEPAD VIBRATION - call this method to use vibration on the gamepad
        /// </summary>
        /// <param name="vibTime">duration of the vibration</param>
        /// <returns></returns>
        public void GamepadVibration(float vibTime)
        {
            if (inputDevice == InputDevice.Joystick)
            {
                StartCoroutine(GamepadVibrationRotine(vibTime));
            }
        }

        private IEnumerator GamepadVibrationRotine(float vibTime)
        {
            if (inputDevice == InputDevice.Joystick)
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

                XInputDotNetPure.GamePad.SetVibration(0, 1, 1);
                yield return new WaitForSeconds(vibTime);
                XInputDotNetPure.GamePad.SetVibration(0, 0, 0);

#else
	            yield return new WaitForSeconds(0f);
#endif
            }
        }
        private bool isMobileInput()
        {
#if UNITY_EDITOR && UNITY_MOBILE
            if (EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
            {
                return true;
            }
		
#elif MOBILE_INPUT
            if (EventSystem.current.IsPointerOverGameObject() || (Input.touches.Length > 0))
                return true;
#endif
            return false;
        }

        private bool isMouseKeyboard()
        {
#if MOBILE_INPUT
                return false;
#else
            // mouse & keyboard buttons
            if (Event.current.isKey || Event.current.isMouse)
                return true;
            // mouse movement
            if (Input.GetAxis("Mouse X") != 0.0f || Input.GetAxis("Mouse Y") != 0.0f)
                return true;

            return false;
#endif
        }

        private bool isJoystickInput()
        {
            // joystick buttons
            if (Input.GetKey(KeyCode.Joystick1Button0) ||
                Input.GetKey(KeyCode.Joystick1Button1) ||
                Input.GetKey(KeyCode.Joystick1Button2) ||
                Input.GetKey(KeyCode.Joystick1Button3) ||
                Input.GetKey(KeyCode.Joystick1Button4) ||
                Input.GetKey(KeyCode.Joystick1Button5) ||
                Input.GetKey(KeyCode.Joystick1Button6) ||
                Input.GetKey(KeyCode.Joystick1Button7) ||
                Input.GetKey(KeyCode.Joystick1Button8) ||
                Input.GetKey(KeyCode.Joystick1Button9) ||
                Input.GetKey(KeyCode.Joystick1Button10) ||
                Input.GetKey(KeyCode.Joystick1Button11) ||
                Input.GetKey(KeyCode.Joystick1Button12) ||
                Input.GetKey(KeyCode.Joystick1Button13) ||
                Input.GetKey(KeyCode.Joystick1Button14) ||
                Input.GetKey(KeyCode.Joystick1Button15) ||
                Input.GetKey(KeyCode.Joystick1Button16) ||
                Input.GetKey(KeyCode.Joystick1Button17) ||
                Input.GetKey(KeyCode.Joystick1Button18) ||
                Input.GetKey(KeyCode.Joystick1Button19))
            {
                return true;
            }

            // joystick axis
            if (Input.GetAxis("LeftAnalogHorizontal") != 0.0f ||
                Input.GetAxis("LeftAnalogVertical") != 0.0f ||
                Input.GetAxis("RightAnalogHorizontal") != 0.0f ||
                Input.GetAxis("RightAnalogVertical") != 0.0f ||
                Input.GetAxis("LT") != 0.0f ||
                Input.GetAxis("RT") != 0.0f ||
                Input.GetAxis("D-Pad Horizontal") != 0.0f ||
                Input.GetAxis("D-Pad Vertical") != 0.0f)
            {
                return true;
            }
            return false;
        }
        void OnChangeInput()
        {
            if(onChangeInputType != null)
            {
                onChangeInputType(inputDevice);
            }
        }
    }

    [System.Serializable]
    public class SOAGenericInput
    {
        protected InputDevice inputDevice
        {
            get { return SOAInput.instance.inputDevice; }
        }

        [SerializeField]
        private bool useInput = true;
        private bool isAxisInUse;
        [SerializeField]
        private string keyboard;
        [SerializeField]
        private bool keyboardAxis;
        [SerializeField]
        private string joystick;
        [SerializeField]
        private bool joystickAxis;
        [SerializeField]
        private string mobile;
        [SerializeField]
        private bool mobileAxis;

        private float buttomTimer;
        private bool inButtomTimer;
        private float mulTapTimer;
        private int mulTapCounter;

        public bool isAxis
        {
            get
            {
                bool value = false;
                switch(inputDevice)
                {
                    case InputDevice.Joystick:
                        value = joystickAxis;
                        break;
                    case InputDevice.MouseKeyboard:
                        value = keyboardAxis;
                        break;
                    case InputDevice.Mobile:
                        value = mobileAxis;
                        break;
                }
                return value;
            }
        }

        /// <summary>
        /// Initialise a new GenericInput
        /// </summary>
        /// <param name="keyboard"></param>
        /// <param name="joystick"></param>
        /// <param name="mobile"></param>
        public SOAGenericInput(string keyboard, string joystick, string mobile)
        {
            this.keyboard = keyboard;
            this.joystick = joystick;
            this.mobile = mobile;
        }

        /// <summary>
        /// Initialise a new GnericInput
        /// </summary>
        /// <param name="keyboard"></param>
        /// <param name="keyboardAxis"></param>
        /// <param name="joystick"></param>
        /// <param name="joystickAxis"></param>
        /// <param name="mobile"></param>
        /// <param name="mobileAxis"></param>
        public SOAGenericInput(string keyboard, bool keyboardAxis, string joystick, bool joystickAxis, string mobile, bool mobileAxis)
        {
            this.keyboard = keyboard;
            this.keyboardAxis = keyboardAxis;
            this.joystick = joystick;
            this.joystickAxis = joystickAxis;
            this.mobile = mobile;
            this.mobileAxis = mobileAxis;
        }

        /// <summary>
        /// Button Name
        /// </summary>
        public string buttonName
        {
            get
            {
                if(SOAInput.instance != null)
                {
                    if(SOAInput.instance.inputDevice == InputDevice.MouseKeyboard)
                    {
                        return keyboard;
                    }
                    else if(SOAInput.instance.inputDevice == InputDevice.MouseKeyboard)
                    {
                        return joystick;
                    }
                    else
                    {
                        return mobile;
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Check if button is a Key
        /// </summary>
        public bool isKey
        {
            get
            {
                if(SOAInput.instance != null)
                {
                    if (System.Enum.IsDefined(typeof(KeyCode), buttonName))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Get <see cref="KeyCode"/> value
        /// </summary>
        public KeyCode key
        {
            get
            {
                return (KeyCode)System.Enum.Parse(typeof(KeyCode), buttonName);
            }
        }

        bool IsButtonAvailable(string btnName)
        {
            if (!useInput) return false;
            try {
                if (isKey) return true;
                Input.GetButton(buttonName);
                return true;
            }
            catch(System.Exception exc)
            {
                Debug.LogWarning(" Failure to try access button : " + buttonName + "\n" + exc.Message);
                return false;
            }
        }

        /// <summary>
        /// Get Axis Raw
        /// </summary>
        /// <returns></returns>
        public float GetAxisRaw()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName) || !isKey) return 0;
            if(inputDevice == InputDevice.Mobile)
            {
                return CrossPlatformInputManager.GetAxisRaw(this.buttonName);
            }
            else if(inputDevice == InputDevice.MouseKeyboard)
            {
                return Input.GetAxisRaw(this.buttonName);
            }
            else if(inputDevice == InputDevice.Joystick)
            {
                return Input.GetAxisRaw(this.buttonName);
            }
            return 0;
        }

        /// <summary>
        /// Get Axis like a button
        /// </summary>
        /// <param name="value">Value to check need to be diferent 0</param>
        /// <returns></returns>
        public bool GetAxisButton(float value = 0.5f)
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            if(value > 0)
            {
                return GetAxisRaw() >= value;
            }
            else if (value < 0)
            {
                return GetAxisRaw() <= value;
            }
            return false;
        }


        public bool GetButton()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            if (isAxis) return GetAxisButton();

            if(inputDevice == InputDevice.Mobile)
            {
                if (CrossPlatformInputManager.GetButton(this.buttonName))
                    return true;
            }
            else if(inputDevice == InputDevice.MouseKeyboard)
            {
                if(isKey)
                {
                    if (Input.GetKey(key))
                        return true;
                }
                else
                {
                    if (Input.GetButton(this.buttonName))
                        return true;
                }
            }
            else if(inputDevice == InputDevice.Joystick)
            {
                if (Input.GetButton(this.buttonName))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get ButtonDown
        /// </summary>
        /// <returns></returns>
        public bool GetButtonDown()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            if (isAxis) return GetAxisButtonDown();
            // mobile
            if (inputDevice == InputDevice.Mobile)
            {
                if (CrossPlatformInputManager.GetButtonDown(this.buttonName))
                    return true;
            }
            // keyboard/mouse
            else if (inputDevice == InputDevice.MouseKeyboard)
            {
                if (isKey)
                {
                    if (Input.GetKeyDown(key))
                        return true;
                }
                else
                {
                    if (Input.GetButtonDown(this.buttonName))
                        return true;
                }
            }
            // joystick
            else if (inputDevice == InputDevice.Joystick)
            {
                if (Input.GetButtonDown(this.buttonName))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get Button Up
        /// </summary>
        /// <returns></returns>
        public bool GetButtonUp()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            if (isAxis) return GetAxisButtonUp();

            // mobile
            if (inputDevice == InputDevice.Mobile)
            {
                if (CrossPlatformInputManager.GetButtonUp(this.buttonName))
                    return true;
            }
            // keyboard/mouse
            else if (inputDevice == InputDevice.MouseKeyboard)
            {
                if (isKey)
                {
                    if (Input.GetKeyUp(key))
                        return true;
                }
                else
                {
                    if (Input.GetButtonUp(this.buttonName))
                        return true;
                }
            }
            // joystick
            else if (inputDevice == InputDevice.Joystick)
            {
                if (Input.GetButtonUp(this.buttonName))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get Axis
        /// </summary>
        /// <returns></returns>
        public float GetAxis()
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName) || isKey) return 0;

            // mobile
            if (inputDevice == InputDevice.Mobile)
            {
                return CrossPlatformInputManager.GetAxis(this.buttonName);
            }
            // keyboard/mouse
            else if (inputDevice == InputDevice.MouseKeyboard)
            {
                return Input.GetAxis(this.buttonName);
            }
            // joystick
            else if (inputDevice == InputDevice.Joystick)
            {
                return Input.GetAxis(this.buttonName);
            }
            return 0;
        }

        public bool GetAxisButtonDown(float value = 0.5f)
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            if(value > 0)
            {
                if(!isAxisInUse && GetAxisRaw() >= value)
                {
                    isAxisInUse = true;
                    return true;
                }
                else if(isAxisInUse && GetAxisRaw() == 0)
                {
                    isAxisInUse = false;
                }
            }
            else if (value < 0)
            {
                if(!isAxisInUse && GetAxisRaw() <= value)
                {
                    isAxisInUse = true;
                    return true;
                }
                else if(isAxisInUse && GetAxisRaw() == 0)
                {
                    isAxisInUse = false;
                }
            }
            return false;
        }

        /// <summary>
        /// Get Axis like a buttonUp
        /// Check if Axis is zero after press
        /// </summary>
        /// <returns></returns>
        public bool GetAxisButtonUp()
        {
            if(isAxisInUse && GetAxisRaw() == 0)
            {
                isAxisInUse = false;
                return true;
            }
            else if(!isAxisInUse && GetAxisRaw() != 0)
            {
                isAxisInUse = true;
            }
            return false;
        }
        
        /// <summary>
        /// Get Double Button Down Check if button is pressed Within the define time;
        /// </summary>
        /// <param name="inputTime"></param>
        /// <returns></returns>
        public bool GetDoubleButtonDown(float inputTime = 1)
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            if(mulTapCounter == 0 && GetButtonDown())
            {
                mulTapTimer = Time.time;
                mulTapCounter = 1;
                return false;
            }

            if(mulTapCounter == 1 && GetButtonDown())
            {
                var time = mulTapTimer + inputTime;
                var valid = (Time.time < time);
                mulTapTimer = 0;
                mulTapCounter = 0;
                return valid;
            }
            return false;
        }
        
        /// <summary>
        /// Get Button Timer Check if button is pressed for defined time
        /// </summary>
        /// <param name="inputTime">time to check button press</param>
        /// <returns></returns>
        public bool GetButtonTimer(float inputTime = 2)
        {
            if (string.IsNullOrEmpty(buttonName) || !IsButtonAvailable(this.buttonName)) return false;
            if(GetButtonDown() && !inButtomTimer)
            {
                buttomTimer = Time.time;
                inButtomTimer = true;
            }
            if (inButtomTimer)
            {
                var time = buttomTimer + inputTime;
                var valid = (time - Time.time <= 0);
                if (GetButtonUp())
                {
                    inButtomTimer = false;
                    return valid;
                }
                if (valid)
                {
                    inButtomTimer = false;
                }
                return valid;
            }
            return false;
        }
    }
}
