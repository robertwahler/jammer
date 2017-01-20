using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jammer.Behaviours {

  /// <summary>
  /// Attach to gameObject to prevent the entire hierarchy from being destroyed
  /// when a new scene is loaded.
  /// </summary>
  public class DontDestroy : BehaviourBase {

    void Awake() {
      DontDestroyOnLoad(transform.gameObject);
    }

  }
}
