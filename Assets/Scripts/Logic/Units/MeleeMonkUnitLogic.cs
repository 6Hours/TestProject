using System;
using Conf;
using Core;

namespace Logic
{
    public class MeleeMonkUnitLogic : UnitLogic
    {
        private readonly int _attackDistance;
        private readonly int _healDamagePercent;
        private readonly int _abilityShieldAbsorbingPercent;
        private readonly int _abilityShieldExplosionDamage;
        private readonly int _abilityShieldExplosionHeal;
        private readonly int _damage;
        private readonly int _manaRegen;
        private readonly int _stunFreeDamage;

        private int _abilityShieldValue;

        private int _turnReceiveDamage;
        private int _stunned;

        public MeleeMonkUnitLogic(MeleeMonkUnitInfo info, IUnit unit, ICore core) : base(unit, core)
        {
            _damage = info.Damage;
            _manaRegen = info.ManaRegen;
            _attackDistance = info.AttackDistance;
            _abilityShieldValue = info.AbilityShieldValue;
            _healDamagePercent = info.HealDamagePercent;
            _abilityShieldAbsorbingPercent = info.AbilityShieldAbsorbingPercent;
            _abilityShieldExplosionDamage = info.AbilityShieldExplosionDamage;
            _abilityShieldExplosionHeal = info.AbilityShieldExplosionHeal;
            _stunFreeDamage = info.StunFreeDamage;
            _turnReceiveDamage = 0;
            _stunned = 0;
        }

        public override void OnTurn()
        {
            if (_stunned == 0)
            {
                var target = Core.GetNearestEnemy(Unit);
                if (target != null && target.IsAlive())
                {
                    if (Core.GetDistance(Unit, target) > _attackDistance)
                    {
                        Unit.MoveTo(target.X, target.Y);
                    }
                    else
                    {
                        target.Damage(_damage);
                    }
                }
                Unit.AddMana(_manaRegen);
            }
            Unit.Heal((int)Math.Round(_turnReceiveDamage * _healDamagePercent / 100f));
            _stunned = Math.Max(0, _stunned - 1);
            _turnReceiveDamage = 0;
        }

        public override int OnDamage(int damage)
        {
            if (_abilityShieldValue > 0)
            {
                _abilityShieldValue -= (int)Math.Round(damage * _abilityShieldAbsorbingPercent / 100f);
                damage -= (int)Math.Round(damage * _abilityShieldAbsorbingPercent / 100f);
                damage += _abilityShieldValue > 0 ? 0 : -_abilityShieldValue;
                if(_abilityShieldValue < 0)
                {
                    var target = Core.GetNearestEnemy(Unit);
                    if (target != null && target.IsAlive())
                    {
                        target.Damage(_abilityShieldExplosionDamage);
                    }
                    Unit.Heal(_abilityShieldExplosionHeal);
                }
            }
            _turnReceiveDamage += damage;
            _stunned = _turnReceiveDamage > _stunFreeDamage ? 0 : _stunned;
            return damage;
        }

        public override int OnBeforeManaChange(int delta)
        {
            if (delta > 0)
            {
                _abilityShieldValue += _abilityShieldValue > 0 ? _manaRegen : 0;
                delta = 0;
            }
            return delta;
        }

        public override void OnDie()
        {
            var target = Core.GetNearestFriend(Unit);
            if (target != null && target.IsAlive())
            {
                target.Heal(_abilityShieldValue);
            }
        }

        public override void OnStun()
        {
            _stunned = 3;
        }
    }
}

