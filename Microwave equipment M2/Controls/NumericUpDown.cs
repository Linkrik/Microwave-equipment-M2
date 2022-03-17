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
    //[TemplateVisualState(Name = "Positive", GroupName = "ValueStates")]
    //[TemplateVisualState(Name = "Negative", GroupName = "ValueStates")]
    //[TemplateVisualState(Name = "Focused", GroupName = "FocusedStates")]
    //[TemplateVisualState(Name = "Unfocused", GroupName = "FocusedStates")]
    public class NumericUpDown : Control
    {
        public NumericUpDown()
        {
            DefaultStyleKey = typeof(NumericUpDown);
            this.IsTabStop = true;
            
        }
        
        #region Value property
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(decimal), typeof(NumericUpDown),
                new PropertyMetadata(0.0m,
                    new PropertyChangedCallback(ValueChangedCallback),
                    new CoerceValueCallback(CoerceValue)));


        public decimal Value
        {
            get => (decimal)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }


        private static object CoerceValue(DependencyObject element, object value)
        {
            decimal newValue = (decimal)value;
            NumericUpDown control = (NumericUpDown)element;

            newValue = Math.Max(control.MinValue, Math.Min(control.MaxValue, newValue));

            return newValue;
        }


        /// <summary>
        /// ChangedCallback for Value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private static void ValueChangedCallback(DependencyObject obj,
                                                 DependencyPropertyChangedEventArgs args)
        {
            NumericUpDown control = (NumericUpDown)obj;

            #region Run command
            if (control.Command != null)
            {
                if (control.Command.CanExecute(control.Value))
                {
                    control.Command.Execute(control.Value);
                }
            }
            #endregion

            // Call UpdateStates because the Value might have caused the
            // control to change ValueStates.
            //control.UpdateStates(true);

            RoutedPropertyChangedEventArgs<decimal> e = new RoutedPropertyChangedEventArgs<decimal>(
                (decimal)args.OldValue, (decimal)args.NewValue, ValueChangedEvent);
            
            // Call OnValueChanged to raise the ValueChanged event.
            control.OnValueChanged(e);
        }


        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the ValueChanged event.</param>
        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<decimal> e)
        {
            // Raise the ValueChanged event so applications can be alerted
            // when Value changes.
            RaiseEvent(e);
        }


        /// <summary>
        /// Identifies the ValueChanged routed event.
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, //RoutingStrategy.Direct
                                             typeof(RoutedPropertyChangedEventHandler<decimal>),
                                             typeof(NumericUpDown));


        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<decimal> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
        #endregion

        #region MaxValue property
        public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register(
            "MaxValue", typeof(decimal), typeof(NumericUpDown),
            new PropertyMetadata(decimal.MaxValue));


        public decimal MaxValue
        {
            get => (decimal)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }
        #endregion

        #region MinValue property
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(
                "MinValue", typeof(decimal), typeof(NumericUpDown),
                new PropertyMetadata(decimal.MinValue));
        

        public decimal MinValue
        {
            get => (decimal)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }
        #endregion

        #region StepValue property
        public static readonly DependencyProperty StepValueProperty =
            DependencyProperty.Register(
                "StepValue", typeof(decimal), typeof(NumericUpDown),
                new PropertyMetadata(1.0m),
                new ValidateValueCallback(ShirtValidateCallback));


        public decimal StepValue
        {
            get => (decimal)GetValue(StepValueProperty);
            set => SetValue(StepValueProperty, value);
        }

        private static bool ShirtValidateCallback(object value)
        {
            decimal stepValue = (decimal)value;
            return 0 < stepValue;
        }
        #endregion

        #region Command and CommandParameter property 
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command", typeof(ICommand), typeof(NumericUpDown));

        public static readonly DependencyProperty CommandParameterProperty =
           DependencyProperty.Register(
               "CommandParameter", typeof(ICommand), typeof(NumericUpDown));


        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        //public ICommand CommandParameter
        //{
        //    get => (ICommand)GetValue(CommandParameterProperty);
        //    set => SetValue(CommandParameterProperty, value);
        //}

        #endregion


        //----------------------------------------------------------------------

        //private void UpdateStates(bool useTransitions)
        //{
        //    if (Value >= 0)
        //    {
        //        VisualStateManager.GoToState(this, "Positive", useTransitions);
        //    }
        //    else
        //    {
        //        VisualStateManager.GoToState(this, "Negative", useTransitions);
        //    }

        //    if (IsFocused)
        //    {
        //        VisualStateManager.GoToState(this, "Focused", useTransitions);
        //    }
        //    else
        //    {
        //        VisualStateManager.GoToState(this, "Unfocused", useTransitions);
        //    }
        //}

        public override void OnApplyTemplate()
        {
            UpButtonElement = GetTemplateChild("UpButton") as RepeatButton;
            DownButtonElement = GetTemplateChild("DownButton") as RepeatButton;
            //TextElement = GetTemplateChild("TextBlock") as TextBlock;

            //UpdateStates(false);
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
                    _upButtonElement.Click -= new RoutedEventHandler(upButtonElement_Click);
                }
                _upButtonElement = value;

                if (_upButtonElement != null)
                {
                    _upButtonElement.Click += new RoutedEventHandler(upButtonElement_Click);
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
            //UpdateStates(true);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            //UpdateStates(true);
            //todo: Написать проверку на валидацию вводимого текста (Я думаю нужно использовать TryPars для проверки число это или нет)
        }
    }

}
