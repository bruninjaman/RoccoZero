namespace O9K.Core.Extensions
{
    using Divine.Update;

    public static class UpdateHandlerExtensions
    {
        public static void SetUpdateRate(this UpdateHandler updateHandler, int rate)
        {
            if (updateHandler.Executor is TimeoutHandler timeoutHandler)
            {
                timeoutHandler.Timeout = rate;
            }
        }
    }
}