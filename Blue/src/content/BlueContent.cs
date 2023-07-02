using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace BlueFw.Content;

/// <summary>
/// Loads and manages game content.
/// </summary>
public class BlueContent : IDisposable {

    /// <summary>
    /// The root directory containing the game's content files.
    /// </summary>
    public string RootContentDirectory {
        get => rootContentDir;
        set {
            rootContentDir = value;
            xnaContent.RootDirectory = value;
        }
    }

    Dictionary<string, object> loadedAssetsByName = new Dictionary<string, object>();
    HashSet<IDisposable> disposableAssets = new HashSet<IDisposable>();

    readonly GraphicsDevice graphicsDevice;
    readonly ContentManager xnaContent;

    string rootContentDir;
    bool disposed;

    internal BlueContent(BlueContent clone) :
        this(clone.graphicsDevice, new ContentManager(clone.xnaContent.ServiceProvider), clone.rootContentDir) { }

    internal BlueContent(GraphicsDevice graphicsDevice, ContentManager xnaContent, string rootContentDir) {
        this.graphicsDevice = graphicsDevice;
        this.xnaContent = xnaContent;
        this.rootContentDir = rootContentDir;

        xnaContent.RootDirectory = rootContentDir;
    }

    /// <summary>
    /// Loads a 2D texture directly from a bmp, gif, jpg, png, tif or dds.
    /// </summary>
    /// <param name="name">The asset name relative to the root content directory.</param>
    /// <param name="premultiplyAlpha">Whether or not to premultiply the RBG values of pixels by its alpha.</param>
    public Texture2D LoadTexture(string name, bool premultiplyAlpha = true) {
        Validate(name);
        return LoadTexture(name, premultiplyAlpha ? DefaultColorProcessors.PremultiplyAlpha : DefaultColorProcessors.ZeroTransparentPixels);
    }

    /// <summary>
    /// Loads a 2D texture directly from a bmp, gif, jpg, png, tif or dds.
    /// </summary>
    /// <param name="name">The asset name relative to the root content directory.</param>
    /// <param name="colorProcessor">Function that is applied to the data in RGBA format before the texture is sent to video memory. Could be null (no processing).</param>
    public Texture2D LoadTexture(string name, Action<byte[]> colorProcessor) {
        Validate(name);

        if (TryGetLoadedAsset(name, out Texture2D tex)) {
            return tex;
        }

        using Stream stream = OpenAssetStream(name);
        Texture2D texture = Texture2D.FromStream(graphicsDevice, stream, colorProcessor);

        loadedAssetsByName.Add(name, texture);
        disposableAssets.Add(texture);

        return texture;
    }

    /// <summary>
    /// Loads content using the XNA content pipeline.
    /// </summary>
    public T LoadXNB<T>(string name, bool localized = false) {
        Validate(name);

        if (Path.HasExtension(name)) {
            name = Path.ChangeExtension(name, null);
        }

        return localized ? xnaContent.LoadLocalized<T>(name) : xnaContent.Load<T>(name);
    }

    /// <summary>
    /// Unloads and disposes mulitple assets.
    /// </summary>
    public void UnloadAssets(IEnumerable<string> assetNames) {
        Validate(assetNames);

        foreach (string name in assetNames) {
            UnloadAsset(name);
        }
    }

    /// <summary>
    /// Unloads and disposes an asset.
    /// </summary>
    public void UnloadAsset(string assetName) {
        Validate(assetName);

        if (loadedAssetsByName.TryGetValue(assetName, out object asset)) {
            if (asset is IDisposable disposable) {
                disposable.Dispose();
                disposableAssets.Remove(disposable);
            }

            loadedAssetsByName.Remove(assetName);
        }
        else {
            xnaContent.UnloadAsset(assetName);
        }
    }

    /// <summary>
    /// Unloads all currently loaded assets.
    /// </summary>
    public void Unload() {
        foreach (IDisposable disposable in disposableAssets) {
            disposable?.Dispose();
        }

        disposableAssets.Clear();
        loadedAssetsByName.Clear();

        xnaContent.Unload();
    }

    bool TryGetLoadedAsset<T>(string name, out T asset) {
        if (loadedAssetsByName.TryGetValue(name, out object val) && val is T loadedAsset) {
            asset = loadedAsset;
            return true;
        }

        asset = default;
        return false;
    }

    Stream OpenAssetStream(string name) {
        string path = Path.Combine(RootContentDirectory, name);
        return Path.IsPathRooted(path) ? File.OpenRead(path) : TitleContainer.OpenStream(path);
    }

    void Validate(object param) {
        if (param == null) {
            throw new ArgumentNullException(nameof(param), $"A null parameter was passed into a {nameof(BlueContent)} function");
        }
        if (disposed) {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }

    #region Disposing

    ~BlueContent() {
        Dispose(false);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    internal void Dispose(bool disposing) {
        if (disposed) {
            return;
        }

        if (disposing) {
            Unload();
            xnaContent.Dispose();
        }

        disposed = true;
    }

    #endregion
}