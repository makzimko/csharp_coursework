using System;
using System.Collections.Generic;

namespace pr_coursework
{
	public class Rectangle
	{
		public List<Point> points = new List<Point>();
		public List<Line> lines = new List<Line>();
		public Cairo.Color color = new Cairo.Color(0, 0, 0);


		public Rectangle(params Point[] points)
		{
			foreach (Point point in points)
			{
				this.points.Add(point);
			}

			CalculateSideLines();
		}

		// Calculate side lines of rectangle
		void CalculateSideLines()
		{
			for (int i = 0; i < 4; i++)
			{
				Point p1 = points[i];
				Point p2 = points[i < points.Count - 1 ? i + 1 : 0];
				lines.Add(new Line(p1, p2));
			}
		}

		// Get perimeter of the rectangle
		public double GetPerimeter()
		{
			double perimeter = 0;
			lines.ForEach((line) => perimeter += line.GetLength());
			return perimeter;
		}

		// Get square of the rectangle
		public double GetSquare()
		{
			Line diagonal1 = new Line(points[0], points[2]);
			Line diagonal2 = new Line(points[1], points[3]);

			double angle = Line.GetAngleBetweenLines(diagonal1, diagonal2);
			double square = (diagonal1.GetLength() * diagonal2.GetLength() * Math.Sin(angle)) / 2;

			return square;
		}

		// Draw 
		public void Draw(Cairo.Context cr, int offsetX = 0, int offsetY = 0)
		{
			cr.LineWidth = 1;
			cr.SetSourceColor(this.color);

			cr.MoveTo(
				this.points[3].x + offsetX,
				offsetY - points[3].y
			);

			this.points.ForEach((point) => {
				cr.LineTo(
					point.x + offsetX,
					offsetY - point.y
				);
			});
			cr.Stroke();
			cr.Save();
		}

		// Check if two rectangles intersects
		public static bool IsIntersects(Rectangle r1, Rectangle r2)
		{
			bool intersects = false;

			r1.lines.ForEach((r1Line) => {
				r2.lines.ForEach((r2Line) => {
					bool inter = Line.IsIntersects(r1Line, r2Line);
					if (inter == true)
					{
						intersects = true;
					}
				});
			});

			return intersects;
		}
	}
}
