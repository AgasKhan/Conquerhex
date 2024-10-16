
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace DialogueSystem
{
    [System.Serializable]
    public class DialogueNode
    {
        [TextArea(3, 2)]
        public string dialogueText;

        public UnityEvent ExecuteAction;
        public List<DialogueResponse> responses;

        
        internal bool IsLastNode()
        {
            return responses.Count <= 0;
        }
    }
}
