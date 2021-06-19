using System;
using System.Collections.Generic;

namespace nfynt
{
    public class MessagePool
    {
        private static readonly Dictionary < Type, Queue < SerializableMessage > > s_Pool =
            new Dictionary < Type, Queue < SerializableMessage > >();

        #region Public

        public static T Get < T >() where T : SerializableMessage, new()
        {
            if ( s_Pool.TryGetValue( typeof( T ), out Queue < SerializableMessage > queue ) && queue.Count > 0 )
            {
                T message = queue.Dequeue() as T;

                message.Reset();

                return message;
            }

            return new T();
        }

        public static SerializableMessage Get( Type type )
        {
            if (s_Pool.TryGetValue(type, out Queue<SerializableMessage> queue) && queue.Count > 0)
            {
                SerializableMessage message = queue.Dequeue();

                message.Reset();

                return message;
            }

            return ( SerializableMessage ) Activator.CreateInstance( type );
        }

        public static void Put < T >( T message ) where T : SerializableMessage
        {
            Type type = message.GetType();

            if ( s_Pool.TryGetValue( type, out Queue < SerializableMessage > queue ) )
            {
                queue.Enqueue( message );

                return;
            }

            Queue < SerializableMessage > newQueue = new Queue < SerializableMessage >();
            s_Pool[type] = newQueue;
            newQueue.Enqueue( message );
        }

        public static void Put < T >( List < T > messages ) where T : SerializableMessage
        {
            foreach ( T message in messages )
            {
                Put( message );
            }
        }

        #endregion
    }

}
