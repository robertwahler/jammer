using SDD.Events;

namespace Jammer {

  /// <summary>
  /// Game published event fires anytime the game changes state
  /// </summary>
  public class GameStateEvent : Event {

    public GameManager Game { get; set; }

    public GameState State { get; set; }

    /// <summary>
    /// Return as a string
    /// </summary>
    public override string ToString(){
      return string.Format("{0}, State {1}", base.ToString(), State);
    }

  }
}

