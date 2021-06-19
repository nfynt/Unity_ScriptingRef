using UnityEngine;

namespace nfynt
{

    // simple fade script - attach to the main camera to be able to fade
    public class FadeCamera : MonoBehaviour
    {
        [SerializeField]
        private Color m_FadeColor = Color.black;

        [SerializeField]
        private Material m_FadeMaterial = null;

        [SerializeField]
        private float m_DefaultDuration = 1f;

        private static readonly int s_PropertyHashOpacity = Shader.PropertyToID( "_Opacity" );

        private float m_CurrentOpacity = 0f;

        private float m_StartTime = 0f;
        private float m_StartOpacity = 0f;
        private float m_EndOpacity = 1f;
        private float m_Duration = 0f;

        public bool IsFading { get; private set; } = false;

        #region Unity Event Functions

        private void Awake()
        {
            if ( m_FadeMaterial == null )
            {
                m_FadeMaterial = new Material( Shader.Find( "Custom/FadeCamera" ) );
            }

            m_FadeMaterial.color = m_FadeColor;
        }

        private void OnRenderImage( RenderTexture source, RenderTexture destination )
        {
            if ( IsFading && m_Duration > 0f )
            {
                m_CurrentOpacity = Mathf.Lerp( m_StartOpacity, m_EndOpacity, ( Time.unscaledTime - m_StartTime ) / m_Duration );
                IsFading = Time.unscaledTime - m_StartTime <= m_Duration;
            }

            if ( m_CurrentOpacity <= 0f )
            {
                Graphics.Blit( source, destination );

                return;
            }

            m_FadeMaterial.SetFloat( s_PropertyHashOpacity, m_CurrentOpacity );
            Graphics.Blit( source, destination, m_FadeMaterial );
        }

        #endregion

        #region Public

        public void FadeIn()
        {
            FadeIn( m_DefaultDuration );
        }

        public void FadeIn( float duration )
        {
            m_Duration = duration;
            m_StartTime = Time.unscaledTime;
            m_StartOpacity = m_CurrentOpacity;
            m_EndOpacity = 0;
            IsFading = true;
        }

        public void FadeOut()
        {
            FadeOut( m_DefaultDuration );
        }

        public void FadeOut( float duration )
        {
            m_Duration = duration;
            m_StartTime = Time.unscaledTime;
            m_StartOpacity = m_CurrentOpacity;
            m_EndOpacity = 1;
            IsFading = true;
        }

        #endregion
    }

}
