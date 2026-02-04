namespace WellMind.Models;

public enum TonePack
{
    Grounding,
    Focus,
    Recovery,
    Confidence
}

public static class TonePackExtensions
{
    public static string ToStorageValue(this TonePack tonePack)
    {
        return tonePack.ToString().ToLowerInvariant();
    }

    public static string ToDisplayName(this TonePack tonePack)
    {
        return tonePack switch
        {
            TonePack.Grounding => "Grounding",
            TonePack.Focus => "Focus",
            TonePack.Recovery => "Recovery",
            TonePack.Confidence => "Confidence",
            _ => "Grounding"
        };
    }

    public static bool TryParse(string? value, out TonePack tonePack)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            tonePack = TonePack.Grounding;
            return false;
        }

        return Enum.TryParse(value, ignoreCase: true, out tonePack);
    }
}
