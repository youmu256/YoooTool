using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoooTool.Code.Slk
{

    /// <summary>
    /// 导出jass配置文本
    /// </summary>

    public class ExportHelper
    {
        public static string GetPathFileName(string fileName, string ext=".jass")
        {
            return "LevelCfg/" + fileName + ext;
        }

        public void ExportAllLevel2Jass(bool split = false)
        {
            var levels = SlkManager.Instance.LevelTab.GetAllData();
            if (split)
            {
                foreach (var slkLevel in levels)
                {
                    File.WriteAllText(GetPathFileName(slkLevel.Id), slkLevel.GetJassConfig());
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var slkLevel in levels)
                {
                    sb.AppendLine(string.Format("//------{0}------", slkLevel.Id));
                    sb.AppendLine(slkLevel.GetJassConfig());
                }
                File.WriteAllText(GetPathFileName("AllLevel"), sb.ToString());
            }
        }

        public void ExportEnemySpawnner2Jass()
        {
            //导出Spawnner 的jass
            StringBuilder sb = new StringBuilder();
            var list = SlkManager.Instance.EnemySpawnnerTab.GetAllData();
            for (int i = 0; i < list.Count; i++)
            {
                var spawnner = list[i];
                var poolName = spawnner.Id;
                var lastTime = spawnner.LastTime.ToString("f2");
                var interval = spawnner.Interval.ToString("f2");
                sb.AppendLine(string.Format("//--ConfigBegin--{0}--", poolName));
                foreach (var pair in spawnner.EnemyIdPool.GetMapCopy())
                {
                    var data = SlkParseUtil.GetIdRefObjectJass<SLK_Unit>(pair.Key);
                    var weight = pair.Value.ToString("f2");
                    sb.AppendLine(string.Format("call WeightPoolLib_RegistPool_Int(\"{0}\",{1},{2})", poolName, data, weight));
                }
                sb.AppendLine(string.Format("call RecordSpawnnerCfg(\"{0}\",{1},{2})", poolName, lastTime, interval));
                sb.AppendLine();
            }
            File.WriteAllText(GetPathFileName("EnemySpawnner"), sb.ToString());
        }

        public void ExportEnemyGroup2Jass()
        {
            //导出EnemyGroup
            StringBuilder sb = new StringBuilder();
            var list = SlkManager.Instance.EnemyGroupTab.GetAllData();
            for (int i = 0; i < list.Count; i++)
            {
                var data = list[i];
                for (int j = 0; j < data.EnemyList.Count; j++)
                {
                    var enemy = SlkParseUtil.GetIdRefObjectJass<SLK_Unit>(data.EnemyList[j]);
                    sb.AppendLine(string.Format("set dataArr[{0}] = {1}", j + 1, enemy));
                }
                sb.AppendLine(string.Format("set dataLength = {0}", data.EnemyList.Count));
                sb.AppendLine(string.Format("call RecordCurrentToLevel_WithId(\"{0}\",{1})", data.Id, data.Level));
            }
            File.WriteAllText(GetPathFileName("EnemyGroup"), sb.ToString());
        }
        public void ExportLoot2Jass()
        {
            //导出Loot
            StringBuilder sb = new StringBuilder();
            var list = SlkManager.Instance.LootTab.GetAllData();
            for (int i = 0; i < list.Count; i++)
            {
                //列表存 随机池存
                var data = list[i];

                var dataKey = data.Id;
                sb.AppendLine(string.Format("//--ConfigBegin--{0}--", dataKey));
                for (int j = 0; j < data.Items.Count; j++)
                {
                    var item = SlkParseUtil.GetIdRefObjectJass<SLK_LootItem>(data.Items[j]);
                    sb.AppendLine(string.Format("set dataArr[{0}] = {1}", j + 1, item));
                }
                sb.AppendLine(string.Format("set dataLength = {0}", data.Items.Count));

                foreach (var pair in data.ItemPool.GetMapCopy())
                {
                    var item = SlkParseUtil.GetIdRefObjectJass<SLK_LootItem>(pair.Key);
                    var weight = pair.Value.ToString("f2");
                    sb.AppendLine(string.Format("call WeightPoolLib_RegistPool_Int(\"{0}\",{1},{2})", dataKey, item, weight));
                }
                //key poolname count list count
                //jass里按照dataArr等记录数据
                sb.AppendLine(string.Format("call RecordLoot(\"{0}\",{1},{2})", dataKey,data.ConstCount,data.RandomCount));
                sb.AppendLine();
            }
            File.WriteAllText(GetPathFileName("Loot"), sb.ToString());
        }
    }

}
