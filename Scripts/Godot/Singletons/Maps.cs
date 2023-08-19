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
        [Export] public PackedScene EndlessMode;

        public override void _Ready()
        {
            Instance = this;
        }

        public void GoToTitleScreen()
        {
            GetTree().ChangeSceneToPacked(TitleScreen);
        }

        public void GoToClassicMode()
        {
            GetTree().ChangeSceneToPacked(ClassicMode);
        }

        public void GoToEndlessMode()
        {
            GetTree().ChangeSceneToPacked(EndlessMode);
        }
    }
}