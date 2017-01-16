using System;
namespace pr_coursework
{
	public class Line
	{
		Point a, b;

		public Line(Point a, Point b)
		{
			this.a = a;
			this.b = b;
		}

		// Get length of the line
		public double GetLength()
		{
			double length = Math.Sqrt(
				Math.Pow(this.a.x - this.b.x, 2) +
				Math.Pow(this.a.y - this.b.y, 2)
			);
			return length;
		}

		// Get inclination angle of the line
		public double GetTheta()
		{
			double theta = Math.Atan2(
				this.a.y - this.b.y,
				this.a.x - this.b.x
			);
			return theta;
		}

		// Get angle between two lines for square
		public static double GetAngleBetweenLines(Line l1, Line l2)
		{
			double theta1 = l1.GetTheta();
			double theta2 = l2.GetTheta();

			double diff = Math.Abs(theta1 - theta2);
			double angle = Math.Min(diff, Math.Abs(180 - diff));

			return angle;
		}

		// Check if two lines intersects
		public static bool IsIntersects(Line l1, Line l2)
		{
			int v1 = (l2.b.x - l2.a.x) * (l1.a.y - l2.a.y) - (l2.b.y - l2.a.y) * (l1.a.x - l2.a.x);
			int v2 = (l2.b.x - l2.a.x) * (l1.b.y - l2.a.y) - (l2.b.y - l2.a.y) * (l1.b.x - l2.a.x);
			int v3 = (l1.b.x - l1.a.x) * (l2.a.y - l1.a.y) - (l1.b.y - l1.a.y) * (l2.a.x - l1.a.x);
			int v4 = (l1.b.x - l1.a.x) * (l2.b.y - l1.a.y) - (l1.b.y - l1.a.y) * (l2.b.x - l1.a.x);
			return (v1 * v2 < 0) && (v3 * v4 < 0);
		}
	}
}
