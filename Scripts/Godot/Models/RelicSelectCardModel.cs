using System;
using TFCardBattle.Core;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class RelicSelectCardModel : Control
    {
        public void SetRelic(Relic relic)
        {
            GetNode<Label>("%NameLabel").Text = relic.Name;
            GetNode<Label>("%DescLabel").Text = relic.Description;
            GetNode<Label>("%CostLabel").Text = relic.TFCost.ToString();

            if (ResourceLoader.Exists(relic.IconPath))
            {
                var texture = ResourceLoader.Load<Texture2D>(relic.IconPath);
                GetNode<TextureRect>("%Art").Texture = texture;
            }
        }
    }
}