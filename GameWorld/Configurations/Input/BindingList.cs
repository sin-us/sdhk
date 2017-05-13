using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameWorld.Configurations.Input
{
    public class Binding
    {
        public string Description { get; set; }
        public ActionType Action { get; set; }
        public KeyBindingInfo KeyBindingInfo { get; set; }

        public Binding(string description, ActionType action, KeyBindingInfo keyBindingInfo)
        {
            Description = description;
            Action = action;
            KeyBindingInfo = keyBindingInfo;
        }
    }

    public class BindingList
    {
        public List<Binding> Bindings { get; set; }

        public BindingList()
        {
            Bindings = new List<Binding>();
        }

        public Binding GetBindingByDescription(string description)
        {
            foreach (Binding binding in Bindings)
            {
                if (binding.Description == description)
                {
                    return binding;
                }
            }

            return null;
        }

        public Binding GetBindingByAction(ActionType action)
        {
            foreach (Binding binding in Bindings)
            {
                if (binding.Action == action)
                {
                    return binding;
                }
            }

            return null;
        }

        public Binding GetBindingByKeyBindingInfo(KeyBindingInfo keyBindingInfo)
        {
            foreach (Binding binding in Bindings)
            {
                if (binding.KeyBindingInfo.Key.Equals(keyBindingInfo.Key))
                {
                    return binding;
                }
            }

            return null;
        }
    }
}
