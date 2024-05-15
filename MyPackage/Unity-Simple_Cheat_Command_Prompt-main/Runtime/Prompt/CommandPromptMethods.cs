#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;

namespace CheatCommandsPrompt
{
    public partial class CommandPrompt : MonoBehaviour
    {
        private bool PredictionExists => _predictedCommands.Length > 0;

        /*////////////////////////////////////////////////////////////////////////////////////////////*/

        private void ExecuteCommand()
        {
            if (_commands.TryGetValue(_inputText.ToLower(), out var command))
            {
                _currentCommand = command;
            }

            if (_currentCommand == null)
            {
                Debug.LogWarning($"El comando <{_inputText}> no se encontro.");
                return;
            }

            string[] parameters = _inputText.Split(' ')[1..];
            var values = _currentCommand.Parameters.Zip(parameters, (p, v) => Convert.ChangeType(v, p.Item2)).ToArray();
            _currentCommand.Action.DynamicInvoke(values);
        }

        /// <summary>
        /// Update the list for predicted commands.
        /// </summary>
        private void UpdatePredictedCommandsList()
        {
            _predictedCommands = _commands.Keys.
                Where(c => c.ToLower().StartsWith(_inputText.ToLower())).
                ToArray();

            _selectionIndex = Mathf.Clamp(_selectionIndex, 0, _predictedCommands.Length - 1);
        }

        /// <summary>
        /// Set the prompt text and reset the cursor position if necessary.
        /// </summary>
        private void SetPromptText()
        {
            _inputText = GUILayout.TextField(_inputText, GUILayout.Width(Screen.width));

            if (_resetCursor)
            {
                _textField.ResetCursorPosition();
                _resetCursor = false;
            }
        }

        /// <summary>
        /// Update the selected prediction index.
        /// </summary>
        private void NavigatePredictedList()
        {
            if (Event.current.keyCode == PREDICTION_NAVIGATION_KEY && Event.current.type == EventType.KeyDown)
            {
                _selectionIndex += Event.current.shift ? -1 : 1;
                _selectionIndex = (int)Mathf.Repeat(_selectionIndex, _predictedCommands.Length);
            }
        }

        /// <summary>
        /// Draw the list of predicted commands.
        /// </summary>
        private void DrawPredictionList()
        {
            if (!PredictionExists)
            {
                GUILayout.Label("<color=red>No command detected.</color>");
            }
            else
            {
                for (int i = 0; i < _predictedCommands.Length; i++)
                {
                    CheatCommand command = _commands[_predictedCommands[i]];
                    string commandName = _selectionIndex == i ? $"<color=lime>{command.Name}</color>" : command.Name;
                    GUILayout.Label($"{commandName}: <color=grey>{command.Description}</color>");
                }
            }
        }

        /// <summary>
        /// Draw the logs area.
        /// </summary>
        private void DrawLogsArea()
        {
            GUILayout.BeginVertical(GUI.skin.GetStyle("box"));
            if (_currentCommand != null)
            {
                string parameters = string.Join(' ', _currentCommand.Parameters.Select(
                    p => $"{p.Item1}<color=grey><{p.Item2.Name}></color>"
                ));
                parameters += parameters.Length > 0 ? "\n\n" : string.Empty;
                string description = _currentCommand.Description;

                GUILayout.Label(parameters + description);
            }
            else
            {
                DrawPredictionList();
            }
            GUILayout.EndVertical();
        }
    }
#endif
}
