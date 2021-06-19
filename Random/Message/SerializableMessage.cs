using System;

namespace nfynt
{

    public abstract class SerializableMessage : ISerializable, IDisposable
    {
        public abstract int MaxSerializedLength { get; }

        public abstract MessageType MessageType { get; }

        public abstract bool RequiresScene { get; }

        public abstract bool Transient { get; }

        public abstract void Reset();

        #region Public

        public abstract int Deserialize( byte[] buffer, int offset );

        public abstract int Serialize( byte[] buffer, int offset );

        public void Dispose()
        {
            MessagePool.Put( this );
        }

        public virtual byte[] Serialize()
        {
            return SerializationUtils.CreateExactBufferAndSerialize( MaxSerializedLength, Serialize );
        }

        public virtual byte[] Serialize( out int length )
        {
            return SerializationUtils.CreateBufferAndSerialize( MaxSerializedLength, Serialize, out length );
        }

        #endregion
    }

}
