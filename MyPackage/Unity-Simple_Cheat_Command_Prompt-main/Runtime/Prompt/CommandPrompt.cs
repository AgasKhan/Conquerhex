
using System.Collections.Generic;
using UnityEngine;

namespace CheatCommandsPrompt
{
    public partial class CommandPrompt : MonoBehaviour
    {
        private const KeyCode START_PROMPT_KEY = KeyCode.F1;
        private const KeyCode CLOSE_PROMPT_KEY = KeyCode.Escape;
        private const KeyCode INVOKE_CHEAT_KEY = KeyCode.Return;
        private const KeyCode PREDICTION_NAVIGATION_KEY = KeyCode.Tab;
        private const string TEXT_FIELD_CONTROL_NAME = "commandInputControl";

        /*////////////////////////////////////////////////////////////////////////////////////////////*/

        [SerializeField]
        private GUISkin _promptSkin;
        private int _selectionIndex;
        private bool _promptEnabled;
        private Dictionary<string, CheatCommand> _commands;
        private string[] _predictedCommands = { };
        private string _inputText = string.Empty;
        private CheatCommand _currentCommand;
        private TextEditor _textField;
        private bool _resetCursor;

        /*////////////////////////////////////////////////////////////////////////////////////////////*/

        private string CurrentPredictionMatch
        {
            get => _predictedCommands.Length == 0 ? string.Empty : _predictedCommands[_selectionIndex];
        }

        /*////////////////////////////////////////////////////////////////////////////////////////////*/
        #if UNITY_EDITOR
        private void Awake()
        {
            
            _commands = Utility.GetMethodsWithCheatCommandAttribute();
        }

        private void OnGUI()
        {
            SetPromptSkin();
            SwitchPromptEnableState();

            if (_promptEnabled)
            {
                SetPromptText();
                NavigatePredictedList();
                DrawLogsArea();

                if (Event.current.keyCode == INVOKE_CHEAT_KEY && Event.current.type == EventType.KeyUp)
                {
                    ExecuteCommand();
                    _inputText = string.Empty;
                    _currentCommand = null;
                    UpdatePredictedCommandsList();
                }
            }

            if (GUI.changed)
            {
                // Use the command prediction.
                var firstPart = _inputText.Split(" ");
                if (firstPart.Length > 1)
                {
                    if (CurrentPredictionMatch != string.Empty)
                    {
                        var command = _commands[CurrentPredictionMatch];
                        _inputText = command.Name + " ";
                        _currentCommand = command;
                        _resetCursor = true;
                    }
                }
                else
                {
                    _currentCommand = null;
                }

                UpdatePredictedCommandsList();
            }
        }

        /*////////////////////////////////////////////////////////////////////////////////////////////*/

        private void SetPromptSkin()
        {
            GUI.skin = _promptSkin != null ? _promptSkin : GUI.skin;
        }

        private void SwitchPromptEnableState()
        {
            /// Switch the prompt state when the user press the start prompt key.
            if (Event.current.keyCode == START_PROMPT_KEY && Event.current.type == EventType.KeyDown)
            {
                UpdatePredictedCommandsList();
                _promptEnabled = !_promptEnabled;
                _inputText = string.Empty;
                _selectionIndex = 0;
                _currentCommand = null;

                GUI.SetNextControlName(TEXT_FIELD_CONTROL_NAME);
                SetPromptText();
                GUI.FocusControl(TEXT_FIELD_CONTROL_NAME);

                _textField = GUIUtility.QueryStateObject(typeof(TextEditor), GUIUtility.keyboardControl) as TextEditor;
            }

            /// Close when the user press the close key.
            if (Event.current.keyCode == CLOSE_PROMPT_KEY)
            {
                _promptEnabled = false;
            }
        }
        #endif
    }


}