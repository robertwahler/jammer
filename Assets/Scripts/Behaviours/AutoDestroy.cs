using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jammer.Behaviours {

  /// <summary>
  /// Destroy the gameObject after duration given in seconds.
  /// </summary>
  public class AutoDestroy : BehaviourBase {

    /// <summary>
    /// Wait for x seconds
    /// </summary>
    public float secondsToWait = 5;

    private IEnumerator Start() {
      yield return WaitFor.Seconds(secondsToWait);

      GameObject.DestroyImmediate(gameObject);
    }
  }
}

