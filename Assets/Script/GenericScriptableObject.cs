using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericScriptableObject<T> : ScriptableObject
{
    public T data;
}

[CreateAssetMenu(menuName = "Scriptables/GameObject")]
public class GameObjectScriptableObject : GenericScriptableObject<GameObject> {}

[CreateAssetMenu(menuName = "Scriptables/String")]
public class StringScriptableObject : GenericScriptableObject<string> {}

[CreateAssetMenu(menuName = "Scriptables/float")]
public class FloatScriptableObject : GenericScriptableObject<float> {}