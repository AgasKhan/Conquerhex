using CustomGraph;
using UnityEngine;

[Info("Timer", "Time/Timer")]
public class ND_Timer : CG_Node
{
    [ExposedProperty()]
    public int time;

    Timer _timer;

    public override string GetNextNode(CG_AssetGraph graph)
    {
        _timer = TimersManager.Create(time);
        return base.GetNextNode(graph);
    }

    public override void Update()
    {
        if (_timer.Chck) _canTransition = true;
    }



}

