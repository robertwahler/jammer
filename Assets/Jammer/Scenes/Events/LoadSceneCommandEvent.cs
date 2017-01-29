using Jammer.Commands;

namespace Jammer.Scenes {

  /// <summary>
  /// Load a scene
  /// </summary>
  /// <remarks>
  /// This a command if Handled==false and an announcement if Handled==true
  /// </remarks>
  public class LoadSceneCommandEvent : CommandEvent {

    /// <summary>
    /// </summary>
    public string SceneName { get; set; }

    /// <summary>
    /// </summary>
    public LoadSceneMode Mode { get; set; }

    /// <summary>
    /// Return a string
    /// </summary>
    public override string ToString(){
      return string.Format("{0}, SceneName {1}, Mode {2}", base.ToString(), SceneName, Mode);
    }
  }
}
