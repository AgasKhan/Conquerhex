
using CustomGraph;
using UnityEngine;

[Info("Animation", "Animations/New Animation")]
public class ND_AnimationExecute : CG_Node
{
    [ExposedProperty()]
    public AnimationClip animationInfo;
    public override string OnProcess(CG_AssetGraph graph)
    {
        //Ejecutar animacion
        return base.OnProcess(graph);
    }
}
