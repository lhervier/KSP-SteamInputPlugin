using com.github.lhervier.ksp;

namespace com.github.lhervier.ksp.ui.model
{
    public readonly struct UISection
    {
        public readonly string GroupId;
        public readonly bool Modeshift;

        public UISection(string groupId, bool modeshift)
        {
            GroupId = groupId;
            Modeshift = modeshift;
        }
    }
}