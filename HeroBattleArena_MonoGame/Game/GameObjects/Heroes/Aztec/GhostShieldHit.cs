using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class GhostShieldHit: HitObject
    {
    Vector2 m_dir;
	float m_time = 0;
	float m_startAngle = 0;
	float m_radius = 0;
	float m_speed = 0;
	float m_animationAngle = 0;
        public GhostShieldHit(float angle, Unit owner): base(owner)
        {
            m_startAngle = angle;
        }

public override void Initialize()
{
	Life = Configuration.GetValue("GhostShieldHit_Life");
	Damage = Configuration.GetValue("GhostShieldHit_Damage");
	m_radius = Configuration.GetValue("GhostShieldHit_Radius");
	m_speed = Configuration.GetValue("GhostShieldHit_Speed");

    Vector2 position = Position;

	AABB boundingBox = new AABB();
	boundingBox.Owner = this;
	boundingBox.LayerMask = AABBLayers.LayerSensitiveProjectile;
    boundingBox.CollisionMask = AABBLayers.CollisionSensitiveProjectile;
	boundingBox.MinX = position.X - 20;
	boundingBox.MaxX = position.X + 20;
	boundingBox.MinY = position.Y - 20;
	boundingBox.MaxY = position.Y + 20;
	AddAABB(boundingBox);

	AnimationManager animations = new AnimationManager(80, 80, 20, 1);
	animations.AddAnimation(new Animation(1,0,0,false,0));
	Animations = animations;

	DrawOffset = new Vector2(position.X - 40,	position.Y - 40);

    Texture = Graphics.GetTexture("aztec_ghost");
}

public override void Update(float delta){
	
    
   base.Update(delta);
	m_time+=delta*m_speed;

	Vector2 newPos = new Vector2(0,0);
	Vector2 parPos = Owner.Position;

    newPos.X = (float)parPos.X + (float)Math.Cos(m_time + m_startAngle) * m_radius;
    newPos.Y = (float)parPos.Y + (float)Math.Sin(m_time + m_startAngle) * m_radius;
    m_animationAngle = (float)Math.IEEERemainder(m_time + m_startAngle + 3.1415f / 2, 3.1415f * 2.0f);
	m_animationAngle /= 3.1415f*2;
    m_animationAngle = 10.0f - (float)Math.Floor(m_animationAngle * 20);


	Position = newPos;

    if (!Owner.IsAlive){
        Life = 0;
    }
}

public override void OnCollide( AABB otherCol){
	if (otherCol.Owner == Owner) return;
	Unit other = (otherCol.Owner as Unit);
	if(other != null)
	{
		if((other).IsAlive){
            SoundCenter.Instance.Play(SoundNames.AztecGhostShieldHit);
			List<Buff> buffs = (other).Buffs;
				for(int i = 0; i < buffs.Count; i++)
				{
					if(buffs[i] is MicroBotBuff)
					{
						buffs[i].Remove();
					}
					if(buffs[i] is ElectricCelerityBuff)
					{
                        buffs[i].Remove();
					}
                    if (buffs[i] is GhostShieldBuff)
					{
                        buffs[i].Remove();
					}
				}
			(other).ApplyDamage(Damage, Owner);
			Remove();
			return;
		}

	}

	DamageObject dmgobj = otherCol.Owner as DamageObject; 
	if(dmgobj != null && (dmgobj as SacredTorch == null))
	{
		if(dmgobj.Owner != Owner)
		{
			dmgobj.Remove();	

			Remove();
		}
	}
}

public override void Draw()
{
	if(IsVisible)
	{
		/*Rectangle rect = getDrawSourceRect();
		rect.x = m_animationAngle*80;
		rect.y = 0;*/
        
		Graphics.Draw(
			Texture,
			DrawOffset,
            1.0f, 
			new Rectangle((int)m_animationAngle*80, 0, 80, 80),
			(float)(Layer + (float)Position.Y/768.0),Color.White);
	}

    for(int i = 0; i < BoundingBoxes.Count; ++i) {
        Graphics.DrawAABB( BoundingBoxes[i], DebugAABBMode.Body);
    }
}
    }
}
