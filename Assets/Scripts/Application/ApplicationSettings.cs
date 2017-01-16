namespace Jammer {

  /// <summary>
  /// Application settings
  /// </summary>
  [System.Serializable]
  public class ApplicationSettings {

    /// <summary>
    /// Play background animations, turn this off if annoying or using too much CPU.
    /// </summary>
    //ShowBackgroundAnimations = 1 << 2,
    [System.ComponentModel.DefaultValue(true)]
    public bool ShowBackgroundAnimations { get; set; }

    /// <summary>
    /// Invert the controller Y axis
    /// </summary>
    //InvertYAxis = 1 << 5,
    public bool InvertYAxis { get; set; }

    /// <summary>
    /// Generic constructor
    /// </summary>
    public ApplicationSettings() {
      // Defaults set separately in attributes to prevent writing
      // value when at default. These are the real defaults.
      InvertYAxis= false;
    }

    /// <summary>
    /// Return a string
    /// </summary>
    public override string ToString(){
      return string.Format("{0}, InvertYAxis {1}", base.ToString(), InvertYAxis);
    }
  }
}

