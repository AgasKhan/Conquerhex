using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _chatBox;
    [SerializeField] private InputField _messageInput;
    [SerializeField] private Scrollbar _verticalScrollBar;

    private bool _newMsg;
    private bool _showConsole = false;

    public static DebugCommand<string, int> GIVE;

    public static DebugCommand COINS;

    public static DebugCommand HELP;

    public List<object> commandList;
    
    private void Awake()
    {
        GIVE = new DebugCommand<string, int>("give", "Gives any item to the player", "give", (string item, int amount) =>
        {
            GameManager.instance.playerCharacter.AddOrSubstractItems(item, amount);
        });

        COINS = new DebugCommand("coins", "Gives 100 coins to the player", "coins", () =>
        {
            GameManager.instance.playerCharacter.AddOrSubstractItems("Coin", 100);
        });

        HELP = new DebugCommand("help", "Shows the list of commands", "help", () =>
        {
            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;

                string label = $"{command.commandFormat} - {command.commandDescription}";

                WriteMsg(label);
            }
        });

        commandList = new List<object>
        {
            GIVE,
            COINS,
            HELP
        };
    }


    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(_showConsole);
        _chatBox.text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _chatBox.text = "";
            _showConsole = !_showConsole;
            transform.GetChild(0).gameObject.SetActive(_showConsole);
            _messageInput.ActivateInputField();
        }
        

        if (Input.GetKeyDown(KeyCode.RightShift) && _messageInput.isFocused)
        {
            TryEnterMessage();
        }

        UpdateScrollBar();
    }

    public void TryEnterMessage()
    {
        if (string.IsNullOrWhiteSpace(_messageInput.text)) 
            return;
        
        ReceiveNewMsg(_messageInput.text);

        _messageInput.text = "";
    }

    public void WriteMsg(string text)
    {
        _chatBox.text += '\n' + text;

        _newMsg = true;
    }

    public void ReceiveNewMsg(string input)
    {
        WriteMsg(input);

        string[] properties = input.Split(' ');

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (input.Contains(commandBase.commandID))
            {
                if (commandList[i] as DebugCommand != null)
                    (commandList[i] as DebugCommand).Invoke();
                else if (commandList[i] as DebugCommand<string, int> != null)
                    (commandList[i] as DebugCommand<string, int>).Invoke(properties[1], int.Parse(properties[2]));
            }
        }
    }

    public void UpdateScrollBar()
    {
        if (!_newMsg) return;
        
        _verticalScrollBar.value = 0;

        _newMsg = false;
    }
}
