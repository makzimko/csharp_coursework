using System;
using System.IO;
using Gtk;
using System.Collections.Generic;
using pr_coursework;

public partial class MainWindow : Gtk.Window
{
	StreamReader file;
	List<Rectangle> rectangles = new List<Rectangle>();

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		Build();

		OpenFile();
		ParseFile();

		CalculateRectanglesIntersects();
		SortBySquare();

		DrawingArea darea = new DrawingArea();
		darea.ExposeEvent += Draw;

		Add(darea);

		ShowAll();
	}

	// Show dialog for opening file
	protected void OpenFile()
	{
		FileChooserDialog openDialog = new FileChooserDialog(
			"Open file",
			this,
			Gtk.FileChooserAction.Open,
			"Open", ResponseType.Accept
		);
		openDialog.SelectMultiple = false;
		int response = openDialog.Run();
		openDialog.Hide();
		if (response != (int)ResponseType.Accept)
		{
			
			MessageDialog msg = new MessageDialog(
				this,
				DialogFlags.Modal,
				MessageType.Info,
				ButtonsType.Ok,
				"Please select file first"
			);
			msg.Run();
			msg.Destroy();
			OpenFile();
			return;
		}
		this.file = new StreamReader(openDialog.Filename);
	}

	// Parse opened file
	protected void ParseFile()
	{
		String line;
		while ((line = this.file.ReadLine()) != null)
		{
			Point[] points = new Point[4];

			string[] coordsPair = line.Split(";".ToCharArray(), 4);

			for (int i = 0; i < 4; i++)
			{
				string[] coords = coordsPair[i].Split(",".ToCharArray(), 2);
				points[i] = new Point(
					Int32.Parse(coords[0]),
					Int32.Parse(coords[1])
				);
			}
			this.rectangles.Add(new Rectangle(points));
		}

	}

	// Draw grid and rectangles
	void Draw(object sender, ExposeEventArgs args)
	{
		DrawingArea area = (DrawingArea)sender;
		Cairo.Context cr = Gdk.CairoHelper.Create(area.GdkWindow);

		DrawGrid(cr);

		this.rectangles.ForEach((rectangle) => {
			rectangle.Draw(cr, Allocation.Width / 2, Allocation.Height / 2);
		});


		((IDisposable)cr).Dispose();
	}

	// Draw grid
	void DrawGrid(Cairo.Context cr)
	{
		int width = Allocation.Width;
		int height = Allocation.Height;

		cr.LineWidth = 0.5;

		cr.MoveTo(width / 2, height - 10);
		cr.LineTo(width / 2, 10);
		cr.LineTo(width / 2 - 3, 15);
		cr.LineTo(width / 2 + 3, 15);
		cr.LineTo(width / 2, 10);
		cr.Stroke();


		cr.MoveTo(10, height / 2);
		cr.LineTo(width - 10, height / 2);
		cr.LineTo(width - 15, height / 2 - 3);
		cr.LineTo(width - 15, height / 2 + 3);
		cr.LineTo(width - 10, height / 2);
		cr.Stroke();

		cr.Save();
	}

	// Calculate rectangle's intersects and highlight bigger and smaller rectangle
	void CalculateRectanglesIntersects()
	{
		List<Rectangle> intersectsRectangles = new List<Rectangle>();

		for (int i = 0; i < rectangles.Count; i++)
		{
			for (int j = i + 1; j < rectangles.Count; j++)
			{
				bool inter = Rectangle.IsIntersects(rectangles[i], rectangles[j]);
				if (inter == true)
				{
					intersectsRectangles.Add(rectangles[i]);
					intersectsRectangles.Add(rectangles[j]);
				}
			}
		}

		if (intersectsRectangles.Count > 0)
		{
			// sort rectangles by perimeter
			intersectsRectangles.Sort((x, y) =>
			{
				var p1 = x.GetPerimeter();
				var p2 = y.GetPerimeter();
				if (p1 > p2)
				{
					return 1;
				}
				else if (p1 < p2)
				{
					return -1;
				}
				else {
					return 0;
				}
			});

			intersectsRectangles[0].color = new Cairo.Color(255, 0, 0);
			intersectsRectangles[intersectsRectangles.Count - 1].color = new Cairo.Color(0, 0, 255);
		}
	}

	// Sort rectangles by square and save to file
	void SortBySquare()
	{
		MessageDialog msg = new MessageDialog(
			this,
			DialogFlags.Modal,
			MessageType.Question,
			ButtonsType.YesNo,
			"Do you want to save result sorted by square?"
		);
		int response = msg.Run();
		msg.Hide();
		if (response != (int)ResponseType.Yes)
		{
			return;
		}

		rectangles.Sort((x, y) => {
			double s1 = x.GetSquare();
			double s2 = y.GetSquare();

			if (s1 > s2)
			{
				return 1;
			}
			else if (s1 < s2)
			{
				return -1;
			}
			else {
				return 0;
			}
		});

		FileChooserDialog saveDialog = new FileChooserDialog(
			"Save sorted result",
			this,
			FileChooserAction.Save,
			"Save", ResponseType.Accept
		);
		response = saveDialog.Run();
		saveDialog.Hide();

		if (response != (int)ResponseType.Accept)
		{
			return;
		}

		StreamWriter outputStream = new StreamWriter(saveDialog.Filename);
		rectangles.ForEach((rectangle) => {
			List<String> points = rectangle.points.ConvertAll(new Converter<Point, String>((point) => {
				return point.x + "," + point.y;
			}));
			var rectangleString = String.Join(";", points.ToArray());
			outputStream.WriteLine(rectangleString);
		});

		outputStream.Close();
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}
}
