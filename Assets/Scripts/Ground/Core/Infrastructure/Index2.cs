using System;

namespace Game.Infrastructure
{
	public readonly struct Index2 : IEquatable<Index2>
	{
		public readonly int x;
		public readonly int y;

		public int R => (int) Math.Sqrt(R2);
		public int R2 => x * x + y * y;

		public Index2(short x, short y)
		{
			this.x = x;
			this.y = y;
		}

		public Index2(int x, int y)
		{
			this.x = Math.Min(x, short.MaxValue);
			this.y = Math.Min(y, short.MaxValue);
		}

		public override bool Equals(object obj)
		{
			return obj is Index2 other && Equals(other);
		}

		public static bool operator ==(Index2 a, Index2 b)
		{
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Index2 a, Index2 b)
		{
			return !(a == b);
		}

		public static Index2 operator +(Index2 a, Index2 b) => new Index2(a.x + b.x, a.y + b.y);
		public static Index2 operator -(Index2 a, Index2 b) => new Index2(a.x - b.x, a.y - b.y);

		public override string ToString() => $"({x.ToString()}, {y.ToString()} R{R.ToString()})";

		public bool Equals(Index2 other)
		{
			return x == other.x && y == other.y;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (x * 397) ^ y;
			}
		}
	}
}