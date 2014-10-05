using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ArmaBrowser.Shader
{
    class multishader_frag : ShaderEffect
    {

        #region Member Data

        private static PixelShader _pixelShader = new PixelShader();

        #endregion

        #region Constructors

        static multishader_frag()
        {
            _pixelShader.UriSource = Global.MakePackUri("multishader_frag.ps");
        }

        public multishader_frag()
        {
            this.PixelShader = _pixelShader;

            // Update each DependencyProperty that's registered with a shader register.  This
            // is needed to ensure the shader gets sent the proper default value.
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(MasterProperty);
            UpdateShaderValue(GlowIntensityProperty);
            UpdateShaderValue(BlurIntensityProperty);
            UpdateShaderValue(BlurRadiusProperty);
            UpdateShaderValue(NoiseIntensityProperty);
            UpdateShaderValue(TextureHeightProperty);
            UpdateShaderValue(TextureWidthProperty);
            UpdateShaderValue(DirectionHorizontalProperty);
           
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
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(multishader_frag), 0);



        //public Color ColorFilter
        //{
        //    get { return (Color)GetValue(ColorFilterProperty); }
        //    set { SetValue(ColorFilterProperty, value); }
        //}

        //// Scalar-valued properties turn into shader constants with the register
        //// number sent into PixelShaderConstantCallback().
        //public static readonly DependencyProperty ColorFilterProperty =
        //    DependencyProperty.Register("ColorFilter", typeof(Color), typeof(multishader_frag),
        //            new UIPropertyMetadata(Colors.Yellow, PixelShaderConstantCallback(0)));



        public float Master
        {
            get { return (float)GetValue(MasterProperty); }
            set { SetValue(MasterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Master.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MasterProperty =
            DependencyProperty.Register("Master", typeof(float), typeof(multishader_frag),
            new UIPropertyMetadata(1.0f, PixelShaderConstantCallback(0)));




        public float GlowIntensity
        {
            get { return (float)GetValue(GlowIntensityProperty); }
            set { SetValue(GlowIntensityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for glow_intensity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GlowIntensityProperty =
            DependencyProperty.Register("GlowIntensity", typeof(float), typeof(multishader_frag),
                    new UIPropertyMetadata(0.38f, PixelShaderConstantCallback(8)));




        public float BlurIntensity
        {
            get { return (float)GetValue(BlurIntensityProperty); }
            set { SetValue(BlurIntensityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BlurIntensity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlurIntensityProperty =
            DependencyProperty.Register("BlurIntensity", typeof(float), typeof(multishader_frag),
                    new UIPropertyMetadata(0.7f, PixelShaderConstantCallback(6)));





        public double BlurRadius
        {
            get { return (double)GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BlurRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register("BlurRadius", typeof(double), typeof(multishader_frag),
                new UIPropertyMetadata(2.7d, PixelShaderConstantCallback(6)));




        public float NoiseIntensity
        {
            get { return (float)GetValue(NoiseIntensityProperty); }
            set { SetValue(NoiseIntensityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NoiseIntensity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoiseIntensityProperty =
            DependencyProperty.Register("NoiseIntensity", typeof(float), typeof(multishader_frag),
                    new UIPropertyMetadata(.004f, PixelShaderConstantCallback(9)));




        public float TextureWidth
        {
            get { return (float)GetValue(TextureWidthProperty); }
            set { SetValue(TextureWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextureWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextureWidthProperty =
            DependencyProperty.Register("TextureWidth", typeof(float), typeof(multishader_frag),
            new UIPropertyMetadata(512f, PixelShaderConstantCallback(1)));



        public float TextureHeight
        {
            get { return (float)GetValue(TextureHeightProperty); }
            set { SetValue(TextureHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for textureHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextureHeightProperty =
            DependencyProperty.Register("TextureHeight", typeof(float), typeof(multishader_frag),
                new UIPropertyMetadata(512f, PixelShaderConstantCallback(2)));




        public double DirectionHorizontal
        {
            get { return (double)GetValue(DirectionHorizontalProperty); }
            set { SetValue(DirectionHorizontalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DirectionHorizontal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DirectionHorizontalProperty =
            DependencyProperty.Register("DirectionHorizontal", typeof(double), typeof(multishader_frag),
                new UIPropertyMetadata(2.0d, PixelShaderConstantCallback(3)));



        #endregion

    }
}
