/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
* 
*  This software is provided 'as-is', without any express or implied
*  warranty.  In no event will the authors be held liable for any damages
*  arising from the use of this software.
*
*  Permission is granted to anyone to use this software for any purpose,
*  including commercial applications, and to alter it and redistribute it
*  freely, subject to the following restrictions:
*
*  1. The origin of this software must not be misrepresented; you must not
*      claim that you wrote the original software. If you use this software
*      in a product, an acknowledgment in the product documentation would be
*      appreciated but is not required.
*  2. Altered source versions must be plainly marked as such, and must not be
*      misrepresented as being the original software.
*  3. This notice may not be removed or altered from any source distribution. 
*/

using System;
using UnityEngine;

namespace TrueSync
{
    /// <summary>
    ///矢量结构。
    /// </summary>
    [Serializable]
    public struct TSVector3
    {
        private static FP ZeroEpsilonSq = TSMathf.Epsilon;
        internal static TSVector3 InternalZero;
        internal static TSVector3 Arbitrary;

        /// <summary>The X component of the vector.</summary>
        public FP x;
        /// <summary>The Y component of the vector.</summary>
        public FP y;
        /// <summary>The Z component of the vector.</summary>
        public FP z;

        #region Static readonly variables
        /// <summary>
        ///（0，0，0）
        /// </summary>
        public static readonly TSVector3 zero;
        /// <summary>
        ///（-1,0,0）
        /// </summary>
        public static readonly TSVector3 left;
        /// <summary>
        ///（1，0，0）
        /// </summary>
        public static readonly TSVector3 right;
        /// <summary>
        ///（0，1，0）
        /// </summary>
        public static readonly TSVector3 up;
        /// <summary>
        ///（0，-1,0）
        /// </summary>
        public static readonly TSVector3 down;
        /// <summary>
        ///（0，0，-1）
        /// </summary>
        public static readonly TSVector3 back;
        /// <summary>
        ///（0，0，1）
        /// </summary>
        public static readonly TSVector3 forward;
        /// <summary>
        ///（1，1，1）
        /// </summary>
        public static readonly TSVector3 one;
        /// <summary>
        ///有分量的向量
        /// (浮点最小值,浮点最小值,浮点最小值);
        /// </summary>
        public static readonly TSVector3 MinValue;
        /// <summary>
        ///有分量的向量
        /// (浮点最大值,浮点最大值,浮点最大值);
        /// </summary>
        public static readonly TSVector3 MaxValue;
        #endregion

        #region Private static constructor
        static TSVector3()
        {
            one = new TSVector3(1, 1, 1);
            zero = new TSVector3(0, 0, 0);
            left = new TSVector3(-1, 0, 0);
            right = new TSVector3(1, 0, 0);
            up = new TSVector3(0, 1, 0);
            down = new TSVector3(0, -1, 0);
            back = new TSVector3(0, 0, -1);
            forward = new TSVector3(0, 0, 1);
            MinValue = new TSVector3(FP.MinValue);
            MaxValue = new TSVector3(FP.MaxValue);
            Arbitrary = new TSVector3(1, 1, 1);
            InternalZero = zero;
        }
        #endregion

        public static TSVector3 Abs(TSVector3 other)
        {
            return new TSVector3(FP.Abs(other.x), FP.Abs(other.y), FP.Abs(other.z));
        }

        /// <summary>
        ///获取向量的平方长度。
        /// </summary>
        ///<returns>返回
        public FP sqrMagnitude
        {
            get
            {
                return (x * x) + (y * y) + (z * z);
            }
        }

        /// <summary>
        ///获取向量的长度。
        /// </summary>
        ///<returns>
        public FP magnitude
        {
            get
            {
                FP num = (x * x) + (y * y) + (z * z);
                return FP.Sqrt(num);
            }
        }

        public static TSVector3 ClampMagnitude(TSVector3 vector, FP maxLength)
        {
            return Normalize(vector) * maxLength;
        }

        /// <summary>
        ///获取向量的规范化版本。
        /// </summary>
        ///<returns>
        public TSVector3 normalized
        {
            get
            {
                TSVector3 result = new TSVector3(x, y, z);
                result.Normalize();
                return result;
            }
        }

        public static TSVector3 Zero { get { return zero; } }

        public long RawX { get { return x._serializedValue; } set { x._serializedValue = value; } }
        public long RawY { get { return y._serializedValue; } set { y._serializedValue = value; } }
        public long RawZ { get { return z._serializedValue; } set { z._serializedValue = value; } }

        public FP Magnitude { get { return magnitude; } }

        public FP SqrMagnitude { get { return sqrMagnitude; } }

        public TSVector3 Normalized { get { return normalized; } }

        /// <summary>
        ///构造函数初始化结构的新实例
        /// </summary>
        ///<param name="x">
        ///<param name="y">
        ///<param name="z">

        public TSVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public TSVector3(long x, long y, long z)
        {
            this.x._serializedValue = x;// = new FP(x);
            this.y._serializedValue = y;// = new FP(y);
            this.z._serializedValue = z;// = new FP(z);
        }

        public TSVector3(FP x, FP y, FP z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        ///将向量的每个分量乘以所提供向量的相同分量。
        /// </summary>
        public void Scale(TSVector3 other)
        {
            x = x * other.x;
            y = y * other.y;
            z = z * other.z;
        }

        /// <summary>
        ///将所有矢量分量设置为特定值。
        /// </summary>
        ///<param name="x">
        ///<param name="y">
        ///<param name="z">
        public void Set(FP x, FP y, FP z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        ///构造函数初始化结构的新实例
        /// </summary>
        ///xyz</param>
        public TSVector3(FP xyz)
        {
            x = xyz;
            y = xyz;
            z = xyz;
        }

        public static TSVector3 Lerp(TSVector3 from, TSVector3 to, FP percent)
        {
            return from + (to - from) * percent;
        }

        /// <summary>
        ///从JVector第一章
        /// </summary>
        ///<returns>返回
        #region public override string ToString()
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1})", new object[]
            {
                x,
                y,
                z
            });
        }

        // Token: 0x06004917 RID: 18711 RVA: 0x0007E2EC File Offset: 0x0007C4EC
        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2})", new object[]
            {
                x.ToString(format),
                y.ToString(format),
                z.ToString(format)
            });
        }
        #endregion

        /// <summary>
        ///测试对象是否等于此向量。
        /// </summary>
        ///<param name="obj">回报率
        ///<returns>
        #region public override bool Equals(object obj)
        public override bool Equals(object obj)
        {
            if (!(obj is TSVector3)) return false;
            TSVector3 other = (TSVector3)obj;

            return (((x == other.x) && (y == other.y)) && (z == other.z));
        }
        #endregion

        /// <summary>
        ///将向量的每个分量乘以所提供向量的相同分量。
        /// </summary>
        public static TSVector3 Scale(TSVector3 vecA, TSVector3 vecB)
        {
            TSVector3 result;
            result.x = vecA.x * vecB.x;
            result.y = vecA.y * vecB.y;
            result.z = vecA.z * vecB.z;

            return result;
        }

        /// <summary>
        ///试两个JVector retired是componedi等
        /// </summary>
        ///<param name="value1">一级。</param>
        ///<param name="value2">
        ///<returns>
        #region public static bool operator ==(JVector value1, JVector value2)
        public static bool operator ==(TSVector3 value1, TSVector3 value2)
        {
            return (((value1.x == value2.x) && (value1.y == value2.y)) && (value1.z == value2.z));
        }
        #endregion

        /// <summary>
        ///试两个JVector return是不coordinable等
        /// </summary>
        ///<param name="value1">一级。</param>
        ///<param name="value2">
        ///<returns>
        #region public static bool operator !=(JVector value1, JVector value2)
        public static bool operator !=(TSVector3 value1, TSVector3 value2)
        {
            if ((value1.x == value2.x) && (value1.y == value2.y))
            {
                return (value1.z != value2.z);
            }
            return true;
        }
        #endregion

        /// <summary>
        ///x、y z
        /// </summary>
        ///<param name="value1">一级。</param>
        ///<param name="value2">
        ///返回向量的最小值
        #region public static JVector Min(JVector value1, JVector value2)

        public static TSVector3 Min(TSVector3 value1, TSVector3 value2)
        {
            TSVector3.Min(ref value1, ref value2, out TSVector3 result);
            return result;
        }

        /// <summary>
        ///x、y z
        /// </summary>
        ///<param name="value1">一级。</param>
        ///<param name="value2">
        ///<param name="result">
        public static void Min(ref TSVector3 value1, ref TSVector3 value2, out TSVector3 result)
        {
            result.x = (value1.x < value2.x) ? value1.x : value2.x;
            result.y = (value1.y < value2.y) ? value1.y : value2.y;
            result.z = (value1.z < value2.z) ? value1.z : value2.z;
        }
        #endregion

        /// <summary>
        ///x、y、z
        /// </summary>
        ///<param name="value1">一级。</param>
        ///<param name="value2">
        ///返回
        #region public static JVector Max(JVector value1, JVector value2)
        public static TSVector3 Max(TSVector3 value1, TSVector3 value2)
        {
            Max(ref value1, ref value2, out TSVector3 result);
            return result;
        }

        internal static TSVector3 MoveTowards(TSVector3 current, TSVector3 target, FP maxDistanceDelta)
        {
            TSVector3 a = target - current;
            FP magnitude = a.magnitude;
            TSVector3 result;
            if (magnitude <= maxDistanceDelta || magnitude < 1E-45f)
            {
                result = target;
            }
            else
            {
                result = current + a / magnitude * maxDistanceDelta;
            }
            return result;
        }

        public static FP Distance(TSVector3 v1, TSVector3 v2)
        {
            return FP.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z));
        }

        /// <summary>
        ///x、y、z
        /// </summary>
        ///<param name="value1">一级。</param>
        ///<param name="value2">
        ///<param name="result">
        public static void Max(ref TSVector3 value1, ref TSVector3 value2, out TSVector3 result)
        {
            result.x = (value1.x > value2.x) ? value1.x : value2.x;
            result.y = (value1.y > value2.y) ? value1.y : value2.y;
            result.z = (value1.z > value2.z) ? value1.z : value2.z;
        }
        #endregion

        /// <summary>
        ///将向量的长度设置为零。
        /// </summary>
        #region public void MakeZero()
        public void MakeZero()
        {
            x = FP.Zero;
            y = FP.Zero;
            z = FP.Zero;
        }
        #endregion

        /// <summary>
        ///检查矢量的长度是否为零。
        /// </summary>
        ///<returns>
        #region public bool IsZero()
        public bool IsZero()
        {
            return (sqrMagnitude == FP.Zero);
        }

        /// <summary>
        ///检查矢量的长度是否接近零。
        /// </summary>
        ///<returns>
        public bool IsNearlyZero()
        {
            return (sqrMagnitude < ZeroEpsilonSq);
        }
        #endregion

        /// <summary>
        ///用给定的矩阵变换向量。
        /// </summary>
        ///<param name="position">
        ///<param name="matrix">
        ///<returns>返回
        #region public static JVector Transform(JVector position, JMatrix matrix)
        public static TSVector3 Transform(TSVector3 position, TSMatrix3 matrix)
        {
            Transform(ref position, ref matrix, out TSVector3 result);
            return result;
        }

        /// <summary>
        ///用给定的矩阵变换向量。
        /// </summary>
        ///<param name="position">
        ///<param name="matrix">
        ///<param name="result">
        public static void Transform(ref TSVector3 position, ref TSMatrix3 matrix, out TSVector3 result)
        {
            FP num0 = (position.x * matrix.M11) + (position.y * matrix.M21) + (position.z * matrix.M31);
            FP num1 = (position.x * matrix.M12) + (position.y * matrix.M22) + (position.z * matrix.M32);
            FP num2 = (position.x * matrix.M13) + (position.y * matrix.M23) + (position.z * matrix.M33);

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }

        /// <summary>
        ///通过对给定矩阵的转置来变换向量。
        /// </summary>
        ///<param name="position">
        ///<param name="matrix">
        ///<param name="result">
        public static void TransposedTransform(ref TSVector3 position, ref TSMatrix3 matrix, out TSVector3 result)
        {
            FP num0 = ((position.x * matrix.M11) + (position.y * matrix.M12)) + (position.z * matrix.M13);
            FP num1 = ((position.x * matrix.M21) + (position.y * matrix.M22)) + (position.z * matrix.M23);
            FP num2 = ((position.x * matrix.M31) + (position.y * matrix.M32)) + (position.z * matrix.M33);

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion

        /// <summary>
        ///计算两个向量的点积。
        /// </summary>
        ///<param name="vector1">
        ///΋
        ///<returns>
        #region public static FP Dot(JVector vector1, JVector vector2)
        public static FP Dot(TSVector3 vector1, TSVector3 vector2)
        {
            return Dot(ref vector1, ref vector2);
        }


        /// <summary>
        ///计算两个向量的点积。
        /// </summary>
        ///<param name="vector1">
        ///΋
        ///<returns>
        public static FP Dot(ref TSVector3 vector1, ref TSVector3 vector2)
        {
            return (vector1.x * vector2.x) + (vector1.y * vector2.y) + (vector1.z * vector2.z);
        }
        #endregion

        //将一个矢量投影到另一个矢量上。
        public static TSVector3 Project(TSVector3 vector, TSVector3 onNormal)
        {
            FP sqrtMag = Dot(onNormal, onNormal);
            if (sqrtMag < TSMathf.Epsilon)
                return zero;
            else
                return onNormal * Dot(vector, onNormal) / sqrtMag;
        }

        //将向量投影到由与该平面正交的法线定义的平面上。
        public static TSVector3 ProjectOnPlane(TSVector3 vector, TSVector3 planeNormal)
        {
            return vector - Project(vector, planeNormal);
        }


        //返回/从/到/之间的角度（以度为单位）。这总是最小的
        public static FP Angle(TSVector3 from, TSVector3 to)
        {
            return TSMathf.Acos(TSMathf.Clamp(Dot(from.normalized, to.normalized), -FP.ONE, FP.ONE)) * TSMathf.Rad2Deg;
        }

        //第一条，第一百八十条
        //从1到4轴
        //两个矢量之间的测量角度顺时针方向为正，逆时针方向为负。
        public static FP SignedAngle(TSVector3 from, TSVector3 to, TSVector3 axis)
        {
            TSVector3 fromNorm = from.normalized, toNorm = to.normalized;
            FP unsignedAngle = TSMathf.Acos(TSMathf.Clamp(Dot(fromNorm, toNorm), -FP.ONE, FP.ONE)) * TSMathf.Rad2Deg;
            FP sign = TSMathf.Sign(Dot(axis, Cross(fromNorm, toNorm)));
            return unsignedAngle * sign;
        }

        /// <summary>
        ///添加两个向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///返回
        #region public static void Add(JVector value1, JVector value2)
        public static TSVector3 Add(TSVector3 value1, TSVector3 value2)
        {
            Add(ref value1, ref value2, out TSVector3 result);
            return result;
        }

        /// <summary>
        ///添加到向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///<param name="result">
        public static void Add(ref TSVector3 value1, ref TSVector3 value2, out TSVector3 result)
        {
            FP num0 = value1.x + value2.x;
            FP num1 = value1.y + value2.y;
            FP num2 = value1.z + value2.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion

        /// <summary>
        ///将向量除以因子。
        /// </summary>
        ///<param name="value1">
        ///<param name="scaleFactor">
        ///<returns>
        public static TSVector3 Divide(TSVector3 value1, FP scaleFactor)
        {
            Divide(ref value1, scaleFactor, out TSVector3 result);
            return result;
        }

        /// <summary>
        ///将向量除以因子。
        /// </summary>
        ///<param name="value1">
        ///<param name="scaleFactor">
        ///Ň
        public static void Divide(ref TSVector3 value1, FP scaleFactor, out TSVector3 result)
        {
            result.x = value1.x / scaleFactor;
            result.y = value1.y / scaleFactor;
            result.z = value1.z / scaleFactor;
        }

        /// <summary>
        ///减去两个向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///<returns>
        #region public static JVector Subtract(JVector value1, JVector value2)
        public static TSVector3 Subtract(TSVector3 value1, TSVector3 value2)
        {
            Subtract(ref value1, ref value2, out TSVector3 result);
            return result;
        }

        /// <summary>
        ///减去向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///<param name="result">
        public static void Subtract(ref TSVector3 value1, ref TSVector3 value2, out TSVector3 result)
        {
            FP num0 = value1.x - value2.x;
            FP num1 = value1.y - value2.y;
            FP num2 = value1.z - value2.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion

        /// <summary>
        ///两个向量的叉积。
        /// </summary>
        ///<param name="vector1">
        ///΋
        ///<returns>
        #region public static JVector Cross(JVector vector1, JVector vector2)
        public static TSVector3 Cross(TSVector3 vector1, TSVector3 vector2)
        {
            Cross(ref vector1, ref vector2, out TSVector3 result);
            return result;
        }

        /// <summary>
        ///两个向量的叉积。
        /// </summary>
        ///<param name="vector1">
        ///΋
        ///<param name="result">
        public static void Cross(ref TSVector3 vector1, ref TSVector3 vector2, out TSVector3 result)
        {
            FP num3 = (vector1.y * vector2.z) - (vector1.z * vector2.y);
            FP num2 = (vector1.z * vector2.x) - (vector1.x * vector2.z);
            FP num = (vector1.x * vector2.y) - (vector1.y * vector2.x);
            result.x = num3;
            result.y = num2;
            result.z = num;
        }
        #endregion

        /// <summary>
        ///获取向量的哈希代码。
        /// </summary>
        ///<returns>
        #region public override int GetHashCode()
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }
        #endregion

        /// <summary>
        ///反转矢量的方向。
        /// </summary>
        #region public static JVector Negate(JVector value)
        public void Negate()
        {
            x = -x;
            y = -y;
            z = -z;
        }

        /// <summary>
        ///反转向量的方向。
        /// </summary>
        ///<param name="value">
        ///<returns>
        public static TSVector3 Negate(TSVector3 value)
        {
            TSVector3.Negate(ref value, out TSVector3 result);
            return result;
        }

        /// <summary>
        ///反转向量的方向。
        /// </summary>
        ///<param name="value">
        ///<param name="result">
        public static void Negate(ref TSVector3 value, out TSVector3 result)
        {
            FP num0 = -value.x;
            FP num1 = -value.y;
            FP num2 = -value.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion

        /// <summary>
        ///规范化给定向量。
        /// </summary>
        ///<param name="value">
        ///<returns>
        #region public static JVector Normalize(JVector value)
        public static TSVector3 Normalize(TSVector3 value)
        {
            Normalize(ref value, out TSVector3 result);
            return result;
        }

        /// <summary>
        ///规范化此向量。
        /// </summary>
        public void Normalize()
        {
            FP num2 = (x * x) + (y * y) + (z * z);
            FP num = FP.One / FP.Sqrt(num2);
            x *= num;
            y *= num;
            z *= num;
        }

        /// <summary>
        ///规范化给定向量。
        /// </summary>
        ///<param name="value">
        ///<param name="result">
        public static void Normalize(ref TSVector3 value, out TSVector3 result)
        {
            FP num2 = ((value.x * value.x) + (value.y * value.y)) + (value.z * value.z);
            FP num = FP.One / FP.Sqrt(num2);
            result.x = value.x * num;
            result.y = value.y * num;
            result.z = value.z * num;
        }
        #endregion

        #region public static void Swap(ref JVector vector1, ref JVector vector2)

        /// <summary>
        ///交换两个向量的组件。
        /// </summary>
        ///<param name="vector1">
        ///<param name="vector2">
        public static void Swap(ref TSVector3 vector1, ref TSVector3 vector2)
        {
            FP temp;

            temp = vector1.x;
            vector1.x = vector2.x;
            vector2.x = temp;

            temp = vector1.y;
            vector1.y = vector2.y;
            vector2.y = temp;

            temp = vector1.z;
            vector1.z = vector2.z;
            vector2.z = temp;
        }
        #endregion

        /// <summary>
        ///用一个因子乘以一个向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="scaleFactor">
        ///<returns>
        #region public static JVector Multiply(JVector value1, FP scaleFactor)
        public static TSVector3 Multiply(TSVector3 value1, FP scaleFactor)
        {
            Multiply(ref value1, scaleFactor, out TSVector3 result);
            return result;
        }

        /// <summary>
        ///用一个因子乘以一个向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="scaleFactor">
        ///<param name="result">
        public static void Multiply(ref TSVector3 value1, FP scaleFactor, out TSVector3 result)
        {
            result.x = value1.x * scaleFactor;
            result.y = value1.y * scaleFactor;
            result.z = value1.z * scaleFactor;
        }
        #endregion

        /// <summary>
        ///计算两个向量的叉积。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///<returns>
        #region public static JVector operator %(JVector value1, JVector value2)
        public static TSVector3 operator %(TSVector3 value1, TSVector3 value2)
        {
            Cross(ref value1, ref value2, out TSVector3 result);
            return result;
        }
        #endregion

        /// <summary>
        ///计算两个向量的点积。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///返回
        #region public static FP operator *(JVector value1, JVector value2)
        public static FP operator *(TSVector3 value1, TSVector3 value2)
        {
            return Dot(ref value1, ref value2);
        }
        #endregion

        /// <summary>
        ///将矢量乘以比例因子。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///<returns>
        #region public static JVector operator *(JVector value1, FP value2)
        public static TSVector3 operator *(TSVector3 value1, FP value2)
        {
            Multiply(ref value1, value2, out TSVector3 result);
            return result;
        }
        #endregion

        /// <summary>
        ///将矢量乘以比例因子。
        /// </summary>
        ///<param name="value2">
        ///<param name="value1">
        ///<returns>
        #region public static JVector operator *(FP value1, JVector value2)
        public static TSVector3 operator *(FP value1, TSVector3 value2)
        {
            Multiply(ref value2, value1, out TSVector3 result);
            return result;
        }
        #endregion

        /// <summary>
        ///减去两个向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///<returns>
        #region public static JVector operator -(JVector value1, JVector value2)
        public static TSVector3 operator -(TSVector3 value1, TSVector3 value2)
        {
            Subtract(ref value1, ref value2, out TSVector3 result);
            return result;
        }
        #endregion

        /// <summary>
        ///添加两个向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///返回
        #region public static JVector operator +(JVector value1, JVector value2)
        public static TSVector3 operator +(TSVector3 value1, TSVector3 value2)
        {
            Add(ref value1, ref value2, out TSVector3 result);
            return result;
        }
        #endregion

        /// <summary>
        ///将向量除以因子。
        /// </summary>
        ///<param name="value1">
        ///<param name="scaleFactor">
        ///<returns>
        public static TSVector3 operator /(TSVector3 value1, FP value2)
        {
            Divide(ref value1, value2, out TSVector3 result);
            return result;
        }

        public static TSVector3 operator -(TSVector3 value1)
        {
            return new TSVector3(-value1.x, -value1.y, -value1.z);
        }

        public TSVector2 ToTSVector2()
        {
            return new TSVector2(x, y);
        }

        public TSVector4 ToTSVector4()
        {
            return new TSVector4(x, y, z, FP.One);
        }

        public static implicit operator Vector3(TSVector3 v)
        {
            return new Vector3((float)v.x, (float)v.y, (float)v.z);
        }

        public static implicit operator TSVector3(Vector3 v)
        {
            return new TSVector3(v.x, v.y, v.z);
        }

        /// <summary>
        /// 向量乘法
        /// </summary>
        public static TSVector3 Multiply(TSVector3 v1, TSVector3 v2)
        {
            return new TSVector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        public TSVector3 Multiply(TSVector3 other)
        {
            return Multiply(this, other);
        }

        /// <summary>
        /// 向量绝对点积
        /// </summary>
        public static FP AbsDot(TSVector3 v0, TSVector3 v1)
        {
            return FP.Abs(Dot(v0, v1));
        }

        /// <summary>
        /// 下标读写
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public FP this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException("Vector3d index out of range.");
                }
            }
            set
            {
                switch (i)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Vector3d index out of range.");
                }
            }
        }
    }

}
