using Godot;

namespace TFCardBattle.Godot
{
    public partial class Maps : Node
    {
        public static Maps Instance {get; private set;}

        [Export] public PackedScene TitleScreen;
        [Export] public PackedScene BattleScreen;

        public override void _Ready()
        {
            Instance = this;
        }
    }
}