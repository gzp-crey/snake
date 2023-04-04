using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool ShowDebugHUD = true;

    void Update()
    {
        DebugHUD.StartFrame(ShowDebugHUD);
    }
}
