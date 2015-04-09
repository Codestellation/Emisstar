using System;
using System.Collections;

namespace Codestellation.Emisstar
{
    public static class PublisherExtensions
    {
        /// <summary>
        /// Publish messages to subscribed handlers.
        /// </summary>
        /// <param name="self">An intance of <see cref="IPublisher"/></param>
        /// <param name="message1">A message to deliver</param>
        /// <param name="message2">A message to deliver</param>
        public static void Publish(this IPublisher self, object message1, object message2)
        {
            if (self == null)
            {
                throw new ArgumentNullException("self");
            }

            self.Publish(message1);
            self.Publish(message2);
        }

        /// <summary>
        /// Publish messages to subscribed handlers.
        /// </summary>
        /// <param name="self">An intance of <see cref="IPublisher"/></param>
        /// <param name="message1">A message to deliver</param>
        /// <param name="message2">A message to deliver</param>
        /// <param name="message3">A message to deliver</param>
        public static void Publish(this IPublisher self, object message1, object message2, object message3)
        {
            if (self == null)
            {
                throw new ArgumentNullException("self");
            }

            self.Publish(message1);
            self.Publish(message2);
            self.Publish(message3);
        }

        /// <summary>
        /// Publish messages to subscribed handlers.
        /// </summary>
        /// <param name="self">An intance of <see cref="IPublisher"/></param>
        /// <param name="message1">A message to deliver</param>
        /// <param name="message2">A message to deliver</param>
        /// <param name="message3">A message to deliver</param>
        /// <param name="message4">A message to deliver</param>
        public static void Publish(this IPublisher self, object message1, object message2, object message3, object message4)
        {
            if (self == null)
            {
                throw new ArgumentNullException("self");
            }

            self.Publish(message1);
            self.Publish(message2);
            self.Publish(message3);
            self.Publish(message4);
        }

        /// <summary>
        /// Publish messages to subscribed handlers.
        /// </summary>
        /// <param name="self">An intance of <see cref="IPublisher"/></param>
        /// <param name="message1">A message to deliver</param>
        /// <param name="message2">A message to deliver</param>
        /// <param name="message3">A message to deliver</param>
        /// <param name="message4">A message to deliver</param>
        /// <param name="message5">A message to deliver</param>
        public static void Publish(this IPublisher self, object message1, object message2, object message3, object message4, object message5)
        {
            if (self == null)
            {
                throw new ArgumentNullException("self");
            }

            self.Publish(message1);
            self.Publish(message2);
            self.Publish(message3);
            self.Publish(message4);
            self.Publish(message5);
        }

        /// <summary>
        /// Publish messages to subscribed handlers.
        /// </summary>
        /// <param name="self">An intance of <see cref="IPublisher"/></param>
        /// <param name="messages">Messages to deliver.</param>
        public static void Publish(this IPublisher self, params object[] messages)
        {
            Publish(self, (IEnumerable)messages);
        }

        /// <summary>
        /// Publish messages to subscribed handlers.
        /// </summary>
        /// <param name="self">An intance of <see cref="IPublisher"/></param>
        /// <param name="messages">Messages to deliver.</param>
        public static void Publish(this IPublisher self, IEnumerable messages)
        {
            if (self == null)
            {
                throw new ArgumentNullException("self");
            }

            if (messages == null)
            {
                throw new ArgumentNullException("messages", "Messages should not be null");
            }

            foreach (var message in messages)
            {
                self.Publish(message);
            }
        }
    }
}