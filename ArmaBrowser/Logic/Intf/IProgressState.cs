namespace ArmaBrowser.Logic
{
    interface IProgressState
    {
        int Maximum { get; set; }

        int Minimum { get; set; }

        int Value { get; set; }
    }
}
