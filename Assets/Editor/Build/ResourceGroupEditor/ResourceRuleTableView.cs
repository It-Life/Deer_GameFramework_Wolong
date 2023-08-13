using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Game.Main.Editor
{
    public class ResourceRuleTableView<T> : TableView<T> where T : class, new()
    {
        public ResourceRuleTableView(List<T> datas, List<TableColumn<T>> columns) : base(datas, columns)
        {
            
        }


        protected override bool CanBeParent(TreeViewItem item)
        {
            return false;
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return true;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            DragAndDrop.PrepareStartDrag();
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            if (IsValidDragDrop())
            {
                return DragAndDropVisualMode.Copy;//Move;
            }
            return DragAndDropVisualMode.Rejected;
        }

        protected bool IsValidDragDrop()
        {
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length != 0)
                return true;

            return false;
        }
    }
}