﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Testing.Skills
{
    public class 力量爆发 : Skill
    {
        public override long Id => (long)SuperSkillID.力量爆发;
        public override string Name => "力量爆发";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double EPCost => 100;
        public override double CD => 55;
        public override double HardnessTime { get; set; } = 0;

        public 力量爆发(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 力量爆发特效(this));
        }
    }

    public class 力量爆发特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => "力量爆发";
        public override string Description => $"获得 150% 力量 [ {攻击力加成:0.##} ] 的攻击力加成，但每次攻击都会损失 9% 当前生命值 [ {当前生命值:0.##} ]，持续 {Duration:0.##} 时间。";
        public override bool TargetSelf => true;
        public override bool Durative => true;
        public override double Duration => 10 + 1 * (Level - 1);

        private double 攻击力加成 => Skill.Character?.STR * 1.5 ?? 0;
        private double 当前生命值 => Skill.Character?.HP * 0.09 ?? 0;
        private double 实际攻击力加成 = 0;

        public override void OnEffectGained(Character character)
        {
            实际攻击力加成 = 攻击力加成;
            character.ExATK2 += 实际攻击力加成;
            WriteLine($"[ {character} ] 的攻击力增加了 [ {实际攻击力加成:0.##} ] ！");
        }

        public override void OnEffectLost(Character character)
        {
            // 恢复到原始攻击力
            character.ExATK2 -= 实际攻击力加成;
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && isNormalAttack)
            {
                double 生命值减少 = 当前生命值;
                character.HP -= 生命值减少;
                WriteLine($"[ {character} ] 由于自身力量过于强大而被反噬，损失了 [ {生命值减少:0.##} ] 点生命值！");
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> enemys, List<Character> teammates, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                实际攻击力加成 = 0;
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
        }
    }
}
