using System;

namespace NetTest
{
    public partial class Util
    {

        public static void SplitMessage(string args, out string id, out string message)
        {
            id = null;
            message = null;

            if (null == args)
            {
                return;
            }

            string[] result = args.Split(TestDefine.messageSplit);
            id = result.Length > 0 ?
                 result[0] :
                 null;

            message = result.Length > 1 ?
                 result[1] :
                 null;
        }

        public class Debug
        {
            public static void Log(string format, params object[] args)
            {
                Console.WriteLine(string.Format(format, args));
            }

            public static void ClientLog(string format, params object[] args)
            {
                lock (logLockObject)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Log("[CLIENT] " + format, args);
                    Console.ResetColor();
                }
            }

            public static void ServerLog(string format, params object[] args)
            {
                lock (logLockObject)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Log("[SERVER] " + format, args);
                    Console.ResetColor();
                }
            }

            private static object logLockObject = new object();
        }
    }
}
