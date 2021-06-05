using ICSharpCode.AvalonEdit;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NioTupApp.Behaviours
{
    public sealed class AvalonEditBehaviour : Behavior<TextEditor>
    {
        public static readonly DependencyProperty ChangeOnLostFocusProperty =
           DependencyProperty.Register("ChangeOnLostFocus", typeof(bool), typeof(AvalonEditBehaviour),
           new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty GiveMeTheTextProperty =
            DependencyProperty.Register("GiveMeTheText", typeof(string), typeof(AvalonEditBehaviour),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        public string GiveMeTheText
        {
            get { return (string)GetValue(GiveMeTheTextProperty); }
            set { SetValue(GiveMeTheTextProperty, value); }
        }

        public bool ChangeOnLostFocus
        {
            get { return (bool)GetValue(ChangeOnLostFocusProperty); }
            set { SetValue(ChangeOnLostFocusProperty, value); }
        }

        private bool bChanged;

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
                AssociatedObject.PreviewLostKeyboardFocus += AssociatedObject_PreviewLostKeyboardFocus;
            }
        }

        private void AssociatedObject_PreviewLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            var textEditor = sender as TextEditor;

            if (textEditor != null)
            {
                if (ChangeOnLostFocus && bChanged)
                {
                    bChanged = false;
                    if (textEditor.Document != null)
                        GiveMeTheText = textEditor.Document.Text;
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
            {
                AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
                AssociatedObject.PreviewLostKeyboardFocus -= AssociatedObject_PreviewLostKeyboardFocus;
            }
        }

        private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
        {
            var textEditor = sender as TextEditor;
            if (textEditor != null)
            {
                bChanged = true;
                if (!ChangeOnLostFocus && textEditor.Document != null)
                    GiveMeTheText = textEditor.Document.Text;
            }
        }

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = dependencyObject as AvalonEditBehaviour;
            if (behavior.AssociatedObject != null)
            {
                var editor = behavior.AssociatedObject as TextEditor;
                if (editor.Document != null)
                {
                    var caretOffset = editor.CaretOffset;
                    if (dependencyPropertyChangedEventArgs.NewValue == null)
                    {
                        editor.Document.Text = string.Empty;
                    }
                    else
                    {
                        editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue.ToString();
                    }
                    editor.CaretOffset = (editor.Document.Text.Length > caretOffset) ? caretOffset : editor.Document.Text.Length;
                }
            }
        }
    }
}
