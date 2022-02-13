using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Microwave_equipment_M2.Controls
{
    /// <summary>
    /// Выполните шаги 1a или 1b, а затем 2, чтобы использовать этот пользовательский элемент управления в файле XAML.
    ///
    /// Шаг 1a. Использование пользовательского элемента управления в файле XAML, существующем в текущем проекте.
    /// Добавьте атрибут XmlNamespace в корневой элемент файла разметки, где он 
    /// будет использоваться:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Microwave_equipment_M2.Controls"
    ///
    ///
    /// Шаг 1б. Использование пользовательского элемента управления в файле XAML, существующем в другом проекте.
    /// Добавьте атрибут XmlNamespace в корневой элемент файла разметки, где он 
    /// будет использоваться:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Microwave_equipment_M2.Controls;assembly=Microwave_equipment_M2.Controls"
    ///
    /// Потребуется также добавить ссылку из проекта, в котором находится файл XAML,
    /// на данный проект и пересобрать во избежание ошибок компиляции:
    ///
    ///     Щелкните правой кнопкой мыши нужный проект в обозревателе решений и выберите
    ///     "Добавить ссылку"->"Проекты"->[Поиск и выбор проекта]
    ///
    ///
    /// Шаг 2)
    /// Теперь можно использовать элемент управления в файле XAML.
    ///
    ///     <MyNamespace:NumericUpDown/>
    ///
    /// </summary>
    [TemplatePart(Name = "UpButtonElement", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "DownButtonElement", Type = typeof(RepeatButton))]
    [TemplateVisualState(Name = "Positive", GroupName = "ValueStates")]
    [TemplateVisualState(Name = "Negative", GroupName = "ValueStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusedStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusedStates")]
    public class NumericUpDown : Control
    {
        public NumericUpDown()
        {
            DefaultStyleKey = typeof(NumericUpDown);
            this.IsTabStop = true;
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(int), typeof(NumericUpDown),
                new PropertyMetadata(0,
                    new PropertyChangedCallback(ValueChangedCallback),
                    new CoerceValueCallback(CoerceValue)));

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(
                "MaxValue", typeof(int), typeof(NumericUpDown),
                new PropertyMetadata(int.MaxValue));

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(
                "MinValue", typeof(int), typeof(NumericUpDown),
                new PropertyMetadata(int.MinValue));

        public static readonly DependencyProperty StepValueProperty =
            DependencyProperty.Register(
                "StepValue", typeof(int), typeof(NumericUpDown),
                new PropertyMetadata(1));


        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public int MinValue
        {
            get => (int)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public int StepValue
        {
            get => (int)GetValue(StepValueProperty);
            set => SetValue(StepValueProperty, value);
        }

        private static object CoerceValue(DependencyObject element, object value)
        {
            int newValue = (int)value;
            NumericUpDown control = (NumericUpDown)element;

            newValue = Math.Max(control.MinValue, Math.Min(control.MaxValue, newValue));

            return newValue;
        }


        private static object CoerceStepValue(DependencyObject element, object value)
        {
            int newValue = (int)value;
            NumericUpDown control = (NumericUpDown)element;

            newValue = Math.Max(1, newValue);

            return newValue;
        }


        private static void ValueChangedCallback(DependencyObject obj,
            DependencyPropertyChangedEventArgs args)
        {
            NumericUpDown control = (NumericUpDown)obj;
            int newValue = (int)args.NewValue;

            // Call UpdateStates because the Value might have caused the
            // control to change ValueStates.
            control.UpdateStates(true);

            // Call OnValueChanged to raise the ValueChanged event.
            control.OnValueChanged(
                new ValueChangedEventArgs(NumericUpDown.ValueChangedEvent,
                    newValue));
        }

        private static void StepValueChangedCallback(DependencyObject obj,
            DependencyPropertyChangedEventArgs args)
        {
            NumericUpDown control = (NumericUpDown)obj;
            int newValue = (int)args.NewValue;

            // Call OnValueChanged to raise the ValueChanged event.
            control.OnStepValueChanged(
                new ValueChangedEventArgs(NumericUpDown.ValueChangedEvent,
                    newValue));
        }


        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct,
                          typeof(ValueChangedEventHandler), typeof(NumericUpDown));

        public static readonly RoutedEvent StepValueChangedEvent =
            EventManager.RegisterRoutedEvent("StepValueChanged", RoutingStrategy.Bubble,
                          typeof(ValueChangedEventHandler), typeof(NumericUpDown));

        public event ValueChangedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public event ValueChangedEventHandler StepValueChanged
        {
            add { AddHandler(StepValueChangedEvent, value); }
            remove { RemoveHandler(StepValueChangedEvent, value); }
        }

        protected virtual void OnValueChanged(ValueChangedEventArgs e)
        {
            // Raise the ValueChanged event so applications can be alerted
            // when Value changes.
            RaiseEvent(e);
        }

        protected virtual void OnStepValueChanged(ValueChangedEventArgs e)
        {
            // Raise the StepValueChanged event so applications can be alerted
            // when Value changes.
            RaiseEvent(e);
        }


        private void UpdateStates(bool useTransitions)
        {
            if (Value >= 0)
            {
                VisualStateManager.GoToState(this, "Positive", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Negative", useTransitions);
            }

            if (IsFocused)
            {
                VisualStateManager.GoToState(this, "Focused", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Unfocused", useTransitions);
            }
        }

        public override void OnApplyTemplate()
        {
            UpButtonElement = GetTemplateChild("UpButton") as RepeatButton;
            DownButtonElement = GetTemplateChild("DownButton") as RepeatButton;
            //TextElement = GetTemplateChild("TextBlock") as TextBlock;

            UpdateStates(false);
        }

        private RepeatButton _downButtonElement;

        private RepeatButton DownButtonElement
        {
            get
            {
                return _downButtonElement;
            }

            set
            {
                if (_downButtonElement != null)
                {
                    _downButtonElement.Click -=
                        new RoutedEventHandler(downButtonElement_Click);
                }
                _downButtonElement = value;

                if (_downButtonElement != null)
                {
                    _downButtonElement.Click +=
                        new RoutedEventHandler(downButtonElement_Click);
                }
            }
        }

        void downButtonElement_Click(object sender, RoutedEventArgs e) => Value -= StepValue;

        private RepeatButton _upButtonElement;

        private RepeatButton UpButtonElement
        {
            get
            {
                return _upButtonElement;
            }

            set
            {
                if (_upButtonElement != null)
                {
                    _upButtonElement.Click -=
                        new RoutedEventHandler(upButtonElement_Click);
                }
                _upButtonElement = value;

                if (_upButtonElement != null)
                {
                    _upButtonElement.Click +=
                        new RoutedEventHandler(upButtonElement_Click);
                }
            }
        }

        void upButtonElement_Click(object sender, RoutedEventArgs e) => Value += StepValue;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            Focus();
        }


        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            UpdateStates(true);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            UpdateStates(true);
        }
    }

    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);

    public class ValueChangedEventArgs : RoutedEventArgs
    {
        private int _value;

        public ValueChangedEventArgs(RoutedEvent id, int num)
        {
            _value = num;
            RoutedEvent = id;
        }

        public int Value
        {
            get { return _value; }
        }
    }
}
