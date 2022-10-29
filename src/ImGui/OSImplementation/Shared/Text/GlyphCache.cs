﻿using System.Collections.Generic;
using ImGui.Internal;

namespace ImGui
{
    public class GlyphData
    {
        public char Character;
        public string FontFamily;
        public List<List<Point>> Polygons;
        public List<(Point, Point, Point)> QuadraticCurveSegments;

        public GlyphData(char character, string fontFamily,
            List<List<Point>> polygons,
            List<(Point, Point, Point)> quadraticCurveSegments)
        {
            this.Character = character;
            this.FontFamily = fontFamily;
            this.Polygons = polygons;
            this.QuadraticCurveSegments = quadraticCurveSegments;
        }
    }

    internal class GlyphCache
    {
        public static GlyphCache Default { get; } = new GlyphCache();

        private MemoryCache cache = new MemoryCache();

        private int CalcKey(char character, string fontFamily)
        {
            int hash = 17;
            hash = hash * 23 + character.GetHashCode();
            hash = hash * 23 + fontFamily.GetHashCode();
            //TODO consider fontStyle and fontWeight when Typography is ready.
            return hash;
        }

        public GlyphData AddGlyph(char character, string fontFamily,
            List<List<Point>> polygons,
            List<(Point, Point, Point)> quadraticCurveSegments)
        {
            GlyphData glyph = new GlyphData(character, fontFamily, polygons, quadraticCurveSegments);

            int key = CalcKey(character, fontFamily);

            cache.Set(key, glyph);

            return glyph;
        }

        public GlyphData GetGlyph(char character, string fontFamily)
        {
            int key = CalcKey(character, fontFamily);
            return cache.Get<GlyphData>(key);
        }
    }
}
