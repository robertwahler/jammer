namespace Jammer {

  /// <summary>
  /// Game state used to implement a simple FSM
  /// </summary>
  public enum GameState {

    /// <summary>
    /// Initiate a new game. All games are new until they are intialized.
    /// </summary>
    New = 1000,

    /// <summary>
    /// Deserializing and setup
    /// </summary>
    Loading = 2000,

    /// <summary>
    /// Startup sequence finished, happens only once per session
    /// </summary>
    Started = 3000,

    /// <summary>
    /// Ready for user input
    /// </summary>
    Ready = 4000,
  }
}
