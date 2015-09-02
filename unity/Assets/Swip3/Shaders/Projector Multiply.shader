    Shader "Projector/Projector Multiply"
    {
        Properties
        {
            _Color ("Main Colour", Color) = (1,1,1,0)
            _ShadowTex ("Cookie", 2D) = "gray" { TexGen ObjectLinear }
        }
     
        Subshader
        {
            Tags { "RenderType"="Transparent"  "Queue"="Transparent+100"}
            Pass
            {
                ZWrite Off
                Offset -1, -1
     
                Fog { Mode Off }
               
                //AlphaTest Greater .1
                AlphaTest Less 1
                ColorMask RGB
                Blend One SrcAlpha
     
                SetTexture [_ShadowTex]
                {
                    constantColor [_Color]
                    combine texture * constant, One - texture
                    Matrix [_Projector]
                }
            }
        }
    }
     
