namespace Overwolf.Controls.EventArgs
{
    internal sealed class TooglerEventArgs : System.EventArgs
    {
        internal bool Value { get; set; }
        internal TooglerEventArgs(bool value)
        {
            Value = value;
        }
    }
}