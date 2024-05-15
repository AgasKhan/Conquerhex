using System;

namespace CheatCommandsPrompt
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public CommandAttribute(string description = null, string name = null)
        {
            Name = name;
            Description = description;
        }
    }

}
