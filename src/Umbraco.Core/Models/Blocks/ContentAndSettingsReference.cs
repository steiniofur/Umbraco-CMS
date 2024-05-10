// Copyright (c) Umbraco.
// See LICENSE for more details.

namespace Umbraco.Cms.Core.Models.Blocks;

public struct ContentAndSettingsReference : IEquatable<ContentAndSettingsReference>
{
    public ContentAndSettingsReference(Udi contentUdi, Udi? settingsUdi)
    {
        ContentUdi = contentUdi;
        SettingsUdi = settingsUdi;
    }

    public Udi ContentUdi { get; }

    public Udi? SettingsUdi { get; }

    public static bool operator ==(ContentAndSettingsReference left, ContentAndSettingsReference right)
        => left.Equals(right);

    public static bool operator !=(ContentAndSettingsReference left, ContentAndSettingsReference right)
        => !(left == right);

    public override bool Equals(object? obj)
        => obj is ContentAndSettingsReference reference &&
        Equals(reference);

    public bool Equals(ContentAndSettingsReference other)
        => EqualityComparer<Udi>.Default.Equals(ContentUdi, other.ContentUdi) &&
        EqualityComparer<Udi>.Default.Equals(SettingsUdi, other.SettingsUdi);

    public override int GetHashCode()
        => (ContentUdi, SettingsUdi).GetHashCode();
}
