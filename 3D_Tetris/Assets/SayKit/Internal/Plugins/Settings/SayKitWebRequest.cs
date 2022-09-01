using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace SayKitInternal
{
    public class SayKitWebRequest
    {
        private const string _defaultText = "";

        public string Url { get; }
        public string Text { get; private set; } = _defaultText;

        public string ErrorMessage { get; private set; }
        public bool IsDone { get; private set; }



        public SayKitWebRequest(string url)
        {
            Url = url;
        }

        /// <summary>
        /// timeout - seconds.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool SendAndWait(float timeout = -1)
        {
            IsDone = false;
            ErrorMessage = null;


            float timestamp = Time.realtimeSinceStartup;
           
            using (var client = UnityWebRequest.Get(Url))
            {
                var op = client.SendWebRequest();
                while (!op.isDone)
                {
                    if (timeout > 0f && timeout < Time.realtimeSinceStartup - timestamp)
                    {
                        ErrorMessage = "Timeout";
                        return false;
                    }

                    Thread.Sleep(100);
                }

                IsDone = true;

                if (client.isNetworkError || client.isHttpError)
                {
                    ErrorMessage = client.error;
                    return false;
                }
                else
                {
                    Text = client.downloadHandler?.text ?? _defaultText;
                    return true;
                }
            }
        }

    }

}