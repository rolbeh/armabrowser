using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace ArmaBrowser.Shader
{
    public class Glow : ShaderEffect
    {
        #region Member Data


        private static PixelShader _pixelShader = new PixelShader();

        #endregion

        #region Constructors

        static Glow()
        {
            _pixelShader.UriSource = Global.MakePackUri("Glow.ps");
        }

        public Glow()
        {
            this.PixelShader = _pixelShader;

            // Update each DependencyProperty that's registered with a shader register.  This
            // is needed to ensure the shader gets sent the proper default value.
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(ThresholdProperty);
            UpdateShaderValue(GlowFactorProperty);

            new DispatcherTimer(TimeSpan.FromMilliseconds(33), DispatcherPriority.Render, Render_OnTick, Dispatcher);

        }

        bool calcUp = true;

        private void Render_OnTick(object sender, EventArgs e)
        {
            var glow = GlowFactor;

            if (glow > 1.5d)
                calcUp = false;

            if (glow < 0.3d)
                calcUp = true;

            if (calcUp)
                GlowFactor = glow + 0.002;
            else
                GlowFactor = glow - 0.002;
            
 
        }

        #endregion

        #region Dependency Properties

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        // Brush-valued properties turn into sampler-property in the shader.
        // This helper sets "ImplicitInput" as the default, meaning the default
        // sampler is whatever the rendering of the element it's being applied to is.
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(Glow), 0);



        public double Threshold
        {
            get { return (double)GetValue(ThresholdProperty); }
            set { SetValue(ThresholdProperty, value); }
        }

        // Scalar-valued properties turn into shader constants with the register
        // number sent into PixelShaderConstantCallback().
        public static readonly DependencyProperty ThresholdProperty =
            DependencyProperty.Register("BloomThreshold", typeof(double), typeof(Glow),
                    new UIPropertyMetadata(0.5d, PixelShaderConstantCallback(0)));




        public double GlowFactor
        {
            get { return (double)GetValue(GlowFactorProperty); }
            set { SetValue(GlowFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GlowFactorProperty =
            DependencyProperty.Register("GlowFactor", typeof(double), typeof(Glow), 
            new UIPropertyMetadata(2d, PixelShaderConstantCallback(1)));



        #endregion


    }
}
