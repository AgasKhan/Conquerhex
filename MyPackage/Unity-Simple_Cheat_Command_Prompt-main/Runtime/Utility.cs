using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace CheatCommandsPrompt
{
    public static class Utility
    {
        /// <summary>
        /// Get all the methods with the CommandAttribute custom attribute.
        /// </summary>
        public static Dictionary<string, CheatCommand> GetMethodsWithCheatCommandAttribute()
        {
            Dictionary<string, CheatCommand> cheatCommands = new Dictionary<string, CheatCommand>();

            // Obtener todos los ensamblados cargados en el dominio de la aplicación.
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                IEnumerable<MethodInfo> detectedMethods = assembly.GetTypes()
                    .Where(t => t.IsClass)
                    .SelectMany(
                        t => t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                            .Where(
                                m => m.GetCustomAttributes<CommandAttribute>().Any() &&
                                     !HasDefaultParameters(m)
                            )
                    );

                foreach (MethodInfo method in detectedMethods)
                {
                    CommandAttribute attribute = method.GetCustomAttribute<CommandAttribute>();
                    string name = attribute.Name ?? method.Name;
                    string description = attribute.Description ?? string.Empty;
                    Delegate action = Delegate.CreateDelegate(GetDelegateType(method), method);
                    cheatCommands.Add(name.ToLower(), new CheatCommand(name, action, description));
                }
            }

            return cheatCommands;
        }

        /// <summary>
        /// Check if a method has default parameters.
        /// </summary>
        public static bool HasDefaultParameters(this MethodInfo method)
        {
            return method.GetParameters().Any(p => p.HasDefaultValue);
        }

        /// <summary>
        /// Get the delegate type of a method.
        /// </summary>
        public static Type GetDelegateType(this MethodInfo methodInfo)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            Type[] parameterTypes = new Type[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                parameterTypes[i] = parameters[i].ParameterType;
            }

            Type returnType = methodInfo.ReturnType;
            Type delegateType = Expression.GetDelegateType(parameterTypes.Concat(new[] { returnType }).ToArray());

            return delegateType;
        }

        /// <summary>
        /// Set the cursor position to the end of the text.
        /// </summary>
        public static void ResetCursorPosition(this TextEditor textEditor)
        {
            textEditor.cursorIndex = textEditor.text.Length;
            textEditor.SelectNone();
        }
    }
}