using NStack;
using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui.Extensions
{
    public static class ViewExtensions
    {

        public static KeyValuePair<ustring, Label> AddField(this View previous, string label, View container)
        {
            Label labelView;

            if (previous.Equals(container))
                labelView = FirstLabel(label);
            else
                labelView = previous.UnderLabel(label);

            var fieldView = labelView.CreateFieldFor();

            container.Add(labelView, fieldView);

            return new KeyValuePair<ustring, Label>(labelView.Id, fieldView);
        }

        private static Label FirstLabel(string label) => new($"{label}: ") { X = 0, Y = 0, Id = label };
        private static Label UnderLabel(this View view, string label) => new($"{label}: ") { X = 0, Y = Pos.Bottom(view), Id = label };
        private static Label CreateFieldFor(this View view) => new() { X = Pos.Right(view), Y = view.Y, Height = 1 };
    }
}
