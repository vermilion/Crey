namespace Crey.Helpers;

public class NetworkOptions
{
    public bool UseOnlySiteLocalInterfaces { get; set; }
    public string PreferredNetworks { get; set; }
    public string IgnoredInterfaces { get; set; }

    internal IEnumerable<string> GetIgnoredInterfaces()
    {
        if (string.IsNullOrEmpty(IgnoredInterfaces))
            return Array.Empty<string>();

        return IgnoredInterfaces.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    }

    internal IEnumerable<string> GetPreferredNetworks()
    {
        if (string.IsNullOrEmpty(PreferredNetworks))
            return Array.Empty<string>();

        return PreferredNetworks.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    }
}
