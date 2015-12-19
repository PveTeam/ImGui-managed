﻿namespace ImGui
{
    /// <summary>
    /// The font weight enumeration describes common values for degree of blackness or thickness of strokes of characters in a font.
    /// Font weight values less than 1 or greater than 999 are considered to be invalid, and they are rejected by font API functions.
    /// </summary>
    public enum FontWeight
    {
        /// <summary>
        /// Predefined font weight : Thin (100).
        /// </summary>
        Thin = 100,

        /// <summary>
        /// Predefined font weight : Extra-light (200).
        /// </summary>
        ExtraLight = 200,

        /// <summary>
        /// Predefined font weight : Ultra-light (200).
        /// </summary>
        UltraLight = 200,

        /// <summary>
        /// Predefined font weight : Light (300).
        /// </summary>
        Light = 300,

        /// <summary>
        /// Predefined font weight : Semi-light (350).
        /// </summary>
        SemiLight = 350,

        /// <summary>
        /// Predefined font weight : Normal (400).
        /// </summary>
        Normal = 400,

        /// <summary>
        /// Predefined font weight : Regular (400).
        /// </summary>
        Regular = 400,

        /// <summary>
        /// Predefined font weight : Medium (500).
        /// </summary>
        Medium = 500,

        /// <summary>
        /// Predefined font weight : Demi-bold (600).
        /// </summary>
        DemiBold = 600,

        /// <summary>
        /// Predefined font weight : Semi-bold (600).
        /// </summary>
        SemiBold = 600,

        /// <summary>
        /// Predefined font weight : Bold (700).
        /// </summary>
        Bold = 700,

        /// <summary>
        /// Predefined font weight : Extra-bold (800).
        /// </summary>
        ExtraBold = 800,

        /// <summary>
        /// Predefined font weight : Ultra-bold (800).
        /// </summary>
        UltraBold = 800,

        /// <summary>
        /// Predefined font weight : Black (900).
        /// </summary>
        Black = 900,

        /// <summary>
        /// Predefined font weight : Heavy (900).
        /// </summary>
        Heavy = 900,

        /// <summary>
        /// Predefined font weight : Extra-black (950).
        /// </summary>
        ExtraBlack = 950,

        /// <summary>
        /// Predefined font weight : Ultra-black (950).
        /// </summary>
        UltraBlack = 950
    }

    /// <summary>
    /// The font stretch enumeration describes relative change from the normal aspect ratio
    /// as specified by a font designer for the glyphs in a font.
    /// Values less than 1 or greater than 9 are considered to be invalid, and they are rejected by font API functions.
    /// </summary>
    public enum FontStretch
    {
        /// <summary>
        /// Predefined font stretch : Not known (0).
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Predefined font stretch : Ultra-condensed (1).
        /// </summary>
        UltraCondensed = 1,

        /// <summary>
        /// Predefined font stretch : Extra-condensed (2).
        /// </summary>
        ExtraCondensed = 2,

        /// <summary>
        /// Predefined font stretch : Condensed (3).
        /// </summary>
        Condensed = 3,

        /// <summary>
        /// Predefined font stretch : Semi-condensed (4).
        /// </summary>
        SemiCondensed = 4,

        /// <summary>
        /// Predefined font stretch : Normal (5).
        /// </summary>
        Normal = 5,

        /// <summary>
        /// Predefined font stretch : Medium (5).
        /// </summary>
        Medium = 5,

        /// <summary>
        /// Predefined font stretch : Semi-expanded (6).
        /// </summary>
        SemiExpanded = 6,

        /// <summary>
        /// Predefined font stretch : Expanded (7).
        /// </summary>
        Expanded = 7,

        /// <summary>
        /// Predefined font stretch : Extra-expanded (8).
        /// </summary>
        ExtraExpanded = 8,

        /// <summary>
        /// Predefined font stretch : Ultra-expanded (9).
        /// </summary>
        UltraExpanded = 9
    };

    /// <summary>
    /// The font style enumeration describes the slope style of a font face, such as Normal, Italic or Oblique.
    /// Values other than the ones defined in the enumeration are considered to be invalid, and they are rejected by font API functions.
    /// </summary>
    public enum FontStyle
    {
        /// <summary>
        /// Font slope style : Normal.
        /// </summary>
        Normal,

        /// <summary>
        /// Font slope style : Oblique.
        /// </summary>
        Oblique,

        /// <summary>
        /// Font slope style : Italic.
        /// </summary>
        Italic
    };

}