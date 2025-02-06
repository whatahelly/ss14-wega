namespace Content.Shared.Paper;

[RegisterComponent]
public sealed partial class PenComponent : Component
{
    [DataField("signature")]
    public bool Signature = false;
}
