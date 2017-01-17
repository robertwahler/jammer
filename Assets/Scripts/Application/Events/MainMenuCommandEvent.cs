using Jammer.Commands;

namespace Jammer {

  /// <summary>
  /// Load the main menu
  /// </summary>
  /// <remarks>
  /// This a command if Handled==false and an announcement if Handled==true
  /// </remarks>
  public class MainMenuCommandEvent : CommandEvent {

    /// <summary>
    /// Menu state.  Set when handled
    /// </summary>
    public MenuState State { get; set; }

    /// <summary>
    /// Return a string
    /// </summary>
    public override string ToString(){
      return string.Format("{0}, State {1}", base.ToString(), State);
    }
  }
}
