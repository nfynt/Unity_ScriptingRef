using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace nfynt
{

    public static class SerializationUtils
    {
        [StructLayout( LayoutKind.Explicit )]
        private struct DoubleHelper
        {
            [FieldOffset( 0 )]
            public double Double;

            [FieldOffset( 0 )]
            public long Int64;

            // allow individual access to bytes - a fixed size buffer would actually be
            // more elegant here, but requires unsafe code, which we want to avoid if
            // not necessary
            [FieldOffset( 0 )]
            public byte Byte0;

            [FieldOffset( 1 )]
            public byte Byte1;

            [FieldOffset( 2 )]
            public byte Byte2;

            [FieldOffset( 3 )]
            public byte Byte3;

            [FieldOffset( 4 )]
            public byte Byte4;

            [FieldOffset( 5 )]
            public byte Byte5;

            [FieldOffset( 6 )]
            public byte Byte6;

            [FieldOffset( 7 )]
            public byte Byte7;
        }

        [StructLayout( LayoutKind.Explicit )]
        private struct FloatHelper
        {
            [FieldOffset( 0 )]
            public float Float;

            [FieldOffset( 0 )]
            public int Int32;

            [FieldOffset( 0 )]
            public byte Byte0;

            [FieldOffset( 1 )]
            public byte Byte1;

            [FieldOffset( 2 )]
            public byte Byte2;

            [FieldOffset( 3 )]
            public byte Byte3;
        }

        #region Public

        public static byte[] CreateBufferAndSerialize(
            int maxBufferSize,
            Func < byte[], int, int > serializationFunction,
            out int actualLength )
        {
            byte[] buffer = new byte[maxBufferSize];
            actualLength = serializationFunction( buffer, 0 );

            return buffer;
        }

        public static byte[] CreateExactBufferAndSerialize(
            int maxBufferSize,
            Func < byte[], int, int > serializationFunction )
        {
            byte[] buffer = new byte[maxBufferSize];
            int length = serializationFunction( buffer, 0 );

            if ( buffer.Length == length )
            {
                return buffer;
            }

            byte[] shortBuffer = new byte[length];
            Array.Copy( buffer, shortBuffer, length );

            return shortBuffer;
        }

        public static Color DeserializeColor( byte[] buffer, int offset = 0 )
        {
            const float OneDiv255 = 1f / 255f;

            return new Color(
                buffer[offset] * OneDiv255,
                buffer[offset + 1] * OneDiv255,
                buffer[offset + 2] * OneDiv255,
                buffer[offset + 3] * OneDiv255 );
        }

        public static Color DeserializeColor( byte[] buffer, int offset, out int length )
        {
            const float OneDiv255 = 1f / 255f;
            length = 4;

            return new Color(
                buffer[offset] * OneDiv255,
                buffer[offset + 1] * OneDiv255,
                buffer[offset + 2] * OneDiv255,
                buffer[offset + 3] * OneDiv255 );
        }

        public static Color DeserializeColor32( byte[] buffer, int offset, out int length )
        {
            length = 4;

            return new Color32( buffer[offset], buffer[offset + 1], buffer[offset + 2], buffer[offset + 3] );
        }

        public static Color DeserializeColor32( byte[] buffer, int offset = 0 )
        {
            return new Color32( buffer[offset], buffer[offset + 1], buffer[offset + 2], buffer[offset + 3] );
        }

        public static double DeserializeDouble( byte[] buffer, int offset = 0 )
        {
            return BitConverter.ToDouble( buffer, offset );
        }

        public static float DeserializeFloat( byte[] buffer, int offset = 0 )
        {
            return BitConverter.ToSingle( buffer, offset );
        }

        public static float DeserializeFloat01Fixed16( byte[] buffer, int offset = 0 )
        {
            ushort value = ( ushort ) ( ( buffer[offset + 0] << 8 ) + buffer[offset + 1] );

            return value / 65535f;
        }

        public static float DeserializeFloat01Fixed16( byte[] buffer, int offset, out int length )
        {
            length = 2;
            ushort value = ( ushort ) ( ( buffer[offset + 0] << 8 ) + buffer[offset + 1] );

            return value / 65535f;
        }

        public static short DeserializeInt16( byte[] buffer, int offset = 0 )
        {
            // always big endian (network order)!
            ushort numberUnsigned =
                ( ushort ) (
                    ( buffer[offset + 0] << 8 ) +
                    buffer[offset + 1] );

            return ( short ) numberUnsigned;
        }

        public static int DeserializeInt32( byte[] buffer, int offset = 0 )
        {
            // always big endian (network order)!
            uint numberUnsigned =
                ( uint ) (
                    ( buffer[offset + 0] << 24 ) +
                    ( buffer[offset + 1] << 16 ) +
                    ( buffer[offset + 2] << 8 ) +
                    buffer[offset + 3] );

            return ( int ) numberUnsigned;
        }

        public static long DeserializeInt64( byte[] buffer, int offset = 0 )
        {
            // always big endian (network order)!
            ulong numberUnsigned =
                ( ulong ) (
                    ( ( ulong ) buffer[offset + 0] << 56 ) +
                    ( ( ulong ) buffer[offset + 1] << 48 ) +
                    ( ( ulong ) buffer[offset + 2] << 40 ) +
                    ( ( ulong ) buffer[offset + 3] << 32 ) +
                    ( ( ulong ) buffer[offset + 4] << 24 ) +
                    ( ( ulong ) buffer[offset + 5] << 16 ) +
                    ( ( ulong ) buffer[offset + 6] << 8 ) +
                    ( ulong ) buffer[offset + 7] );

            return ( long ) numberUnsigned;
        }

        public static Quaternion DeserializeQuaternion(
            byte[] buffer,
            FloatSerializationMethod method,
            int offset,
            out int length )
        {
            switch ( method )
            {
                case FloatSerializationMethod.Float32:
                {
                    length = 3 * sizeof( float ) + 1;
                    bool wPositive = buffer[offset] != 0;
                    float x = BitConverter.ToSingle( buffer, offset + 1 );
                    float y = BitConverter.ToSingle( buffer, offset + 1 + sizeof( float ) );
                    float z = BitConverter.ToSingle( buffer, offset + 1 + 2 * sizeof( float ) );
                    float w = Mathf.Sqrt( Mathf.Max( 0f, 1f - x * x - y * y - z * z ) );

                    if ( !wPositive )
                    {
                        w *= -1f;
                    }

                    return new Quaternion( x, y, z, w );
                }

                case FloatSerializationMethod.Fixed16:
                {
                    length = 3 * sizeof( short );
                    short firstShort = DeserializeInt16( buffer, offset );

                    // last bit of first 16bit integer is sign of w (1 = negative)
                    bool wPositive = firstShort % 2 == 0;
                    firstShort /= 2;
                    float x = ( float ) firstShort / 16383f;
                    float y = ( float ) DeserializeInt16( buffer, offset + sizeof( short ) ) / 32767f;
                    float z = ( float ) DeserializeInt16( buffer, offset + 2 * sizeof( short ) ) / 32767f;
                    float w = Mathf.Sqrt( Mathf.Max( 0f, 1f - x * x - y * y - z * z ) );

                    if ( !wPositive )
                    {
                        w *= -1f;
                    }

                    return new Quaternion( x, y, z, w );
                }

                default:
                    throw new NotImplementedException( "Not implemented" );
            }
        }

        public static string DeserializeString( byte[] buffer, int offset, out int serializedLength )
        {
            int byteCount = DeserializeInt32( buffer, offset );
            serializedLength = byteCount + 4;

            return Encoding.UTF8.GetString( buffer, offset + 4, byteCount );
        }

        public static string DeserializeString( byte[] buffer, int offset )
        {
            return DeserializeString( buffer, offset, out int _ );
        }

        public static Vector2 DeserializeVector2Float01Fixed16( byte[] buffer, int offset, out int length )
        {
            length = 4;

            return new Vector2(
                DeserializeFloat01Fixed16( buffer, offset ),
                DeserializeFloat01Fixed16( buffer, offset + 2 ) );
        }

        public static Vector2 DeserializeVector2Float01Fixed16( byte[] buffer, int offset = 0 )
        {
            return new Vector2(
                DeserializeFloat01Fixed16( buffer, offset ),
                DeserializeFloat01Fixed16( buffer, offset + 2 ) );
        }

        public static Vector3 DeserializeVector3(
            byte[] buffer,
            FloatSerializationMethod method,
            int offset,
            out int length )
        {
            switch ( method )
            {
                case FloatSerializationMethod.Float32:
                    length = 3 * sizeof( float );

                    return new Vector3(
                        BitConverter.ToSingle( buffer, offset ),
                        BitConverter.ToSingle( buffer, offset + sizeof( float ) ),
                        BitConverter.ToSingle( buffer, offset + 2 * sizeof( float ) )
                    );

                case FloatSerializationMethod.Fixed16:
                    length = 3 * sizeof( short );

                    return new Vector3(
                        ( float ) DeserializeInt16( buffer, offset ) * 0.001f,
                        ( float ) DeserializeInt16( buffer, offset + sizeof( short ) ) * 0.001f,
                        ( float ) DeserializeInt16( buffer, offset + 2 * sizeof( short ) ) * 0.001f );

                default:
                    throw new NotImplementedException( "Not implemented" );
            }
        }

        // first byte is method
        public static Vector3 DeserializeVector3Auto( byte[] buffer, int offset, out int length )
        {
            FloatSerializationMethod method = ( FloatSerializationMethod ) buffer[offset];

            Vector3 result = DeserializeVector3( buffer, method, offset + 1, out length );
            length += 1;

            return result;
        }

        public static long Double2LongBitwise( double value )
        {
            DoubleHelper helper = new DoubleHelper() { Double = value };

            return helper.Int64;
        }

        public static int Float2IntBitwise( float value )
        {
            FloatHelper helper = new FloatHelper() { Float = value };

            return helper.Int32;
        }

        public static int GetQuaternionSerializedLength( FloatSerializationMethod method )
        {
            switch ( method )
            {
                case FloatSerializationMethod.Float32:
                    return 3 * sizeof( float ) + 1;

                case FloatSerializationMethod.Fixed16:
                    return 3 * sizeof( short );
            }

            return 0;
        }

        public static int GetSerializedStringLength( string str )
        {
            return 4 + Encoding.UTF8.GetByteCount( str );
        }

        public static int GetVector3SerializedLength( FloatSerializationMethod method )
        {
            switch ( method )
            {
                case FloatSerializationMethod.Float32:
                    return 12;

                case FloatSerializationMethod.Fixed16:
                    return 6;

                case FloatSerializationMethod.DifferentialFixed8:
                    return 3;
            }

            return 0;
        }

        public static float Int2FloatBitwise( int value )
        {
            FloatHelper helper = new FloatHelper() { Int32 = value };

            return helper.Float;
        }

        public static double Long2DoubleBitwise( long value )
        {
            DoubleHelper helper = new DoubleHelper() { Int64 = value };

            return helper.Double;
        }

        public static int SerializeColor( Color color, byte[] buffer, int offset = 0 )
        {
            buffer[offset + 0] = ( byte ) Mathf.Round( color.r * 255f );
            buffer[offset + 1] = ( byte ) Mathf.Round( color.g * 255f );
            buffer[offset + 2] = ( byte ) Mathf.Round( color.b * 255f );
            buffer[offset + 3] = ( byte ) Mathf.Round( color.a * 255f );

            return 4;
        }

        public static int SerializeColor( Color32 color, byte[] buffer, int offset = 0 )
        {
            buffer[offset + 0] = color.r;
            buffer[offset + 1] = color.g;
            buffer[offset + 2] = color.b;
            buffer[offset + 3] = color.a;

            return 4;
        }

        public static int SerializeDouble( double value, byte[] buffer, int offset = 0 )
        {
            DoubleHelper helper = new DoubleHelper() { Double = value };
            buffer[offset + 0] = helper.Byte0;
            buffer[offset + 1] = helper.Byte1;
            buffer[offset + 2] = helper.Byte2;
            buffer[offset + 3] = helper.Byte3;
            buffer[offset + 4] = helper.Byte4;
            buffer[offset + 5] = helper.Byte5;
            buffer[offset + 6] = helper.Byte6;
            buffer[offset + 7] = helper.Byte7;

            return 8;
        }

        public static int SerializeFloat( float value, byte[] buffer, int offset = 0 )
        {
            FloatHelper helper = new FloatHelper() { Float = value };
            buffer[offset + 0] = helper.Byte0;
            buffer[offset + 1] = helper.Byte1;
            buffer[offset + 2] = helper.Byte2;
            buffer[offset + 3] = helper.Byte3;

            return 4;
        }

        public static int SerializeFloat01Fixed16( float value, byte[] buffer, int offset = 0 )
        {
            ushort valueUshort = ( ushort ) Mathf.RoundToInt( value * 65535f );
            buffer[offset + 0] = ( byte ) ( valueUshort >> 8 );
            buffer[offset + 1] = ( byte ) valueUshort;

            return 2;
        }

        public static int SerializeInt16( short value, byte[] buffer, int offset = 0 )
        {
            // always big endian (network order)!
            ushort numberUnsigned = ( ushort ) value;
            buffer[offset + 0] = ( byte ) ( numberUnsigned >> 8 );
            buffer[offset + 1] = ( byte ) numberUnsigned;

            return 2;
        }

        public static byte[] SerializeInt16( short value )
        {
            byte[] buffer = new byte[2];
            SerializeInt16( value, buffer );

            return buffer;
        }

        public static int SerializeInt32( int value, byte[] buffer, int offset = 0 )
        {
            // always big endian (network order)!
            uint numberUnsigned = ( uint ) value;
            buffer[offset + 0] = ( byte ) ( numberUnsigned >> 24 );
            buffer[offset + 1] = ( byte ) ( numberUnsigned >> 16 );
            buffer[offset + 2] = ( byte ) ( numberUnsigned >> 8 );
            buffer[offset + 3] = ( byte ) numberUnsigned;

            return 4;
        }

        public static byte[] SerializeInt32( int value )
        {
            byte[] buffer = new byte[4];
            SerializeInt32( value, buffer );

            return buffer;
        }

        public static int SerializeInt64( long value, byte[] buffer, int offset = 0 )
        {
            // always big endian (network order)!
            ulong numberUnsigned = ( ulong ) value;
            buffer[offset + 0] = ( byte ) ( numberUnsigned >> 56 );
            buffer[offset + 1] = ( byte ) ( numberUnsigned >> 48 );
            buffer[offset + 2] = ( byte ) ( numberUnsigned >> 40 );
            buffer[offset + 3] = ( byte ) ( numberUnsigned >> 32 );
            buffer[offset + 4] = ( byte ) ( numberUnsigned >> 24 );
            buffer[offset + 5] = ( byte ) ( numberUnsigned >> 16 );
            buffer[offset + 6] = ( byte ) ( numberUnsigned >> 8 );
            buffer[offset + 7] = ( byte ) numberUnsigned;

            return 8;
        }

        public static int SerializeQuaternionFixed16( Quaternion quat, byte[] buffer, int offset = 0 )
        {
            // note: assumes that the quaternion is normalized

            short v = ( short ) Mathf.RoundToInt( quat.x * 16383 );

            // v is shifted left (x2), last bit is used for sign (1 = negative)
            if ( quat.w < 0f )
            {
                v = ( short ) ( v * 2 + 1 );
            }
            else
            {
                v *= 2;
            }

            SerializeInt16( v, buffer, offset );
            v = ( short ) Mathf.RoundToInt( quat.y * 32767 );
            SerializeInt16( v, buffer, offset + sizeof( short ) );
            v = ( short ) Mathf.RoundToInt( quat.z * 32767 );
            SerializeInt16( v, buffer, offset + 2 * sizeof( short ) );

            return 6;
        }

        public static byte[] SerializeQuaternionFixed16( Quaternion quat )
        {
            byte[] buffer = new byte[6];
            SerializeQuaternionFixed16( quat, buffer );

            return buffer;
        }

        public static int SerializeQuaternionFloat32( Quaternion quat, byte[] buffer, int offset = 0 )
        {
            buffer[offset] = quat.w > 0f ? ( byte ) 1 : ( byte ) 0;

            SerializeFloat( quat.x, buffer, offset + 1 );
            SerializeFloat( quat.y, buffer, offset + 1 + sizeof( float ) );
            SerializeFloat( quat.z, buffer, offset + 1 + 2 * sizeof( float ) );

            return 3 * sizeof( float ) + 1;
        }

        public static byte[] SerializeQuaternionFloat32( Quaternion quat )
        {
            byte[] buffer = new byte[3 * sizeof( float ) + 1];
            SerializeQuaternionFloat32( quat, buffer );

            return buffer;
        }

        public static int SerializeString( string str, byte[] buffer, int maxLength, int offset = 0 )
        {
            int byteCount = Encoding.UTF8.GetByteCount( str );

            if ( maxLength < 0 || byteCount <= maxLength - 4 )
            {
                SerializeInt32( byteCount, buffer, offset );

                return Encoding.UTF8.GetBytes( str, 0, str.Length, buffer, offset + 4 ) + 4;
            }
            else
            {
                byte[] bufferTemp = Encoding.UTF8.GetBytes( str );
                SerializeInt32( byteCount, buffer, offset );
                Array.Copy( bufferTemp, 0, buffer, offset + 4, maxLength - 4 );

                return maxLength;
            }
        }

        public static byte[] SerializeString( string str )
        {
            int byteCount = Encoding.UTF8.GetByteCount( str );
            byte[] buffer = new byte[byteCount + 4];
            SerializeInt32( byteCount, buffer, 0 );
            Encoding.UTF8.GetBytes( str, 0, str.Length, buffer, 4 );

            return buffer;
        }

        public static int SerializeVector2Float01Fixed16( Vector2 vector, byte[] buffer, int offset = 0 )
        {
            int length = SerializeFloat01Fixed16( vector.x, buffer, offset );
            length += SerializeFloat01Fixed16( vector.y, buffer, offset + length );

            return length;
        }

        public static int SerializeVector3Auto(
            Vector3 vector,
            Vector3? previousVector,
            byte[] buffer,
            int offset,
            out FloatSerializationMethod method )
        {
            if ( CanStoreVectorAsFixed16( vector ) )
            {
                method = FloatSerializationMethod.Fixed16;

                return SerializeVector3Fixed16( vector, buffer, offset );
            }

            method = FloatSerializationMethod.Float32;

            return SerializeVector3Float32( vector, buffer, offset );
        }

        public static byte[] SerializeVector3Auto(
            Vector3 vector,
            Vector3? previousVector,
            out FloatSerializationMethod method )
        {
            if ( CanStoreVectorAsFixed16( vector ) )
            {
                method = FloatSerializationMethod.Fixed16;

                return SerializeVector3Fixed16( vector );
            }

            method = FloatSerializationMethod.Float32;

            return SerializeVector3Float32( vector );
        }

        public static int SerializeVector3AutoIncludingMethod(
            Vector3 vector,
            Vector3? previousVector,
            byte[] buffer,
            int offset )
        {
            int length = SerializeVector3Auto(
                vector,
                previousVector,
                buffer,
                offset + 1,
                out FloatSerializationMethod method );

            buffer[offset] = ( byte ) method;

            return length + 1;
        }

        public static int SerializeVector3Fixed16( Vector3 vector, byte[] buffer, int offset = 0 )
        {
            short v = ( short ) Mathf.RoundToInt( Mathf.Clamp( vector.x, -32.767f, 32.767f ) * 1000f );
            SerializeInt16( v, buffer, offset );
            v = ( short ) Mathf.RoundToInt( Mathf.Clamp( vector.y, -32.767f, 32.767f ) * 1000f );
            SerializeInt16( v, buffer, offset + sizeof( short ) );
            v = ( short ) Mathf.RoundToInt( Mathf.Clamp( vector.z, -32.767f, 32.767f ) * 1000f );
            SerializeInt16( v, buffer, offset + 2 * sizeof( short ) );

            return 3 * sizeof( short );
        }

        public static byte[] SerializeVector3Fixed16( Vector3 vector )
        {
            byte[] buffer = new byte[3 * sizeof( short )];
            SerializeVector3Fixed16( vector, buffer );

            return buffer;
        }

        public static byte[] SerializeVector3Float32( Vector3 vector )
        {
            byte[] buffer = new byte[3 * sizeof( float )];
            SerializeVector3Float32( vector, buffer );

            return buffer;
        }

        public static int SerializeVector3Float32( Vector3 vector, byte[] buffer, int offset = 0 )
        {
            SerializeFloat( vector.x, buffer, offset );
            SerializeFloat( vector.y, buffer, offset + sizeof( float ) );
            SerializeFloat( vector.z, buffer, offset + 2 * sizeof( float ) );

            return 3 * sizeof( float );
        }

        #endregion

        #region Private

        private static bool CanStoreVectorAsFixed16( Vector3 vector )
        {
            return Mathf.Abs( vector.x ) <= 32.767f &&
                   Mathf.Abs( vector.y ) <= 32.767f &&
                   Mathf.Abs( vector.z ) <= 32.767f;
        }

        #endregion
    }

}
