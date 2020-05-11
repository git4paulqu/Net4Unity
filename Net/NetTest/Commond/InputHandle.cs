using System;
using System.Collections.Generic;
using System.Threading;

namespace NetTest
{
    public class InputHandle
    {
        public static void Start()
        {
            Thread inputThread = new Thread(HandleInput);
            inputThread.Start();
        }

        public static void Register(Commond.Commond commond, Action<string> callback)
        {
            commondCallback[commond.ToString()] = callback;
        }

        public static void UnRegister(Commond.Commond commond)
        {
            commondCallback.Remove(commond.ToString());
        }

        private static void HandleInput()
        {
            while (true)
            {
                string input = System.Console.ReadLine();
                TryHandleInput(input);
            }
        }

        private static void TryHandleInput(string input)
        {
            if (input == string.Empty)
            {
                return;
            }

            string commond = string.Empty;
            int index = input.IndexOf(" ", 0);
            if (index < 1)
            {
                commond = input;
            }
            else
            {
                commond = input.Substring(0, index).Trim();
            }

            Action<string> callback = null;
            if (commondCallback.TryGetValue(commond, out callback))
            {
                string args = string.Empty;
                PareCommond(input, commond, out args);
                if (null != callback)
                {
                    callback(args);
                }
            }
            else
            {
                Util.Debug.Log("commond not found: {0}", input);
            }
        }

        private static void PareCommond(string input, string define, out string args)
        {
            args = string.Empty;
            if (input.StartsWith(define + " "))
            {
                args = input.Substring(define.Length).Trim();
            }
        }

        private static Dictionary<string, Action<string>> commondCallback = new Dictionary<string, Action<string>>();
    }
}
