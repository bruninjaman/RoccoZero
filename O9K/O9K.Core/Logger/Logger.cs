namespace O9K.Core.Logger
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

    using Data;

    using Divine;
    using Divine.SDK.Managers.Log;

    using Exceptions;

    //using PlaySharp.Sentry;
    //using PlaySharp.Sentry.Data;

    using SharpDX;

    public static class Logger
    {
        public enum ErrorLevel
        {
            Fatal = 0,

            Error = 1,

            Warning = 2,

            Info = 3,

            Debug = 4
        }

        private static readonly ConcurrentDictionary<int, int> Cache = new ConcurrentDictionary<int, int>();

        //private static readonly SentryClient Client;

        private static readonly bool DisableCapture;

        private static readonly HashSet<GameMode> DisabledCaptureModes = new HashSet<GameMode>
        {
            GameMode.Custom,
            GameMode.Event,
        };

        private static readonly HashSet<Type> IgnoredExceptionTypes = new HashSet<Type>
        {
            typeof(MissingMethodException),
        };

        static Logger()
        {
            /*DisableCapture = GameManager.ExpectedPlayers == 1 || DisabledCaptureModes.Contains(Game.GameMode);

            const string Key = "ed70f139a30e4c3e8481e629e541dec6:c07ecbe5b1c243aeb9b19b8d40b73461";
            const string Project = "1277552";

            Client = new SentryClient($"https://{Key}@sentry.io/{Project}");

            var metadata = Assembly.GetExecutingAssembly().GetMetadata();
            if (metadata == null)
            {
                return;
            }

            Client.Client.Release = metadata.Commit;
            Client.Tags["core"] = () => metadata.Version;
            Client.Tags["mode"] = () => Game.GameMode.ToString();
            Client.Tags["hero"] = () => ObjectManager.LocalHero.Name;*/
        }

        public static void Error(string message, string info = null, ErrorLevel level = ErrorLevel.Debug)
        {
            var exception = new O9KException(message);

            if (!string.IsNullOrEmpty(info))
            {
                exception.Data["Info"] = info;
            }

            LogManager.Error(exception);
            //CaptureException(exception, Assembly.GetCallingAssembly(), level);
        }

        public static void Error(Exception exception, string info = null, ErrorLevel level = ErrorLevel.Error)
        {
            if (!string.IsNullOrEmpty(info))
            {
                exception.Data["Info"] = info;
            }

            LogManager.Error(exception);
            //CaptureException(exception, Assembly.GetCallingAssembly(), level);
        }

        public static void Error(Exception exception, Entity entity, string info = null, ErrorLevel level = ErrorLevel.Error)
        {
            if (entity != null)
            {
                exception.Data[entity.GetType().Name] = new EntityLogData(entity);
            }
            else
            {
                exception.Data["Entity"] = "null";
            }

            if (!string.IsNullOrEmpty(info))
            {
                exception.Data["Info"] = info;
            }

            LogManager.Error(exception);
            //CaptureException(exception, Assembly.GetCallingAssembly(), level);
        }

        public static void Warn(string text)
        {
            LogManager.Warn(text);
        }

        /*private static void CaptureException(Exception exception, Assembly assembly, ErrorLevel errorLevel)
        {
            if (DisableCapture || IgnoredExceptionTypes.Contains(exception.GetType()) || !GameManager.IsInGame)
            {
                return;
            }

            var key = exception.ToString().GetHashCode();
            if (Cache.TryGetValue(key, out var count) && count >= 15)
            {
                return;
            }

            Cache[key] = count + 1;

            Client.Tags["thread"] = () => Thread.CurrentThread.ManagedThreadId.ToString();
            Client.Tags["version"] = () => assembly.GetMetadata()?.Version ?? "local";
            Client.Client.Logger = assembly.GetName().Name;

            UpdateManager.BeginInvoke(
                () =>
                    {
                        exception.Data["Game"] = new GameLogData();
                        exception.Data["Heroes"] = new HeroesLogData();

                        Client.CaptureAsync(
                            new SentryEvent(exception)
                            {
                                Level = (PlaySharp.Sentry.Data.ErrorLevel)errorLevel,
                            });
                    });
        }*/
    }
}
