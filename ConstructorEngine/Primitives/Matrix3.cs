using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorEngine
{
   public struct Matrix3 : IEquatable<Matrix3>
   {
      public static readonly Matrix3 Zero = new Matrix3(0, 0, 0, 0, 0, 0, 0, 0, 0);
      public static readonly Matrix3 Identity = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);
      
      public Vector3 Right;
      public Vector3 Up;
      public Vector3 Forward;

      public Matrix3(Vector3 right, Vector3 up, Vector3 forward)
      {
         Right = right;
         Up = up;
         Forward = forward;
      }

      public Matrix3(double rX, double rY, double rZ, double uX, double uY, double uZ, double fX, double fY, double fZ)
      {
         Right = new Vector3(rX, rY, rZ);
         Up = new Vector3(uX, uY, uZ);
         Forward = new Vector3(fX, fY, fZ);
      }

      public double GetDeterminant()
      {
         double a = (Up.Y * Forward.Z - Up.Z * Forward.Y) * Right.X;
         double b = (Up.X * Forward.Z - Up.Z * Forward.X) * Right.Y;
         double c = (Up.X * Forward.Y - Up.Y * Forward.X) * Right.Z;
         return a + c - b;
      }

      public Vector3 Transform(Vector3 a)
      {
         return new Vector3(
            a.X * Right.X + a.Y * Right.Y + a.Z * Right.Z,
            a.X * Up.X + a.Y * Up.Y + a.Z * Up.Z,
            a.X * Forward.X + a.Y * Forward.Y + a.Z * Forward.Z
         );
      }

      public Matrix3 Orthonormalise()
      {
         Vector3 f, r, u, v = Right.CrossProduct(Up);
         if (v.DotProduct(Forward) > 0)
         {
            f = Forward + v;
            r = Right + Up.CrossProduct(Forward);
            u = Up + Forward.CrossProduct(Right);
         }
         else
         {
            f = Forward - v;
            r = Right + Forward.CrossProduct(Up);
            u = Up + Right.CrossProduct(Forward);
         }
         return new Matrix3(r.Normalise(), u.Normalise(), f.Normalise());
      }

      public Matrix3 Invert()
      {
         double det = GetDeterminant();
         if (det == 0) { return Matrix3.Identity; }
         Matrix3 t = Transpose();
         det = 1.0 / det;
         return new Matrix3(
            (t.Up.Y * t.Forward.Z - t.Up.Z * t.Forward.Y) * det,
            (t.Up.Z * t.Forward.X - t.Up.X * t.Forward.Z) * det,
            (t.Up.X * t.Forward.Y - t.Up.Y * t.Forward.X) * det,
            (t.Right.Z * t.Forward.Y - t.Right.Y * t.Forward.Z) * det,
            (t.Right.X * t.Forward.Z - t.Right.Z * t.Forward.X) * det,
            (t.Right.Y * t.Forward.X - t.Right.X * t.Forward.Y) * det,
            (t.Right.Y * t.Up.Z - t.Right.Z * t.Up.Y) * det,
            (t.Right.Z * t.Up.X - t.Right.X * t.Up.Z) * det,
            (t.Right.X * t.Up.Y - t.Right.Y * t.Up.X) * det
         );
      }

      public Matrix3 Transpose()
      {
         return new Matrix3(
            Right.X, Up.X, Forward.X,
            Right.Y, Up.Y, Forward.Y,
            Right.Z, Up.Z, Forward.Z
         );
      }

      public Matrix3 Multiply(Matrix3 a)
      {
         Matrix3 t = a.Transpose();
         return new Matrix3(
            t.Transform(Right),
            t.Transform(Up),
            t.Transform(Forward)
         );
      }

      public Matrix3 RotateRelX(double a) { return RotateX(a).Multiply(this); }
      public Matrix3 RotateRelY(double a) { return RotateY(a).Multiply(this); }
      public Matrix3 RotateRelZ(double a) { return RotateZ(a).Multiply(this); }
      public Matrix3 RotateAbsX(double a) { return Multiply(RotateX(a)); }
      public Matrix3 RotateAbsY(double a) { return Multiply(RotateY(a)); }
      public Matrix3 RotateAbsZ(double a) { return Multiply(RotateZ(a)); }

      public static Matrix3 Scale(double s) { return new Matrix3(s, 0, 0, 0, s, 0, 0, 0, s); }
      public static Matrix3 Scale(Vector3 s) { return new Matrix3(s.X, 0, 0, 0, s.Y, 0, 0, 0, s.Z); }
      public static Matrix3 RotateX(double a) { double s = Math.Sin(a), c = Math.Cos(a); return new Matrix3(1, 0, 0, 0, c, -s, 0, s, c); }
      public static Matrix3 RotateY(double a) { double s = Math.Sin(a), c = Math.Cos(a); return new Matrix3(c, 0, s, 0, 1, 0, -s, 0, c); }
      public static Matrix3 RotateZ(double a) { double s = Math.Sin(a), c = Math.Cos(a); return new Matrix3(c, -s, 0, s, c, 0, 0, 0, 1); }

      public static Matrix3 operator *(Matrix3 a, Matrix3 b) { return a.Multiply(b); }
      public static Matrix3 operator *(Matrix3 a, double s) { return new Matrix3(a.Right * s, a.Up * s, a.Forward * s); }
      public static Vector3 operator *(Matrix3 x, Vector3 a) { return x.Transform(a); }
      public static Vector3 operator *(Vector3 a, Matrix3 x) { return x.Transform(a); }
      public static bool operator ==(Matrix3 a, Matrix3 b) { return a.Equals(b); }
      public static bool operator !=(Matrix3 a, Matrix3 b) { return !a.Equals(b); }

      public bool Equals(Matrix3 other)
      {
         return (Forward.Equals(other.Forward) &&
                 Right.Equals(other.Right) &&
                 Up.Equals(other.Up));
      }

      public override int GetHashCode()
      {
         return (int)(Forward.GetHashCode() +
                      Right.GetHashCode() * 1000 +
                      Up.GetHashCode() * 1000000);
      }

      public override bool Equals(object obj)
      {
         Matrix3 o;
         if (!(obj is Matrix3)) { return false; }
         o = (Matrix3)obj;
         return Forward.Equals(o.Forward) &&
                Right.Equals(o.Right) &&
                Up.Equals(o.Up);
      }
   }
}
