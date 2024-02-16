using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    bool showConsole;
    bool showHelp;

    string input;

    //public static DebugCommand<string, int> GIVE;

    public static DebugCommand COINS;

    public static DebugCommand HELP;

    public List<object> commandList;
    /*
    public void OnDebugConsole(InputValue value)
    {
        showConsole = !showConsole;
    }

    public void OnReturn(InputValue value)
    {
        if(showConsole)
        {
            HandleInput();
            input = "";
        }
    }
    */
    /*
    private void Awake()
    {
        GIVE = new DebugCommand<string, int> ("give", "Gives any item to the player", "give", (string item, int amount) => 
        { 
            GameManager.instance.playerCharacter.AddOrSubstractItems(item, amount);
        });

        COINS = new DebugCommand("coins", "Gives 100 coins to the player", "coins", () =>
        {
            GameManager.instance.playerCharacter.AddOrSubstractItems("Coin", 100);
        });

        HELP = new DebugCommand("help", "Shows the list of commands", "help", () =>
        {
            showHelp = true;
        });

        commandList = new List<object>
        {
            GIVE,
            COINS,
            HELP
        };
    }

    private void HandleInput()
    {
        string[] properties = input.Split(' ');

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if(input.Contains(commandBase.commandID))
            {
                if (commandList[i] as DebugCommand != null)
                    (commandList[i] as DebugCommand).Invoke();
                else if (commandList[i] as DebugCommand<string, int> != null)
                    (commandList[i] as DebugCommand<string, int>).Invoke(properties[1], int.Parse(properties[2]));
            }
        }
    }
    Vector2 scroll;

    private void OnGUI()
    {
        if (!showConsole)
            return;

        float y = 0f;

        if (showHelp)
        {
            GUI.Box(new Rect(0, y, Screen.width, 100), "");

            Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);

            scroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), scroll, viewport);

            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;

                string label = $"{command.commandFormat} - {command.commandDescription}";

                Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);

                GUI.Label(labelRect, label);
            }

            GUI.EndScrollView();

            y += 100;
        }

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0);
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
    }*/

}
