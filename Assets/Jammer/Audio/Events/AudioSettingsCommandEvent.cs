using Jammer.Commands;

namespace Jammer {

  /// <summary>
  /// Change audio settings
  /// </summary>
  /// <remarks>
  /// This a command if Handled==false and an announcement if Handled==true
  /// </remarks>
  public class AudioSettingsCommandEvent : CommandEvent {

    /// <summary>
    /// Mute or unmute
    /// </summary>
    public bool MuteAudio { get; set; }

    /// <summary>
    /// Return a string
    /// </summary>
    public override string ToString(){
      return string.Format("{0}, MuteAudio {1}", base.ToString(), MuteAudio);
    }
  }
}
