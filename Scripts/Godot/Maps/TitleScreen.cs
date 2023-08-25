using System;
using System.Collections.Generic;
using System.Linq;
using TFCardBattle.Core;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class TitleScreen : Control
    {
        private static bool _contentRegistryLoaded = false;

        public override void _Ready()
        {
            if (!_contentRegistryLoaded)
            {
                LoadContentRegistry();
                _contentRegistryLoaded = true;
            }
        }

        public void OnClassicModeClicked()
        {
            Maps.Instance.GoToClassicMode();
        }

        public void OnEndlessModeClicked()
        {
            Maps.Instance.GoToEndlessMode();
        }

        private static void LoadContentRegistry()
        {
            foreach (string packId in IdsInFolder("res://Content/CardPacks"))
            {
                string path = $"res://Content/CardPacks/{packId}.json";
                ContentRegistry.ImportCardPack(packId, FileAccess.GetFileAsString(path));
            }

            foreach (string fileNameWithoutExt in IdsInFolder("res://Content/ConsumablePacks"))
            {
                string path = $"res://Content/ConsumablePacks/{fileNameWithoutExt}.json";
                ContentRegistry.ImportConsumables(FileAccess.GetFileAsString(path));
            }

            foreach (string tfId in IdsInFolder("res://Content/Transformations"))
            {
                string path = $"res://Content/Transformations/{tfId}.json";
                ContentRegistry.ImportTransformation(tfId, FileAccess.GetFileAsString(path));
            }

            foreach (string path in FilePathsInFolder("res://Content/CustomResourcePacks"))
            {
                ContentRegistry.ImportCustomResources(FileAccess.GetFileAsString(path));
            }

            foreach (string relicId in IdsInFolder("res://Content/Relics"))
            {
                string path = $"res://Content/Relics/{relicId}.json";
                ContentRegistry.ImportRelic(relicId, FileAccess.GetFileAsString(path));
            }

            IEnumerable<string> IdsInFolder(string folder)
            {
                return DirAccess
                    .GetFilesAt(folder)
                    .Select(f => f.Split(".json")[0]);
            }

            IEnumerable<string> FilePathsInFolder(string folder)
            {
                return DirAccess
                    .GetFilesAt(folder)
                    .Select(f => $"{folder}/{f}");
            }
        }
    }
}