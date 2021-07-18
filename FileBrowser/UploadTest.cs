using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Nfynt
{

    public class UploadTest : MonoBehaviour
    {

        [SerializeField]
        private Button m_OpenFileBtn;

        [SerializeField]
        private TMP_Text m_FilePathTxt;

        [SerializeField]
        private Button m_UploadFileBtn;

        [SerializeField]
        private Button m_CloseUploadWindowBtn;

        private UploadedFileInfo m_uploadedFile = null;

        private void OnDisable()
        {
#if UNITY_WEBGL
            WebGLFileUploadManager.onFileUploaded -= OnFileSelected;
            WebGLFileUploadManager.Dispose();
#endif
        }

        private void OnEnable()
        {
#if UNITY_WEBGL
            WebGLFileUploadManager.onFileUploaded += OnFileSelected;
#endif
        }

        private void Start()
        {
#if UNITY_WEBGL
            WebGLFileUploadManager.SetDescription( "Select a file" );
            WebGLFileUploadManager.SetAllowedFileName("\\.(png|jpe?g|gif)$");
#endif
        }

        /// <summary>
        ///     Event raised after any file is selected.
        /// </summary>
        /// <param name="result">Uploaded file infos.</param>
        private void OnFileSelected( UploadedFileInfo[] result )
        {
            if ( result.Length == 0 )
            {
                UpdateInfoText( "File selection Error!", 10f );
                m_uploadedFile = null;

                return;
            }

            foreach ( UploadedFileInfo file in result )
            {
                if ( file.isSuccess )
                {
                    m_uploadedFile = file;
                    m_FilePathTxt.text = file.filePath;

                    if ( File.Exists( file.filePath ) )
                    {
                        Debug.Log( "Located the file!");
                        m_UploadFileBtn.interactable = true;
                    }
                    else
                    {
                        Debug.Log( "File not found!");
                        m_UploadFileBtn.interactable = false;
                    }

                    break;
                }
            }

            if ( m_uploadedFile == null )
            {
                m_FilePathTxt.text = "Invalid file(s): " + result[0].name + " (" + result.Length + ")";
                m_UploadFileBtn.interactable = false;
            }
        }

        private void OpenFileExplorer()
        {
            //UpdateInfoText("<b>Error: </b>Local Upload is not supported yet!");
            m_uploadedFile = null;
            m_FilePathTxt.text = "/";
            m_UploadFileBtn.interactable = false;
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLFileUploadManager.Dispose();
            WebGLFileUploadManager.PopupDialog( "Select a file" );
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

            string path = WinFileBrowser.OpenFileDialog( WinFileBrowser.FileType.Ifc, Application.dataPath );

            if ( !string.IsNullOrEmpty( path ) )
            {
                UploadedFileInfo[] mFile = new UploadedFileInfo[1];

                mFile[0] = new UploadedFileInfo();
                FileInfo info = new FileInfo( path );
                mFile[0].name = info.Name;
                mFile[0].type = info.Extension;
                mFile[0].size = ( int ) info.Length;
                mFile[0].filePath = path;
                mFile[0].isSuccess = true;

                OnFileSelected( mFile );
            }
#else
            Debug.Log("<b>Error: </b>Local Upload is not supported in this mode yet!");

#endif
        }
        
    }

}
