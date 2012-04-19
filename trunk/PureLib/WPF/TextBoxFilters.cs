using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PureLib.WPF {
    public static class TextBoxFilters {
        // List of allowed keys. Put them here if you want to allow that key to pressed
        private static readonly List<Key> _controlKeys = new List<Key>
                                                            {
                                                                Key.Back,
                                                                Key.CapsLock,
                                                                Key.Down,
                                                                Key.End,
                                                                Key.Enter,
                                                                Key.Escape,
                                                                Key.Home,
                                                                Key.Insert,
                                                                Key.Left,
                                                                Key.PageDown,
                                                                Key.PageUp,
                                                                Key.Right,
                                                                Key.LeftShift,
                                                                Key.RightShift,
                                                                Key.Tab,
                                                                Key.Up
                                                            };

        #region Alpha Numeric Filter

        public static DependencyProperty IsAlphaNumericFilterProperty = DependencyProperty.RegisterAttached(
            "IsAlphaNumericFilter", typeof(bool), typeof(TextBoxFilters), new PropertyMetadata(false, IsAlphaNumericFilterChanged));

        public static bool GetIsAlphaNumericFilter(DependencyObject src) {
            return (bool)src.GetValue(IsAlphaNumericFilterProperty);
        }

        public static void SetIsAlphaNumericFilter(DependencyObject src, bool value) {
            src.SetValue(IsAlphaNumericFilterProperty, value);
        }

        public static void IsAlphaNumericFilterChanged(DependencyObject src, DependencyPropertyChangedEventArgs args) {
            if ((src != null) && (src is TextBox)) {
                TextBox textBox = (TextBox)src;
                InputMethod.SetIsInputMethodEnabled(src, false);

                if ((bool)args.NewValue) {
                    textBox.KeyDown += AlphaNumericTextBoxKeyDown;
                    DataObject.AddPastingHandler(textBox, AlphaNumericTextBoxPasting);
                }
            }
        }

        private static void AlphaNumericTextBoxKeyDown(object sender, KeyEventArgs e) {
            e.Handled = (!_controlKeys.Contains(e.Key) && !IsDigit(e.Key) && !IsLetter(e.Key));
        }

        private static void AlphaNumericTextBoxPasting(object sender, DataObjectPastingEventArgs e) {
            TextBoxPasting("^[a-zA-Z0-9 ]+$", e);
        }

        #endregion

        #region Numeric Filter

        public static DependencyProperty IsNumericFilterProperty = DependencyProperty.RegisterAttached(
            "IsNumericFilter", typeof(bool), typeof(TextBoxFilters), new PropertyMetadata(false, IsNumericFilterChanged));

        public static bool GetIsNumericFilter(DependencyObject src) {
            return (bool)src.GetValue(IsNumericFilterProperty);
        }

        public static void SetIsNumericFilter(DependencyObject src, bool value) {
            src.SetValue(IsNumericFilterProperty, value);
        }

        public static void IsNumericFilterChanged(DependencyObject src, DependencyPropertyChangedEventArgs args) {
            if ((src != null) && (src is TextBox)) {
                TextBox textBox = (TextBox)src;
                InputMethod.SetIsInputMethodEnabled(src, false);

                if ((bool)args.NewValue) {
                    textBox.PreviewKeyDown += FilterSpaceKeyDown;
                    textBox.KeyDown += NumericTextBoxKeyDown;
                    DataObject.AddPastingHandler(textBox, NumericTextBoxPasting);
                }
            }
        }

        private static void NumericTextBoxKeyDown(object sender, KeyEventArgs e) {
            e.Handled = (!_controlKeys.Contains(e.Key) && !IsDigit(e.Key));
        }

        private static void NumericTextBoxPasting(object sender, DataObjectPastingEventArgs e) {
            TextBoxPasting("^[0-9]+$", e);
        }

        #endregion

        #region Private Methods

        private static void FilterSpaceKeyDown(object sender, KeyEventArgs e) {
            e.Handled = (e.Key == Key.Space);
        }

        private static void TextBoxPasting(string regex, DataObjectPastingEventArgs e) {
            if (e.DataObject.GetDataPresent(typeof(string))) {
                string value = e.DataObject.GetData(typeof(string)).ToString();
                if (!Regex.IsMatch(value, regex))
                    e.CancelCommand();
            }
        }

        private static bool IsDigit(Key key) {
            return (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && (key >= Key.D0) && (key <= Key.D9))
                || ((key >= Key.NumPad0) && (key <= Key.NumPad9));
        }

        private static bool IsLetter(Key key) {
            return (key >= Key.A) && (key <= Key.Z);
        }

        #endregion
    }
}
