﻿using System;
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
            var list = SlkManager.Instance.UnitSpawnnerTab.GetAllData();
            for (int i = 0; i < list.Count; i++)
            {
                var spawnner = list[i];
                var poolName = "";
                var lastTime = spawnner.PoolLastTime.ToString("f2");
                var poolInterval = spawnner.PoolInterval.ToString("f2");
                var listName = "";
                var listInterval = spawnner.ListInterval.ToString("f2");
                sb.AppendLine(string.Format("//--ConfigBegin--{0}--", spawnner.Id));
                if (!spawnner.UnitPool.IsEmpty())
                {
                    poolName = "pool_" + spawnner.Id;
                    foreach (var pair in spawnner.UnitPool.GetMapCopy())
                    {
                        var data = SlkParseUtil.GetIdRefObjectJass<SLK_Unit>(pair.Key);
                        var weight = pair.Value.ToString("f2");
                        sb.AppendLine(string.Format("call WeightPoolLib_RegistPool_Int(\"{0}\",{1},{2})", poolName, data,
                            weight));
                    }
                }
                if (spawnner.UnitList.Count > 0)
                {
                    listName = "list_" + spawnner.Id;
                    foreach (var unit in spawnner.UnitList)
                    {
                        var data = SlkParseUtil.GetIdRefObjectJass<SLK_Unit>(unit);
                        var weight = 1;
                        sb.AppendLine(string.Format("call WeightPoolLib_RegistPool_Int(\"{0}\",{1},{2})", listName, data,
                            weight));
                    }
                }
                sb.AppendLine(string.Format("call RecordSpawnnerCfg(\"{0}\",{1},{2},\"{3}\",{4})", poolName, lastTime, poolInterval,listName,listInterval));
                sb.AppendLine();
            }
            File.WriteAllText(GetPathFileName(SlkManager.Instance.UnitSpawnnerTab.GetExportFileName()), sb.ToString());
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
            File.WriteAllText(GetPathFileName(SlkManager.Instance.LootTab.GetExportFileName()), sb.ToString());
        }
    }

}
