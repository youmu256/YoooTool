using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoooTool.Code.Slk
{
    public class LevelEditor
    {
        public void Init()
        {

        }
        public SLK_Level EditLevel { get; private set; } = new SLK_Level();
        
        public void NewLevel()
        {
            EditLevel = new SLK_Level();
        }

        public void LoadLevel(string levelId)
        {
            //EditLevel = SlkManager.Instance.
        }

        public void SaveLevel()
        {
            if (string.IsNullOrEmpty(EditLevel.Id))
            {
                return;
            }
            //覆盖保存
        }
        public void SaveAsLevel(string exId)
        {
            Level_SetId(exId);
            SaveLevel();
        }


        public void DrawLevelGui()
        {
            //WinForm
        }

        #region Detail Operation

        public void Level_AddRoom(string roomId)
        {
            EditLevel.RefRooms.Add(roomId);
        }

        public void Level_RemoveRoom(string roomId)
        {
            EditLevel.RefRooms.Remove(roomId);
        }

        public void Level_ClearRooms()
        {
            EditLevel.RefRooms.Clear();
        }
        public void Level_SetRooms(string roomsId)
        {
            Level_ClearRooms();
            string[] ids = roomsId.Split(';');
            foreach (var id in ids)
            {
                Level_AddRoom(id);
            }
        }
        public void Level_SetId(string levelId)
        {
            EditLevel.Id = levelId;
        }
        public void Level_SetRandomSort(bool isRandom)
        {
            EditLevel.IsRandom = isRandom;
        }

        #endregion

    }
}
