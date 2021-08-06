namespace O9K.AIO.Heroes.ArcWarden.Utils
{
    using System.Runtime.Serialization;

    public enum Lane
    {
        [EnumMember(Value = "top")]
        TOP,

        [EnumMember(Value = "mid")]
        MID,

        [EnumMember(Value = "bot")]
        BOT,

        [EnumMember(Value = "auto")]
        AUTO,
    }
}