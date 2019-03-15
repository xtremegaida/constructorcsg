using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorEngine
{
   public struct Vector3 : IEquatable<Vector3>
   {
      public static readonly Vector3 Zero = new Vector3(0, 0, 0);

      public double X, Y, Z;

      public Vector3(double x, double y, double z) { X = x; Y = y; Z = z; }

      public Vector3 Normalise()
      {
         double f = X * X + Y * Y + Z * Z;
         if (f == 0) { return Zero; }
         f = 1.0 / Math.Sqrt(f);
         return new Vector3(X * f, Y * f, Z * f);
      }

      public Vector3 Add(Vector3 other) { return new Vector3(X + other.X, Y + other.Y, Z + other.Z); }
      public Vector3 Subtract(Vector3 other) { return new Vector3(X - other.X, Y - other.Y, Z - other.Z); }
      public Vector3 Scale(Vector3 other) { return new Vector3(X * other.X, Y * other.Y, Z * other.Z); }
      public Vector3 Scale(double s) { return new Vector3(X * s, Y * s, Z * s); }
      public double GetMagnitudeSquared() { return X * X + Y * Y + Z * Z; }
      public double GetMagnitude() { return Math.Sqrt(X * X + Y * Y + Z * Z); }
      public double DotProduct(Vector3 other) { return X * other.X + Y * other.Y + Z * other.Z; }

      public Vector3 CrossProduct(Vector3 other)
      {
         return new Vector3(Y * other.Z - Z * other.Y, Z * other.X - X * other.Z, X * other.Y - Y * other.X);
      }

      public Vector3 Interpolate(Vector3 other, double l)
      {
         return new Vector3((other.X - X) * l + X, (other.Y - Y) * l + Y, (other.Z - Z) * l + Z);
      }

      public static Vector3 operator +(Vector3 a, Vector3 b) { return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
      public static Vector3 operator -(Vector3 a, Vector3 b) { return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
      public static double operator *(Vector3 a, Vector3 b) { return a.X * b.X + a.Y * b.Y + a.Z * b.Z; }
      public static Vector3 operator *(Vector3 a, double s) { return new Vector3(a.X * s, a.Y * s, a.Z * s); }
      public static Vector3 operator /(Vector3 a, double s) { s = 1.0 / s; return new Vector3(a.X * s, a.Y * s, a.Z * s); }
      public static bool operator ==(Vector3 a, Vector3 b) { return a.X == b.X && a.Y == b.Y && a.Z == b.Z; }
      public static bool operator !=(Vector3 a, Vector3 b) { return a.X != b.X || a.Y != b.Y || a.Z != b.Z; }

      public bool Equals(Vector3 other)
      {
         return other.X == X && other.Y == Y && other.Z == Z;
      }

      public override int GetHashCode()
      {
         return (int)(X * 100 + Y * 800 + Z * 1600);
      }

      public override bool Equals(object obj)
      {
         Vector3 o;
         if (!(obj is Vector3)) { return false; }
         o = (Vector3)obj;
         return o.X == X && o.Y == Y && o.Z == Z;
      }
   }
}
