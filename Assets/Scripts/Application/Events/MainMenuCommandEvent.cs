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
    /// The current (announcement) or requested (command) Id
    /// </summary>
    public MenuId MenuId { get; set; }

    /// <summary>
    /// The current (announcement) or requested (command) State
    /// </summary>
    public MenuState State { get; set; }

    /// <summary>
    /// Return a string
    /// </summary>
    public override string ToString(){
      return string.Format("{0}, MenuId {1}, State {2}", base.ToString(), MenuId, State);
    }
  }
}
