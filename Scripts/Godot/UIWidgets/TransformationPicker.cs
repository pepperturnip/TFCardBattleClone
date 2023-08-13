using System;
using System.Linq;
using TFCardBattle.Core;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class TransformationPicker : ItemList
    {
        public Transformation SelectedChoice => _choices[GetSelectedItems()[0]];
        private Transformation[] _choices = Array.Empty<Transformation>();

        public void SetChoices(Transformation[] choices)
        {
            _choices = choices;

            Clear();

            foreach (var tf in _choices)
            {
                AddItem(tf.Name);
            }

            Select(0);
        }
    }
}