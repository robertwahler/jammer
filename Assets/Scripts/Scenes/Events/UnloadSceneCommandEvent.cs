using SDD;

using Jammer.Commands;

namespace Jammer.Scenes {

  /// <summary>
  /// Unload a scene
  /// </summary>
  /// <remarks>
  /// This a command if Handled==false and an announcement if Handled==true
  /// </remarks>
  public class UnloadSceneCommandEvent : CommandEvent {

    /// <summary>
    /// </summary>
    public string SceneName { get; set; }

    /// <summary>
    /// Return a string
    /// </summary>
    public override string ToString(){
      return string.Format("{0}, SceneName {1}", base.ToString(), SceneName);
    }
  }
}

