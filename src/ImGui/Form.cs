﻿using System;
using System.IO;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Window;
using SixLabors.ImageSharp.Processing;

namespace ImGui
{
    /// <summary>
    /// Represents a window that makes up an application's user interface.
    /// </summary>
    public abstract partial class Form
    {
        private readonly struct ConstructionParameters
        {
            public ConstructionParameters(string title, Point position, Size size, WindowTypes type)
            {
                Title = title;
                Position = position;
                Size = size;
                Type = type;
            }

            public string Title { get; }
            public Point Position { get; }
            public Size Size { get; }
            public WindowTypes Type { get; }
        }

        public static Form current;
        
        private string debugName => constructionParameters.Title;
        
        private readonly ConstructionParameters constructionParameters;

        private IWindow nativeWindow;

        internal IRenderer renderer;

        internal int ID;
        internal int LastFrameActive;
        internal bool PlatformWindowCreated;
        internal Window Window;
        internal string LastName;
        internal Point LastPlatformPos;
        internal Size LastPlatformSize;
        internal Size LastRendererSize;
        internal Point PlatformPos;
        internal Size PlatformSize;
        internal bool PlatformRequestMove;    // Platform window requested move (e.g. window was moved by the OS / host window manager, authoritative position will be OS window position)
        internal bool PlatformRequestResize;  // Platform window requested resize (e.g. window was resized by the OS / host window manager, authoritative size will be OS window size)
        internal bool PlatformRequestClose;   // Platform window requested closure (e.g. window was moved by the OS / host window manager, e.g. pressing ALT-F4)
        internal float Alpha;
        internal float LastAlpha;
        internal ImGuiViewportFlags Flags;// See ImGuiViewportFlags
        internal int LastFrontMostStampCount;  // Last stamp number from when a window hosted by this viewport was made front-most (by comparing this value between two viewport we have an implicit viewport z-order
        
        public Color BackgroundColor { get; set; } = Color.Argb(255, 114, 144, 154);

        /// <summary>
        /// Initializes a new instance of the <see cref="Form"/> class at specific rectangle.
        /// </summary>
        /// <param name="rect">initial rectangle of the form</param>
        protected Form(Rect rect):this(rect.TopLeft, rect.Size)
        {
        }

        internal Form(Rect rect, string title, WindowTypes type)
            : this(rect.TopLeft, rect.Size, title, type)
        {
        }

        internal Form(Point position, Size size, string title = "ImGui Form",
            WindowTypes type = WindowTypes.Regular)
        {
            constructionParameters = new ConstructionParameters(title, position, size, type);
            PlatformWindowCreated = false;
        }

        internal void InitializeForm()
        {
            Profile.Start("Create Window");
            this.nativeWindow = Application.PlatformContext.CreateWindow(
                constructionParameters.Position,
                constructionParameters.Size,
                constructionParameters.Type);
            if (constructionParameters.Type == WindowTypes.Regular)
            {
                this.nativeWindow.Title = constructionParameters.Title;
            }
            PlatformWindowCreated = true;
            Profile.End();
        }

        internal void InitializeRenderer()
        {
            Profile.Start("Initialize Renderer");
            this.renderer = Application.PlatformContext.CreateRenderer();
            this.renderer.Init(this.Pointer, this.nativeWindow.ClientSize);
            Profile.End();

            this.InitializeBackForegroundRenderContext();
        }

        internal void MainLoop(Action loopMethod)
        {
            this.nativeWindow.MainLoop(loopMethod);
        }

        #region window management

        internal bool Closed { get; private set; }

        internal IntPtr Pointer => this.nativeWindow.Pointer;

        internal Size Size
        {
            get => this.nativeWindow.Size;
            set => this.nativeWindow.Size = value;
        }

        internal Point ClientPosition
        {
            get => this.nativeWindow.ClientPosition;
            set => this.nativeWindow.ClientPosition = value;
        }

        internal Size ClientSize
        {
            get => this.nativeWindow.ClientSize;
            set => this.nativeWindow.ClientSize = value;
        }

        internal Point Position
        {
            get => this.nativeWindow.Position;
            set => this.nativeWindow.Position = value;
        }

        internal Rect Rect => new Rect(this.Position, this.Size);

        internal bool Focused => throw new NotImplementedException();

        internal void Show()
        {
            this.nativeWindow.Show();
        }

        internal void Hide()
        {
            this.nativeWindow.Hide();
        }

        internal void Close()
        {
            this.renderer.ShutDown();
            this.nativeWindow.Close();
            this.Closed = true;
        }

        internal void Platform_SetWindowTitle(string title)
        {
            nativeWindow.Title = title;
        }

        #endregion

        internal void Renderer_SetWindowSize(Size size)
        {
            //TODO
            //renderer.OnWindowSizeChanged();
        }

        internal Point ScreenToClient(Point point)
        {
            return this.nativeWindow.ScreenToClient(point);
        }

        internal Point ClientToScreen(Point point)
        {
            return this.nativeWindow.ClientToScreen(point);
        }

        internal void SaveClientAreaToPng(string filePath)
        {
            byte[] data = this.renderer.GetRawBackBuffer(out var width, out var height);
            var image = SixLabors.ImageSharp.Image.LoadPixelData<SixLabors.ImageSharp.PixelFormats.Rgba32>(SixLabors.ImageSharp.Configuration.Default, data, width, height);
            image.Mutate(x => x.Flip(FlipMode.Vertical));
            using (var stream = File.OpenWrite(filePath))
            {
                SixLabors.ImageSharp.ImageExtensions.SaveAsPng(image, stream);
            }
        }

        public void Platform_SetWindowAlpha(float alpha)
        {
            //TODO
            //nativeWindow.SetAlpha(alpha)
        }

        public void Platform_UpdateWindow()
        {
            //TODO
        }

        public void ClearRequestFlags()
        {
            PlatformRequestClose = PlatformRequestMove = PlatformRequestResize = false;
        }

        public bool Platform_GetWindowFocus()
        {
            //TODO
            return Focused;
        }

        public void Platform_RenderWindow(object platformRenderArg)
        {
            throw new NotImplementedException();
        }

        public void Renderer_RenderWindow(object rendererRenderArg)
        {
            throw new NotImplementedException();
        }

        public void Platform_SwapBuffers(object platformRenderArg)
        {
            throw new NotImplementedException();
        }

        public void Renderer_SwapBuffers(object rendererRenderArg)
        {
            throw new NotImplementedException();
        }
    }

    [Flags]
    enum ImGuiViewportFlags
    {
        None                     = 0,
        NoDecoration             = 1 << 0,   // Platform Window: Disable platform decorations: title bar, borders, etc. (generally set all windows, but if ImGuiConfigFlags_ViewportsDecoration is set we only set this on popups/tooltips)
        NoTaskBarIcon            = 1 << 1,   // Platform Window: Disable platform task bar icon (generally set on popups/tooltips, or all windows if ImGuiConfigFlags_ViewportsNoTaskBarIcon is set)
        NoFocusOnAppearing       = 1 << 2,   // Platform Window: Don't take focus when created.
        NoFocusOnClick           = 1 << 3,   // Platform Window: Don't take focus when clicked on.
        NoInputs                 = 1 << 4,   // Platform Window: Make mouse pass through so we can drag this window while peaking behind it.
        NoRendererClear          = 1 << 5,   // Platform Window: Renderer doesn't need to clear the framebuffer ahead (because we will fill it entirely).
        TopMost                  = 1 << 6,   // Platform Window: Display on top (for tooltips only).
        Minimized                = 1 << 7,   // Platform Window: Window is minimized, can skip render. When minimized we tend to avoid using the viewport pos/size for clipping window or testing if they are contained in the viewport.
        NoAutoMerge              = 1 << 8,   // Platform Window: Avoid merging this window into another host window. This can only be set via ImGuiWindowClass viewport flags override (because we need to now ahead if we are going to create a viewport in the first place!).
        CanHostOtherWindows      = 1 << 9    // Main viewport: can host multiple imgui windows (secondary viewports are associated to a single window).
    }

}