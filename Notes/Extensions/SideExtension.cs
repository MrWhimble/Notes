using System.Windows.Controls;

namespace Notes.Extensions;

public static class SideExtension
{
    public static Dock ToDock(this Side s)
    {
        return (Dock)s;
    }

    public static Side ToSide(this Dock d)
    {
        return (Side)d;
    }
}