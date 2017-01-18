using Newtonsoft.Json;

namespace Jammer {

  /// <summary>
  /// Audio settings serialization
  /// </summary>
  public class AudioSettings {

    /// <summary>
    /// Mute all audio. This will actually stop all audio sources.
    /// </summary>
    public bool MuteAudio { get; set; }

    /// <summary>
    /// Music volume
    /// </summary>
    // TODO: Implement volume control and remove JsonIgnore
    [JsonIgnore]
    public float VolumeMusic { get; set; }

    /// <summary>
    /// Sound effects volume
    /// </summary>
    // TODO: Implement volume control and remove JsonIgnore
    [JsonIgnore]
    public float VolumeSFX { get; set; }

    /// <summary>
    /// Return a string
    /// </summary>
    public override string ToString(){
      return string.Format("{0}, MuteAudio {1}, VolumeMusic {2}, VolumeSFX {3}", base.ToString(), MuteAudio, VolumeMusic, VolumeSFX);
    }
  }
}

