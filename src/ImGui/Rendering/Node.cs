﻿using System;
using System.Collections.Generic;
using ImGui.Layout;
using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    internal class Node
    {
        public int id { get; set; }
        public string str_id { get; set; }

        #region hierarchy
        public Node Parent { get; set; }
        public List<Node> Children { get; set; }
        #endregion

        public Rect Rect { get; set; }
        public GUIStyle Style { get; set; }

        private LayoutGroup Group { get; set; }
        private LayoutEntry Entry { get; set; }

        private static readonly ObjectPool<LayoutEntry> EntryPool = new ObjectPool<LayoutEntry>(1024);
        private static readonly ObjectPool<LayoutGroup> GroupPool = new ObjectPool<LayoutGroup>(1024);

        /// <summary>
        /// Make this node a group
        /// </summary>
        public void AttachLayoutGroup(bool isVertical, LayoutOptions? options = null)
        {
            var group = GroupPool.Get();
            group.Init(this.id, isVertical, options);
            group.StrId = this.str_id;
            this.Group = group;
            if(this.Entry != null)
            {
                EntryPool.Put(this.Entry);
            }
        }

        /// <summary>
        /// Make this node an entry
        /// </summary>
        public void AttachLayoutEntry(Size contentSize, LayoutOptions options)
        {
            var entry = EntryPool.Get();
            entry.Init(id, contentSize, options);
            entry.StrId = str_id;
            this.Entry = entry;
            if (this.Group != null)
            {
                GroupPool.Put(this.Group);
            }
        }

        /// <summary>
        /// Layout the sub-tree rooted at this node
        /// </summary>
        public void Layout()
        {
            this.Group.CalcWidth(this.Group.ContentWidth);
            this.Group.CalcHeight(this.Group.ContentHeight);
            this.Group.SetX(this.Rect.X);
            this.Group.SetY(this.Rect.Y);
        }
    }
}
