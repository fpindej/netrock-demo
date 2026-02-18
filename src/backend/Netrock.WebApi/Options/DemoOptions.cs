namespace Netrock.WebApi.Options;

/// <summary>
/// Configuration options for demo mode features.
/// </summary>
public class DemoOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Demo";

    /// <summary>
    /// Gets or sets a value indicating whether demo mode is enabled.
    /// When disabled, demo endpoints return 404.
    /// </summary>
    public bool Enabled { get; set; }
}
