namespace Overwolf.Controls.EventArgs
{
    internal sealed class ButtonEventArgs : System.EventArgs
    {
        public bool Value { get; set; }

        public ButtonEventArgs(bool value)
        {
            Value = value;
        }
    }
}
