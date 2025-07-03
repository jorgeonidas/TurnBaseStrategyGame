using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Intearact(Action onInteractionCompleted);
}
