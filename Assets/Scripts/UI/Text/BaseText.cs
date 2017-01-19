using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Jammer.UI {

  public class BaseText : BehaviourBase {

    /// <summary>
    /// The text component
    /// </summary>
    public Text Text {
      get {
        if (text == null) {
          text = GetComponent<Text>();
        }
        return text;
      }
    }
    [System.NonSerialized]
    private Text text;

  }
}

