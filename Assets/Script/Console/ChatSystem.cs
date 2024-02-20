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

    public static DebugCommand<(string, int)> GIVE;

    public static DebugCommand COINS;

    public static DebugCommand HELP;

    public List<object> commandList;

    [SerializeField]
    NewEventManager eventManager;

    EventParam<(string, int)> giveEvent = new EventParam<(string, int)>();
    EventParam coinEvent = new EventParam();
    EventParam helpEvent = new EventParam();

    private void Start()
    {
        coinEvent.delegato += () => GameManager.instance.playerCharacter.inventory.AddOrSubstractItems("Coin", 100);
        helpEvent.delegato += () =>
        {
            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;
                string label = $"{command.commandID} - {command.commandDescription}";
                WriteMsg(label);
            }
        };
        giveEvent.delegato += (tupla)=> GameManager.instance.playerCharacter.inventory.AddOrSubstractItems(tupla.Item1, tupla.Item2);
        
        eventManager.events.AddRange(new Pictionarys<string, Euler.SpecificEventParent>()
        {
            {CommandsList.Coins, coinEvent },
            {CommandsList.Help,  helpEvent}
        });

        eventManager.events.Add(CommandsList.Coins, coinEvent);
        eventManager.events.Add(CommandsList.Help, helpEvent);
        eventManager.events.Add(CommandsList.Give, giveEvent);

        COINS = new DebugCommand(CommandsList.Coins, "Gives 100 coins to the player", ref eventManager);
        HELP = new DebugCommand("help", "Shows the list of commands", ref eventManager);
        GIVE = new DebugCommand<(string, int)>(CommandsList.Give, "Gives any item to the player", ref eventManager);

        /*
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
*/
        commandList = new List<object>
        {
            COINS,
            HELP,
            GIVE
        };

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
                else if (commandList[i] as DebugCommand<(string,int)> != null)
                    (commandList[i] as DebugCommand<(string, int)>).Invoke((properties[1], int.Parse(properties[2])));
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

public static class CommandsList
{
    public static string Give = "Give";
    public static string Coins = "Coins";
    public static string Help = "Help";
}