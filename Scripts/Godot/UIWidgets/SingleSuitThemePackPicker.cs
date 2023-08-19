using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class SingleSuitThemePackPicker : Control
    {
        [Signal] public delegate void SelectionChangedEventHandler();

        public int RequiredSelections {get; private set;}
        public bool SelectionsValid => SelectedPacks.Count == RequiredSelections;

        public IReadOnlySet<CardPack> SelectedPacks => _selectedPacks;
        private HashSet<CardPack> _selectedPacks = new HashSet<CardPack>();
        private CardPack[] _choices;

        private Label _countLabel => GetNode<Label>("%CountLabel");
        private Container _container => GetNode<Container>("%CheckBoxContainer");

        public void SetChoices(CardPack[] choices, int requiredSelections)
        {
            _choices = choices;
            RequiredSelections = requiredSelections;

            // Create a checkbox for each choice
            for (int i = 0; i < choices.Length; i++)
            {
                var cardPack = choices[i];

                var checkBox = new CheckBox();
                _container.AddChild(checkBox);
                checkBox.Text = cardPack.Name;
                checkBox.Toggled += (bool pressed) => OnPackToggled(cardPack, pressed);
            }
        }

        public void SetSelectedPacks(IEnumerable<CardPack> selectedPacks)
        {
            _selectedPacks = selectedPacks.ToHashSet();

            for (int i = 0; i < _choices.Length; i++)
            {
                var cardPack = _choices[i];
                var checkBox = _container.GetChild<CheckBox>(i);

                checkBox.ButtonPressed = _selectedPacks.Contains(cardPack);
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