namespace SDD {

  /// <summary>
  /// Log levels
  /// </summary>
  [System.Flags]
  public enum LogLevels {

    /// <summary>
    /// Verbose
    /// </summary>
    Verbose = 1 << 0,

    /// <summary>
    /// Debug, normal unity debug level
    /// </summary>
    Debug = 1 << 1,

    /// <summary>
    /// Info, user informational messages
    /// </summary>
    Info = 1 << 2,

    /// <summary>
    /// Warning, normal unity error level
    /// </summary>
    Warning = 1 << 3,

    /// <summary>
    /// Error, normal unity error level
    /// </summary>
    Error = 1 << 4,
  }
}
