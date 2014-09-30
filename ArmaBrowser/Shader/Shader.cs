using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ArmaBrowser.Shader
{
    class Shader : ShaderEffect
    {
        #region Member Data

        private static PixelShader _pixelShader = new PixelShader();

        #endregion

        #region Constructors

        static Shader()
        {
            _pixelShader.UriSource = Global.MakePackUri("Shader.ps");
            _pixelShader.ShaderRenderMode = ShaderRenderMode.HardwareOnly;
        }

        public Shader()
        {
            this.PixelShader = _pixelShader;

            // Update each DependencyProperty that's registered with a shader register.  This
            // is needed to ensure the shader gets sent the proper default value.
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(Color1Property);
            UpdateShaderValue(Threshold1Property);

            UpdateShaderValue(Color2Property);
            UpdateShaderValue(Threshold2Property);
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
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(Shader), 0);



        public Color Color1
        {
            get { return (Color)GetValue(Color1Property); }
            set { SetValue(Color1Property, value); }
        }

        // Scalar-valued properties turn into shader constants with the register
        // number sent into PixelShaderConstantCallback().
        public static readonly DependencyProperty Color1Property =
            DependencyProperty.Register("Color1", typeof(Color), typeof(Shader),
                    new UIPropertyMetadata(Colors.Yellow, PixelShaderConstantCallback(0)));



        public double Threshold1
        {
            get { return (double)GetValue(Threshold1Property); }
            set { SetValue(Threshold1Property, value); }
        }

        // Using a DependencyProperty as the backing store for Threshold1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Threshold1Property =
            DependencyProperty.Register("Threshold1", typeof(double), typeof(Shader),
                new UIPropertyMetadata(0.0d, PixelShaderConstantCallback(1)));




        public Color Color2
        {
            get { return (Color)GetValue(Color2Property); }
            set { SetValue(Color2Property, value); }
        }

        // Using a DependencyProperty as the backing store for Color2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Color2Property =
            DependencyProperty.Register("Color2", typeof(Color), typeof(Shader),
                new UIPropertyMetadata(Colors.Yellow, PixelShaderConstantCallback(2)));



        public double Threshold2
        {
            get { return (double)GetValue(Threshold2Property); }
            set { SetValue(Threshold2Property, value); }
        }

        // Using a DependencyProperty as the backing store for Threshold2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Threshold2Property =
            DependencyProperty.Register("Threshold2", typeof(double), typeof(Shader),
                new UIPropertyMetadata(0.0d, PixelShaderConstantCallback(3)));

        #endregion


    }

    
}
