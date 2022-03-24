﻿using System;
using ImGui.OSAbstraction.Graphics;
using System.Collections.Generic;
using ImGui.Style;

namespace ImGui
{
    /// <summary>
    /// The interface for GUI with manual positioning.
    /// </summary>
    public partial class GUI
    {
        internal static GUIContext GetCurrentContext()
        {
            return Application.ImGuiContext;
        }

        internal static Window GetCurrentWindow()
        {
            GUIContext g = Application.ImGuiContext;
            Window window = g.WindowManager.CurrentWindow;
            window.Accessed = true;
            return window;
        }

        #region Constant

        public const GUIState Normal = GUIState.Normal;
        public const GUIState Hover = GUIState.Hover;
        public const GUIState Active = GUIState.Active;

        #endregion

        #region Helper

        public static ITexture CreateTexture(string filePath)
        {
            var texture = Application.PlatformContext.CreateTexture();
            texture.LoadImage(filePath);
            return texture;
        }

        #endregion

        public static void SetSkin(CustomSkin skin)
        {
            GUISkin.Custom = new GUISkin(skin.Rules);
        }

        public static void SetDefaultSkin()
        {
            GUISkin.Custom = null;
        }

        #region SetNextXXX
        public static void SetNextWindowPos(Point pos)
        {
            Application.ImGuiContext.WindowManager.NextWindowData.NextWindowPosition = pos;
            if (GetCurrentWindow().Viewport != Application.MainForm)
            {
                throw new NotSupportedException("SetNextWindowPos in non-MainForm hasn't been implemented.");
            }
        }

        public static void SetNextWindowPadding((int left, int top, int right, int bottom) padding)
        {
            Application.ImGuiContext.WindowManager.NextWindowData.Padding = padding;
        }
        public static void SetNextWindowBorder((int left, int top, int right, int bottom) border)
        {
            Application.ImGuiContext.WindowManager.NextWindowData.Border = border;
        }
        #endregion

    }

}
