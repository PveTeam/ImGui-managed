﻿using ImGui.Rendering;
using System;
using System.Collections.Generic;
using System.Reflection;
using ImGui.OSAbstraction.Text;
using ImGui.Style;
using static ImGui.GUILayout;

namespace ImGui.Development
{
    public class Metrics
    {
        public static long VertexNumber {get; internal set; }
        public static long IndexNumber {get; internal set; }
        public static int RenderWindows { get; set; }
        public static int ActiveWindows { get; set; }

        public const string WindowName = "Hello ImGui Metrics";

        public static void ShowWindow(ref bool windowOpened)
        {
            if(!GUI.Begin(WindowName, ref windowOpened))
            {
                GUI.End();
                return;
            }
            
            // Basic info
            GUIContext g = GetCurrentContext();
            Text("Hello ImGui {0}##Metrics.Version", GetVersion());
            Text("Application average {0:F3} ms/frame ({1:F1} FPS)##Metrics.FPS", 1000.0f / g.fps, g.fps);
            Text("{0} vertices, {1} indices ({2} triangles)##Metrics.VI",
                VertexNumber, IndexNumber, IndexNumber / 3);
            Text("{0} active windows ({0} visible)##Metrics.Window", ActiveWindows, RenderWindows);
            Separator("separator0");

            //Windows
            var windowManager = g.WindowManager;
            NodeWindows(windowManager.Windows.ToArray(), "Windows");

            //Internal state
            if (TreeNode("Internal state"))
            {
                var w = g.WindowManager;
                Text("HoveredWindow: '{0}'",
                    w.HoveredWindow != null ? w.HoveredWindow.Name : "null");
                Text("HoveredRootWindow: '{0}'",
                    w.HoveredRootWindow!= null ? w.HoveredRootWindow.Name : "null");
                // Data is "in-flight" so depending on when the Metrics window is called we may see
                // current frame information or not
                Text("HoveredId: {0:X8}/{1:X8}, AllowOverlap: {2}",
                    g.HoverId, g.HoveredIdPreviousFrame, g.HoverIdAllowOverlap);
                Text("ActiveId: {0:X8}/{1:X8}, AllowOverlap: {2}",
                    g.ActiveId, g.ActiveIdPreviousFrame, g.ActiveIdAllowOverlap);
                Text("ActiveIdWindow: '{0}'",
                    w.ActiveIdWindow != null ? w.ActiveIdWindow.Name : "null");
                Text("MovedWindow: '{0}'", w.MovedWindow != null ? w.MovedWindow.Name : "null");
                TreePop();
            }

            if (TreeNode("Tools"))
            {
                // The Item Picker tool is super useful to visually select an item and break into the call-stack of where it was submitted.
                if (Button("Item Picker.."))
                {
                    g.DebugStartItemPicker();
                }

                if (g.DebugItemPickerBreakID != 0)
                {
                    lastPickedItemId = g.DebugItemPickerBreakID;
                }

                if(lastPickedItemId != 0)
                {
                    //get picking node
                    Node pickedNode = null;
                    var w = g.WindowManager.Windows;
                    foreach (var window in w)
                    {
                        var node = window.RenderTree.GetNodeById(lastPickedItemId);
                        if (node != null)
                        {
                            pickedNode = node;
                            break;
                        }
                    }
                    
                    DoBoxModel(pickedNode);
                }

                TreePop();
            }

            GUI.End();
        }
        
        private static string GetVersion()
        {
            var entryAssembly = Assembly.GetCallingAssembly();
            var informationalVersion =
                entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
            return informationalVersion;
        }

        private static void NodeWindows(IEnumerable<Window> windows, string label)
        {
            if (!TreeNode(label))
            {
                return;
            }
            foreach (var window in windows)
            {
                NodeWindow(window, "Window");
            }
            TreePop();
        }

        private static void NodeWindow(Window window, string label)
        {
            if(window == null)
            {
                BulletText("Window: null");
                return;
            }

            if(TreeNode($"{(window.Collapsed ? "[ ]" : "[x]")}" +
                         $"Window '{window.Name}', " +
                         $"{(window.Active || window.WasActive ? 1 : 0)}"))
            {
                NodeMeshBuffer(window, "MeshBuffer");
                BulletText(
                    "Pos: ({0:F1},{1:F1}), Size: ({2:F1},{3:F1}), ContentSize ({4:F1},{5:F1})",
                    window.Position.X, window.Position.Y, window.Size.Width, window.Size.Height,
                    window.ContentRect.Width, window.ContentRect.Height);
                BulletText("Scroll: ({0:F2},{1:F2})",
                    window.ClientAreaNode.ScrollOffset.X, window.ClientAreaNode.ScrollOffset.Y);
                BulletText("Active: {0}/{1}, ", window.Active, window.WasActive, window.Accessed);
                if (window.RootWindow != window)
                {
                    NodeWindow(window.RootWindow, "RootWindow");
                }
                BulletText("Storage: {0} entries, ~{1} bytes", window.StateStorage.EntryCount,
                    window.StateStorage.EstimatedDataSizeInBytes);
                TreePop();
            }
        }

        private static void NodeMeshBuffer(Window nodeWindow, string label)
        {
            var buffer = nodeWindow.MeshBuffer;
            var vertexCount = buffer.ImageMesh.VertexBuffer.Count
                              + buffer.ShapeMesh.VertexBuffer.Count
                              + buffer.TextMesh.VertexBuffer.Count;
            var indexCount = buffer.ImageMesh.IndexBuffer.Count
                             + buffer.ShapeMesh.IndexBuffer.Count
                             + buffer.TextMesh.IndexBuffer.Count;
            var cmdCount = buffer.ImageMesh.CommandBuffer.Count
                +buffer.ShapeMesh.CommandBuffer.Count
                +buffer.TextMesh.Commands.Count;

            bool nodeOpen = TreeNode(buffer,
                $"{label}: '{buffer.OwnerName}'" +
                $" {vertexCount} vtx, {indexCount} indices, {cmdCount} cmds");

            if (nodeOpen)
            {
                if (buffer == GetCurrentWindow().MeshBuffer)
                {
                    Text("CURRENTLY APPENDING"); // Can't display stats for active draw list! (we don't have the data double-buffered)
                }
                else
                {
                    NodeShapeMesh(buffer);
                    NodeImageMesh(buffer);
                    NodeTextMesh(buffer);
                }
                TreePop();
            }

            if(IsItemHovered())
            {
                var g = GetCurrentContext();
                g.ForegroundDrawingContext.DrawRectangle(
                    null, new Pen(Color.Yellow, 1), nodeWindow.Rect);
            }
        }

        private static void NodeShapeMesh(MeshBuffer buffer)
        {
            var mesh = buffer.ShapeMesh;
            var cmds = mesh.CommandBuffer;
            for (var i = 0; i < cmds.Count; i++)
            {
                NodeDrawCommand(mesh, i);
            }
        }

        private static void NodeImageMesh(MeshBuffer buffer)
        {
            var mesh = buffer.ShapeMesh;
            var cmds = mesh.CommandBuffer;
            for (var i = 0; i < cmds.Count; i++)
            {
                NodeDrawCommand(mesh, i);
            }
        }

        private static void NodeTextMesh(MeshBuffer buffer)
        {
            
        }

        private static void NodeDrawCommand(Mesh mesh, int cmdIndex)
        {
            DrawCommand cmd = mesh.CommandBuffer[cmdIndex];
            if (cmd.ElemCount == 0)
            {
                return;
            }
            var tex = cmd.TextureData;
            var texId = 0;
            if (tex != null)
            {
                texId = tex.GetNativeTextureId();
            }
            var clipRect = cmd.ClipRect;
            var minX = clipRect.Min.X;
            var minY = clipRect.Min.Y;
            var maxX = clipRect.Max.X;
            var maxY = clipRect.Max.Y;
            if (TreeNode($"Draw {cmd.ElemCount/3,4} triangles," +
                         $" tex 0x{texId:X8}," +
                         $" clip_rect ({minX,7:.0},{minY,7:.0})-({maxX,7:.0},{maxY,7:.0})"))
            {
                Text($"Mesh: ElemCount: {cmd.ElemCount}, ElemCount/3: {cmd.ElemCount/3}");
                if (IsItemHovered())
                {
                    var pen = new Pen(Color.Yellow, 1);
                    var g = GetCurrentContext();
                    for (int i = 0; i < cmd.ElemCount; i+=3)
                    {
                        var v0 = mesh.VertexBuffer[i].pos;
                        var v1 = mesh.VertexBuffer[i + 1].pos;
                        var v2 = mesh.VertexBuffer[i + 2].pos;
                        g.ForegroundDrawingContext.DrawLine(pen, v0, v1);
                        g.ForegroundDrawingContext.DrawLine(pen, v1, v2);
                        g.ForegroundDrawingContext.DrawLine(pen, v2, v0);
                    }
                }
                var indexBuffer = mesh.IndexBuffer;
                var triangles_pos = new Point[3];
                for (int elemIndex = 0, idx_i = 0; elemIndex < cmd.ElemCount; elemIndex++)
                {
                    string str = "";
                    for (int n = 0; n < 3; n++, idx_i++)
                    {
                        var vtx_i = indexBuffer[idx_i];
                        DrawVertex v = mesh.VertexBuffer[vtx_i];
                        triangles_pos[n] = v.pos;
                        str += string.Format(
                            "{0} {1:0000}: pos ({2,8:F2},{3,8:F2}), uv ({4:F6}, {5:F6}), col {6}\n",
                            (n == 0) ? "elem" : "    ", idx_i, v.pos.x, v.pos.y, v.uv.x, v.uv.y, v.color);
                    }
                    str = str.TrimEnd('\n');
                    Selectable(str, false);
                    if (IsItemHovered())
                    {
                        var pen = new Pen(Color.Yellow, 1);
                        var g = GetCurrentContext();
                        g.ForegroundDrawingContext.DrawLine(pen, triangles_pos[0], triangles_pos[1]);
                        g.ForegroundDrawingContext.DrawLine(pen, triangles_pos[1], triangles_pos[2]);
                        g.ForegroundDrawingContext.DrawLine(pen, triangles_pos[2], triangles_pos[0]);
                    }
                }
                TreePop();
            }
        }

        private static void DoBoxModel(Node targetNode)
        {
            if (targetNode == null)
            {
                return;
            }

            var window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            //get or create the root node
            var id = window.GetID(targetNode);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            if (node == null)
            {
                //create node
                node = new Node(id, "MetricsBoxModel");
                node.UseBoxModel = true;
                var size = new Size(155, 200);
                node.AttachLayoutEntry(size);
                node.RuleSet.HorizontalStretchFactor = 1;
            }
            container.AppendChild(node);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);

            // interact
            GUIBehavior.ButtonBehavior(node.Rect, node.Id, out var hovered, out var held);
            node.State = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            
            // last item state
            window.TempData.LastItemState = node.State;

            // draw
            using (var dc = node.RenderOpen())
            {
                var center = node.Rect.Center;

                PathGeometryBuilder builder = new PathGeometryBuilder();

                var targetRuleSet = targetNode.RuleSet;
                var contentSize = node.ContentSize;
                var padding = targetRuleSet.Padding;
                var border = targetRuleSet.Border;

                // content box
                var contentLabel = $"{contentSize.Width} x {contentSize.Height}";
                var contentBoxSize = labelRuleSet.CalcContentBoxSize(contentLabel, GUIState.Normal);
                contentBoxSize.Width = Math.Max(88, contentBoxSize.Width);
                var contentRect = Rect.FromCenterSize(center, contentBoxSize);
                builder.Rect(contentRect, true);
                builder.Stroke();
                dc.DrawRectangle(null, new Pen(Color.Red, 1), contentRect);
                dc.DrawBoxModel(contentLabel, centeredLabelRuleSet, contentRect);


                //padding box
                var paddingLeft = 
                    labelRuleSet.CalcContentBoxSize(padding.left.ToString(), GUIState.Normal).Width
                    +5;
                paddingLeft = Math.Max(paddingLeft, 18);
                var paddingRight = 
                    labelRuleSet.CalcContentBoxSize(padding.right.ToString(), GUIState.Normal).Width
                    +5;
                var paddingTop = 23;
                var paddingBottom = 23;
                var paddingBoxSize = contentBoxSize +
                    new Vector(paddingLeft + paddingRight, paddingTop + paddingBottom);
                paddingBoxSize.Width = Math.Max(122, paddingBoxSize.Width);
                var paddingBoxRect = Rect.FromCenterSize(center, paddingBoxSize);
                builder.Rect(paddingBoxRect, true);
                builder.Stroke();
                dc.DrawGlyphRun(labelRuleSet, "padding", paddingBoxRect.TopLeft + new Vector(1, 1));


                //border box
                var borderLeft = 
                    labelRuleSet.CalcContentBoxSize(border.left.ToString(), GUIState.Normal).Width
                    +5;
                borderLeft = Math.Max(borderLeft, 18);
                var borderRight = 
                    labelRuleSet.CalcContentBoxSize(border.right.ToString(), GUIState.Normal).Width
                    +5;
                borderRight = Math.Max(borderRight, 18);
                var borderTop = 23;
                var borderBottom = 23;
                var boderBoxSize = paddingBoxSize
                    + new Vector(borderLeft + borderRight, borderTop + borderBottom);
                boderBoxSize.Width = Math.Max(156, boderBoxSize.Width);
                var borderBoxRect = Rect.FromCenterSize(center, boderBoxSize);
                builder.Rect(borderBoxRect, true);
                builder.Stroke();

                dc.DrawGlyphRun(labelRuleSet, "border", borderBoxRect.TopLeft + new Vector(1, 1));
                
                var geometry = builder.ToGeometry();
                dc.DrawGeometry(null, new Pen(Color.Black, 1), geometry);
            }
        }

        static Metrics()
        {
            labelRuleSet = new StyleRuleSet();
            labelRuleSet.Padding = (1, 1, 1, 1);
            labelRuleSet.AlignmentHorizontal = Alignment.Start;
            labelRuleSet.AlignmentVertical = Alignment.Center;
            
            centeredLabelRuleSet = new StyleRuleSet();
            centeredLabelRuleSet.Padding = (1, 1, 1, 1);
            centeredLabelRuleSet.AlignmentHorizontal = Alignment.Center;
            centeredLabelRuleSet.AlignmentVertical = Alignment.Center;
        }

        private static StyleRuleSet labelRuleSet;
        private static StyleRuleSet centeredLabelRuleSet;
        private static int lastPickedItemId = -1;

    }
}
