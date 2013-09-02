using System;
using System.Collections.Generic;

namespace HeroBattleArena.Game.GameObjects
{
    public interface IEntity : IDisposable
    {
        string Name { get; }
        List<AABB> BoundingBoxes { get; }
        bool IsRemoved { get; }
        bool IsVisible { get; }
        bool IsSolid { get; }
        void Initialize();
        void Update(float delta);
        void LateUpdate(float delta);
        void Draw();
        void Show();
        void Hide();
        void Remove();
        void OnCollide(AABB other);
    }
}
