using Newtonsoft.Json;
using SDD.Events;

namespace Jammer.Commands {

  /// <summary>
  /// Base command event. Commands can be requested or completed actions.
  /// </summary>
  /// <remarks>
  /// This a command if Handled==false and an announcement if Handled==true
  /// </remarks>
  public class CommandEvent : Event {

    /// <summary>
    /// MonoBehaviour that initiated this event. Optional
    /// </summary>
    [JsonIgnore]
    public UnityEngine.MonoBehaviour Sender { get; set; }

    /// <summary>
    /// Events with Handled==false are requests for action.  If Handled==true
    /// then the event is an annoucement that an action occurred.
    /// </summary>
    [JsonIgnore]
    public bool Handled { get; set; }

    /// <summary>
    /// Optional unique Id for locating and internationalization. The Id should
    /// be unique in its own context. i.e. within a recording file the Id
    /// should be unique. Uniqueness unit tested but not runtime tested.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The animation (if any) duration in seconds. Leave at zero for no animation.
    /// </summary>
    public float Duration { get; set; }

    /// <summary>
    /// Return a string
    /// </summary>
    public override string ToString(){
      if (Id != null) {
        return string.Format("{0}, Id {0}, Sender {2}, Handled {3}", base.ToString(), Id, Sender, Handled);
      }
      else {
        // don't bother with junking up the console with null Id values
        return string.Format("{0}, Sender {1}, Handled {2}", base.ToString(), Sender, Handled);
      }
    }

  }
}


