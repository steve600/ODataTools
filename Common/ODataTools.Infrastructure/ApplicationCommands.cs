using Prism.Commands;

namespace ODataTools.Infrastructure
{
    /// <summary>
    /// Class with global application commands
    /// </summary>
    public static class ApplicationCommands
    {
        /// <summary>
        /// Show flyout command
        /// </summary>
        public static CompositeCommand ShowFlyoutCommand = new CompositeCommand();

        /// <summary>
        /// Show project on GitHub
        /// </summary>
        public static DelegateCommand ShowOnGitHubCommand = new DelegateCommand(ShowOnGitHub);

        /// <summary>
        /// Show on GitHub
        /// </summary>
        private static void ShowOnGitHub()
        {
            System.Diagnostics.Process.Start("https://github.com/steve600/ODataTools");
        }

        /// <summary>
        /// Show application info
        /// </summary>
        public static CompositeCommand ShowApplicationInfoCommand = new CompositeCommand();

        /// <summary>
        /// Navigate to a view
        /// </summary>
        public static DelegateCommand<string> NavigateToViewCommand = null;
    }

    public interface IApplicationCommands
    {
        CompositeCommand ShowFlyoutCommand { get; }
        DelegateCommand ShowOnGitHubCommand { get; }
        CompositeCommand ShowApplicationInfoCommand { get; }
        DelegateCommand<string> NavigateToViewCommand { get; set; }
    }

    public class ApplicationCommandsProxy : IApplicationCommands
    {
        /// <summary>
        /// Show flyout command
        /// </summary>
        public CompositeCommand ShowFlyoutCommand
        {
            get { return ApplicationCommands.ShowFlyoutCommand; }
        }

        /// <summary>
        /// Show project on GitHub
        /// </summary>
        public DelegateCommand ShowOnGitHubCommand
        {
            get { return ApplicationCommands.ShowOnGitHubCommand; }
        }

        /// <summary>
        /// Show application info command
        /// </summary>
        public CompositeCommand ShowApplicationInfoCommand
        {
            get { return ApplicationCommands.ShowApplicationInfoCommand; }
        }

        /// <summary>
        /// Navigate to view command
        /// </summary>
        public DelegateCommand<string> NavigateToViewCommand
        {
            get { return ApplicationCommands.NavigateToViewCommand; }
            set { ApplicationCommands.NavigateToViewCommand = value; }
        }
    }
}
