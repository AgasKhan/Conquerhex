using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CheatCommandsPrompt
{
    public class CheatCommand
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<Tuple<string, Type>> Parameters { get; private set; }

        public Delegate Action => m_action;

        /*////////////////////////////////////////////////////////////////////////////////////////////*/

        private readonly Delegate m_action;

        /*////////////////////////////////////////////////////////////////////////////////////////////*/

        public CheatCommand(string name, Delegate action, string description)
        {
            m_action = action;
            Description = description;
            Name = name;
            Parameters = action.Method.GetParameters().Select(p => Tuple.Create(p.Name, p.ParameterType)).ToList();
        }

        /*////////////////////////////////////////////////////////////////////////////////////////////*/

        /// <summary>
        /// Invokes the command with the given parameters.
        /// </summary>
        public bool Invoke(params string[] parameters)
        {
            object[] convertedParameters = new object[parameters.Length];

            for (int i = 0; i < convertedParameters.Length; i++)
            {
                try
                {
                    convertedParameters[i] = Convert.ChangeType(parameters[i], Parameters[i].Item2);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    return false;
                }
            }

            try
            {
                m_action.DynamicInvoke(convertedParameters);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return false;
            }

            return true;
        }
    }
}
