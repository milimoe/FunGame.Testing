﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Testing.Skills
{
    public class 平衡强化 : Skill
    {
        public override long Id => 4011;
        public override string Name => "平衡强化";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double EPCost => 100;
        public override double CD => 55 - (1 * (Level - 1));
        public override double HardnessTime => 12;

        public 平衡强化(Character character) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 平衡强化特效(this));
        }
    }

    public class 平衡强化特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"敏捷提高 20%，然后将目前的力量补充到与敏捷持平，持续 {Duration} 时间。";
        public override bool TargetSelf => true;
        public override bool Durative => true;
        public override double Duration => 30;

        private double 本次提升的敏捷 = 0;
        private double 本次提升的力量 = 0;

        public override void OnEffectGained(Character character)
        {
            double pastHP = character.HP;
            double pastMaxHP = character.MaxHP;
            double pastMP = character.MP;
            double pastMaxMP = character.MaxMP;
            本次提升的敏捷 = character.BaseAGI * 0.2;
            character.ExAGI += 本次提升的敏捷;
            本次提升的力量 = character.AGI - character.STR;
            character.ExSTR += 本次提升的力量;
            character.Recovery(pastHP, pastMP, pastMaxHP, pastMaxMP);
            WriteLine($"[ {character} ] 敏捷提升了 {本次提升的敏捷:f2}，力量提升了 {本次提升的力量:f2}！");
        }

        public override void OnEffectLost(Character character)
        {
            double pastHP = character.HP;
            double pastMaxHP = character.MaxHP;
            double pastMP = character.MP;
            double pastMaxMP = character.MaxMP;
            character.ExAGI -= character.BaseAGI * 0.2;
            character.ExSTR -= 本次提升的力量;
            character.Recovery(pastHP, pastMP, pastMaxHP, pastMaxMP);
        }

        public override void OnSkillCasted(Character caster, List<Character> enemys, List<Character> teammates, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                本次提升的敏捷 = 0;
                本次提升的力量 = 0;
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
        }
    }
}
