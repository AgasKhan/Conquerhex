using UnityEngine;
using Cursor = UnityEngine.Cursor;

namespace DialogueSystem
{
    public class DialogueManager : SingletonMono<DialogueManager>
    {
        [Header("Propiedades")] [Space(10)]
        public GameObject responseButtonPrefab; // Prefab for generating response buttons

        public Transform responseButtonContainer; // Container to hold response buttons


        [SerializeField] private Character _player;

        IState<Character> playerIA;

        // Starts the dialogue with given title and dialogue node
        public void StartDialogue(string title, DialogueNode node, bool end = false)
        {
            // Display the dialogue UI
            ShowDialogue(node.dialogueText);
            node.ExecuteAction?.Invoke();

            foreach (Transform child in responseButtonContainer)
            {
                Destroy(child.gameObject);
            }

            if (end)
            {
                LastDialogue();
                return;
            }

            playerIA = _player.CurrentState;
            _player.CurrentState = null;
            Cursor.visible = true;

            CalculateResponseNodes(title, node);


            //
            // // Set dialogue title and body text
            // DialogTitleText.text = title;
            //
            // StopAllCoroutines();
            // StartCoroutine(ShowDialogue(title, node));
        }

        void CalculateResponseNodes(string title, DialogueNode node)
        {
            // Create and setup response buttons based on current dialogue node
            foreach (DialogueResponse response in node.responses)
            {
                GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
                EventsCall events = buttonObj.GetComponent<EventsCall>();
                events.Set(response.responseText, () => SelectResponse(response, title), "");


                // buttonObj.GetComponentInChildren<Text>().text = response.responseText;
                // buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response, title));
            }
        }

        public void SelectResponse(DialogueResponse response, string title)
        {
            // Check if there's a follow-up node
            if (!response.nextNode.IsLastNode())
            {
                StartDialogue(title, response.nextNode); // Start next dialogue
            }
            else
            {
                // If no follow-up node, end the dialogue
                StartDialogue(title, response.nextNode, true);
            }
        }

        void LastDialogue()
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
            EventsCall events = buttonObj.GetComponent<EventsCall>();
            events.Set("Salir", () => StopDialogue(), "");
        }

        // Hide the dialogue UI
        public void HideDialogue()
        {
            UI.Interfaz.instance["Subtitulo"].CompleteMsg();
        }

        public void StopDialogue()
        {
            HideDialogue();
            _player.CurrentState = playerIA;
            //Cursor.visible = false; Ver como bloquearlo en perspectiva.
            
            foreach (Transform child in responseButtonContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // Show the dialogue UI
        private void ShowDialogue(string text)
        {
            UI.Interfaz.instance["Subtitulo"].ClearMsg();
            UI.Interfaz.instance["Subtitulo"].AddMsg(text);
        }

        //Deprecated
        // IEnumerator ShowDialogue(string title, DialogueNode node)
        // {
        //     DialogBodyText.text = "";
        //
        //     foreach (var letter in node.dialogueText.ToCharArray())
        //     {
        //         DialogBodyText.text += letter;
        //         yield return null;
        //     }
        //
        //     CalculateResponseNodes(title, node);
        // }


        // Handles response selection and triggers next dialogue node

        //
        // // Check if dialogue is currently active
        // public bool IsDialogueActive()
        // {
        //     //return DialogueParent.activeSelf;
        //     UI.Interfaz.instance["Subtitulo"].
        // }


        #region Deprecated

        // public static DialogueManager Instance { get; private set; }

        // private void Awake()
        // {
        //     if (Instance == null) Instance = this;
        //     else Destroy(this);
        // }

        // Queue<string> _dialogueSentences;

        // [Header("Propiedades")]
        // [Space(10)]
        // [SerializeField] Text _dialogueText;
        // [SerializeField] GameObject _nextBTN;
        // [SerializeField] Animator _animator;
        // [SerializeField] float _delayOfChars = 0.2f;

        // Dialogue _dialogue;

        // StartDialogueScriptWithButton _starter;

        // AudioManager _audio;


        // public void StartDialogue(Dialogue dialogue, StartDialogueScriptWithButton start)
        // {
        //     _nextBTN.SetActive(false);
        //     _animator.SetBool("IsOpen", true);
        //     Cursor.visible = true;

        //     GameManager.Instance.Player.GetComponent<Animator>().SetFloat("MoveVelocity", 0);
        //     GameManager.Instance.Player.GetComponent<MainCharacter>().enabled = false;

        //     if (_audio == null) _audio = AudioManager.Instance;

        //     _dialogue = dialogue;
        //     _starter = start;
        //     _dialogueText.font = _dialogue.ChosenFont;
        //     _dialogueText.fontStyle = _dialogue.FontStyle;

        //     _dialogueSentences = dialogue.Sentences.Aggregate(new Queue<string>(), (x, y) =>
        //     {
        //         x.Enqueue(y);
        //         return x;
        //     });

        //     DisplayNextSentence();

        // }

        // public void DisplayNextSentence()
        // {
        //     _nextBTN.SetActive(false);

        //     if (_dialogueSentences.Count == 0)
        //     {
        //         EndDialogue();
        //         return;
        //     }

        //     //if(_dialogueSentences.Count == index) _dialogue.OnDialogueDisplayed?.Invoke();

        //     string sentence = _dialogueSentences.Dequeue();
        //     StopAllCoroutines();
        //     StartCoroutine(DisplayText(sentence));
        // }

        // public void EndDialogue()
        // {
        //     _nextBTN.SetActive(false);
        //     _animator.SetBool("IsOpen", false);
        //     MainCharacter.Player.GetComponent<MainCharacter>().enabled = true;
        //     _starter.RestartDialogue();
        //     Cursor.visible = false;
        //     Cursor.lockState = CursorLockMode.Locked;
        // }

        // IEnumerator DisplayText(string sentence)
        // {
        //     _dialogueText.text = "";

        //     foreach (var letter in sentence.ToCharArray())
        //     {
        //         _dialogueText.text += letter;
        //         _audio?.PlayRandomPitchSFX(_dialogue.AudioName);
        //         yield return new WaitForSeconds(_delayOfChars);
        //     }
        //     _nextBTN.SetActive(true);
        // }

        #endregion
    }
}