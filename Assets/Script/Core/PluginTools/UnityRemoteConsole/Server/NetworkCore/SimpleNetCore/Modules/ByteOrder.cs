using UnityEngine;
using System.Collections;

namespace SimpleNetCore
{
    public enum ByteOrder:byte
    {
        /// <summary>
        ///  Constant denoting big-endian byte order.  In this order, the bytes of a multibyte value are ordered from most significant to least significant.
        /// </summary>
        BIG_ENDIAN=0,
        /// <summary>
        /// Constant denoting little-endian byte order.  In this order, the bytes of a multibyte value are ordered from least significant to most significant.
        /// </summary>
        LITTLE_ENDIAN=1,
    }
}

