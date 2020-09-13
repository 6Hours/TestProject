using Conf;
using Core;

namespace Logic
{
    public class MeleeSoldierUnitLogic : UnitLogic
    {
        private readonly int _attackDistance;
        private readonly int _abilityDamageRate;
        private readonly int _damage;
        private readonly int _manaRegen;
        private readonly int _stunFreeDamage;

        private int _turnReceiveDamage;
        private int _stunned;

        public MeleeSoldierUnitLogic(MeleeSoldierUnitInfo info, IUnit unit, ICore core) : base(unit, core)
        {
            _damage = info.Damage;
            _manaRegen = info.ManaRegen;
            _attackDistance = info.AttackDistance;
            _abilityDamageRate = info.AbilityDamageRate;

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
            _stunned = System.Math.Max(0, _stunned - 1);
            _turnReceiveDamage = 0;
        }
    
        public override void OnAbility()
        {
            var target = Core.GetNearestEnemy(Unit);
            if (target != null && target.IsAlive())
            {
                target.Damage(_abilityDamageRate * _damage);
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