using Godot;

namespace TFCardBattle.Godot
{
    public static class AnimationPlayerExtensions
    {
        public static void Reset(this AnimationPlayer anim)
        {
            anim.Play("RESET");
            anim.Advance(0);
        }

        public static void PlayAndAdvance(
            this AnimationPlayer anim,
            string name,
            float customBlend = -1,
            float customSpeed = 1,
            bool fromEnd = false
        )
        {
            anim.Play(name, customBlend, customSpeed, fromEnd);
            anim.Advance(0);
        }

        public static void ResetAndPlay(
            this AnimationPlayer anim,
            string name,
            float customBlend = -1,
            float customSpeed = 1,
            bool fromEnd = false
        )
        {
            anim.Reset();
            anim.PlayAndAdvance(name, customBlend, customSpeed, fromEnd);
        }

        public static void PlayFixedDuration(
            this AnimationPlayer anim,
            string name,
            float duration,
            float customBlend = -1,
            bool fromEnd = false
        )
        {
            float animLength = anim.GetAnimation(name).Length;

            anim.ResetAndPlay(
                name,
                customBlend: customBlend,
                customSpeed: animLength / duration,
                fromEnd: fromEnd
            );
        }
    }
}