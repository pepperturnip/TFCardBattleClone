using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class ThemePackPicker : Control
    {
        [Signal] public delegate void SelectionChangedEventHandler();

        public int RequiredSelections {get; private set;}
        public bool SelectionsValid => SelectedPacks.Count == RequiredSelections;

        public IReadOnlySet<CardPack> SelectedPacks => _selectedPacks;
        private HashSet<CardPack> _selectedPacks = new HashSet<CardPack>();

        private Label _countLabel => GetNode<Label>("%CountLabel");
        private VBoxContainer _container => GetNode<VBoxContainer>("%CheckBoxContainer");

        public void SetChoices(
            CardPack[] choices,
            CardPack[] defaultSelections,
            int requiredSelections
        )
        {
            RequiredSelections = requiredSelections;

            // Create a checkbox for each choice
            for (int i = 0; i < choices.Length; i++)
            {
                var cardPack = choices[i];

                var checkBox = new CheckBox();
                _container.AddChild(checkBox);
                checkBox.Text = cardPack.Name;
                checkBox.Toggled += (bool pressed) => OnPackToggled(cardPack, pressed);
                checkBox.ButtonPressed = defaultSelections.Contains(cardPack);
            }
        }

        private void OnPackToggled(CardPack cardPack, bool pressed)
        {
            if (pressed)
                _selectedPacks.Add(cardPack);
            else
                _selectedPacks.Remove(cardPack);

            EmitSignal(SignalName.SelectionChanged);

            _countLabel.Text = $"({SelectedPacks.Count}/{RequiredSelections})";
        }
    }
}