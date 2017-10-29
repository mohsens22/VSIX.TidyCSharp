﻿using Geeks.VSIX.TidyCSharp.Cleanup.Infra;
using Geeks.VSIX.TidyCSharp.Menus.Cleanup.CommandsHandlers;
using System.Linq;

namespace Geeks.VSIX.TidyCSharp.Cleanup.SimplifyClassFieldDeclaration
{
    public class Options : ICleanupOption
    {
        public const int MAX_FIELD_DECLARATION_LENGTH = 80;

        public Options()
        {
        }

        public CleanupTypes? CleanupItems { get; private set; }

        public int? CleanupItemsInteger => (int?)CleanupItems;

        public void Accept(IMainCleanup mainCleanup)
        {
            if (mainCleanup.MainCleanupItemType == CodeCleanerType.SimplifyClassFieldDeclarations)
            {
                var selectedItems = mainCleanup.GetSubItems().Select(x => (CleanupTypes)x.CleanerType).ToArray();

                CleanupItems = selectedItems.FirstOrDefault();

                foreach (var item in selectedItems)
                {
                    CleanupItems |= item;
                }
            }
        }
    }

}