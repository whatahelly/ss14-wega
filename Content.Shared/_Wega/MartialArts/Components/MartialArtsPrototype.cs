using Robust.Shared.Prototypes;

namespace Content.Shared.Martial.Arts.Prototypes
{
    /// <summary>
    /// When creating these types of prototypes
    /// Please do not use them in any way other than recording actions related to combat style and logic.
    /// </summary>
    [Prototype("martialarts")]
    public sealed partial class MartialArtsPrototype : IPrototype
    {
        [IdDataField]
        public string ID { get; private set; } = default!;

        /// <summary>
        /// Please do not assign anything other than actions here.
        /// </summary>
        [DataField("actions")]
        public List<string>? Actions;
    }
}
