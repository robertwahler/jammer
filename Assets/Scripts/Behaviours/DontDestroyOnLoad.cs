using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jammer.Behaviours {

  public class DontDestroyOnLoad : BehaviourBase {

    void Awake() {
      DontDestroyOnLoad(transform.gameObject);
    }

  }
}
