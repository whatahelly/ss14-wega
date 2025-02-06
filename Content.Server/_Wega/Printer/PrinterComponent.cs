namespace Content.Server.PrinterInsert
{
    [RegisterComponent]
    public sealed partial class PrinterComponent : Component
    {
        [DataField]
        public string UserName = string.Empty;

        [DataField]
        public string UserJob = string.Empty;
    }
}
