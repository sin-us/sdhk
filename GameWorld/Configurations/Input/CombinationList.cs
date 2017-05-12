using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameWorld.Configurations.Input
{
    public class Combination
    {
        public string Description { get; set; }
        public ActionType Action { get; set; }
        public List<KeyBinding> Binding { get; set; }

        public Combination(string description, ActionType action, List<KeyBinding> binding)
        {
            Description = description;
            Action = action;
            Binding = binding;
        }
    }

    public class CombinationList
    {
        public List<Combination> Combinations;

        public CombinationList()
        {
            Combinations = new List<Combination>();
        }

        public Combination GetCombinationByDescription(string description)
        {
            foreach (Combination combination in Combinations)
            {
                if (combination.Description == description)
                {
                    return combination;
                }
            }

            return null;
        }

        public Combination GetCombinationByAction(ActionType action)
        {
            foreach (Combination combination in Combinations)
            {
                if (combination.Action == action)
                {
                    return combination;
                }
            }

            return null;
        }

        public Combination GetCombinationByBinding(List<KeyBinding> binding)
        {
            foreach (Combination combination in Combinations)
            {
                if (combination.Binding == binding)
                {
                    return combination;
                }
            }

            return null;
        }
    }
}
