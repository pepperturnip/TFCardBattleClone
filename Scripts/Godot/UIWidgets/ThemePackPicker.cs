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

        public IReadOnlySet<CardPack> SelectedPacks => _selectedPacks;
        private HashSet<CardPack> _selectedPacks = new HashSet<CardPack>();

        private VBoxContainer _container => GetNode<VBoxContainer>("%CheckBoxContainer");

        public void SetChoices(CardPack[] choices, CardPack[] defaultSelections)
        {
            for (int i = 0; i < choices.Length; i++)
            {
                var cardPack = choices[i];

                var checkBox = new CheckBox();
                _container.AddChild(checkBox);
                checkBox.Text = cardPack.Name;
                checkBox.Toggled += (bool pressed) =>
                {
                    if (pressed)
                        _selectedPacks.Add(cardPack);
                    else
                        _selectedPacks.Remove(cardPack);

                    EmitSignal(SignalName.SelectionChanged);
                };

                checkBox.ButtonPressed = defaultSelections.Contains(cardPack);
            }
        }
    }
}