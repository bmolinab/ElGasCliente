using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElGas.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterMenu : ContentView
    {
        public event EventHandler ItemTapped;

        /// <summary>
        /// Bindable Property isOpen
        /// </summary>

        private bool _isAnimating = false;
        private uint _animationDelay = 300;

        public FilterMenu()
        {
            InitializeComponent();

            InnerButtonClose.IsVisible = false;
            InnerButtonMenu.IsVisible = true;
            HandleMenuCenterClicked();
            HandleCloseClicked();

            HandleOptionsClicked();

        }

        private async void abrir()
        {
            if (!_isAnimating)
            {
                _isAnimating = true;

                InnerButtonClose.IsVisible = true;
                InnerButtonMenu.IsVisible = true;

                InnerButtonMenu.RotateTo(360, _animationDelay);
                InnerButtonMenu.FadeTo(0, _animationDelay);
                InnerButtonClose.RotateTo(360, _animationDelay);
                InnerButtonClose.FadeTo(1, _animationDelay);

                await OuterCircle.ScaleTo(3.3, 1000, Easing.BounceIn);
                await ShowButtons();
                InnerButtonMenu.IsVisible = false;

                _isAnimating = false;

            }
        }

        private void HandleOptionsClicked()
        {
            HandleOptionClicked(Dte, "Date");
            HandleOptionClicked(Chck, "Check");
            HandleOptionClicked(Dir, "Direction");
        }

        // BindableProperty implementation
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FilterMenu), null);
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static void Execute(ICommand command)
        {
            if (command == null) return;
            if (command.CanExecute(null))
            {
                command.Execute(null);
            }
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(FilterMenu), null);
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }







        // this is the command that gets bound by the control in the view
        // (ie. a Button, TapRecognizer, or MR.Gestures)



        private void HandleOptionClicked(Image image, string value)
        {
            image.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                // Command = new Command(() => Execute(Command)
                //),

                Command = new Command(() =>
                {
                    Text = value;
                    ItemTapped?.Invoke(this, new SelectedItemChangedEventArgs(value));
                    Execute(Command);
                    CloseMenu();
                }),

                NumberOfTapsRequired = 1
            });
        }







        private void HandleCloseClicked()
        {
            InnerButtonClose.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await CloseMenu();
                }),
                NumberOfTapsRequired = 1
            });

        }

        private async Task CloseMenu()
        {
            if (!_isAnimating)
            {

                _isAnimating = true;

                InnerButtonMenu.IsVisible = true;
                InnerButtonClose.IsVisible = true;
                await HideButtons();

                InnerButtonClose.RotateTo(0, _animationDelay);
                InnerButtonClose.FadeTo(0, _animationDelay);
                InnerButtonMenu.RotateTo(0, _animationDelay);
                InnerButtonMenu.FadeTo(1, _animationDelay);
                await OuterCircle.ScaleTo(1, 1000, Easing.BounceOut);
                InnerButtonClose.IsVisible = false;

                _isAnimating = false;
            }
        }

        private void HandleMenuCenterClicked()
        {
            InnerButtonMenu.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    if (!_isAnimating)
                    {
                        _isAnimating = true;

                        InnerButtonClose.IsVisible = true;
                        InnerButtonMenu.IsVisible = true;

                        InnerButtonMenu.RotateTo(360, _animationDelay);
                        InnerButtonMenu.FadeTo(0, _animationDelay);
                        InnerButtonClose.RotateTo(360, _animationDelay);
                        InnerButtonClose.FadeTo(1, _animationDelay);

                        await OuterCircle.ScaleTo(3.3, 1000, Easing.BounceIn);
                        await ShowButtons();
                        InnerButtonMenu.IsVisible = false;

                        _isAnimating = false;

                    }
                }),
                NumberOfTapsRequired = 1
            });

        }

        private async Task HideButtons()
        {
            var speed = 25U;
            await Dte.FadeTo(0, speed);
            await Chck.FadeTo(0, speed);

            await Dir.FadeTo(0, speed);
        }

        private async Task ShowButtons()
        {
            var speed = 25U;
            await Dte.FadeTo(1, speed);
            await Chck.FadeTo(1, speed);
            await Dir.FadeTo(1, speed);
        }
    }
}