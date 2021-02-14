using Divine.SDK.Managers.Update;

namespace Divine.Core.ComboFactory
{
    public abstract class BaseUpdateHandler : BaseUtils
    {
        private readonly int Timeout;

        public BaseUpdateHandler(int timeout = 0)
        {
            Timeout = timeout;
        }

        private UpdateHandler UpdateHandler;

        public virtual void Run()
        {
            if (UpdateHandler == null)
            {
                UpdateHandler = UpdateManager.Subscribe((uint)Timeout, Execute);
            }

            if (UpdateHandler.IsEnabled)
            {
                return;
            }

            UpdateHandler.IsEnabled = true;
        }

        public void Cancel()
        {
            if (!UpdateHandler.IsEnabled)
            {
                return;
            }

            UpdateHandler.IsEnabled = false;
        }

        public virtual void Dispose()
        {
            if (UpdateHandler == null)
            {
                return;
            }

            UpdateManager.Unsubscribe(Execute);
            UpdateHandler = null;
        }

        protected abstract void Execute();
    }
}
