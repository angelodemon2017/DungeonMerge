using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    public class CalculateMenu : EditorWindow
    {
        public Config Config;
        public int fromLevel;
        private bool _showCalculating;

        private bool _damageHeroes;
        private int startHeroLevel;

        private bool _PerkStats;
        private int _startLevelPerk;
        private TypePerk _selectPerk;

        public int rowCount = 3;
        public int columnCount = 10;

        [MenuItem("HELPER/Calculate Menu")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CalculateMenu));
        }

        void OnGUI()
        {
            Config = (Config)EditorGUILayout.ObjectField("Scriptable Object", Config, typeof(Config), false);

            ShowCalculate();
        }

        private void ShowCalculate()
        {
            MonstersByLevel();
            DamageAndCostByHeroLevel();
            PerkStats();
        }

        private void MonstersByLevel()
        {
            _showCalculating = EditorGUILayout.Foldout(_showCalculating, $"Monsters:", true);
            if (_showCalculating && Config != null)
            {
                fromLevel = EditorGUILayout.IntField("From Level:", fromLevel);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUI.indentLevel++;
                GUILayout.Label("Level:   HP(mob/wave):   Reward(m/w):");
                for (int l = fromLevel; l <= fromLevel + 9; l++)
                {
                    var hpMob = Config.GetHPMonster(l);
                    var waveHP = hpMob * l.CountMobsByLevel();
                    var reward = Config.GetRewardByMonster(l);
                    var rewardWave = reward * l.CountMobsByLevel();

                    var levelLab = MakeString(l.ToString(), 9);
                    var hpLab = MakeString($"{hpMob}/{waveHP}", 15);
                    var rewardLab = MakeString($"{reward}/{rewardWave}", 20);

                    GUILayout.Label($"{levelLab}{hpLab}{rewardLab}");
//                    GUILayout.Label($"Hp on level({l}): {hpMob} at {(l.IsBossLevel() ? 1 : 10)} mobs. Reward by kill:{reward}", EditorStyles.boldLabel);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }

        private string MakeString(string baseString, int needChars)
        {
            var needAdd = needChars - baseString.Length;

            for (int i = 0; i < needAdd; i++)
            {
                baseString = $"{baseString}  ";
            }

            return baseString;
        }

        private void DamageAndCostByHeroLevel()
        {
            _damageHeroes = EditorGUILayout.Foldout(_damageHeroes, $"Damage heroes:", true);
            if (_damageHeroes && Config != null)
            {
                startHeroLevel = EditorGUILayout.IntField("From hero level:", startHeroLevel);

                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUI.indentLevel++;
                for (int i = startHeroLevel; i < startHeroLevel + 10; i++)
                {
                    var damage = Config.GetDamageHero(i);
                    var costSpawn = Config.CostSpawn(i);
                    GUILayout.Label($"Level({i}) cost:{costSpawn}g. damage:{damage} p/s", EditorStyles.boldLabel);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }

        private void PerkStats()
        {
            _PerkStats = EditorGUILayout.Foldout(_PerkStats, $"Perks stats:", true);
            if (_PerkStats && Config != null)
            {
                _startLevelPerk = EditorGUILayout.IntField("From perk level:", _startLevelPerk);
                _selectPerk = (TypePerk)EditorGUILayout.EnumPopup("Select perk:", _selectPerk);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUI.indentLevel++;
                for (int i = _startLevelPerk; i < _startLevelPerk + 10; i++)
                {
                    var cost = Config.GetDiamondCost(i, _selectPerk);
                    var needCard = Config.GetNeedCard(i,_selectPerk);
                    GUILayout.Label($"Level perk({i}) cost:{cost}, need card:{needCard}", EditorStyles.boldLabel);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }
    }
}