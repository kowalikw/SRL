using System;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using Texture = SharpDX.Direct3D9.Texture;

namespace SRL.MonoGameControl
{
    /// <summary>
    /// Wraps the <see cref="D3DImage"/> to make it compatible with Direct3D 11.
    /// </summary>
    /// <remarks>
    /// The <see cref="D3D11"/> should be disposed if no longer needed!
    /// Code source: https://github.com/CartBlanche/MonoGame-Samples/tree/master/WpfInteropSample
    /// </remarks>
    internal class D3D11 : D3DImage, IDisposable
    {
        private D3D9 _d3D9;
        private int _referenceCount;
        private readonly object _d3D9Lock = new object();

        private bool _disposed;
        private Texture _backBuffer;

        public D3D11()
        {
            InitializeD3D9();
        }

        ~D3D11()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources used by an instance of the <see cref="D3D11"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by an instance of the <see cref="D3D11"/> class 
        /// and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; 
        /// <see langword="false"/> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    SetBackBuffer(null);
                    if (_backBuffer != null)
                    {
                        _backBuffer.Dispose();
                        _backBuffer = null;
                    }
                }

                UninitializeD3D9();
                _disposed = true;
            }
        }

        private void InitializeD3D9()
        {
            lock (_d3D9Lock)
            {
                _referenceCount++;
                if (_referenceCount == 1)
                    _d3D9 = new D3D9();
            }
        }

        private void UninitializeD3D9()
        {
            lock (_d3D9Lock)
            {
                _referenceCount--;
                if (_referenceCount == 0)
                {
                    _d3D9.Dispose();
                    _d3D9 = null;
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        /// <summary>
        /// Invalidates the front buffer.
        /// </summary>
        public void Invalidate()
        {
            ThrowIfDisposed();

            if (_backBuffer != null)
            {
                Lock();
                AddDirtyRect(new Int32Rect(0, 0, PixelWidth, PixelHeight));
                Unlock();
            }
        }

        /// <summary>
        /// Sets the back buffer of the <see cref="D3D11"/>.
        /// </summary>
        /// <param name="texture">The Direct3D 11 texture to be used as the back buffer.</param>
        public void SetBackBuffer(Texture2D texture)
        {
            ThrowIfDisposed();

            var previousBackBuffer = _backBuffer;

            _backBuffer = _d3D9.GetSharedTexture(texture);
            if (_backBuffer != null)
            {
                using (Surface surface = _backBuffer.GetSurfaceLevel(0))
                {
                    Lock();
                    SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
                    Unlock();
                }
            }
            else
            {
                Lock();
                SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                Unlock();
            }

            if (previousBackBuffer != null)
                previousBackBuffer.Dispose();
        }
    }
}