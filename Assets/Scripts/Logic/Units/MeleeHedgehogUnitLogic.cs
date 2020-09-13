using System;
using Conf;
using Core;

namespace Logic
{
    public class MeleeHedgehogUnitLogic : UnitLogic
    {
        private readonly int _attackDistance;
        private readonly int _healHpThreshold;
        private readonly int _healValue;
        private readonly int _abilityDamageIncreaseStep;
        private readonly int _damage;
        private readonly int _manaRegen;
        private readonly int _stunFreeDamage;

        private int _abilityDamage;
        private int _turnReceiveDamage;
        private int _stunned;

        public MeleeHedgehogUnitLogic(MeleeHedgehogUnitInfo info, IUnit unit, ICore core) : base(unit, core)
        {
            _damage = info.Damage;
            _manaRegen = info.ManaRegen;
            _abilityDamage = 0;
            _healHpThreshold = (int)Math.Round(info.MaxHealth * info.HealHpThresholdPercent / 100f);
            _healValue = info.HealValue;
            _attackDistance = info.AttackDistance;
            _abilityDamageIncreaseStep = info.AbilityDamageIncreaseStep;

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
            _stunned = Math.Max(0, _stunned - 1);
            _turnReceiveDamage = 0;
        }
    
        public override void OnAbility()
        {
            var target = Core.GetNearestEnemy(Unit);
            if (target != null && target.IsAlive() && Core.GetDistance(Unit, target) < _attackDistance)
            {
                target.Damage(_abilityDamage);
            }
        }
    
        public override int OnDamage(int damage)
        {
            _turnReceiveDamage += damage;
            _stunned = _turnReceiveDamage > _stunFreeDamage ? 0 : _stunned;
            if (damage >= _healHpThreshold)
            {
                Unit.Heal(_healValue);
            }
            return damage;
        }
    
        public override int OnBeforeManaChange(int delta)
        {
            if (delta > 0)
            {
                _abilityDamage += _abilityDamageIncreaseStep;
            }
            return delta;
        }

        public override void OnStun()
        {
            _stunned = 3;
        }
    }
}