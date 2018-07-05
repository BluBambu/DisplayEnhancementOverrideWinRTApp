using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace DisplayEnhancementOverride
{
    using Windows.Graphics.Display;

    public sealed partial class MainPage : Page
    {
        private DisplayEnhancementOverride deo;

        public MainPage()
        {
            this.InitializeComponent();

            deo = DisplayEnhancementOverride.GetForCurrentView();

            SetNoBrightnessSettings();
            SetNoColorScenario();

            deo.IsOverrideActiveChanged += Deo_IsOverrideActiveChanged;
            deo.CanOverrideChanged += Deo_CanOverrideChanged;
            deo.DisplayEnhancementOverrideCapabilitiesChanged += Deo_DisplayEnhancementOverrideCapabilitiesChanged;
        }

        #region DEO Callbacks   

        private void Deo_DisplayEnhancementOverrideCapabilitiesChanged(DisplayEnhancementOverride sender, DisplayEnhancementOverrideCapabilitiesChangedEventArgs args)
        {
            BrightnessPercentageSupportedStateTextBlock.Text = args.Capabilities.IsBrightnessControlSupported ? "Yes" : "No";
            BrightnessNitsSupportedStateTextBlock.Text = args.Capabilities.IsBrightnessNitsControlSupported ? "Yes" : "No";
        }

        private void Deo_CanOverrideChanged(DisplayEnhancementOverride sender, object args)
        {
            CanOverrideActiveStateTextBlock.Text = sender.CanOverride ? "Yes" : "No";
        }

        private void Deo_IsOverrideActiveChanged(DisplayEnhancementOverride sender, object args)
        {
            IsOverrideActiveStateTextBlock.Text = sender.IsOverrideActive ? "Yes" : "No";
        }

        #endregion // DEO Callbacks

        #region Brightness Settings

        private void SetBrightnessPercentage(double level)
        {
            BrightnessSettingStateTextBlock.Text = level + "%";
            deo.BrightnessOverrideSettings = BrightnessOverrideSettings.CreateFromLevel(level / 100);
            CheckOverrideToggleEnableState();
        }

        private void SetBrightnessNits(float nits)
        {
            BrightnessSettingStateTextBlock.Text = nits + " nits";
            deo.BrightnessOverrideSettings = BrightnessOverrideSettings.CreateFromNits(nits);
            CheckOverrideToggleEnableState();
        }

        private void SetBrightnessScenario(DisplayBrightnessOverrideScenario scenario)
        {
            string scenarioText = "";
            switch (scenario)
            {
                case DisplayBrightnessOverrideScenario.FullBrightness:
                    scenarioText = "Full Brightness";
                    break;
                case DisplayBrightnessOverrideScenario.BarcodeReadingBrightness:
                    scenarioText = "Barcode Brightness";
                    break;
                case DisplayBrightnessOverrideScenario.IdleBrightness:
                    scenarioText = "Idle Brightness";
                    break;
            }

            BrightnessSettingStateTextBlock.Text = scenarioText;
            deo.BrightnessOverrideSettings = BrightnessOverrideSettings.CreateFromDisplayBrightnessOverrideScenario(scenario);
            CheckOverrideToggleEnableState();
        }

        private void SetNoBrightnessSettings()
        {
            BrightnessSettingStateTextBlock.Text = "None";
            deo.BrightnessOverrideSettings = null;
            CheckOverrideToggleEnableState();
        }

        #endregion // Brightness Settings

        #region Color Settings

        private void SetColorScenario(DisplayColorOverrideScenario scenario)
        {
            string scenarioText = "";
            switch (scenario)
            {
                case DisplayColorOverrideScenario.Accurate:
                    scenarioText = "Accurate Colors";
                    break;
            }

            ColorSettingStateTextBlock.Text = scenarioText;
            deo.ColorOverrideSettings = ColorOverrideSettings.CreateFromDisplayColorOverrideScenario(DisplayColorOverrideScenario.Accurate);
            CheckOverrideToggleEnableState();
        }

        private void SetNoColorScenario()
        {
            ColorSettingStateTextBlock.Text = "None";
            deo.ColorOverrideSettings = null;
            CheckOverrideToggleEnableState();
        }

        #endregion // Color Settings

        #region Brightness Controls

        private void PercentageBrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SetBrightnessPercentage(e.NewValue);
        }

        private void FullBrightnessScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            SetBrightnessScenario(DisplayBrightnessOverrideScenario.FullBrightness);
        }

        private void BarcodeBrightnessScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            SetBrightnessScenario(DisplayBrightnessOverrideScenario.BarcodeReadingBrightness);
        }

        private void IdleBrightnessScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            SetBrightnessScenario(DisplayBrightnessOverrideScenario.IdleBrightness);
        }

        private void NoneBrightnessScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            SetNoBrightnessSettings();
        }

        #endregion // Brightness Controls

        #region Color Controls


        private void AccurateColorScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            SetColorScenario(DisplayColorOverrideScenario.Accurate);
        }

        private void NonColorScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            SetNoColorScenario();
        }

        #endregion // Color Controls

        #region General Controls

        private void CheckOverrideToggleEnableState()
        {
            if (((deo.ColorOverrideSettings != null) || (deo.BrightnessOverrideSettings != null)))
            {
                DebugTextBlock.Text = "";
                OverrideToggle.IsEnabled = true;
            }
            else if (!OverrideToggle.IsOn)
            {
                DebugTextBlock.Text = "Please select a brightness or color setting before requesting an override";
                OverrideToggle.IsEnabled = false;
            }
        }

        private void OverrideToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                if (toggleSwitch.IsOn)
                {
                    deo.RequestOverride();
                }
                else
                {
                    CheckOverrideToggleEnableState();

                    deo.StopOverride();
                }
            }
        }

        #endregion // General Controls

    }
}
