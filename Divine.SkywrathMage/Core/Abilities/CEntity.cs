using System;

using SharpDX;

namespace Divine.Core.Entities
{
    public abstract class CEntity : IEquatable<CEntity>
    {
        protected CEntity(Entity entity)
        {
            Base = entity;

            Handle = entity.Handle;
            Team = entity.Team;
            Name = entity.Name;
            NetworkName = entity.NetworkName;
        }

        internal virtual void Dispose()
        {
        }

        public Entity Base { get; }

        public virtual Unit Owner { get; internal set; }

        public uint Handle { get; }

        public int Index
        {
            get
            {
                return Base.Index;
            }
        }

        public int MaximumHealth
        {
            get
            {
                return Base.MaximumHealth;
            }
        }

        public int Health
        {
            get
            {
                return Base.Health;
            }
        }

        public float CreateTime
        {
            get
            {
                return Base.CreateTime;
            }
        }

        public virtual float Speed
        {
            get
            {
                return Base.Speed;
            }
        }

        public Team Team { get; }

        public string Name { get; }

        public string NetworkName { get; }

        public LifeState LifeState
        {
            get
            {
                return Base.LifeState;
            }
        }

        public Vector3 Angles
        {
            get
            {
                return Base.Angles;
            }
        }

        public virtual bool IsAlive
        {
            get
            {
                return Base.IsAlive;
            }
        }

        public bool IsDormant
        {
            get
            {
                return Base.IsDormant;
            }
        }

        public virtual bool IsValid
        {
            get
            {
                return Base.IsValid;
            }
        }

        public float Rotation
        {
            get
            {
                return Base.Rotation;
            }
        }

        public float RotationRad
        {
            get
            {
                return Base.RotationRad;
            }
        }

        public float Scale
        {
            get
            {
                return Base.Scale;
            }
        }

        public string DisplayName { get; }

        public bool Select(bool addToCurrentSelection)
        {
            return Base.Select(addToCurrentSelection);
        }

        public bool Select()
        {
            return Base.Select();
        }

        public static bool operator ==(CEntity left, CEntity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CEntity left, CEntity right)
        {
            return !Equals(left, right);
        }

        public bool Equals(CEntity entity)
        {
            if (entity is null)
            {
                return false;
            }

            if (ReferenceEquals(this, entity))
            {
                return true;
            }

            return Handle == entity.Handle;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CEntity);
        }

        public override int GetHashCode()
        {
            return (int)Handle;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return Base.NetworkName;
            }

            return Name;
        }

        public static implicit operator Entity(CEntity entity)
        {
            return entity.Base;
        }
    }
}