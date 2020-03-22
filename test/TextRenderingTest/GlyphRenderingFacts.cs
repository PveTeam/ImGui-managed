﻿using ImGui;
using ImGui.Rendering;
using ImGui.Style;
using ImGui.OSAbstraction.Text;
using ImGui.Development;
using System;
using System.Diagnostics;
using Xunit;
using Typography.OpenFont;
using System.IO;
using System.Linq;
using ImGui.UnitTest;

namespace TextRenderingTest
{
    public class GlyphRenderingFacts
    {
        public GlyphRenderingFacts()
        {
            Application.InitSysDependencies();
            Directory.CreateDirectory(OutputDir);
        }

        private static readonly string OutputDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                + Path.DirectorySeparatorChar + nameof(GlyphRenderingFacts);

        private const string ModelViewerPath = @"C:\Program Files\Autodesk\FBX Review\fbxreview.exe";
        //"E:\Program Files (green)\open3mod_1_1_standalone\open3mod.exe";

        //glyph settings
        string fontFileName = "DroidSans.ttf";
        const char character = 'e';
        const int fontPointSize = 650;

        //drawing settings
        const double polygonLineWidth = 4;
        Color polygonColor = Color.Black;
        const double quadraticLineWidth = 2;
        Color quadraticSegmentColor = Color.Rgb(0, 122, 104);
        Color startPointColor = Color.Argb(200, 0, 104, 43);

        [Fact]
        public void ShowGlyphAsDirectedGraph_Builtin()
        {
            //load a glyph
            Typeface typeFace;
            using (var fs = new FileStream(Utility.FontDir + fontFileName, FileMode.Open))
            {
                var fontReader = new OpenFontReader();
                typeFace = fontReader.Read(fs);
            }
            var glyph = typeFace.Lookup(character);

            //read polygons and curves (quadratic-bezier) from the glyph
            GlyphLoader.Read(glyph, out var polygons, out var quadraticBezierSegments);

            //calculate the baseline origin
            var scale = typeFace.CalculateToPixelScaleFromPointSize(fontPointSize);
            var ascent = typeFace.Ascender * scale;
            var offset = new Vector(0, ascent);

            //create a geometry for polygons
            Geometry polygonGeometry;
            {
                var d = new PathGeometryBuilder();
                d.BeginPath();
                foreach (var polygon in polygons)
                {
                    d.MoveTo(polygon[0] * scale + offset);
                    foreach (var p in polygon.Skip(1))
                    {
                        d.LineTo(p * scale + offset);
                    }
                    d.ClosePath();
                    d.Stroke();
                }
                polygonGeometry = d.ToGeometry();
            }
            var polygonPen = new Pen(Color.Black, 1);

            Geometry quadraticGeometry;
            {
                var d = new PathGeometryBuilder();
                d.BeginPath();
                foreach (var qs in quadraticBezierSegments)
                {
                    d.MoveTo(qs.Item1 * scale + offset);
                    d.QuadraticCurveTo(qs.Item2 * scale + offset, qs.Item3 * scale + offset);
                    d.Stroke();
                }
                quadraticGeometry = d.ToGeometry();
            }
            var quadraticPen = new Pen(Color.Yellow, 1);

            //draw the geometry
            Application.Run(new Form1(() => {
                var g = Form.current.ForegroundDrawingContext;
                g.DrawGeometry(null, polygonPen, polygonGeometry);
                g.DrawGeometry(null, quadraticPen, quadraticGeometry);
            }));
        }

        [Fact]
        public void ShowGlyphAsDirectedGraph_Cairo()
        {
            //load the glyph
            Typeface typeFace;
            using (var fs = Utility.ReadFile(Utility.FontDir + fontFileName))
            {
                var reader = new OpenFontReader();
                typeFace = reader.Read(fs);
            }
            Glyph glyph = typeFace.Lookup(character);

            //read polygons and quadratic bezier segments
            GlyphLoader.Read(glyph, out var polygons, out var quadraticBezierSegments);//TODO unit test GlyphLoader.Read

            //calcualte the aabb
            Rect aabb = new Rect(polygons[0][0], polygons[0][1]);
            for (int i = 0; i < polygons.Count; i++)
            {
                var polygon = polygons[i];
                foreach (var p in polygon)
                {
                    aabb.Union(p);
                }
            }

            foreach (var segment in quadraticBezierSegments)
            {
                aabb.Union(segment.Item1);
                aabb.Union(segment.Item2);
                aabb.Union(segment.Item3);
            }
            aabb.Offset(-20, -20);

            // draw to an image
            using (Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.Argb32,
                MathEx.RoundToInt(2048), MathEx.RoundToInt(2048)))
            using (Cairo.Context g = new Cairo.Context(surface))
            {
                g.Translate(-aabb.Min.X, -aabb.Min.Y);

                //set surface back ground to white (1,1,1,1)
                g.SetSourceColor(CairoEx.ColorWhite);
                g.Paint();

                //polygons
                for (var i = 0; i < polygons.Count; i++)
                {
                    var polygon = polygons[i];
                    var startPoint = polygon[0];
                    g.MoveTo(startPoint.X, startPoint.Y);
                    var lastPoint = startPoint;
                    foreach (var point in polygon)
                    {
                        g.LineTo(point.X, point.Y);
                        DrawArrow(g, lastPoint, point);
                        lastPoint = point;
                    }
                    g.LineTo(startPoint.X, startPoint.Y);
                    DrawArrow(g, lastPoint, startPoint);
                }
                g.LineWidth = polygonLineWidth;
                g.SetSourceColor(polygonColor.ToCairoColor());
                g.Stroke();

                //quadratic bezier segments
                foreach (var segment in quadraticBezierSegments)
                {
                    var p0 = segment.Item1;
                    var c = segment.Item2;
                    var p1 = segment.Item3;
                    g.MoveTo(p0.X, p0.Y);
                    g.QuadraticTo(c.X, c.Y, p1.X, p1.Y);
                }
                g.LineWidth = quadraticLineWidth;
                g.SetSourceColor(quadraticSegmentColor.ToCairoColor());
                g.Stroke();

                //start points
                g.SetSourceColor(startPointColor.ToCairoColor());
                for (var i = 0; i < polygons.Count; i++)
                {
                    var polygon = polygons[i];
                    var startPoint = polygon[0];
                    g.Arc(startPoint.x, startPoint.y, 10, 0, System.Math.PI * 2);
                    g.Fill();
                }

                //show the image of contours
                var path = $"{OutputDir}{Path.DirectorySeparatorChar}{fontFileName}_{character.GetHashCode()}.png";
                surface.WriteToPng(path);
                Util.OpenImage(path);
            }
        }

        private void DrawArrow(Cairo.Context g, Point p0, Point p1)
        {
            var x0 = p0.X;
            var y0 = p0.Y;
            var x1 = p1.X;
            var y1 = p1.Y;

            var dx = x1 - x0;
            var dy = y1 - y0;

            if (MathEx.AmostZero(dx) && MathEx.AmostZero(dy))
            {
                return;
            }

            var n0 = new Vector(-dy, dx); n0.Normalize();
            var n1 = new Vector(dy, -dx); n1.Normalize();

            var B = new Point(x1, y1);
            var d = new Vector(x0 - x1, y0 - y1); d.Normalize();

            var arrowEnd0 = B + 20 * (d + n0);
            var arrowEnd1 = B + 20 * (d + n1);
            g.MoveTo(x1, y1);
            g.LineTo(new Cairo.PointD(arrowEnd0.X, arrowEnd0.Y));
            g.MoveTo(x1, y1);
            g.LineTo(new Cairo.PointD(arrowEnd1.X, arrowEnd1.Y));
            g.MoveTo(x1, y1);
        }
    }
}
