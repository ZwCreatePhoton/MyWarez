using System.Collections.Generic;

namespace MyWarez.Core
{
    public interface IGeneratable
    {
        // Produces the sample, depedencies, etc to reproduce the attack
        public void Generate();
    }

    // Abstraction over a particular full attack scenerio
    public interface IAttack : IGeneratable
    {
        public string Name { get; set; }
        public string Notes { get; set; }
    }

    public class Attack : IAttack
    {
        public Attack(IGeneratable generatable, string name="", string notes="") : this (new[] { generatable}, name, notes)
        { }
        public Attack(IEnumerable<IGeneratable> generatables, string name="", string notes="")
        {
            Generatables = generatables;
            Name = name;
            Notes = notes;
        }
        private IEnumerable<IGeneratable> Generatables { get; }
        public void Generate()
        {
            foreach (var generatable in Generatables)
                generatable.Generate();
        }
        public string Name { get; set; }
        public string Notes { get; set; }
    }
}
