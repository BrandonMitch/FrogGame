using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MethodVariableFloatSO", menuName = "Variable/Method Variable/SO->SO, sends float var, callback float var")]
public class MethodVariableFloatSO : MethodVariableBase<ScriptableObject,ScriptableObject,FloatVariable,FloatVariable>
{

}

[CreateAssetMenu(fileName = "MethodVariableFloatMono", menuName = "Variable/Method Variable/Mono->Mono, sends float var, callback float var")]
public class MethodVariableFloatGO : MethodVariableBase<MonoBehaviour, MonoBehaviour, FloatVariable, FloatVariable>
{

}

[CreateAssetMenu(fileName = "MethodVariableIntSO", menuName = "Variable/Method Variable/SO->SO, sends int var, callback int var")]
public class MethodVariableIntSO : MethodVariableBase<ScriptableObject, ScriptableObject, IntegerVariable, IntegerVariable>
{

}

[CreateAssetMenu(fileName = "MethodVariableIntMono", menuName = "Variable/Method Variable/Mono->Mono, sends int var, callback int var")]
public class MethodVariableIntGO : MethodVariableBase<MonoBehaviour, MonoBehaviour, IntegerVariable, IntegerVariable>
{

}
