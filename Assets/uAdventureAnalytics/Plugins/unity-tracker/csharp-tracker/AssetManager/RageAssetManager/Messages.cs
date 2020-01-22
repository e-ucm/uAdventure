/*
 * Copyright 2016 Open University of the Netherlands
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * This project has received funding from the European Union’s Horizon
 * 2020 research and innovation programme under grant agreement No 644187.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace AssetManagerPackage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A Broadcast Messages class.
    /// </summary>
    public static class Messages
    {
        #region Fields

        /// <summary>
        /// The subscription ID generator.
        /// </summary>
        private static Int32 idGenerator = 0;

        /// <summary>
        /// The messages is a dictionary of messages and their subscribers.
        /// </summary>
        private static Dictionary<String, Dictionary<String, MessagesEventCallback>> messages = new Dictionary<String, Dictionary<String, MessagesEventCallback>>();

        #endregion Fields

        #region Delegates

        /// <summary>
        /// Interface for broadcast message callback.
        /// </summary>
        ///
        /// <param name="message"> The message id. </param>
        /// <param name="parameters">  A variable-length parameters list containing arguments. </param>
        public delegate void MessagesEventCallback(String message, params object[] parameters);

        #endregion Delegates

        #region Methods

        /// <summary>
        /// Define a broadcast message.
        /// </summary>
        ///
        /// <param name="message"> The message. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public static Boolean define(String message)
        {
            if (!messages.Keys.Contains(message))
            {
                messages.Add(message, new Dictionary<String, MessagesEventCallback>());

                return true;
            }

            return false;
        }

        /// <summary>
        /// Broadcast a message.
        /// </summary>
        ///
        /// <param name="message"> The message to broadcast. </param>
        /// <param name="parameters">     A variable-length parameters list containing
        ///                         arguments. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public static Boolean broadcast(String message, params object[] parameters)
        {
            if (!messages.Keys.Contains(message))
            {
                return false;
            }

            foreach (KeyValuePair<String, MessagesEventCallback> func in messages[message])
            {
                func.Value(message, parameters);
            }

            return true;
        }

        /// <summary>
        /// Subscribe to a broadcast message.
        /// </summary>
        ///
        /// <remarks>
        /// if the message does not exist yet it's created on-the-fly.
        /// </remarks>
        ///
        /// <param name="message">  The message. </param>
        /// <param name="callback"> The callback function. </param>
        ///
        /// <returns>
        /// The broadcast subscription id.
        /// </returns>
        public static String subscribe(String message, MessagesEventCallback callback)
        {
            if (!messages.Keys.Contains(message))
            {
                messages.Add(message, new Dictionary<String, MessagesEventCallback>());
            }

            String subscriptionId = (++idGenerator).ToString();

            messages[message].Add(subscriptionId, callback);

            return subscriptionId;
        }

        /// <summary>
        ///  Unsubscribes the given broadcast subscription id.
        /// </summary>
        ///
        /// <param name="subscriptionId"> The broadcast subscription id. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public static Boolean unsubscribe(String subscriptionId)
        {
            foreach (String message in messages.Keys)
            {
                Dictionary<String, MessagesEventCallback> subscribers = messages[message];

                foreach (String subscriber in subscribers.Keys)
                {
                    if (subscriptionId.Equals(subscriber))
                    {
                        subscribers.Remove(subscriber);

                        return true;
                    }
                }
            }

            return false;
        }

        #endregion Methods
    }
}