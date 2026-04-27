namespace DancingGoat.Widgets
{
    /// <summary>
    /// View model used to render widget configuration details in Page Builder preview/edit modes.
    /// </summary>
    public class ConditionalCustomActivityWidgetViewModel
    {
        public string ConditionType { get; init; } = string.Empty;

        public string ConditionActivityType { get; init; } = string.Empty;

        public string ConditionActivityValue { get; init; } = string.Empty;

        public string ContactGroup { get; init; } = string.Empty;

        public string ActionActivityType { get; init; } = string.Empty;

        public string ActionActivityValue { get; init; } = string.Empty;

        public string ExecuteWhen { get; init; } = string.Empty;

        public bool IsEditMode { get; init; }
        public bool IsPreviewMode { get; init; }
        
        public bool IsEditOrPreviewMode => IsEditMode || IsPreviewMode;
    }
}
