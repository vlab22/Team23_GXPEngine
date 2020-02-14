namespace GXPEngine
{
    public class PlayerInput : GameObject, IStorkInput
    {
        private int _leftWingPressure;
        private int _rightWingPressure;

        private int _leftPressedTime;
        private int _leftPressedFrame;
        private int _leftReleasedFrame;
        
        private int _rightPressedTime;
        private int _rightPressedFrame;
        private int _rightReleasedFrame;
        
        public PlayerInput()
        {
            
        }
        
        void Update()
        {
            if (Input.GetKey(Key.LEFT))
            {
                _leftPressedTime += Time.deltaTime;
            }
            else
            {
                _leftPressedTime = 0;
            }


            if (Input.GetKey(Key.RIGHT))
            {
                _rightPressedTime += Time.deltaTime;
            }
            else
            {
                _rightPressedTime = 0;
            }

            _leftWingPressure = _leftPressedTime;
            _rightWingPressure = _rightPressedTime;
        }

        int IStorkInput.LeftWingPressure => _leftWingPressure;

        int IStorkInput.RightWingPressure => _rightWingPressure;
    }
}