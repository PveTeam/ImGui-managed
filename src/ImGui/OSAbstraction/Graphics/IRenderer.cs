﻿using System;
using ImGui.OSAbstraction.Window;

namespace ImGui.OSAbstraction.Graphics
{
    /// <summary>
    /// Renderer-related functions
    /// </summary>
    /// <remarks>Renderers should implement this.</remarks>
    public interface IRenderer
    {
        /// <summary>
        /// Initialize the renderer
        /// </summary>
        /// <param name="windowHandle">window handle, this could be some context info needed by the renderer. e.g. win32 HWND</param>
        /// <param name="size">size of default framebuffer</param>
        void Init(IntPtr windowHandle, Size size);

        /// <summary>
        /// Initialize the renderer
        /// </summary>
        /// <param name="windowHandle">window handle, this could be some context info needed by the renderer. e.g. WebGL context as a JSObject</param>
        /// <param name="size">size of default framebuffer</param>
        void Init(object windowHandle, Size size)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Set the window on which the renderer will draw
        /// </summary>
        void SetRenderingWindow(IWindow window)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the window on which the renderer will draw
        /// </summary>
        /// <returns></returns>
        IWindow GetRenderingWindow()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clear the front buffer
        /// </summary>
        void Clear(Color color);

        /// <summary>
        /// Draw meshes
        /// </summary>
        /// <param name="width">viewport width</param>
        /// <param name="height">viewport height</param>
        /// <param name="meshes">meshes</param>
        void DrawMeshes(int width, int height, (Mesh shapeMesh, Mesh imageMesh, TextMesh textMesh) meshes);

        /// <summary>
        /// swap front(what is on the screen) and back(what is rendered by the renderer) buffer
        /// </summary>
        void SwapBuffers();

        /// <summary>
        /// shut down the renderer
        /// </summary>
        void ShutDown();

        /// <summary>
        /// Get back buffer data in R8G8B8A8 format.
        /// </summary>
        byte[] GetRawBackBuffer(out int width, out int height);

        /// <summary>
        /// Called when render target size is changed
        /// </summary>
        /// <param name="size"></param>
        void OnSizeChanged(Size size) => throw new NotImplementedException();
    }
}
