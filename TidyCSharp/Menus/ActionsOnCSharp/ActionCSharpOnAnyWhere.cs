using System;
using System.Threading.Tasks;
using EnvDTE;
using Geeks.GeeksProductivityTools.Utils;
using Geeks.VSIX.TidyCSharp.Cleanup.CommandsHandlers;
using static Geeks.GeeksProductivityTools.Menus.ActionsOnCSharp.CSharpActionDelegate;

namespace Geeks.GeeksProductivityTools.Menus.ActionsOnCSharp
{
    public class ActionCSharpOnAnyWhere
    {
        public static async Task Invoke(TargetAction action, CleanupOptions cleanupOptions)
        {
            await Task.Run(() => SyncInvoke(action, cleanupOptions));
        }
        static void SyncInvoke(TargetAction action, CleanupOptions cleanupOptions)
        {
            try
            {
                var ideSelectedItems = DteServiceProvider.Instance.SelectedItems;

                for (int itemIndex = 1; itemIndex <= ideSelectedItems.Count; itemIndex++)
                {
                    var selectItem = ideSelectedItems.Item(itemIndex);

                    var selectedProjectItem = selectItem.ProjectItem;

                    if (selectedProjectItem != null)
                    {
                        if (selectedProjectItem.ProjectItems == null || selectedProjectItem.ProjectItems.Count == 0)
                        {
                            action(selectedProjectItem, cleanupOptions, true);
                        }
                        else
                        {
                            ActionCSharpOnProjectItem.Action(selectedProjectItem, action, cleanupOptions);
                        }
                    }
                    else if (selectItem.Project != null)
                    {
                        ActionCSharpOnProject.Invoke(action, cleanupOptions);
                    }
                    else
                    {
                        ActionCSharpOnSolution.Invoke(action, cleanupOptions);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorNotification.EmailError(e);
                ProcessActions.GeeksProductivityToolsProcess();
            }
        }

        private static void DoActionForItems(ProjectItems projectItems, TargetAction action, CleanupOptions cleanupOptions)
        {
            for (int subItemIndex = 1; subItemIndex <= projectItems.Count; subItemIndex++)
            {
                var subItem = projectItems.Item(subItemIndex);
                ActionCSharpOnProjectItem.Action(subItem, action, cleanupOptions);
            }
        }
    }
}
