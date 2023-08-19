using System;
using TFCardBattle.Core;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class Maps : Node
    {
        public static Maps Instance {get; private set;}

        [Export] public PackedScene TitleScreen;
        [Export] public PackedScene ClassicMode;
        [Export] public PackedScene PlayerLoadoutSelection;
        [Export] public PackedScene BattleScreen;

        public override void _Ready()
        {
            Instance = this;
        }

        public void GoToClassicMode()
        {
            GetTree().ChangeSceneToPacked(ClassicMode);
        }

        public void GoToLoadoutSelection()
        {
            GetTree().ChangeSceneToPacked(PlayerLoadoutSelection);
        }

        public void GoToBattleScreen(PlayerLoadout loadout)
        {
            var battleUI = BattleScreen.Instantiate<BattleUI>();
            ChangeSceneToNode(battleUI);
            battleUI.StartBattle(loadout);
        }

        private void ChangeSceneToNode(Node n)
        {
            var oldScene = GetTree().CurrentScene;
            GetTree().Root.RemoveChild(oldScene);
            oldScene.QueueFree();

            GetTree().Root.AddChild(n);
            GetTree().CurrentScene = n;
        }
    }
}