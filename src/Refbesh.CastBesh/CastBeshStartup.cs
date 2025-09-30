using Refbesh.CastBesh.Configuration;

namespace Refbesh.CastBesh;

/// <summary>
/// Entry point for configuring CastBesh in your application.
/// </summary>
public static class CastBeshStartup
{
    /// <summary>
    /// Initializes CastBesh configuration.
    /// </summary>
    public static CastConfiguration Configure()
    {
        return new CastConfiguration();
    }

    /// <summary>
    /// Configures CastBesh with a configuration action.
    /// </summary>
    public static void Configure(Action<CastConfiguration> configureAction)
    {
        ArgumentNullException.ThrowIfNull(configureAction);

        var config = new CastConfiguration();
        configureAction(config);
    }
}