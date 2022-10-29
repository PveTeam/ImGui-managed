using System;
using ImGui.Input;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;
using ImGui.OSAbstraction.Window;

namespace ImGui.OSAbstraction
{
    public delegate ITextContext CTextContext(string text, string fontFamily, double fontSize, TextAlignment alignment);

    public abstract class PlatformContext
    {
        public CTextContext CreateTextContext;
        public Func<Point, Size, WindowTypes, IWindow> CreateWindow;
        public Action<Cursor> ChangeCursor;
        public Func<IWindow, IRenderer> CreateRenderer;
        public Func<ITexture> CreateTexture;
    }
}