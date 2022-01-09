namespace Overwolf.Controls.EventArgs
{
    internal sealed class ButtonEventArgs : System.EventArgs
    {
        internal bool Value { get; set; }

        internal ButtonEventArgs(bool value)
        {
            Value = value;
        }
    }
}