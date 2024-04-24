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

    public List<DebugCommandBase> commandList;

    [SerializeField]
    EventManager eventManager;

    SingleEvent<(string, int)> giveEvent = new SingleEvent<(string, int)>();
    SingleEvent coinEvent = new SingleEvent();
    SingleEvent helpEvent = new SingleEvent();

    private void Start()
    {
        //coinEvent.delegato += () => GameManager.instance.playerCharacter.inventory.AddItem("Coin", 100);
        helpEvent.delegato += () =>
        {
            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase command = commandList[i];
                string label = $"{command.commandID} - {command.commandDescription}";
                WriteMsg(label);
            }
        };
        //giveEvent.delegato += (tupla)=> GameManager.instance.playerCharacter.inventory.AddItem(tupla.Item1, tupla.Item2);
        

        eventManager.events.AddRange(new Pictionarys<string, Internal.SpecificEventParent>()
        {
            {CommandsList.Coins, coinEvent},
            {CommandsList.Help,  helpEvent},
            {CommandsList.Give, giveEvent}
        });
        /*
        eventManager.events.Add(CommandsList.Coins, coinEvent);
        eventManager.events.Add(CommandsList.Help, helpEvent);
        eventManager.events.Add(CommandsList.Give, giveEvent);
        */

        COINS = new DebugCommand(CommandsList.Coins, "Gives 100 coins to the player");
        HELP = new DebugCommand(CommandsList.Help, "Shows the list of commands");
        GIVE = new DebugCommand<(string, int)>(CommandsList.Give, "Gives any item to the player");

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
        commandList = new List<DebugCommandBase>
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

        for (int i = 0; i < commandList.Count; i++)
        {
            if(input.Contains(commandList[i].commandID))
            {
                string[] properties = input.Split(' ');

                if (properties.Length > 1)
                {
                    eventManager.Trigger(commandList[i].commandID, (properties[1], int.Parse(properties[2])));
                }
                else
                {
                    eventManager.Trigger(commandList[i].commandID);
                }
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
    public static string Give = "give";
    public static string Coins = "coins";
    public static string Help = "help";
}