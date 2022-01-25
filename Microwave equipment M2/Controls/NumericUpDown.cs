using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                new PropertyMetadata(
                    new PropertyChangedCallback(ValueChangedCallback)));

        public int Value
        {
            get
            {
                return (int)GetValue(ValueProperty);
            }

            set
            {
                SetValue(ValueProperty, value);
            }
        }

        private static void ValueChangedCallback(DependencyObject obj,
            DependencyPropertyChangedEventArgs args)
        {
            NumericUpDown ctl = (NumericUpDown)obj;
            int newValue = (int)args.NewValue;

            // Call UpdateStates because the Value might have caused the
            // control to change ValueStates.
            ctl.UpdateStates(true);

            // Call OnValueChanged to raise the ValueChanged event.
            ctl.OnValueChanged(
                new ValueChangedEventArgs(NumericUpDown.ValueChangedEvent,
                    newValue));
        }

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct,
                          typeof(ValueChangedEventHandler), typeof(NumericUpDown));

        public event ValueChangedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        protected virtual void OnValueChanged(ValueChangedEventArgs e)
        {
            // Raise the ValueChanged event so applications can be alerted
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

        private RepeatButton downButtonElement;

        private RepeatButton DownButtonElement
        {
            get
            {
                return downButtonElement;
            }

            set
            {
                if (downButtonElement != null)
                {
                    downButtonElement.Click -=
                        new RoutedEventHandler(downButtonElement_Click);
                }
                downButtonElement = value;

                if (downButtonElement != null)
                {
                    downButtonElement.Click +=
                        new RoutedEventHandler(downButtonElement_Click);
                }
            }
        }

        void downButtonElement_Click(object sender, RoutedEventArgs e)
        {
            Value--;
        }

        private RepeatButton upButtonElement;

        private RepeatButton UpButtonElement
        {
            get
            {
                return upButtonElement;
            }

            set
            {
                if (upButtonElement != null)
                {
                    upButtonElement.Click -=
                        new RoutedEventHandler(upButtonElement_Click);
                }
                upButtonElement = value;

                if (upButtonElement != null)
                {
                    upButtonElement.Click +=
                        new RoutedEventHandler(upButtonElement_Click);
                }
            }
        }

        void upButtonElement_Click(object sender, RoutedEventArgs e)
        {
            Value++;
        }

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
