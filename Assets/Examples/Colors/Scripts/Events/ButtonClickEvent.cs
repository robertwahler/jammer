using Jammer.Events;

namespace Colors.Events {

  /// <summary>
  /// Raised event signals a button was clicked
  /// </summary>
  public class ButtonClickEvent : BaseEvent {

    /// <summary>
    /// Sender handler
    /// </summary>
    public ButtonHandler ButtonHandler { get; set; }

    public string Name { get; set; }

    public ButtonKind Kind { get; set; }

    /// <summary>
    /// Return a string
    /// </summary>
    public override string ToString(){
      return string.Format("{0}, ButtonHandler {1}", base.ToString(), ButtonHandler);
    }

  }
}
