namespace TFCardBattle.Core
{
    public readonly record struct CustomResourceId(string Id)
    {
        public static implicit operator CustomResourceId(string id) => new CustomResourceId(id);
        public static implicit operator string(CustomResourceId t) => t.Id;
        public override string ToString() => Id;
    }

    public class CustomResource
    {
        public string Name;
        public string IconPath;
    }
}