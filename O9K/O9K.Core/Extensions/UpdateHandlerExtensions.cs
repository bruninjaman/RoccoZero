namespace O9K.Core.Extensions
{
    using Divine.SDK.Managers.Update;

    public static class UpdateHandlerExtensions
    {
        public static void SetUpdateRate(this UpdateHandler updateHandler, int rate)
        {
            if (updateHandler.Executor is TimeoutHandler timeoutHandler)
            {
                timeoutHandler.Timeout = (uint)rate;
            }
        }
    }
}