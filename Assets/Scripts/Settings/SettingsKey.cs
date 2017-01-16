namespace Jammer {

  /// <summary>
  /// Type safe strings used to specify JSON file/folder locations.  Use '.'
  /// for folder slash. The convention is that autosave game data goes in the
  /// data folder, everything else is a machine specific setting (audio, video,
  /// etc) or user initiated snapshots, layouts, recordings that should not be
  /// cloud sync'd.
  /// </summary>
  /// <remarks>
  /// type-safe-enum pattern, allows any characters in the string value
  /// http://stackoverflow.com/questions/424366/c-sharp-string-enums
  /// </remarks>
  public sealed class SettingsKey {

    public int Value { get { return GetValue(); }}
    private readonly int value;

    private readonly string name;

    /// <summary>
    /// Default key not set. Can be used for testing.
    /// </summary>
    public static readonly SettingsKey None = new SettingsKey(0, "none");

    /// <summary>
    /// Application configuration [config]
    /// </summary>
    public static readonly SettingsKey Application = new SettingsKey(100, "application");

    /// <summary>
    /// Audio settings [config]
    /// </summary>
    public static readonly SettingsKey Audio = new SettingsKey(200, "audio");

    /// <summary>
    /// Video settings [config]
    /// </summary>
    public static readonly SettingsKey Video = new SettingsKey(300, "video");

    /// <summary>
    /// Game master file [data]
    /// </summary>
    public static readonly SettingsKey Game = new SettingsKey(400, "data.game");

    /// <summary>
    /// Constructor
    /// </summary>
    private SettingsKey(int value, string name) {
      this.name = name;
      this.value = value;
    }

    private int GetValue() {
      return value;
    }

    /// <summary>
    /// Return a string
    /// </summary>
    public override string ToString() {
      return name;
    }

  }
}
