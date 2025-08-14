using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : BaseManager 
{
    GameTimer timer;

    public TimerManager() {
        timer = new GameTimer();
    }

    public void Register(float time ,System.Action callback)
    {
        timer.Register(time, callback);
    }
    public override void Update(float dt)
    {
        timer.OnUpdate(dt);
    }
}
