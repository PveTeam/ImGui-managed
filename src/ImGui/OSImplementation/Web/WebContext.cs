﻿using ImGui.Input;
using ImGui.OSAbstraction;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;
using ImGui.OSAbstraction.Window;

namespace ImGui.OSImplementation.Web
{
    internal class WebContext : PlatformContext
    {
        public static PlatformContext MapFactory()
        {
            return new WebContext
            {
                CreateTextContext = CTextContext,
                CreateWindow = CWindow,
                ChangeCursor = DoChangeCursor,
                CreateRenderer = CRenderer,
                CreateTexture = CTexture,
            };
        }

        private static ITextContext CTextContext(string text, string fontFamily, double fontSize, TextAlignment alignment)
        {
            return new TypographyTextContext(text, fontFamily, fontSize, alignment);
        }

        private static IWindow CWindow(Point point, Size size, WindowTypes windowType)
        {
            WebWindow window = new WebWindow();
            /*
             * We cannot set the native window's rect for a browser, so point and size is ignored.
             */
            window.Init(Application.WebGLCanvasId);
            return window;
        }

        private static void DoChangeCursor(Cursor cursor)
        {
            WebCursor.ChangeCursor(cursor);
        }

        private static IRenderer CRenderer(IWindow window)
        {
            return new WebGLRenderer();//TODO
        }

        private static ITexture CTexture()
        {
            return new WebGLTexture();
        }
    }
}