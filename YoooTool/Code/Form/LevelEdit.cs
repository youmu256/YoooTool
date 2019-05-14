using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoooTool.Code.Slk;

namespace YoooTool.Code
{
    public partial class LevelEdit : Form
    {
        public LevelEdit()
        {
            InitializeComponent();
            LoadRooms();
        }

        void LoadRooms()
        {
            var rooms = SlkManager.Instance.RoomTab.GetAllData();
            int index = 0;
            //var font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            foreach (var slkRoom in rooms)
            {
                Console.WriteLine(slkRoom.Id);
                CheckBox cb = new CheckBox();
                cb.AutoSize = true;
                cb.Location = new Point(0,0);
                cb.Name = "room_" + index;
                cb.TabIndex = index;
                cb.Text = slkRoom.Id + "/" + slkRoom.Desc;
                cb.UseVisualStyleBackColor = true;
                cb.Size = new System.Drawing.Size(100, 24);
                cb.CheckedChanged += CbOnCheckedChanged;
                cb.Tag = slkRoom.Id;
                this.RoomsContent.Controls.Add(cb);
                index++;
            }
        }

        private void CbOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            CheckBox cb = sender as CheckBox;
            string id =  cb.Tag as string;
            var room = SlkManager.Instance.RoomTab.GetData(id);
            Console.WriteLine("Check : " + room.Id);
        }
    }
}
