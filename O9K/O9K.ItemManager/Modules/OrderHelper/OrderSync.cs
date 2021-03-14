namespace O9K.ItemManager.Modules.OrderHelper
{
    internal interface IOrderSync
    {
        bool ForceNextOrderManual { get; set; }

        bool IgnoreSoulRingOrder { get; set; }
    }

    internal class OrderSync : IOrderSync
    {
        public bool ForceNextOrderManual { get; set; }

        public bool IgnoreSoulRingOrder { get; set; }
    }
}