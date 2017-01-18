namespace Jammer {

  /// <summary>
  /// Application settings serialization
  /// </summary>
  [System.Serializable]
  public class ApplicationSettings {

    /// <summary>
    /// Play background animations, turn this off if annoying or using too much CPU.
    /// </summary>
    [System.ComponentModel.DefaultValue(true)]
    public bool BackgroundAnimations { get; set; }

    /// <summary>
    /// Invert the controller Y axis
    /// </summary>
    public bool InvertYAxis { get; set; }

    /// <summary>
    /// Generic constructor
    /// </summary>
    public ApplicationSettings() {
      // Defaults are set separately in attributes to prevent serializing value when at default
      BackgroundAnimations=true;
      InvertYAxis=false;
    }

    /// <summary>
    /// Return a string
    /// </summary>
    public override string ToString(){
      return string.Format("{0}, BackgroundAnimations {0}, InvertYAxis {1}", base.ToString(), BackgroundAnimations, InvertYAxis);
    }
  }
}

