//--------------------------------------------------------------------------
// <copyright file="PackageCommandSet.cs">
//   MS-PL
// </copyright>
// <license>
//   This source code is subject to terms and conditions of the Microsoft 
//   Public License. A copy of the license can be found in the License.html 
//   file at the root of this distribution. 
//   By using this source code in any fashion, you are agreeing to be bound 
//   by the terms of the Microsoft Public License. You must not remove this 
//   notice, or any other, from this software.
// </license>
//-----------------------------------------------------------------------
namespace StyleCop.VisualStudio
{
    using System;
    using System.Diagnostics;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// Handles the menu commands at the Package level.
    /// </summary>
    internal class PackageCommandSet : CommandSet
    {
        #region Private Fields

        /// <summary>
        /// The helper class for performing analysis.
        /// </summary>
        private AnalysisHelper helper;

        /// <summary>
        /// Instance of the analysis core.
        /// </summary>
        private StyleCopCore core;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the PackageCommandSet class.
        /// </summary>
        /// <param name="serviceProvider">Service Provider.</param>
        public PackageCommandSet(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            Param.AssertNotNull(serviceProvider, "serviceProvider");

            this.AddCommands();
        }

        #endregion Public Constructors

        /// <summary>
        /// Initializes the object instance.
        /// </summary>
        protected internal override void Initialize()
        {
            base.Initialize();

            StyleCopVSPackage package = this.ServiceProvider.GetService(typeof(StyleCopVSPackage)) as StyleCopVSPackage;
            Debug.Assert(package != null, "Unable to locate the package");

            this.core = package.Core;
            this.helper = package.Helper;
        }

        #region Private Methods

        /// <summary>
        /// Add Comands for the menu items handled by the package.
        /// </summary>
        private void AddCommands()
        {
            System.Diagnostics.Debug.Assert(this.CommandList != null, "The internal command list has not been created.");

            // StyleCop -> Analyze
            // Solution Explorer Context menu item, for a single project item 
            this.CommandList.Add(
                new OleMenuCommand(
                    new EventHandler(this.InvokeAnalyzeItem),
                    null,
                    new EventHandler(this.StatusAnalyzeSingleProjectItem),
                    CommandIdList.AnalyzeItem));

            // StyleCop -> Analyze This File
            // Code editor Context menu item, for an open code editor
            this.CommandList.Add(
                new OleMenuCommand(
                    new EventHandler(this.InvokeAnalyzeThisFile),
                    null,
                    new EventHandler(this.StatusAnalyzeThisFile),
                    CommandIdList.AnalyzeThisFile));

            // StyleCop -> Analyze This File
            // Code editor Context menu item, for an open code editor
            this.CommandList.Add(
                new OleMenuCommand(
                    new EventHandler(this.InvokeAutoFixThisFile),
                    null,
                    new EventHandler(this.StatusAutoFixThisFile),
                    CommandIdList.AutoFixThisFile));

            // StyleCop -> Analyze 
            // Tools Main Menu And Solution Explorer Context menu item, for the solution
            this.CommandList.Add(
                new OleMenuCommand(
                    new EventHandler(this.InvokeAnalyzeSolution),
                    null,
                    new EventHandler(this.StatusAnalyzeSolution),
                    CommandIdList.AnalyzeSolution));

            // StyleCop -> Reanalyze 
            // Tools Main Menu And Solution Explorer Context menu item, for the solution
            this.CommandList.Add(
                new OleMenuCommand(
                    new EventHandler(this.InvokeReanalyzeSolution),
                    null,
                    new EventHandler(this.StatusAnalyzeSolution),
                    CommandIdList.ReanalyzeSolution));

            // StyleCop -> Analyze
            // Solution Explorer Context menu item, for a single project 
            this.CommandList.Add(
                new OleMenuCommand(
                    new EventHandler(this.InvokeAnalyzeProject),
                    null,
                    new EventHandler(this.StatusAnalyzeProject),
                    CommandIdList.AnalyzeProject));

            // StyleCop -> Analyze
            // Solution Explorer Context menu item, for a folder
            this.CommandList.Add(
                new OleMenuCommand(
                    new EventHandler(this.InvokeAnalyzeFolder),
                    null,
                    new EventHandler(this.StatusAnalyzeFolder),
                    CommandIdList.AnalyzeFolder));

            // StyleCop -> Project Settings
            // Solution Explorer Context menu item
            this.CommandList.Add(
                new OleMenuCommand(
                    new EventHandler(this.InvokeProjectSettings),
                    null,
                    new EventHandler(this.StatusProjectSettings),
                    CommandIdList.ProjectSettings));

            // StyleCop -> Cancel
            // Tools Main menu, Solution Explorer - & code editor Context menu item
            this.CommandList.Add(
                new OleMenuCommand(
                    new EventHandler(this.InvokeCancel),
                    null,
                    new EventHandler(this.StatusCancel),
                    CommandIdList.Cancel));
        }

        /// <summary>
        /// Fired when status must be determined for the menu item "Analyze" at the solution level.
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void StatusAnalyzeSolution(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            bool show = ProjectUtilities.SupportsStyleCop(this.helper, AnalysisType.Solution);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            this.StatusAnalyzeBase(menuCommand, show);
        }

        /// <summary>
        /// Fired when status must be determined for the menu item "Analyze Item" on a single project item.
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void StatusAnalyzeSingleProjectItem(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            bool show = ProjectUtilities.SupportsStyleCop(this.helper, AnalysisType.Item);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            this.StatusAnalyzeBase(menuCommand, show);
        }

        /// <summary>
        /// Fired when status must be determined for the menu item "Analyze" on the context menu of the code editor.
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void StatusAnalyzeThisFile(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            bool show = ProjectUtilities.SupportsStyleCop(this.helper, AnalysisType.File);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            this.StatusAnalyzeBase(menuCommand, show);
        }

        /// <summary>
        /// Fired when status must be determined for the menu item "AutoFix" on the context menu of the code editor.
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void StatusAutoFixThisFile(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            bool show = ProjectUtilities.SupportsStyleCop(this.helper, AnalysisType.File);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            this.StatusAnalyzeBase(menuCommand, show);
        }

        /// <summary>
        /// Fired when status must be determined for the menu item "Analyze Item" on a single project item.
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void StatusAnalyzeFolder(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            bool show = ProjectUtilities.SupportsStyleCop(this.helper, AnalysisType.Folder);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            this.StatusAnalyzeBase(menuCommand, show);
        }

        /// <summary>
        /// Fired when status must be determined for the menu item "Analyze Item" on a single project item.
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void StatusAnalyzeProject(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            // Get the active project.
            bool show = ProjectUtilities.SupportsStyleCop(this.helper, AnalysisType.Project);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            this.StatusAnalyzeBase(menuCommand, show);
        }

        /// <summary>
        /// Basis status handler for the analyzation menu items.
        /// </summary>
        /// <param name="menuCommand">The menu item object to set the satus on.</param>
        /// <param name="visible">Indicates whether the menu item should be visible.</param>
        /// <returns>false if it has been detemined that this menu item shold be hidden/disabled.</returns>
        private bool StatusAnalyzeBase(OleMenuCommand menuCommand, bool visible)
        {
            Param.AssertNotNull(menuCommand, "menuCommand");
            Param.Ignore(visible);

            menuCommand.Supported = true;
            menuCommand.Visible = visible;
            menuCommand.Enabled = visible;
            
            if (visible && this.core.Analyzing)
            {
                menuCommand.Visible = false;
                menuCommand.Enabled = false;
            }

            return menuCommand.Enabled;
        }

        /// <summary>
        /// Fired when the user invokes the menu item "Analyze Item".
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void InvokeAnalyzeItem(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            CommandSet.CheckMenuItemValidity(menuCommand);

            this.helper.Analyze(false, false, AnalysisType.Item);
        }

        /// <summary>
        /// Fired when the user invokes the menu item Analyze from a code editor window.
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void InvokeAnalyzeThisFile(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            CommandSet.CheckMenuItemValidity(menuCommand);

            this.helper.Analyze(false, false, AnalysisType.File);
        }

        /// <summary>
        /// Fired when the user invokes the menu item AutoFix from a code editor window.
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void InvokeAutoFixThisFile(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            CommandSet.CheckMenuItemValidity(menuCommand);

            this.helper.Analyze(false, true, AnalysisType.File);
        }

        /// <summary>
        /// Fired when the user invokes the menu item Analyze on one or several projects.
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void InvokeAnalyzeProject(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            CommandSet.CheckMenuItemValidity(menuCommand);

            this.helper.Analyze(false, false, AnalysisType.Project);
        }

        /// <summary>
        /// Fired when the user invokes the menu item Analyze on the solution.
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void InvokeAnalyzeSolution(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            CommandSet.CheckMenuItemValidity(menuCommand);

            this.helper.Analyze(false, false, AnalysisType.Solution);
        }

        /// <summary>
        /// Fired when the user invokes the menu item Reanalyze on the solution.
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void InvokeReanalyzeSolution(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            CommandSet.CheckMenuItemValidity(menuCommand);

            this.helper.Analyze(true, false, AnalysisType.Solution);
        }

        /// <summary>
        /// Fired when the user invokes the menu item "Analyze Folder".
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void InvokeAnalyzeFolder(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            CommandSet.CheckMenuItemValidity(menuCommand);

            this.helper.Analyze(false, false, AnalysisType.Folder);
        }

        /// <summary>
        /// Fired when status must be determined for the menu item "Global Settings".
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void StatusProjectSettings(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            bool show = ProjectUtilities.SupportsStyleCop(this.helper, AnalysisType.Project);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            this.StatusAnalyzeBase(menuCommand, show);
        }

        /// <summary>
        /// Fired when the user invokes the menu item "Project Settings".
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void InvokeProjectSettings(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            CommandSet.CheckMenuItemValidity(menuCommand);
            this.helper.LocalProjectSettings();
        }

        /// <summary>
        /// Fired when status must be determined for the menu item "Cancel".
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void StatusCancel(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            menuCommand.Supported = true;
            menuCommand.Visible = this.core.Analyzing;
            menuCommand.Enabled = this.core.Analyzing;
        }

        /// <summary>
        /// Fired when the user invokes the menu item "Cancel".
        /// </summary>
        /// <param name="sender">The <c>OleMenuCommand</c> that represents the menu item.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void InvokeCancel(object sender, EventArgs eventArgs)
        {
            Param.AssertNotNull(sender, "sender");
            Param.Ignore(eventArgs);

            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            CommandSet.CheckMenuItemValidity(menuCommand);
            this.helper.Cancel();
        }

        #endregion Private Methods
    }
}
