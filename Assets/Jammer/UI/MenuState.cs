namespace Jammer.UI {

  /// <summary>
  /// Menu state
  /// </summary>
  public enum MenuState {

    /// <summary>
    /// Menus are closed
    /// </summary>
    Closed = 0,

    /// <summary>
    /// Menus are open
    /// </summary>
    Open = 100,

    /// <summary>
    /// Menus are transitioning open
    /// </summary>
    Opening = 200,

    /// <summary>
    /// Menus are transitioning closed
    /// </summary>
    Closing = 300,

  }
}
