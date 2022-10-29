using System.Diagnostics;

namespace ImGui
{
    [DebuggerDisplay("{Index}")]
    public struct DrawIndex
    {
        public int Index { get; set; }

        public static implicit operator int(DrawIndex v)
        {
            return v.Index;
        }
    }
}