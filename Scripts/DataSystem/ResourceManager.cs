using System.Collections.Generic;
using System.IO;
using CharacterSystem.Stat;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace DataSystem
{
    public static class ResourceManager
    {
        private static Dictionary<string, GameObject> _prefabs = new();
        private static Dictionary<string, Sprite> _sprites = new();
        private static Dictionary<string, SpriteLibraryAsset> _spriteLibraryAssets = new();

        private static Sprite _defaultSprite;
        
        private const string PrefabPath = Constants.Path.PrefabPath;
        private const string SpritePath = Constants.Path.SpritePath;
        private const string SpriteLibraryPath = Constants.Path.SpriteLibraryPath;
        
        public static void Init()
        {
            LoadAllAssets();

            _defaultSprite = GetSprite(Constants.Path.DefaultIconPath);
        }

        private static void LoadAllAssets()
        {
            var sprites = Resources.LoadAll(SpritePath, typeof(Sprite));
            foreach (var sprite in sprites)
            {
                _sprites.TryAdd(sprite.name, (Sprite)sprite);
            }
            foreach (var subfolder in Constants.Path.SpriteSubfolders)
            {
                sprites = Resources.LoadAll(SpritePath + subfolder, typeof(Sprite));
                foreach (var sprite in sprites)
                {
                    _sprites.TryAdd(sprite.name, (Sprite)sprite);
                }
            }

            var prefabs = Resources.LoadAll(PrefabPath, typeof(GameObject));
            foreach (var prefab in prefabs)
            {
                _prefabs.TryAdd(prefab.name, (GameObject)prefab);
            }
            foreach (var subfolder in Constants.Path.PrefabSubfolders)
            {
                prefabs = Resources.LoadAll(PrefabPath + subfolder, typeof(GameObject));
                foreach (var prefab in prefabs)
                {
                    _prefabs.TryAdd(prefab.name, (GameObject)prefab);
                }
            }
            
            var libraryAssets = Resources.LoadAll(SpriteLibraryPath, typeof(SpriteLibraryAsset));
            foreach (var asset in libraryAssets)
            {
                _spriteLibraryAssets.TryAdd(asset.name, (SpriteLibraryAsset)asset);
            }
            foreach (var subfolder in Constants.Path.SpriteLibrarySubfolders)
            {
                libraryAssets = Resources.LoadAll(SpriteLibraryPath + subfolder, typeof(SpriteLibraryAsset));
                foreach (var asset in libraryAssets)
                {
                    _spriteLibraryAssets.TryAdd(asset.name, (SpriteLibraryAsset)asset);
                }
            }
        }

        public static GameObject GetPrefab(string name)
        {
            if (_prefabs.TryGetValue(name, out var prefab))
            {
                return prefab;
            }

            // if cannot find prefab in dictionary, try to load prefab
            var loadPrefab = Resources.Load<GameObject>(Path.Combine(PrefabPath, name));
            if (loadPrefab == null)
            {
                foreach (var subfolder in Constants.Path.PrefabSubfolders)
                {
                    loadPrefab = Resources.Load<GameObject>(Path.Combine(PrefabPath + subfolder, name));
                }

                if (loadPrefab == null)
                {
                    Debug.LogError($"GetPrefab: name:[{name}] is not exist.");
                    return null;
                }
            }
            _prefabs.TryAdd(loadPrefab.name, loadPrefab);
            return loadPrefab;
        }

        public static Sprite GetSprite(string name)
        {
            if (_sprites.TryGetValue(name, out var sprite))
            {
                return sprite;
            }
            
            // if cannot find sprite in dictionary, try to load sprite
            var loadSprite = Resources.Load<Sprite>(Path.Combine(SpritePath, name));
            if (loadSprite == null)
            {
                foreach (var subfolder in Constants.Path.SpriteSubfolders)
                {
                    loadSprite = Resources.Load<Sprite>(Path.Combine(SpritePath + subfolder, name));
                }

                if (loadSprite == null)
                {
                    Debug.LogWarning($"GetSprite: [{name}] is not exist. Returns default sprite");
                    return _defaultSprite;
                }
            }
            _sprites.TryAdd(loadSprite.name, loadSprite);
            return loadSprite;
        }

        public static Sprite GetAttributeSprite(AttributeType type) => GetSprite(Constants.Path.AttributeIconPath[type]);

        public static Sprite GetDefaultSprite() => _defaultSprite;

        public static SpriteLibraryAsset GetSpriteLibrary(string name)
        {
            if (_spriteLibraryAssets.TryGetValue(name, out var asset))
            {
                return asset;
            }
            
            // if cannot find sprite in dictionary, try to load sprite
            var loadLibraryAsset = Resources.Load<SpriteLibraryAsset>(Path.Combine(SpriteLibraryPath, name));
            if (loadLibraryAsset == null)
            {
                foreach (var subfolder in Constants.Path.SpriteSubfolders)
                {
                    loadLibraryAsset = Resources.Load<SpriteLibraryAsset>(Path.Combine(SpriteLibraryPath + subfolder, name));
                }

                if (loadLibraryAsset == null)
                {
                    Debug.LogWarning($"GetSpriteLibrary: [{name}] is not exist. Returns null");
                    return null;
                }
            }
            _spriteLibraryAssets.TryAdd(loadLibraryAsset.name, loadLibraryAsset);
            return loadLibraryAsset;
        }
    }
}