﻿using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Testing.Skills
{
    public class 灵能反射 : Skill
    {
        public override long Id => 4004;
        public override string Name => "灵能反射";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 灵能反射(Character character) : base(SkillType.Passive, character)
        {
            Effects.Add(new 灵能反射特效(this));
        }

        public override IEnumerable<Effect> AddInactiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 灵能反射特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"每释放 {触发硬直次数} 次魔法才会触发硬直时间，且魔法命中时基于 25% 智力 [{获得额外能量值}] 获得额外能量值。";
        public override bool TargetSelf => true;

        public int 触发硬直次数 { get; set; } = 2;
        public int 释放次数 { get; set; } = 0;
        public double 获得额外能量值
        {
            get
            {
                return Calculation.Round2Digits(0.25 * Skill.Character?.INT ?? 0);
            }
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && isMagicDamage && damageResult != DamageResult.Evaded && character.EP < 200)
            {
                double 实际获得能量值 = 获得额外能量值;
                character.EP += 实际获得能量值;
                WriteLine("[ " + character + " ] 发动了灵能反射！额外获得了 " + 实际获得能量值 + " 能量！");
                IEnumerable<Effect> effects = character.Effects.Where(e => e is 三重叠加特效);
                if (effects.Any() && effects.First() is 三重叠加特效 e)
                {
                    double 获得的魔法值 = Calculation.Round2Digits(实际获得能量值 * 10);
                    character.MP += 获得的魔法值;
                    WriteLine("[ " + character + " ] 发动了三重叠加！额外获得了 " + 获得的魔法值 + " 魔法值！");
                }
            }
        }

        public override void AlterHardnessTimeAfterCastSkill(Character character, ref double baseHardnessTime, ref bool isCheckProtected)
        {
            释放次数++;
            if (释放次数 < 触发硬直次数)
            {
                baseHardnessTime = 0;
                WriteLine($"[ {character} ] 发动了灵能反射，消除了硬直时间！！");
            }
            else
            {
                释放次数 = 0;
                IEnumerable<Effect> effects = character.Effects.Where(e => e is 三重叠加特效);
                if (effects.Any() && effects.First() is 三重叠加特效 e)
                {
                    baseHardnessTime = 0;
                    WriteLine($"[ {character} ] 发动了灵能反射，消除了硬直时间！！");
                    e.剩余持续次数--;
                    if (e.剩余持续次数 == 0)
                    {
                        character.Effects.Remove(e);
                        e.OnEffectLost(character);
                    }
                }
            }
        }
    }
}
