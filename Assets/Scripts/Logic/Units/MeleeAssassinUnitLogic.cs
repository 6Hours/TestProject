using Conf;
using Core;
using UnityEngine;

namespace Logic
{
    public class MeleeAssassinUnitLogic : UnitLogic
    {
        private readonly int _attackDistance;
        private readonly int _abilityDamageRate;
        private readonly int _critChance;
        private readonly int _critRate;
        private readonly int _damage;
        private readonly int _manaRegen;
        private readonly int _stunFreeDamage;

        private int _turnReceiveDamage;
        private int _stunned;

        public MeleeAssassinUnitLogic(MeleeAssassinUnitInfo info, IUnit unit, ICore core) : base(unit, core)
        {
            _damage = info.Damage;
            _manaRegen = info.ManaRegen;
            _attackDistance = info.AttackDistance;
            _abilityDamageRate = info.AbilityDamageRate;
            _critChance = info.CritChance;
            _critRate = info.CritRate;

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
                    if (Core.GetDistance(Unit, target) > 1)
                    {
                        Unit.MoveTo(target.X, target.Y);
                    }
                    else
                    {
                        var damage = Random.Range(0, 100) < _critChance ? _damage * _critRate : _damage;
                        target.Damage(damage);
                    }
                }
                Unit.AddMana(_manaRegen);
            }
            _stunned = System.Math.Max(0, _stunned - 1);
            _turnReceiveDamage = 0;
        }
    
        public override void OnAbility()
        {
            var target = Core.GetNearestEnemy(Unit);
            if (target != null && target.IsAlive() && Core.GetDistance(Unit, target) <= _attackDistance)
            {
                target.Damage(_damage * _abilityDamageRate);
            }
        }

        public override int OnDamage(int damage)
        {
            _turnReceiveDamage += damage;
            _stunned = _turnReceiveDamage > _stunFreeDamage ? 0 : _stunned;
            return damage;
        }

        public override void OnStun()
        {
            _stunned = 3;
        }
    }
}