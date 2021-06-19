using System;
using System.Collections.Generic;
using UnityEngine;

namespace nfynt
{

    public class Serializer : IDisposable
    {
        private static readonly Queue < Serializer > s_Pool = new Queue < Serializer >();

        private byte[] m_Array;
        private int m_Offset;
        private int m_Length;

        public int ProcessedBytes => m_Length;

        public int Position => m_Offset + m_Length;

        public void Reset( int offset )
        {
            m_Offset = offset;
            m_Length = 0;
        }

        #region Public

        public static Serializer Get( byte[] buffer, int offset )
        {
            // get an available instance from the pool...
            if ( s_Pool.Count > 0 )
            {
                Serializer serializer = s_Pool.Dequeue();
                serializer.Init( buffer, offset );

                return serializer;
            }

            // or else create a new instance, growing the pool when it's put back
            return new Serializer( buffer, offset );
        }

        public static void Put( Serializer serializer )
        {
            s_Pool.Enqueue( serializer );
        }

        public void Dispose()
        {
            Put( this );
        }

        public bool ReadBool()
        {
            bool value = m_Array[Position] != 0;
            m_Length += 1;

            return value;
        }

        public byte ReadByte()
        {
            byte value = m_Array[Position];
            m_Length += 1;

            return value;
        }

        public byte[] ReadBytes()
        {
            int arraySize = ReadInt32();
            byte[] bytes = new byte[arraySize];
            Array.Copy( m_Array, Position, bytes, 0, arraySize );
            m_Length += arraySize;

            return bytes;
        }

        public int ReadBytes( byte[] targetBuffer, int offset )
        {
            int arraySize = ReadInt32();
            Array.Copy( m_Array, Position, targetBuffer, offset, arraySize );
            m_Length += arraySize;

            return arraySize;
        }

        public char ReadChar()
        {
            return ( char ) ReadUInt16();
        }

        public Color ReadColor()
        {
            Color color = SerializationUtils.DeserializeColor( m_Array, Position, out int length );
            m_Length += length;

            return color;
        }

        public Color ReadColor32()
        {
            Color color = SerializationUtils.DeserializeColor32( m_Array, Position, out int length );
            m_Length += length;

            return color;
        }

        public Color32[] ReadColor32Array()
        {
            int arraySize = ReadInt32();
            Color32[] colors = new Color32[arraySize];

            for ( int i = 0; i < colors.Length; ++i )
            {
                colors[i] = ReadColor32();
            }

            return colors;
        }

        public Color[] ReadColorArray()
        {
            int arraySize = ReadInt32();
            Color[] colors = new Color[arraySize];

            for ( int i = 0; i < colors.Length; ++i )
            {
                colors[i] = ReadColor();
            }

            return colors;
        }

        public DateTime ReadDateTime()
        {
            return DateTime.FromBinary( ReadInt64() );
        }

        public double ReadDouble()
        {
            double value = BitConverter.ToDouble( m_Array, Position );
            m_Length += sizeof( double );

            return value;
        }

        public float ReadFloat()
        {
            float value = BitConverter.ToSingle( m_Array, Position );
            m_Length += sizeof( float );

            return value;
        }

        public float ReadFloat01Fixed16()
        {
            float value = SerializationUtils.DeserializeFloat01Fixed16( m_Array, Position );
            m_Length += 2;

            return value;
        }

        public short ReadInt16()
        {
            short value = SerializationUtils.DeserializeInt16( m_Array, Position );
            m_Length += 2;

            return value;
        }

        public short[] ReadInt16Array()
        {
            int arraySize = ReadInt32();
            short[] shorts = new short[arraySize];

            for ( int i = 0; i < shorts.Length; ++i )
            {
                shorts[i] = ReadInt16();
            }

            return shorts;
        }

        public int ReadInt32()
        {
            int value = SerializationUtils.DeserializeInt32( m_Array, Position );
            m_Length += 4;

            return value;
        }

        public int[] ReadInt32Array()
        {
            int arraySize = ReadInt32();
            int[] ints = new int[arraySize];

            for ( int i = 0; i < ints.Length; ++i )
            {
                ints[i] = ReadInt32();
            }

            return ints;
        }

        public long ReadInt64()
        {
            long value = SerializationUtils.DeserializeInt64( m_Array, Position );
            m_Length += 8;

            return value;
        }

        public Quaternion[] ReadQuaternionArray()
        {
            int arraySize = ReadInt32();
            Quaternion[] quaternions = new Quaternion[arraySize];

            for ( int i = 0; i < quaternions.Length; ++i )
            {
                quaternions[i].x = ReadFloat();
                quaternions[i].y = ReadFloat();
                quaternions[i].z = ReadFloat();
                quaternions[i].w = ReadFloat();
            }

            return quaternions;
        }

        public Quaternion ReadQuaternionFixed16()
        {
            Quaternion quat = SerializationUtils.DeserializeQuaternion(
                m_Array,
                FloatSerializationMethod.Fixed16,
                Position,
                out int length );

            m_Length += length;

            return quat;
        }

        public Quaternion ReadQuaternionFloat32()
        {
            Quaternion quat = SerializationUtils.DeserializeQuaternion(
                m_Array,
                FloatSerializationMethod.Float32,
                Position,
                out int length );

            m_Length += length;

            return quat;
        }

        public sbyte ReadSByte()
        {
            return ( sbyte ) ReadByte();
        }

        public void ReadSerializable( ISerializable serializable )
        {
            m_Length += serializable.Deserialize( m_Array, Position );
        }

        public T[] ReadSerializableArray < T >() where T : ISerializable, new()
        {
            int arraySize = ReadInt32();
            T[] serializables = new T[arraySize];

            for ( int i = 0; i < serializables.Length; ++i )
            {
                serializables[i] = new T();
                ReadSerializable( serializables[i] );
            }

            return serializables;
        }

        public string ReadString()
        {
            string str = SerializationUtils.DeserializeString( m_Array, Position, out int length );
            m_Length += length;

            return str;
        }

        public ushort ReadUInt16()
        {
            return ( ushort ) ReadInt16();
        }

        public uint ReadUInt32()
        {
            return ( uint ) ReadInt32();
        }

        public ulong ReadUInt64()
        {
            return ( ulong ) ReadInt64();
        }

        public Vector2[] ReadVector2Array()
        {
            int arraySize = ReadInt32();
            Vector2[] vectors = new Vector2[arraySize];

            for ( int i = 0; i < vectors.Length; ++i )
            {
                vectors[i].x = ReadFloat();
                vectors[i].y = ReadFloat();
            }

            return vectors;
        }

        public Vector2 ReadVector2Float01Fixed16()
        {
            Vector2 vec = SerializationUtils.DeserializeVector2Float01Fixed16(
                m_Array,
                Position,
                out int length );

            m_Length += length;

            return vec;
        }

        public Vector3 ReadVector3( FloatSerializationMethod method )
        {
            Vector3 vec = SerializationUtils.DeserializeVector3( m_Array, method, Position, out int length );
            m_Length += length;

            return vec;
        }

        public Vector3[] ReadVector3Array()
        {
            int arraySize = ReadInt32();
            Vector3[] vectors = new Vector3[arraySize];

            for ( int i = 0; i < vectors.Length; ++i )
            {
                vectors[i].x = ReadFloat();
                vectors[i].y = ReadFloat();
                vectors[i].z = ReadFloat();
            }

            return vectors;
        }

        // first byte is method
        public Vector3 ReadVector3Auto()
        {
            Vector3 vec = SerializationUtils.DeserializeVector3Auto( m_Array, Position, out int length );
            m_Length += length;

            return vec;
        }

        public void Write( float value )
        {
            m_Length += SerializationUtils.SerializeFloat( value, m_Array, Position );
        }

        public void Write( double value )
        {
            m_Length += SerializationUtils.SerializeDouble( value, m_Array, Position );
        }

        public void Write( Vector3[] vectors )
        {
            Write( vectors?.Length ?? 0 );

            for ( int i = 0; i < ( vectors?.Length ?? 0 ); ++i )
            {
                Write( vectors[i].x );
                Write( vectors[i].y );
                Write( vectors[i].z );
            }
        }

        public void Write( Color[] colors )
        {
            Write( colors.Length );

            for ( int i = 0; i < colors.Length; ++i )
            {
                Write( colors[i] );
            }
        }

        public void Write( Color32[] colors )
        {
            Write( colors.Length );

            for ( int i = 0; i < colors.Length; ++i )
            {
                Write( colors[i] );
            }
        }

        public void Write( Quaternion[] quaternions )
        {
            Write( quaternions.Length );

            for ( int i = 0; i < quaternions.Length; ++i )
            {
                Write( quaternions[i].x );
                Write( quaternions[i].y );
                Write( quaternions[i].z );
                Write( quaternions[i].w );
            }
        }

        public void Write( Vector2[] vectors )
        {
            Write( vectors?.Length ?? 0 );

            for ( int i = 0; i < ( vectors?.Length ?? 0 ); ++i )
            {
                Write( vectors[i].x );
                Write( vectors[i].y );
            }
        }

        public void Write( int[] ints )
        {
            Write( ints?.Length ?? 0 );

            for ( int i = 0; i < ( ints?.Length ?? 0 ); ++i )
            {
                Write( ints[i] );
            }
        }

        public void Write( short[] shorts )
        {
            Write( shorts?.Length ?? 0 );

            for ( int i = 0; i < ( shorts?.Length ?? 0 ); ++i )
            {
                Write( shorts[i] );
            }
        }

        public void Write( byte[] bytes )
        {
            Write( bytes, 0, bytes?.Length ?? 0 );
        }

        public void Write( byte[] bytes, int length )
        {
            Write( bytes, 0, length );
        }

        public void Write( byte[] bytes, int offset, int length )
        {
            Write( length );

            if ( bytes != null )
            {
                Array.Copy( bytes, offset, m_Array, Position, length );
            }

            m_Length += length;
        }

        public void Write( Color color )
        {
            m_Length += SerializationUtils.SerializeColor( color, m_Array, Position );
        }

        public void Write( Color32 color )
        {
            m_Length += SerializationUtils.SerializeColor( color, m_Array, Position );
        }

        public void Write( DateTime dateTime )
        {
            Write( dateTime.ToBinary() );
        }

        public void Write( bool value )
        {
            m_Array[Position] = value ? ( byte ) 1 : ( byte ) 0;
            m_Length += 1;
        }

        public void Write( sbyte value )
        {
            Write( ( byte ) value );
        }

        public void Write( char value )
        {
            Write( ( ushort ) value );
        }

        public void Write( byte value )
        {
            m_Array[Position] = value;
            m_Length += 1;
        }

        public void Write( short value )
        {
            m_Length += SerializationUtils.SerializeInt16( value, m_Array, Position );
        }

        public void Write( ushort value )
        {
            m_Length += SerializationUtils.SerializeInt16( ( short ) value, m_Array, Position );
        }

        public void Write( int value )
        {
            m_Length += SerializationUtils.SerializeInt32( value, m_Array, Position );
        }

        public void Write( long value )
        {
            m_Length += SerializationUtils.SerializeInt64( value, m_Array, Position );
        }

        public void Write( ulong value )
        {
            Write( ( long ) value );
        }

        public void Write( uint value )
        {
            m_Length += SerializationUtils.SerializeInt32( ( int ) value, m_Array, Position );
        }

        public void Write( ISerializable serializable )
        {
            m_Length += serializable.Serialize( m_Array, Position );
        }

        public void Write( string str )
        {
            WriteString( str );
        }

        public void WriteFloat01Fixed16( float value )
        {
            m_Length += SerializationUtils.SerializeFloat01Fixed16( value, m_Array, Position );
        }

        public void WriteQuaternionFixed16( Quaternion quat )
        {
            m_Length += SerializationUtils.SerializeQuaternionFixed16( quat, m_Array, Position );
        }

        public void WriteQuaternionFloat32( Quaternion quat )
        {
            m_Length += SerializationUtils.SerializeQuaternionFloat32( quat, m_Array, Position );
        }

        public void WriteString( string str, int maxLength )
        {
            m_Length += SerializationUtils.SerializeString( str, m_Array, maxLength, Position );
        }

        public void WriteString( string str )
        {
            m_Length += SerializationUtils.SerializeString( str, m_Array, -1, Position );
        }

        public void WriteVector2Float01Fixed16( Vector2 vector )
        {
            m_Length += SerializationUtils.SerializeVector2Float01Fixed16( vector, m_Array, Position );
        }

        public FloatSerializationMethod WriteVector3Auto( Vector3 vector, Vector3? previousVector )
        {
            m_Length += SerializationUtils.SerializeVector3Auto(
                vector,
                previousVector,
                m_Array,
                Position,
                out FloatSerializationMethod method );

            return method;
        }

        public void WriteVector3AutoIncludingMethod( Vector3 vector, Vector3? previousVector )
        {
            m_Length += SerializationUtils.SerializeVector3AutoIncludingMethod(
                vector,
                previousVector,
                m_Array,
                Position );
        }

        public void WriteVector3Fixed16( Vector3 vector )
        {
            m_Length += SerializationUtils.SerializeVector3Fixed16( vector, m_Array, Position );
        }

        public void WriteVector3Float32( Vector3 vector )
        {
            m_Length += SerializationUtils.SerializeVector3Float32( vector, m_Array, Position );
        }

        #endregion

        #region Private

        private Serializer( byte[] buffer, int offset )
        {
            Init( buffer, offset );
        }

        private void Init( byte[] buffer, int offset )
        {
            m_Array = buffer;
            m_Offset = offset;
            m_Length = 0;
        }

        #endregion
    }

}
